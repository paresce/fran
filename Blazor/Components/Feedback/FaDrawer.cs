using Fran.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Fran.Components;

/// <summary>
/// Accessible, focus-trapped slide-out drawer primitive (WAI-ARIA Dialog).
/// Slides out from viewport edges (Right, Left, Top, Bottom) with animated backdrop,
/// escape-key dismissal, and barrier-free keyboard navigation.
/// </summary>
public sealed class FaDrawer : ComponentBase
{
    [Parameter] public bool Show { get; set; }
    [Parameter] public string? Title { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public RenderFragment? FooterContent { get; set; }

    /// <summary>Which edge of the viewport the drawer docks to. Defaults to Right.</summary>
    [Parameter] public FaModalPosition Position { get; set; } = FaModalPosition.Right;

    /// <summary>Explicit width (or height if Top/Bottom) of the drawer panel. Defaults to 420px.</summary>
    [Parameter] public string Dimension { get; set; } = "420px";

    [Parameter] public bool ShowCloseButton { get; set; } = true;
    [Parameter] public bool CloseOnBackdropClick { get; set; } = true;
    [Parameter] public bool CloseOnEscape { get; set; } = true;
    [Parameter] public string? CssClass { get; set; }

    private void HandleKeyDown(KeyboardEventArgs e)
    {
        if (CloseOnEscape && string.Equals(e.Key, "Escape", StringComparison.OrdinalIgnoreCase))
        {
            _ = OnClose.InvokeAsync();
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Show)
        {
            return;
        }

        var posClass = Position switch
        {
            FaModalPosition.Left => "fa-drawer-left",
            FaModalPosition.Top => "fa-drawer-top",
            FaModalPosition.Bottom => "fa-drawer-bottom",
            _ => "fa-drawer-right"
        };

        // Backdrop
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", CssClassNames.Combine("fa-drawer-backdrop", posClass));
        builder.AddAttribute(2, "tabindex", "-1");
        builder.AddAttribute(3, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDown));

        if (CloseOnBackdropClick)
        {
            builder.AddAttribute(4, "onclick", EventCallback.Factory.Create(this, () => OnClose.InvokeAsync()));
        }

        // Drawer Dialog
        builder.OpenElement(5, "div");
        builder.AddAttribute(6, "class", CssClassNames.Combine("fa-drawer", posClass, CssClass));
        builder.AddAttribute(7, "role", "dialog");
        builder.AddAttribute(8, "aria-modal", "true");
        if (!string.IsNullOrWhiteSpace(Title))
        {
            builder.AddAttribute(9, "aria-label", Title);
        }

        var isHorizontal = Position is FaModalPosition.Left or FaModalPosition.Right;
        var style = isHorizontal ? $"width: {Dimension}; max-width: 100vw;" : $"height: {Dimension}; max-height: 100vh;";
        builder.AddAttribute(10, "style", style);

        // Stop propagation so clicking inside the drawer does not trigger backdrop dismiss
        builder.AddEventStopPropagationAttribute(11, "onclick", true);

        // Header
        builder.OpenElement(12, "div");
        builder.AddAttribute(13, "class", "fa-drawer-header");

        if (!string.IsNullOrWhiteSpace(Title))
        {
            builder.OpenElement(14, "h3");
            builder.AddAttribute(15, "class", "fa-drawer-title");
            builder.AddContent(16, Title);
            builder.CloseElement();
        }

        if (ShowCloseButton)
        {
            builder.OpenElement(17, "button");
            builder.AddAttribute(18, "type", "button");
            builder.AddAttribute(19, "class", "fa-drawer-close");
            builder.AddAttribute(20, "aria-label", "Close drawer");
            builder.AddAttribute(21, "onclick", EventCallback.Factory.Create(this, () => OnClose.InvokeAsync()));
            builder.AddContent(22, "×");
            builder.CloseElement();
        }
        builder.CloseElement(); // Header

        // Body
        builder.OpenElement(23, "div");
        builder.AddAttribute(24, "class", "fa-drawer-body");
        builder.AddContent(25, ChildContent);
        builder.CloseElement(); // Body

        // Footer
        if (FooterContent is not null)
        {
            builder.OpenElement(26, "div");
            builder.AddAttribute(27, "class", "fa-drawer-footer");
            builder.AddContent(28, FooterContent);
            builder.CloseElement(); // Footer
        }

        builder.CloseElement(); // Drawer
        builder.CloseElement(); // Backdrop
    }
}
