namespace MargiesTravelAgent.Services;

public class AzureAISettings
{
    public string ProjectEndpoint { get; set; } = string.Empty;
    public string AgentName { get; set; } = string.Empty;
    public string ResponsesEndpoint { get; set; } = string.Empty;
    public string SearchEndpoint { get; set; } = string.Empty;
    public string SearchIndexName { get; set; } = string.Empty;
    public string ModelDeployment { get; set; } = string.Empty;
}
