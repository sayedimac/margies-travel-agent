using Azure.AI.Projects;
using Azure.AI.Extensions.OpenAI;
using Azure.Identity;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Options;
using MargiesTravelAgent.Models;
using OpenAI.Responses;
using System.Collections;

#pragma warning disable OPENAI001

namespace MargiesTravelAgent.Services;

public class AzureAIChatService
{
    private readonly ProjectResponsesClient _responsesClient;
    private readonly SearchClient _searchClient;
    private readonly ILogger<AzureAIChatService> _logger;

    private const string SystemPrompt = """
        You are a helpful travel assistant for Margie's Travel, a travel agency specializing in worldwide travel packages.
        You help customers find information about destinations, hotels, tours, and travel advice.
        Use the provided context from the knowledge base to answer questions accurately.
        When answering, reference specific details from the context where relevant.
        If the context doesn't contain the answer, say you don't have that specific information but offer to help with general travel questions.
        """;

    public AzureAIChatService(IOptions<AzureAISettings> settings, ILogger<AzureAIChatService> logger)
    {
        _logger = logger;
        var options = settings.Value;

        var credential = new DefaultAzureCredential();

        var projectClient = new AIProjectClient(
            new Uri(options.ProjectEndpoint),
            credential);

        _responsesClient = projectClient.ProjectOpenAIClient
            .GetProjectResponsesClientForModel(options.ModelDeployment, null);

        _searchClient = new SearchClient(
            new Uri(options.SearchEndpoint),
            options.SearchIndexName,
            credential);
    }

    public async Task<ChatResponse> SendMessageAsync(
        string message,
        string? previousResponseId,
        byte[]? imageBytes,
        string? imageMediaType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var context = await GetSearchContextAsync(message, cancellationToken);

            var contentParts = new List<ResponseContentPart>
            {
                ResponseContentPart.CreateInputTextPart(message)
            };

            if (imageBytes is { Length: > 0 } && !string.IsNullOrEmpty(imageMediaType))
            {
                var base64 = Convert.ToBase64String(imageBytes);
                var dataUri = new Uri($"data:{imageMediaType};base64,{base64}");
                contentParts.Add(ResponseContentPart.CreateInputImagePart(dataUri));
            }

            var inputItems = new[]
            {
                ResponseItem.CreateUserMessageItem(contentParts)
            };

            var response = await _responsesClient.CreateResponseAsync(
                inputItems,
                cancellationToken: cancellationToken);

            var responseText = ExtractText(response.Value);

            return new ChatResponse
            {
                Success = true,
                ResponseText = responseText,
                ResponseId = response.Value.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Azure AI");
            return new ChatResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while processing your request. Please try again."
            };
        }
    }

    private async Task<string> GetSearchContextAsync(string query, CancellationToken cancellationToken)
    {
        try
        {
            var searchOptions = new SearchOptions
            {
                Size = 5,
                Select = { "snippet", "blob_url" }
            };

            var results = await _searchClient.SearchAsync<SearchDocument>(query, searchOptions, cancellationToken);

            var context = new System.Text.StringBuilder();
            await foreach (var result in results.Value.GetResultsAsync())
            {
                if (result.Document.TryGetValue("snippet", out var snippet) && snippet is string snippetText)
                {
                    context.AppendLine(snippetText);
                    context.AppendLine();
                }
            }

            return context.ToString().Trim();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to retrieve search context, proceeding without it");
            return string.Empty;
        }
    }

    private static string ExtractText(object response)
    {
        var parts = new System.Text.StringBuilder();

        var outputItems = response
            .GetType()
            .GetProperty("OutputItems")
            ?.GetValue(response) as IEnumerable;

        if (outputItems is null)
            return "(No response text)";

        foreach (var item in outputItems)
        {
            if (item is null)
                continue;

            var contentParts = item
                .GetType()
                .GetProperty("Content")
                ?.GetValue(item) as IEnumerable;

            if (contentParts is null)
                continue;

            foreach (var part in contentParts)
            {
                var text = part?
                    .GetType()
                    .GetProperty("Text")
                    ?.GetValue(part) as string;

                if (!string.IsNullOrEmpty(text))
                    parts.Append(text);
            }
        }

        return parts.Length > 0 ? parts.ToString() : "(No response text)";
    }
}
