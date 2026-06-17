using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace MargiesTravelAgent.Tests;

/// <summary>
/// Base class for all Playwright page tests.
/// Sets the base URL from the runsettings parameter (defaults to http://localhost:5296).
/// </summary>
public abstract class PlaywrightTestBase : PageTest
{
    protected string BaseUrl { get; private set; } = "http://localhost:5296";

    [SetUp]
    public async Task SetupAsync()
    {
        var param = TestContext.Parameters["BaseUrl"];
        if (!string.IsNullOrWhiteSpace(param))
            BaseUrl = param.TrimEnd('/');

        await Page.SetViewportSizeAsync(1280, 720);
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            BaseURL = BaseUrl,
            ViewportSize = new ViewportSize { Width = 1280, Height = 720 }
        };
    }
}
