using Fran.Icons;
using Fran.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Fran.Components;

/// <summary>
/// An avatar-triggered profile and session menu for app headers and topbars.
/// In its collapsed resting state, it renders a standard round avatar button (or optionally
/// accompanied by the user's name). When selected, on desktop it opens a contextual modal
/// anchored relative to the avatar button; on mobile, it docks at the bottom of the page
/// as a touch-friendly bottom sheet. Inside the modal is the user's identity, an extensive
/// settings link (for when settings exceed the modal's micro-interaction boundary), custom
/// application-specific items, a Light/Dark/Colorblind theme switcher, and a Login or Logout action.
/// </summary>
public sealed class FaAvatarForm : ComponentBase
{
    private readonly string _id = $"fa-avatar-form-{Guid.NewGuid():N}";
    private bool _isOpen;
    private CancellationTokenSource? _pendingClose;

    [Parameter] public bool IsAuthenticated { get; set; }
    [Parameter] public string? DisplayName { get; set; }
    [Parameter] public string? ImageUrl { get; set; }
    [Parameter] public string? Email { get; set; }
    [Parameter] public string? Subtitle { get; set; }

    /// <summary>
    /// Whether to show the user's display name alongside the avatar in the resting topbar trigger.
    /// Defaults to false (only the avatar is displayed in the topbar).
    /// </summary>
    [Parameter] public bool ShowDisplayName { get; set; }

    /// <summary>
    /// Whether the resting trigger is a standalone round avatar button.
    /// Defaults to true when <see cref="ShowDisplayName"/> is false.
    /// </summary>
    [Parameter] public bool RoundAvatar { get; set; } = true;

    /// <summary>
    /// Whether to render an interactive backdrop overlay when the modal/sheet is open.
    /// Clicking the backdrop dismisses the menu. Defaults to true.
    /// </summary>
    [Parameter] public bool ShowBackdrop { get; set; } = true;

    /// <summary>
    /// Optional URL navigating to an extensive/full settings page when settings
    /// exceed what fits in this micro-interaction modal.
    /// </summary>
    [Parameter] public string? SettingsHref { get; set; }

    /// <summary>Label for the settings link/button. Defaults to "All settings".</summary>
    [Parameter] public string SettingsText { get; set; } = "All settings";

    /// <summary>Whether to show the settings link when configured. Defaults to true.</summary>
    [Parameter] public bool ShowSettingsLink { get; set; } = true;

    /// <summary>Callback invoked when the settings link/button is clicked.</summary>
    [Parameter] public EventCallback OnSettingsClick { get; set; }

    /// <summary>
    /// Whether to render a subtle dropdown caret in the trigger.
    /// Defaults to null, which renders a caret only when <see cref="ShowDisplayName"/> is true.
    /// </summary>
    [Parameter] public bool? ShowCaret { get; set; }

    /// <summary>
    /// Whether to include the Light / Dark / Colorblind theme switcher section.
    /// Defaults to true.
    /// </summary>
    [Parameter] public bool ShowThemeSwitcher { get; set; } = true;

    /// <summary>Heading text above the theme switcher. Defaults to "Theme".</summary>
    [Parameter] public string ThemeSectionTitle { get; set; } = "Theme";

    /// <summary>
    /// Whether to show the account settings link/action when authenticated. Defaults to true.
    /// Only renders when <see cref="AccountHref"/> is set or <see cref="OnAccountClick"/> has a delegate.
    /// </summary>
    [Parameter] public bool ShowAccountLink { get; set; } = true;

    /// <summary>URL navigating to the user's account/profile form.</summary>
    [Parameter] public string? AccountHref { get; set; }

    /// <summary>Callback invoked when the account link/button is clicked.</summary>
    [Parameter] public EventCallback OnAccountClick { get; set; }

    /// <summary>Label for the account link/button. Defaults to "Account settings".</summary>
    [Parameter] public string AccountText { get; set; } = "Account settings";

    /// <summary>Callback invoked when the user clicks the Log in action.</summary>
    [Parameter] public EventCallback OnLogin { get; set; }

    /// <summary>Callback invoked when the user clicks the Log out action.</summary>
    [Parameter] public EventCallback OnLogout { get; set; }

    /// <summary>Label for the Log in button. Defaults to "Log in".</summary>
    [Parameter] public string LoginText { get; set; } = "Log in";

    /// <summary>Label for the Log out button. Defaults to "Log out".</summary>
    [Parameter] public string LogoutText { get; set; } = "Log out";

    /// <summary>
    /// Application-specific content, user information items, or custom links rendered
    /// inside the opened avatar form.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Alignment of the dropdown panel relative to the trigger. Defaults to <see cref="FaAlign.End"/>.</summary>
    [Parameter] public FaAlign MenuAlign { get; set; } = FaAlign.End;

    [Parameter] public string? CssClass { get; set; }
    [Parameter] public string? TriggerCssClass { get; set; }

    private bool ResolvedShowCaret => ShowCaret ?? ShowDisplayName;

    private string AlignClass => MenuAlign switch
    {
        FaAlign.Start => "fa-avatar-form-panel-align-start",
        _ => "fa-avatar-form-panel-align-end"
    };

    private void Toggle()
    {
        CancelPendingClose();
        _isOpen = !_isOpen;
    }

    private void Close()
    {
        CancelPendingClose();
        _isOpen = false;
    }

    private void HandleFocusIn() => CancelPendingClose();

    private async Task HandleFocusOutAsync()
    {
        CancelPendingClose();
        _pendingClose = new CancellationTokenSource();
        var token = _pendingClose.Token;
        try
        {
            await Task.Delay(150, token);
            _isOpen = false;
            StateHasChanged();
        }
        catch (TaskCanceledException)
        {
            // Focus stayed inside or moved to trigger/panel
        }
    }

    private void CancelPendingClose()
    {
        _pendingClose?.Cancel();
        _pendingClose = null;
    }

    private void HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Escape")
        {
            _isOpen = false;
        }
    }

    private async Task HandleAccountClickAsync()
    {
        _isOpen = false;
        if (OnAccountClick.HasDelegate)
        {
            await OnAccountClick.InvokeAsync();
        }
    }

    private async Task HandleSettingsClickAsync()
    {
        _isOpen = false;
        if (OnSettingsClick.HasDelegate)
        {
            await OnSettingsClick.InvokeAsync();
        }
    }

    private async Task HandleLoginAsync()
    {
        _isOpen = false;
        await OnLogin.InvokeAsync();
    }

    private async Task HandleLogoutAsync()
    {
        _isOpen = false;
        await OnLogout.InvokeAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var seq = 0;

        // Interactive backdrop overlay
        if (_isOpen && ShowBackdrop)
        {
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "fa-avatar-form-backdrop");
            builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create(this, Close));
            builder.AddAttribute(seq++, "aria-hidden", "true");
            builder.CloseElement();
        }

        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "id", _id);
        builder.AddAttribute(seq++, "class", CssClassNames.Combine("fa-avatar-form", CssClass));
        builder.AddAttribute(seq++, "onfocusin", EventCallback.Factory.Create(this, HandleFocusIn));
        builder.AddAttribute(seq++, "onfocusout", EventCallback.Factory.Create(this, HandleFocusOutAsync));
        builder.AddAttribute(seq++, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDown));

        // Trigger button (round avatar button by default when name is not displayed)
        var isRoundTrigger = RoundAvatar && !ShowDisplayName;
        var triggerClass = CssClassNames.Combine(
            "fa-avatar-form-trigger",
            isRoundTrigger ? "fa-avatar-form-trigger-round" : null,
            TriggerCssClass);

        builder.OpenElement(seq++, "button");
        builder.AddAttribute(seq++, "type", "button");
        builder.AddAttribute(seq++, "class", triggerClass);
        builder.AddAttribute(seq++, "aria-haspopup", "dialog");
        builder.AddAttribute(seq++, "aria-expanded", _isOpen ? "true" : "false");
        builder.AddAttribute(seq++, "aria-label", IsAuthenticated ? (!string.IsNullOrWhiteSpace(DisplayName) ? $"User menu for {DisplayName}" : "User menu") : "Account menu");
        builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create(this, Toggle));

        builder.OpenComponent<FaAvatar>(seq++);
        builder.AddComponentParameter(seq++, nameof(FaAvatar.DisplayName), IsAuthenticated ? DisplayName : "Guest");
        builder.AddComponentParameter(seq++, nameof(FaAvatar.ImageUrl), IsAuthenticated ? ImageUrl : null);
        builder.CloseComponent();

        if (ShowDisplayName && IsAuthenticated && !string.IsNullOrWhiteSpace(DisplayName))
        {
            builder.OpenElement(seq++, "span");
            builder.AddAttribute(seq++, "class", "fa-avatar-form-trigger-name");
            builder.AddContent(seq++, DisplayName);
            builder.CloseElement();
        }

        if (ResolvedShowCaret)
        {
            builder.OpenElement(seq++, "span");
            builder.AddAttribute(seq++, "class", "fa-avatar-form-caret");
            builder.OpenComponent<FaIcon>(seq++);
            builder.AddComponentParameter(seq++, nameof(FaIcon.Name), FaIconName.ChevronDown);
            builder.AddComponentParameter(seq++, nameof(FaIcon.Size), 14);
            builder.CloseComponent();
            builder.CloseElement();
        }

        builder.CloseElement(); // button.fa-avatar-form-trigger

        // Modal Panel (Relative on Desktop, Bottom Sheet on Mobile)
        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "class", CssClassNames.Combine("fa-avatar-form-panel", AlignClass, _isOpen ? "fa-avatar-form-panel-open" : null));
        builder.AddAttribute(seq++, "role", "dialog");
        builder.AddAttribute(seq++, "aria-label", "User profile and settings");

        // Mobile Drag/Grab Handle
        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "class", "fa-avatar-form-drag-handle");
        builder.AddAttribute(seq++, "aria-hidden", "true");
        builder.CloseElement();

        // Identity Header
        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "class", "fa-avatar-form-header");

        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "class", "fa-avatar-form-identity");

        builder.OpenComponent<FaAvatar>(seq++);
        builder.AddComponentParameter(seq++, nameof(FaAvatar.DisplayName), IsAuthenticated ? DisplayName : "Guest");
        builder.AddComponentParameter(seq++, nameof(FaAvatar.ImageUrl), IsAuthenticated ? ImageUrl : null);
        builder.AddComponentParameter(seq++, nameof(FaAvatar.CssClass), "fa-avatar-form-avatar-lg");
        builder.CloseComponent();

        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "class", "fa-avatar-form-details");

        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "class", "fa-avatar-form-display-name");
        builder.AddContent(seq++, IsAuthenticated ? (!string.IsNullOrWhiteSpace(DisplayName) ? DisplayName : "Authenticated User") : "Guest");
        builder.CloseElement();

        var detailText = Email ?? Subtitle;
        if (IsAuthenticated && !string.IsNullOrWhiteSpace(detailText))
        {
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "fa-avatar-form-subtitle");
            builder.AddContent(seq++, detailText);
            builder.CloseElement();
        }
        else if (!IsAuthenticated)
        {
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "fa-avatar-form-subtitle");
            builder.AddContent(seq++, "Not signed in");
            builder.CloseElement();
        }

        builder.CloseElement(); // .fa-avatar-form-details
        builder.CloseElement(); // .fa-avatar-form-identity

        // Account Link / Action
        if (IsAuthenticated && ShowAccountLink && (!string.IsNullOrEmpty(AccountHref) || OnAccountClick.HasDelegate))
        {
            if (!string.IsNullOrEmpty(AccountHref))
            {
                builder.OpenElement(seq++, "a");
                builder.AddAttribute(seq++, "href", AccountHref);
                builder.AddAttribute(seq++, "class", "fa-avatar-form-account-action");
                builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create(this, Close));

                builder.OpenComponent<FaIcon>(seq++);
                builder.AddComponentParameter(seq++, nameof(FaIcon.Name), FaIconName.Settings);
                builder.AddComponentParameter(seq++, nameof(FaIcon.Size), 14);
                builder.CloseComponent();

                builder.OpenElement(seq++, "span");
                builder.AddContent(seq++, AccountText);
                builder.CloseElement();

                builder.CloseElement(); // a.fa-avatar-form-account-action
            }
            else
            {
                builder.OpenElement(seq++, "button");
                builder.AddAttribute(seq++, "type", "button");
                builder.AddAttribute(seq++, "class", "fa-avatar-form-account-action");
                builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create(this, HandleAccountClickAsync));

                builder.OpenComponent<FaIcon>(seq++);
                builder.AddComponentParameter(seq++, nameof(FaIcon.Name), FaIconName.Settings);
                builder.AddComponentParameter(seq++, nameof(FaIcon.Size), 14);
                builder.CloseComponent();

                builder.OpenElement(seq++, "span");
                builder.AddContent(seq++, AccountText);
                builder.CloseElement();

                builder.CloseElement(); // button.fa-avatar-form-account-action
            }
        }

        builder.CloseElement(); // .fa-avatar-form-header

        // Application-specific items / Custom Content
        if (ChildContent is not null)
        {
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "fa-avatar-form-custom-section");
            builder.AddContent(seq++, ChildContent);
            builder.CloseElement();
        }

        // Settings Page Link (for when app settings exceed the micro-interaction boundary)
        if (ShowSettingsLink && (!string.IsNullOrEmpty(SettingsHref) || OnSettingsClick.HasDelegate))
        {
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "fa-avatar-form-settings-section");

            if (!string.IsNullOrEmpty(SettingsHref))
            {
                builder.OpenElement(seq++, "a");
                builder.AddAttribute(seq++, "href", SettingsHref);
                builder.AddAttribute(seq++, "class", "fa-avatar-form-item");
                builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create(this, Close));

                builder.OpenComponent<FaIcon>(seq++);
                builder.AddComponentParameter(seq++, nameof(FaIcon.Name), FaIconName.Settings);
                builder.AddComponentParameter(seq++, nameof(FaIcon.Size), 14);
                builder.CloseComponent();

                builder.OpenElement(seq++, "span");
                builder.AddContent(seq++, SettingsText);
                builder.CloseElement();

                builder.CloseElement(); // a.fa-avatar-form-item
            }
            else
            {
                builder.OpenElement(seq++, "button");
                builder.AddAttribute(seq++, "type", "button");
                builder.AddAttribute(seq++, "class", "fa-avatar-form-item");
                builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create(this, HandleSettingsClickAsync));

                builder.OpenComponent<FaIcon>(seq++);
                builder.AddComponentParameter(seq++, nameof(FaIcon.Name), FaIconName.Settings);
                builder.AddComponentParameter(seq++, nameof(FaIcon.Size), 14);
                builder.CloseComponent();

                builder.OpenElement(seq++, "span");
                builder.AddContent(seq++, SettingsText);
                builder.CloseElement();

                builder.CloseElement(); // button.fa-avatar-form-item
            }

            builder.CloseElement(); // .fa-avatar-form-settings-section
        }

        // Theme Switcher Section
        if (ShowThemeSwitcher)
        {
            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "fa-avatar-form-theme-section");

            if (!string.IsNullOrWhiteSpace(ThemeSectionTitle))
            {
                builder.OpenElement(seq++, "div");
                builder.AddAttribute(seq++, "class", "fa-avatar-form-section-title");
                builder.AddContent(seq++, ThemeSectionTitle);
                builder.CloseElement();
            }

            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "class", "fa-avatar-form-theme-group");
            builder.AddAttribute(seq++, "role", "group");
            builder.AddAttribute(seq++, "aria-label", ThemeSectionTitle);

            RenderThemeOption(builder, ref seq, "light", "Light", FaIconName.Sun);
            RenderThemeOption(builder, ref seq, "dark", "Dark", FaIconName.Moon);
            RenderThemeOption(builder, ref seq, "colorblind", "Color blind", FaIconName.Eye);

            builder.CloseElement(); // .fa-avatar-form-theme-group
            builder.CloseElement(); // .fa-avatar-form-theme-section
        }

        // Footer: Login / Logout
        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "class", "fa-avatar-form-footer");

        if (IsAuthenticated)
        {
            builder.OpenComponent<FaButton>(seq++);
            builder.AddComponentParameter(seq++, nameof(FaButton.Variant), FaButtonVariant.Secondary);
            builder.AddComponentParameter(seq++, nameof(FaButton.Size), FaSize.Small);
            builder.AddComponentParameter(seq++, nameof(FaButton.CssClass), "fa-avatar-form-btn-block");
            builder.AddComponentParameter(seq++, nameof(FaButton.OnClick), EventCallback.Factory.Create<MouseEventArgs>(this, HandleLogoutAsync));
            builder.AddComponentParameter(seq++, nameof(FaButton.ChildContent), (RenderFragment)(b =>
            {
                b.OpenComponent<FaIcon>(0);
                b.AddComponentParameter(1, nameof(FaIcon.Name), FaIconName.Logout);
                b.AddComponentParameter(2, nameof(FaIcon.Size), 14);
                b.CloseComponent();
                b.OpenElement(3, "span");
                b.AddContent(4, LogoutText);
                b.CloseElement();
            }));
            builder.CloseComponent();
        }
        else
        {
            builder.OpenComponent<FaButton>(seq++);
            builder.AddComponentParameter(seq++, nameof(FaButton.Variant), FaButtonVariant.Primary);
            builder.AddComponentParameter(seq++, nameof(FaButton.Size), FaSize.Small);
            builder.AddComponentParameter(seq++, nameof(FaButton.CssClass), "fa-avatar-form-btn-block");
            builder.AddComponentParameter(seq++, nameof(FaButton.OnClick), EventCallback.Factory.Create<MouseEventArgs>(this, HandleLoginAsync));
            builder.AddComponentParameter(seq++, nameof(FaButton.ChildContent), (RenderFragment)(b =>
            {
                b.OpenComponent<FaIcon>(0);
                b.AddComponentParameter(1, nameof(FaIcon.Name), FaIconName.Login);
                b.AddComponentParameter(2, nameof(FaIcon.Size), 14);
                b.CloseComponent();
                b.OpenElement(3, "span");
                b.AddContent(4, LoginText);
                b.CloseElement();
            }));
            builder.CloseComponent();
        }

        builder.CloseElement(); // .fa-avatar-form-footer

        builder.CloseElement(); // .fa-avatar-form-panel
        builder.CloseElement(); // .fa-avatar-form
    }

    private static void RenderThemeOption(RenderTreeBuilder builder, ref int seq, string theme, string title, FaIconName icon)
    {
        builder.OpenElement(seq++, "button");
        builder.AddAttribute(seq++, "type", "button");
        builder.AddAttribute(seq++, "class", "fa-avatar-form-theme-btn fa-theme-btn");
        builder.AddAttribute(seq++, "data-theme-btn", theme);
        builder.AddAttribute(seq++, "title", title);
        builder.AddAttribute(seq++, "aria-pressed", "false");
        builder.AddAttribute(seq++, "onclick", $"faSetTheme('{theme}')");

        builder.OpenComponent<FaIcon>(seq++);
        builder.AddComponentParameter(seq++, nameof(FaIcon.Name), icon);
        builder.AddComponentParameter(seq++, nameof(FaIcon.Size), 14);
        builder.CloseComponent();

        builder.OpenElement(seq++, "span");
        builder.AddAttribute(seq++, "class", "fa-avatar-form-theme-label");
        builder.AddContent(seq++, title);
        builder.CloseElement();

        builder.CloseElement();
    }
}
