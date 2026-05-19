using Microsoft.AspNetCore.Mvc;
using MargiesTravelAgent.Models;
using MargiesTravelAgent.Services;

namespace MargiesTravelAgent.Controllers;

public class ChatController : Controller
{
    private static readonly HashSet<string> AllowedImageMediaTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp", "image/bmp"
    };

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
    [ValidateAntiForgeryToken]
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
            var contentType = request.Image.ContentType;
            if (!AllowedImageMediaTypes.Contains(contentType))
            {
                return BadRequest(new ChatResponse { Success = false, ErrorMessage = "Only image files are supported (JPEG, PNG, GIF, WebP, BMP)." });
            }

            using var ms = new MemoryStream();
            await request.Image.CopyToAsync(ms, cancellationToken);
            imageBytes = ms.ToArray();
            imageMediaType = contentType;
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
