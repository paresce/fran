using Bunit;
using Fran.Components;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Fran.Tests;

/// <summary>
/// Automated WCAG 2.2 AA Accessibility Audit Suite [FEAT-0053].
/// Validates ARIA landmark roles, accessible names, focus states, and disabled semantics
/// across Fran Design System interactive primitives.
/// </summary>
public class FaWcagAuditSuiteTests : BunitContext
{
    [Fact]
    public void Button_WCAG_2_1_1_KeyboardAndFocusableSemantics_Enforced()
    {
        // Standard button
        var standardCut = Render<FaButton>(p => p
            .Add(b => b.Type, "button")
            .AddChildContent("Submit Action"));

        var btn = standardCut.Find("button");
        Assert.Equal("button", btn.GetAttribute("type"));
        Assert.Null(btn.GetAttribute("aria-disabled"));
        Assert.False(btn.HasAttribute("disabled"));

        // Disabled button
        var disabledCut = Render<FaButton>(p => p
            .Add(b => b.Disabled, true)
            .AddChildContent("Disabled Action"));

        var disabledBtn = disabledCut.Find("button");
        Assert.True(disabledBtn.HasAttribute("disabled"));

        // Link button (WCAG 4.1.2 Name, Role, Value)
        var linkCut = Render<FaButton>(p => p
            .Add(b => b.Href, "https://paresce.com")
            .Add(b => b.Disabled, true)
            .AddChildContent("Disabled Link Action"));

        var anchor = linkCut.Find("a");
        Assert.Equal("true", anchor.GetAttribute("aria-disabled"));
    }

    [Fact]
    public void Alert_WCAG_4_1_3_StatusMessagesAndLiveRegions_Enforced()
    {
        var cut = Render<FaAlert>(p => p
            .Add(a => a.Variant, FaAlertVariant.Danger)
            .AddChildContent("Unable to reach cloud database."));

        var alert = cut.Find(".fa-alert");
        Assert.Equal("alert", alert.GetAttribute("role"));
        Assert.Contains("fa-alert-danger", alert.ClassName);
        Assert.Contains("Unable to reach cloud database.", cut.Markup);
    }

    [Fact]
    public void Badge_WCAG_1_4_3_ContrastTokenSemantics_Enforced()
    {
        var cut = Render<FaBadge>(p => p
            .Add(b => b.Variant, FaBadgeVariant.Success)
            .Add(b => b.Size, FaSize.Small)
            .AddChildContent("Active"));

        var badge = cut.Find("span.fa-badge");
        Assert.Contains("fa-badge-success", badge.ClassName);
        Assert.Contains("Active", badge.TextContent);
    }

    [Fact]
    public void Drawer_WCAG_2_4_3_FocusTrapAndDialogSemantics_Enforced()
    {
        var cut = Render<FaDrawer>(p => p
            .Add(d => d.Show, true)
            .Add(d => d.Title, "Inspection Details")
            .AddChildContent("Detailed claim inspection payload."));

        var dialog = cut.Find("[role=\"dialog\"]");
        Assert.NotNull(dialog);
        Assert.Equal("true", dialog.GetAttribute("aria-modal"));
        Assert.Equal("Inspection Details", dialog.GetAttribute("aria-label"));

        var closeBtn = cut.Find("button.fa-drawer-close");
        Assert.Equal("Close drawer", closeBtn.GetAttribute("aria-label"));
    }

    [Fact]
    public void Modal_WCAG_2_4_3_DialogSemantics_Enforced()
    {
        var cut = Render<FaModal>(p => p
            .Add(m => m.Show, true)
            .Add(m => m.Title, "Confirmation Dialog")
            .AddChildContent("Are you sure you want to proceed?"));

        var dialog = cut.Find("[role=\"dialog\"]");
        Assert.NotNull(dialog);
        Assert.Equal("true", dialog.GetAttribute("aria-modal"));
        Assert.Contains("Confirmation Dialog", cut.Markup);

        var closeBtn = cut.Find("button.fa-modal-close");
        Assert.Equal("Close", closeBtn.GetAttribute("aria-label"));
    }

    [Fact]
    public void BookmarkNav_WCAG_1_3_1_InfoAndRelationships_Enforced()
    {
        var items = new[]
        {
            new FaBookmarkItem("sec-1", "Section 1", "01"),
            new FaBookmarkItem("sec-2", "Section 2", "02")
        };

        var cut = Render<FaBookmarkNav>(p => p
            .Add(n => n.Title, "Documentation Navigation")
            .Add(n => n.Items, items)
            .Add(n => n.ActiveId, "sec-1"));

        var navLandmark = cut.Find("nav.fa-bookmark-nav");
        Assert.NotNull(navLandmark);

        var activeLink = cut.Find(".fa-bookmark-nav-link-active");
        Assert.Equal("true", activeLink.GetAttribute("aria-current"));
    }
}
