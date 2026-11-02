using System.Threading.Tasks;
using Bunit;
using Fran.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;

namespace Fran.Tests;

public class FaAvatarFormTests : BunitContext
{
    [Fact]
    public void Topbar_OnlyAvatarRenderedByDefault()
    {
        var cut = Render<FaAvatarForm>(p => p
            .Add(x => x.IsAuthenticated, true)
            .Add(x => x.DisplayName, "Jane Doe")
            .Add(x => x.ShowDisplayName, false));

        var trigger = cut.Find("button.fa-avatar-form-trigger");
        Assert.NotNull(trigger);
        Assert.Equal("false", trigger.GetAttribute("aria-expanded"));

        // Avatar is rendered inside trigger
        var avatar = trigger.QuerySelector(".fa-avatar");
        Assert.NotNull(avatar);

        // Name is NOT rendered in trigger
        var nameSpan = trigger.QuerySelector(".fa-avatar-form-trigger-name");
        Assert.Null(nameSpan);
    }

    [Fact]
    public void Topbar_ShowsUserName_WhenConfigured()
    {
        var cut = Render<FaAvatarForm>(p => p
            .Add(x => x.IsAuthenticated, true)
            .Add(x => x.DisplayName, "Jane Doe")
            .Add(x => x.ShowDisplayName, true));

        var trigger = cut.Find("button.fa-avatar-form-trigger");
        var nameSpan = trigger.QuerySelector(".fa-avatar-form-trigger-name");
        Assert.NotNull(nameSpan);
        Assert.Equal("Jane Doe", nameSpan.TextContent.Trim());
    }

    [Fact]
    public void SelectingAvatar_OpensFormPanel()
    {
        var cut = Render<FaAvatarForm>(p => p
            .Add(x => x.IsAuthenticated, true)
            .Add(x => x.DisplayName, "Jane Doe")
            .Add(x => x.Email, "jane@example.com"));

        var trigger = cut.Find("button.fa-avatar-form-trigger");
        var panel = cut.Find(".fa-avatar-form-panel");
        Assert.DoesNotContain("fa-avatar-form-panel-open", panel.ClassName);

        // Click trigger to open
        trigger.Click();

        Assert.Contains("fa-avatar-form-panel-open", panel.ClassName);
        Assert.Equal("true", trigger.GetAttribute("aria-expanded"));

        // User identity details inside panel
        var name = cut.Find(".fa-avatar-form-display-name");
        Assert.Equal("Jane Doe", name.TextContent.Trim());

        var email = cut.Find(".fa-avatar-form-subtitle");
        Assert.Equal("jane@example.com", email.TextContent.Trim());
    }

    [Fact]
    public void Panel_IncludesThemeSwitcher_WithLightDarkColorblind()
    {
        var cut = Render<FaAvatarForm>(p => p
            .Add(x => x.IsAuthenticated, true)
            .Add(x => x.DisplayName, "Jane Doe")
            .Add(x => x.ShowThemeSwitcher, true));

        var trigger = cut.Find("button.fa-avatar-form-trigger");
        trigger.Click();

        var themeGroup = cut.Find(".fa-avatar-form-theme-group");
        Assert.NotNull(themeGroup);

        var lightBtn = themeGroup.QuerySelector("[data-theme-btn='light']");
        var darkBtn = themeGroup.QuerySelector("[data-theme-btn='dark']");
        var cbBtn = themeGroup.QuerySelector("[data-theme-btn='colorblind']");

        Assert.NotNull(lightBtn);
        Assert.NotNull(darkBtn);
        Assert.NotNull(cbBtn);
    }

    [Fact]
    public void Panel_RendersApplicationSpecificItems()
    {
        var cut = Render<FaAvatarForm>(p => p
            .Add(x => x.IsAuthenticated, true)
            .Add(x => x.DisplayName, "Jane Doe")
            .AddChildContent("<a class=\"app-custom-item\" href=\"/org\">Switch Organization</a>"));

        var trigger = cut.Find("button.fa-avatar-form-trigger");
        trigger.Click();

        var customItem = cut.Find("a.app-custom-item");
        Assert.NotNull(customItem);
        Assert.Equal("Switch Organization", customItem.TextContent.Trim());
    }

    [Fact]
    public async Task AuthenticatedUser_LogoutButton_InvokesCallbackAndCloses()
    {
        var logoutInvoked = false;
        var cut = Render<FaAvatarForm>(p => p
            .Add(x => x.IsAuthenticated, true)
            .Add(x => x.DisplayName, "Jane Doe")
            .Add(x => x.OnLogout, () => logoutInvoked = true));

        var trigger = cut.Find("button.fa-avatar-form-trigger");
        trigger.Click();

        var logoutBtn = cut.Find("button.fa-avatar-form-btn-block");
        Assert.Contains("Log out", logoutBtn.TextContent);

        await logoutBtn.ClickAsync(new MouseEventArgs());

        Assert.True(logoutInvoked);
        var panel = cut.Find(".fa-avatar-form-panel");
        Assert.DoesNotContain("fa-avatar-form-panel-open", panel.ClassName);
    }

    [Fact]
    public async Task UnauthenticatedUser_LoginButton_InvokesCallbackAndCloses()
    {
        var loginInvoked = false;
        var cut = Render<FaAvatarForm>(p => p
            .Add(x => x.IsAuthenticated, false)
            .Add(x => x.OnLogin, () => loginInvoked = true));

        var trigger = cut.Find("button.fa-avatar-form-trigger");
        trigger.Click();

        var loginBtn = cut.Find("button.fa-avatar-form-btn-block");
        Assert.Contains("Log in", loginBtn.TextContent);

        await loginBtn.ClickAsync(new MouseEventArgs());

        Assert.True(loginInvoked);
        var panel = cut.Find(".fa-avatar-form-panel");
        Assert.DoesNotContain("fa-avatar-form-panel-open", panel.ClassName);
    }

    [Fact]
    public async Task AccountAction_InvokesCallback()
    {
        var accountClicked = false;
        var cut = Render<FaAvatarForm>(p => p
            .Add(x => x.IsAuthenticated, true)
            .Add(x => x.DisplayName, "Jane Doe")
            .Add(x => x.OnAccountClick, () => accountClicked = true));

        var trigger = cut.Find("button.fa-avatar-form-trigger");
        trigger.Click();

        var accountAction = cut.Find("button.fa-avatar-form-account-action");
        Assert.NotNull(accountAction);

        await accountAction.ClickAsync(new MouseEventArgs());
        Assert.True(accountClicked);
    }

    [Fact]
    public void PressingEscape_ClosesPanel()
    {
        var cut = Render<FaAvatarForm>(p => p
            .Add(x => x.IsAuthenticated, true)
            .Add(x => x.DisplayName, "Jane Doe"));

        var trigger = cut.Find("button.fa-avatar-form-trigger");
        trigger.Click();

        var panel = cut.Find(".fa-avatar-form-panel");
        Assert.Contains("fa-avatar-form-panel-open", panel.ClassName);

        var container = cut.Find("div.fa-avatar-form");
        container.KeyDown(new KeyboardEventArgs { Key = "Escape" });

        Assert.DoesNotContain("fa-avatar-form-panel-open", panel.ClassName);
    }

    [Fact]
    public void RoundAvatarButton_RenderedByDefault_WhenNameHidden()
    {
        var cut = Render<FaAvatarForm>(p => p
            .Add(x => x.IsAuthenticated, true)
            .Add(x => x.DisplayName, "Jane Doe")
            .Add(x => x.RoundAvatar, true)
            .Add(x => x.ShowDisplayName, false));

        var trigger = cut.Find("button.fa-avatar-form-trigger");
        Assert.Contains("fa-avatar-form-trigger-round", trigger.ClassName);
    }

    [Fact]
    public void OpeningPanel_RendersDragHandleAndBackdrop_ClickingBackdropCloses()
    {
        var cut = Render<FaAvatarForm>(p => p
            .Add(x => x.IsAuthenticated, true)
            .Add(x => x.DisplayName, "Jane Doe")
            .Add(x => x.ShowBackdrop, true));

        var trigger = cut.Find("button.fa-avatar-form-trigger");
        trigger.Click();

        var handle = cut.Find(".fa-avatar-form-drag-handle");
        Assert.NotNull(handle);

        var backdrop = cut.Find(".fa-avatar-form-backdrop");
        Assert.NotNull(backdrop);

        backdrop.Click();
        var panel = cut.Find(".fa-avatar-form-panel");
        Assert.DoesNotContain("fa-avatar-form-panel-open", panel.ClassName);
    }

    [Fact]
    public void SettingsLink_Rendered_WhenConfigured()
    {
        var cut = Render<FaAvatarForm>(p => p
            .Add(x => x.IsAuthenticated, true)
            .Add(x => x.DisplayName, "Jane Doe")
            .Add(x => x.SettingsHref, "/settings")
            .Add(x => x.SettingsText, "All settings"));

        var trigger = cut.Find("button.fa-avatar-form-trigger");
        trigger.Click();

        var settingsLink = cut.Find("a[href='/settings']");
        Assert.NotNull(settingsLink);
        Assert.Contains("All settings", settingsLink.TextContent);
    }

    [Fact]
    public void FaAccountMenu_RendersRoundAvatarButton()
    {
        var cut = Render<FaAccountMenu>(p => p
            .Add(x => x.IsAuthenticated, true)
            .Add(x => x.DisplayName, "Benjamin Paresce")
            .Add(x => x.RoundAvatar, true)
            .Add(x => x.ShowDisplayName, false));

        var trigger = cut.Find("button.fa-avatar-form-trigger");
        Assert.NotNull(trigger);
        Assert.Contains("fa-avatar-form-trigger-round", trigger.ClassName);
    }
}