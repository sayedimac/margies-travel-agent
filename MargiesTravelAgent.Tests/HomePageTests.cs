using Microsoft.Playwright;
using NUnit.Framework;

namespace MargiesTravelAgent.Tests;

[TestFixture]
[Category("Visual")]
public class HomePageTests : PlaywrightTestBase
{
    [Test]
    public async Task HomePage_LoadsSuccessfully()
    {
        var response = await Page.GotoAsync("/");

        Assert.That(response?.Status, Is.EqualTo(200), "Home page should return HTTP 200");
    }

    [Test]
    public async Task HomePage_HasExpectedTitle()
    {
        await Page.GotoAsync("/");

        var title = await Page.TitleAsync();
        Assert.That(title, Does.Contain("Margie"), "Page title should contain 'Margie'");
    }

    [Test]
    public async Task HomePage_HasNavigationBar()
    {
        await Page.GotoAsync("/");

        var navbar = Page.Locator("nav.navbar");
        await Expect(navbar).ToBeVisibleAsync();
    }

    [Test]
    public async Task HomePage_HasNavbarBrandLink()
    {
        await Page.GotoAsync("/");

        // The navbar brand is the main navigation link back to Chat
        var brandLink = Page.Locator("a.navbar-brand");
        await Expect(brandLink).ToBeVisibleAsync();
        await Expect(brandLink).ToContainTextAsync("Margie's Travel Agent");
    }

    [Test]
    public async Task HomeControllerPage_WelcomeHeadingIsVisible()
    {
        // /Home/Index has the dedicated welcome heading
        await Page.GotoAsync("/Home");

        var heading = Page.Locator("h1");
        await Expect(heading).ToBeVisibleAsync();
        await Expect(heading).ToContainTextAsync("Welcome");
    }

    [Test]
    public async Task HomePage_Screenshot()
    {
        await Page.GotoAsync("/");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Directory.CreateDirectory("screenshots");
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "screenshots/home-page.png",
            FullPage = true
        });

        Assert.That(File.Exists("screenshots/home-page.png"), Is.True, "Screenshot should have been saved");
    }
}
