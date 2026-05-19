namespace MargiesTravelAgent.Models;

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
    public string? PreviousResponseId { get; set; }
    public IFormFile? Image { get; set; }
}

public class ChatResponse
{
    public bool Success { get; set; }
    public string? ResponseText { get; set; }
    public string? ResponseId { get; set; }
    public string? ErrorMessage { get; set; }
}
