using Microsoft.Playwright;
using NUnit.Framework;

namespace MargiesTravelAgent.Tests;

[TestFixture]
[Category("Visual")]
public class ChatPageTests : PlaywrightTestBase
{
    [Test]
    public async Task ChatPage_LoadsSuccessfully()
    {
        var response = await Page.GotoAsync("/Chat");

        Assert.That(response?.Status, Is.EqualTo(200), "Chat page should return HTTP 200");
    }

    [Test]
    public async Task ChatPage_HasExpectedTitle()
    {
        await Page.GotoAsync("/Chat");

        var title = await Page.TitleAsync();
        Assert.That(title, Does.Contain("Chat"), "Page title should contain 'Chat'");
    }

    [Test]
    public async Task ChatPage_ChatMessagesAreaIsVisible()
    {
        await Page.GotoAsync("/Chat");

        var messagesArea = Page.Locator("#chat-messages");
        await Expect(messagesArea).ToBeVisibleAsync();
    }

    [Test]
    public async Task ChatPage_WelcomeMessageIsDisplayed()
    {
        await Page.GotoAsync("/Chat");

        var welcomeText = Page.Locator("#chat-messages").GetByText("Margie's Travel Agent");
        await Expect(welcomeText).ToBeVisibleAsync();
    }

    [Test]
    public async Task ChatPage_MessageInputIsVisible()
    {
        await Page.GotoAsync("/Chat");

        var input = Page.Locator("#message-input");
        await Expect(input).ToBeVisibleAsync();
        await Expect(input).ToBeEnabledAsync();
    }

    [Test]
    public async Task ChatPage_SendButtonIsVisible()
    {
        await Page.GotoAsync("/Chat");

        var sendBtn = Page.Locator("#send-btn");
        await Expect(sendBtn).ToBeVisibleAsync();
    }

    [Test]
    public async Task ChatPage_AttachImageButtonIsVisible()
    {
        await Page.GotoAsync("/Chat");

        var attachBtn = Page.Locator("#attach-image-btn");
        await Expect(attachBtn).ToBeVisibleAsync();
    }

    [Test]
    public async Task ChatPage_ImagePreviewIsHiddenOnLoad()
    {
        await Page.GotoAsync("/Chat");

        // The preview container should be hidden before any image is attached
        var previewContainer = Page.Locator("#image-preview-container");
        await Expect(previewContainer).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("d-none"));
    }

    [Test]
    public async Task ChatPage_MessageInputAcceptsText()
    {
        await Page.GotoAsync("/Chat");

        var input = Page.Locator("#message-input");
        await input.FillAsync("Tell me about Paris");

        await Expect(input).ToHaveValueAsync("Tell me about Paris");
    }

    [Test]
    public async Task ChatPage_MessageInputClearsAfterTypingAndDeleting()
    {
        await Page.GotoAsync("/Chat");

        var input = Page.Locator("#message-input");
        await input.FillAsync("Hello");
        await input.FillAsync("");

        await Expect(input).ToHaveValueAsync("");
    }

    [Test]
    public async Task ChatPage_LayoutHasNavbar()
    {
        await Page.GotoAsync("/Chat");

        var navbar = Page.Locator("nav.navbar");
        await Expect(navbar).ToBeVisibleAsync();
    }

    [Test]
    public async Task ChatPage_InputAreaIsAtBottom()
    {
        await Page.GotoAsync("/Chat");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var form = Page.Locator("#chat-form");
        await Expect(form).ToBeVisibleAsync();

        // Verify the form is in the input area (border-top section)
        var inputArea = form.Locator("xpath=ancestor::div[contains(@class,'border-top')]");
        await Expect(inputArea).ToBeVisibleAsync();
    }

    [Test]
    public async Task ChatPage_FooterShowsPoweredByText()
    {
        await Page.GotoAsync("/Chat");

        var footer = Page.GetByText("Powered by Azure AI Foundry");
        await Expect(footer).ToBeVisibleAsync();
    }

    [Test]
    public async Task ChatPage_Screenshot_InitialState()
    {
        await Page.GotoAsync("/Chat");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Directory.CreateDirectory("screenshots");
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "screenshots/chat-page-initial.png",
            FullPage = true
        });

        Assert.That(File.Exists("screenshots/chat-page-initial.png"), Is.True, "Screenshot should have been saved");
    }

    [Test]
    public async Task ChatPage_Screenshot_WithTextEntered()
    {
        await Page.GotoAsync("/Chat");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var input = Page.Locator("#message-input");
        await input.FillAsync("What are the best beaches in Thailand?");

        Directory.CreateDirectory("screenshots");
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "screenshots/chat-page-with-text.png",
            FullPage = true
        });

        Assert.That(File.Exists("screenshots/chat-page-with-text.png"), Is.True, "Screenshot should have been saved");
    }
}
