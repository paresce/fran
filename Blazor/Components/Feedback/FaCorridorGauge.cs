using System.Globalization;
using Fran.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Fran.Components;

public enum FaCorridorStatus
{
    SubOptimal,
    Optimal,
    Excessive
}

/// <summary>
/// A telemetry corridor gauge visualizing a metric against target operational boundaries
/// (e.g. Guyton-Klinke withdrawal corridors, Strouhal propulsion windows, or memory thresholds).
/// </summary>
public sealed class FaCorridorGauge : ComponentBase
{
    [Parameter] public string? Label { get; set; }
    [Parameter] public double Value { get; set; }
    [Parameter] public double Min { get; set; } = 0.0;
    [Parameter] public double Max { get; set; } = 1.0;
    [Parameter] public double OptimalLower { get; set; } = 0.3;
    [Parameter] public double OptimalUpper { get; set; } = 0.7;
    [Parameter] public string? Unit { get; set; }
    [Parameter] public bool ShowValueBadge { get; set; } = true;
    [Parameter] public string? CssClass { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    public FaCorridorStatus Status
    {
        get
        {
            if (Value < OptimalLower) return FaCorridorStatus.SubOptimal;
            if (Value > OptimalUpper) return FaCorridorStatus.Excessive;
            return FaCorridorStatus.Optimal;
        }
    }

    private double Range => Math.Max(0.0001, Max - Min);
    private double MarkerPercent => Math.Clamp((Value - Min) / Range * 100.0, 0.0, 100.0);
    private double OptimalLowerPercent => Math.Clamp((OptimalLower - Min) / Range * 100.0, 0.0, 100.0);
    private double OptimalUpperPercent => Math.Clamp((OptimalUpper - Min) / Range * 100.0, OptimalLowerPercent, 100.0);
    private double SubOptimalWidth => OptimalLowerPercent;
    private double OptimalWidth => OptimalUpperPercent - OptimalLowerPercent;
    private double ExcessiveWidth => Math.Max(0.0, 100.0 - OptimalUpperPercent);

    private string BadgeClass => Status switch
    {
        FaCorridorStatus.Optimal => "fa-corridor-badge-optimal",
        FaCorridorStatus.SubOptimal => "fa-corridor-badge-warning",
        FaCorridorStatus.Excessive => "fa-corridor-badge-alert",
        _ => "fa-corridor-badge-optimal"
    };

    private string StatusText => Status switch
    {
        FaCorridorStatus.Optimal => "Optimal",
        FaCorridorStatus.SubOptimal => "Below Floor",
        FaCorridorStatus.Excessive => "Above Ceiling",
        _ => "Optimal"
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", CssClassNames.Combine("fa-corridor-gauge", CssClass));
        builder.AddAttribute(2, "role", "meter");
        builder.AddAttribute(3, "aria-valuenow", Value);
        builder.AddAttribute(4, "aria-valuemin", Min);
        builder.AddAttribute(5, "aria-valuemax", Max);
        builder.AddMultipleAttributes(6, AdditionalAttributes);

        // Header with label and status badge
        if (!string.IsNullOrEmpty(Label) || ShowValueBadge)
        {
            builder.OpenElement(7, "div");
            builder.AddAttribute(8, "class", "fa-corridor-header");

            if (!string.IsNullOrEmpty(Label))
            {
                builder.OpenElement(9, "span");
                builder.AddAttribute(10, "class", "fa-corridor-label");
                builder.AddContent(11, Label);
                builder.CloseElement();
            }

            if (ShowValueBadge)
            {
                builder.OpenElement(12, "span");
                builder.AddAttribute(13, "class", CssClassNames.Combine("fa-corridor-badge", BadgeClass));
                var unitText = !string.IsNullOrEmpty(Unit) ? $" {Unit}" : "";
                builder.AddContent(14, $"{Value.ToString("F2", CultureInfo.InvariantCulture)}{unitText} ({StatusText})");
                builder.CloseElement();
            }

            builder.CloseElement(); // fa-corridor-header
        }

        // Track with 3 corridor zones and value marker
        builder.OpenElement(15, "div");
        builder.AddAttribute(16, "class", "fa-corridor-track");

        // Sub-optimal corridor
        builder.OpenElement(17, "div");
        builder.AddAttribute(18, "class", "fa-corridor-segment fa-corridor-sub-optimal");
        builder.AddAttribute(19, "style", $"width:{SubOptimalWidth.ToString("F1", CultureInfo.InvariantCulture)}%");
        builder.CloseElement();

        // Optimal corridor
        builder.OpenElement(20, "div");
        builder.AddAttribute(21, "class", "fa-corridor-segment fa-corridor-optimal");
        builder.AddAttribute(22, "style", $"width:{OptimalWidth.ToString("F1", CultureInfo.InvariantCulture)}%");
        builder.CloseElement();

        // Excessive corridor
        builder.OpenElement(23, "div");
        builder.AddAttribute(24, "class", "fa-corridor-segment fa-corridor-excessive");
        builder.AddAttribute(25, "style", $"width:{ExcessiveWidth.ToString("F1", CultureInfo.InvariantCulture)}%");
        builder.CloseElement();

        // Value needle / pin marker
        builder.OpenElement(26, "div");
        builder.AddAttribute(27, "class", "fa-corridor-marker");
        builder.AddAttribute(28, "style", $"left:{MarkerPercent.ToString("F1", CultureInfo.InvariantCulture)}%");
        builder.CloseElement();

        builder.CloseElement(); // fa-corridor-track

        // Footer with boundary values
        builder.OpenElement(29, "div");
        builder.AddAttribute(30, "class", "fa-corridor-footer");

        builder.OpenElement(31, "span");
        builder.AddContent(32, Min.ToString("F1", CultureInfo.InvariantCulture));
        builder.CloseElement();

        builder.OpenElement(33, "span");
        builder.AddContent(34, $"Target: {OptimalLower.ToString("F2", CultureInfo.InvariantCulture)} - {OptimalUpper.ToString("F2", CultureInfo.InvariantCulture)}");
        builder.CloseElement();

        builder.OpenElement(35, "span");
        builder.AddContent(36, Max.ToString("F1", CultureInfo.InvariantCulture));
        builder.CloseElement();

        builder.CloseElement(); // fa-corridor-footer

        builder.CloseElement(); // fa-corridor-gauge
    }
}
