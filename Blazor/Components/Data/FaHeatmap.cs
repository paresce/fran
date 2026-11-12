using System.Globalization;
using Fran.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Fran.Components;

public enum FaHeatmapColorScale
{
    Green,
    Blue,
    Amber,
    Rose,
    Purple
}

public sealed record FaHeatmapCell(
    string Id,
    double Value,
    string? Label = null,
    string? Tooltip = null,
    int? ExplicitLevel = null);

/// <summary>
/// A responsive, high-density activity and telemetry intensity matrix (GitHub-style calendar grid,
/// server core load matrix, disk IOPS heatmap, swim training load grid).
/// </summary>
public sealed class FaHeatmap : ComponentBase
{
    [Parameter] public IEnumerable<FaHeatmapCell>? Items { get; set; }
    [Parameter] public int Columns { get; set; } = 7;
    [Parameter] public double MaxValue { get; set; } = 100.0;
    [Parameter] public FaHeatmapColorScale ColorScale { get; set; } = FaHeatmapColorScale.Green;
    [Parameter] public int CellSizePx { get; set; } = 14;
    [Parameter] public int CellGapPx { get; set; } = 3;
    [Parameter] public bool ShowLegend { get; set; } = true;
    [Parameter] public string? Label { get; set; }
    [Parameter] public string? CssClass { get; set; }
    [Parameter] public EventCallback<FaHeatmapCell> OnCellClick { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private int ComputeLevel(FaHeatmapCell cell)
    {
        if (cell.ExplicitLevel.HasValue)
        {
            return Math.Clamp(cell.ExplicitLevel.Value, 0, 4);
        }

        if (cell.Value <= 0.0) return 0;
        var ratio = cell.Value / Math.Max(0.0001, MaxValue);
        if (ratio <= 0.25) return 1;
        if (ratio <= 0.50) return 2;
        if (ratio <= 0.75) return 3;
        return 4;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var cells = Items?.ToList() ?? new List<FaHeatmapCell>();
        var scaleClass = $"fa-heatmap-{ColorScale.ToString().ToLowerInvariant()}";
        var ariaLabel = Label ?? $"Activity Heatmap with {cells.Count} data points";

        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", CssClassNames.Combine("fa-heatmap-wrapper", CssClass));
        builder.AddMultipleAttributes(2, AdditionalAttributes);

        if (!string.IsNullOrEmpty(Label))
        {
            builder.OpenElement(3, "div");
            builder.AddAttribute(4, "class", "fa-heatmap-header");
            builder.OpenElement(5, "span");
            builder.AddAttribute(6, "class", "fa-heatmap-title");
            builder.AddContent(7, Label);
            builder.CloseElement();
            builder.CloseElement();
        }

        // Heatmap Grid
        builder.OpenElement(8, "div");
        builder.AddAttribute(9, "class", CssClassNames.Combine("fa-heatmap", scaleClass));
        builder.AddAttribute(10, "role", "grid");
        builder.AddAttribute(11, "aria-label", ariaLabel);
        builder.AddAttribute(12, "style", $"display:grid; grid-template-columns:repeat({Math.Max(1, Columns)}, {CellSizePx}px); gap:{CellGapPx}px;");

        for (int i = 0; i < cells.Count; i++)
        {
            var cell = cells[i];
            var level = ComputeLevel(cell);
            var title = cell.Tooltip ?? (!string.IsNullOrEmpty(cell.Label)
                ? $"{cell.Label}: {cell.Value.ToString("F1", CultureInfo.InvariantCulture)}"
                : $"Value: {cell.Value.ToString("F1", CultureInfo.InvariantCulture)} (Level {level})");

            builder.OpenElement(13, "button");
            builder.AddAttribute(14, "type", "button");
            builder.AddAttribute(15, "class", $"fa-heatmap-cell fa-heatmap-level-{level}");
            builder.AddAttribute(16, "role", "gridcell");
            builder.AddAttribute(17, "title", title);
            builder.AddAttribute(18, "aria-label", title);
            builder.AddAttribute(19, "style", $"width:{CellSizePx}px; height:{CellSizePx}px;");

            if (OnCellClick.HasDelegate)
            {
                builder.AddAttribute(20, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => OnCellClick.InvokeAsync(cell)));
            }

            builder.CloseElement(); // button
        }

        builder.CloseElement(); // fa-heatmap

        // Legend
        if (ShowLegend)
        {
            builder.OpenElement(21, "div");
            builder.AddAttribute(22, "class", "fa-heatmap-legend");

            builder.OpenElement(23, "span");
            builder.AddAttribute(24, "class", "fa-heatmap-legend-label");
            builder.AddContent(25, "Less");
            builder.CloseElement();

            builder.OpenElement(26, "div");
            builder.AddAttribute(27, "class", CssClassNames.Combine("fa-heatmap-legend-cells", scaleClass));
            for (int l = 0; l <= 4; l++)
            {
                builder.OpenElement(28, "span");
                builder.AddAttribute(29, "class", $"fa-heatmap-cell fa-heatmap-level-{l}");
                builder.AddAttribute(30, "style", $"width:{Math.Max(10, CellSizePx - 2)}px; height:{Math.Max(10, CellSizePx - 2)}px;");
                builder.CloseElement();
            }
            builder.CloseElement(); // fa-heatmap-legend-cells

            builder.OpenElement(31, "span");
            builder.AddAttribute(32, "class", "fa-heatmap-legend-label");
            builder.AddContent(33, "More");
            builder.CloseElement();

            builder.CloseElement(); // fa-heatmap-legend
        }

        builder.CloseElement(); // fa-heatmap-wrapper
    }
}
