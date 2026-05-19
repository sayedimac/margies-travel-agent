#pragma warning disable OPENAI001
using Azure.AI.Extensions.OpenAI;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Extensions.Options;
using MargiesTravelAgent.Models;
using OpenAI.Responses;

namespace MargiesTravelAgent.Services;

public class AzureAIChatService
{
    private readonly ProjectResponsesClient _responsesClient;
    private readonly ILogger<AzureAIChatService> _logger;

    public AzureAIChatService(IOptions<AzureAISettings> settings, ILogger<AzureAIChatService> logger)
    {
        _logger = logger;
        var options = settings.Value;

        var projectClient = new AIProjectClient(
            new Uri(options.ProjectEndpoint),
            new DefaultAzureCredential());

        _responsesClient = projectClient.ProjectOpenAIClient
            .GetProjectResponsesClientForAgentEndpoint(options.AgentName, defaultConversationId: null);
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
                previousResponseId,
                cancellationToken);

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
            _logger.LogError(ex, "Error calling Azure AI Foundry");
            return new ChatResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private static string ExtractText(ResponseResult result)
    {
        var parts = new System.Text.StringBuilder();
        foreach (var item in result.OutputItems)
        {
            if (item is MessageResponseItem msg)
            {
                foreach (var part in msg.Content)
                {
                    if (!string.IsNullOrEmpty(part.Text))
                        parts.Append(part.Text);
                }
            }
        }
        return parts.Length > 0 ? parts.ToString() : "(No response text)";
    }
}
