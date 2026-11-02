using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Fran.Components;

/// <summary>
/// The standard Paresce round avatar button and settings menu.
/// Anchors a contextual settings modal relative to the round avatar button on desktop,
/// and presents a mobile bottom sheet on small viewports. Encompasses user identity,
/// Light/Dark/Colorblind theme selection, modular app slots, and login/logout controls.
/// </summary>
public sealed class FaAccountMenu : ComponentBase
{
    [Parameter] public bool IsAuthenticated { get; set; }
    [Parameter] public string? DisplayName { get; set; }
    [Parameter] public string? ImageUrl { get; set; }
    [Parameter] public string? Email { get; set; }
    [Parameter] public string? Subtitle { get; set; }
    [Parameter] public bool ShowDisplayName { get; set; }
    [Parameter] public bool RoundAvatar { get; set; } = true;
    [Parameter] public bool ShowBackdrop { get; set; } = true;
    [Parameter] public string? SettingsHref { get; set; }
    [Parameter] public string SettingsText { get; set; } = "All settings";
    [Parameter] public bool ShowSettingsLink { get; set; } = true;
    [Parameter] public EventCallback OnSettingsClick { get; set; }
    [Parameter] public bool? ShowCaret { get; set; }
    [Parameter] public bool ShowThemeSwitcher { get; set; } = true;
    [Parameter] public string ThemeSectionTitle { get; set; } = "Theme";
    [Parameter] public bool ShowAccountLink { get; set; } = true;
    [Parameter] public string? AccountHref { get; set; }
    [Parameter] public EventCallback OnAccountClick { get; set; }
    [Parameter] public string AccountText { get; set; } = "Account settings";
    [Parameter] public EventCallback OnLogin { get; set; }
    [Parameter] public EventCallback OnLogout { get; set; }
    [Parameter] public string LoginText { get; set; } = "Log in";
    [Parameter] public string LogoutText { get; set; } = "Log out";
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public FaAlign MenuAlign { get; set; } = FaAlign.End;
    [Parameter] public string? CssClass { get; set; }
    [Parameter] public string? TriggerCssClass { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<FaAvatarForm>(0);
        builder.AddComponentParameter(1, nameof(FaAvatarForm.IsAuthenticated), IsAuthenticated);
        builder.AddComponentParameter(2, nameof(FaAvatarForm.DisplayName), DisplayName);
        builder.AddComponentParameter(3, nameof(FaAvatarForm.ImageUrl), ImageUrl);
        builder.AddComponentParameter(4, nameof(FaAvatarForm.Email), Email);
        builder.AddComponentParameter(5, nameof(FaAvatarForm.Subtitle), Subtitle);
        builder.AddComponentParameter(6, nameof(FaAvatarForm.ShowDisplayName), ShowDisplayName);
        builder.AddComponentParameter(7, nameof(FaAvatarForm.RoundAvatar), RoundAvatar);
        builder.AddComponentParameter(8, nameof(FaAvatarForm.ShowBackdrop), ShowBackdrop);
        builder.AddComponentParameter(9, nameof(FaAvatarForm.SettingsHref), SettingsHref);
        builder.AddComponentParameter(10, nameof(FaAvatarForm.SettingsText), SettingsText);
        builder.AddComponentParameter(11, nameof(FaAvatarForm.ShowSettingsLink), ShowSettingsLink);
        builder.AddComponentParameter(12, nameof(FaAvatarForm.OnSettingsClick), OnSettingsClick);
        builder.AddComponentParameter(13, nameof(FaAvatarForm.ShowCaret), ShowCaret);
        builder.AddComponentParameter(14, nameof(FaAvatarForm.ShowThemeSwitcher), ShowThemeSwitcher);
        builder.AddComponentParameter(15, nameof(FaAvatarForm.ThemeSectionTitle), ThemeSectionTitle);
        builder.AddComponentParameter(16, nameof(FaAvatarForm.ShowAccountLink), ShowAccountLink);
        builder.AddComponentParameter(17, nameof(FaAvatarForm.AccountHref), AccountHref);
        builder.AddComponentParameter(18, nameof(FaAvatarForm.OnAccountClick), OnAccountClick);
        builder.AddComponentParameter(19, nameof(FaAvatarForm.AccountText), AccountText);
        builder.AddComponentParameter(20, nameof(FaAvatarForm.OnLogin), OnLogin);
        builder.AddComponentParameter(21, nameof(FaAvatarForm.OnLogout), OnLogout);
        builder.AddComponentParameter(22, nameof(FaAvatarForm.LoginText), LoginText);
        builder.AddComponentParameter(23, nameof(FaAvatarForm.LogoutText), LogoutText);
        builder.AddComponentParameter(24, nameof(FaAvatarForm.ChildContent), ChildContent);
        builder.AddComponentParameter(25, nameof(FaAvatarForm.MenuAlign), MenuAlign);
        builder.AddComponentParameter(26, nameof(FaAvatarForm.CssClass), CssClass);
        builder.AddComponentParameter(27, nameof(FaAvatarForm.TriggerCssClass), TriggerCssClass);
        builder.CloseComponent();
    }
}
