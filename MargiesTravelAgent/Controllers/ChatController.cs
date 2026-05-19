using Microsoft.AspNetCore.Mvc;
using MargiesTravelAgent.Models;
using MargiesTravelAgent.Services;

namespace MargiesTravelAgent.Controllers;

public class ChatController : Controller
{
    private readonly AzureAIChatService _chatService;

    public ChatController(AzureAIChatService chatService)
    {
        _chatService = chatService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromForm] ChatRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new ChatResponse { Success = false, ErrorMessage = "Message cannot be empty." });
        }

        byte[]? imageBytes = null;
        string? imageMediaType = null;

        if (request.Image is { Length: > 0 })
        {
            using var ms = new MemoryStream();
            await request.Image.CopyToAsync(ms, cancellationToken);
            imageBytes = ms.ToArray();
            imageMediaType = request.Image.ContentType;
        }

        var result = await _chatService.SendMessageAsync(
            request.Message,
            request.PreviousResponseId,
            imageBytes,
            imageMediaType,
            cancellationToken);

        return Json(result);
    }
}
