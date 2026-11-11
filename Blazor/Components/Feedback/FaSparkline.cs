using System.Globalization;
using Fran.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Fran.Components;

public enum FaSparklineType
{
    Line,
    Area,
    Bar
}

/// <summary>
/// A lightweight, accessible inline SVG sparkline component for visualizing compact trends,
/// financial yields, server telemetry, and athlete velocity profiles.
/// </summary>
public sealed class FaSparkline : ComponentBase
{
    [Parameter] public IEnumerable<double>? Data { get; set; }
    [Parameter] public double Width { get; set; } = 120.0;
    [Parameter] public double Height { get; set; } = 32.0;
    [Parameter] public double StrokeWidth { get; set; } = 2.0;
    [Parameter] public string? StrokeColor { get; set; }
    [Parameter] public bool Fill { get; set; } = true;
    [Parameter] public bool ShowMinMax { get; set; } = true;
    [Parameter] public bool ShowLastPoint { get; set; } = true;
    [Parameter] public FaSparklineType Type { get; set; } = FaSparklineType.Area;
    [Parameter] public string? Label { get; set; }
    [Parameter] public string? CssClass { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private readonly string _gradientId = $"fa-sparkline-grad-{Guid.NewGuid():N}";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var rawData = Data?.ToList() ?? new List<double>();
        var ariaLabel = Label ?? (rawData.Count > 0
            ? $"Sparkline trend with {rawData.Count} data points from {rawData.Min():F2} to {rawData.Max():F2}"
            : "Sparkline trend without data");

        builder.OpenElement(0, "svg");
        builder.AddAttribute(1, "class", CssClassNames.Combine("fa-sparkline", $"fa-sparkline-{Type.ToString().ToLowerInvariant()}", CssClass));
        builder.AddAttribute(2, "viewBox", $"0 0 {Width.ToString("F1", CultureInfo.InvariantCulture)} {Height.ToString("F1", CultureInfo.InvariantCulture)}");
        builder.AddAttribute(3, "width", Width.ToString("F0", CultureInfo.InvariantCulture));
        builder.AddAttribute(4, "height", Height.ToString("F0", CultureInfo.InvariantCulture));
        builder.AddAttribute(5, "role", "img");
        builder.AddAttribute(6, "aria-label", ariaLabel);
        builder.AddMultipleAttributes(7, AdditionalAttributes);

        if (rawData.Count == 0)
        {
            // Empty placeholder line
            builder.OpenElement(8, "line");
            builder.AddAttribute(9, "x1", "0");
            builder.AddAttribute(10, "y1", (Height / 2.0).ToString("F1", CultureInfo.InvariantCulture));
            builder.AddAttribute(11, "x2", Width.ToString("F1", CultureInfo.InvariantCulture));
            builder.AddAttribute(12, "y2", (Height / 2.0).ToString("F1", CultureInfo.InvariantCulture));
            builder.AddAttribute(13, "class", "fa-sparkline-empty");
            builder.AddAttribute(14, "stroke-dasharray", "3 3");
            builder.CloseElement();
            builder.CloseElement(); // svg
            return;
        }

        var padX = Math.Max(2.0, StrokeWidth + 1.0);
        var padY = Math.Max(3.0, StrokeWidth + 2.0);
        var innerWidth = Math.Max(1.0, Width - (2 * padX));
        var innerHeight = Math.Max(1.0, Height - (2 * padY));

        var minVal = rawData.Min();
        var maxVal = rawData.Max();
        var range = Math.Max(0.0001, maxVal - minVal);

        if (Type == FaSparklineType.Bar)
        {
            var barCount = rawData.Count;
            var slotWidth = innerWidth / barCount;
            var barWidth = Math.Max(1.5, slotWidth * 0.75);

            for (var i = 0; i < barCount; i++)
            {
                var val = rawData[i];
                var normY = (val - minVal) / range;
                var barHeight = Math.Max(2.0, normY * innerHeight);
                var x = padX + (i * slotWidth) + ((slotWidth - barWidth) / 2.0);
                var y = padY + innerHeight - barHeight;

                builder.OpenElement(15, "rect");
                builder.AddAttribute(16, "class", "fa-sparkline-bar");
                builder.AddAttribute(17, "x", x.ToString("F2", CultureInfo.InvariantCulture));
                builder.AddAttribute(18, "y", y.ToString("F2", CultureInfo.InvariantCulture));
                builder.AddAttribute(19, "width", barWidth.ToString("F2", CultureInfo.InvariantCulture));
                builder.AddAttribute(20, "height", barHeight.ToString("F2", CultureInfo.InvariantCulture));
                builder.AddAttribute(21, "rx", "1");
                if (!string.IsNullOrEmpty(StrokeColor))
                {
                    builder.AddAttribute(22, "fill", StrokeColor);
                }
                builder.CloseElement();
            }

            builder.CloseElement(); // svg
            return;
        }

        // Line and Area coordinates
        var points = new List<(double X, double Y, double Val)>();
        if (rawData.Count == 1)
        {
            var midY = padY + (innerHeight / 2.0);
            points.Add((padX, midY, rawData[0]));
            points.Add((padX + innerWidth, midY, rawData[0]));
        }
        else
        {
            for (var i = 0; i < rawData.Count; i++)
            {
                var val = rawData[i];
                var x = padX + (i * innerWidth / (rawData.Count - 1));
                var normY = (val - minVal) / range;
                var y = padY + innerHeight - (normY * innerHeight);
                points.Add((x, y, val));
            }
        }

        var pathData = $"M {points[0].X.ToString("F2", CultureInfo.InvariantCulture)} {points[0].Y.ToString("F2", CultureInfo.InvariantCulture)}"
            + string.Concat(points.Skip(1).Select(p => $" L {p.X.ToString("F2", CultureInfo.InvariantCulture)} {p.Y.ToString("F2", CultureInfo.InvariantCulture)}"));

        // Area Gradient & Fill
        if ((Type == FaSparklineType.Area || Fill) && points.Count > 1)
        {
            builder.OpenElement(23, "defs");
            builder.OpenElement(24, "linearGradient");
            builder.AddAttribute(25, "id", _gradientId);
            builder.AddAttribute(26, "x1", "0");
            builder.AddAttribute(27, "y1", "0");
            builder.AddAttribute(28, "x2", "0");
            builder.AddAttribute(29, "y2", "1");

            builder.OpenElement(30, "stop");
            builder.AddAttribute(31, "offset", "0%");
            builder.AddAttribute(32, "stop-color", !string.IsNullOrEmpty(StrokeColor) ? StrokeColor : "currentColor");
            builder.AddAttribute(33, "stop-opacity", "0.35");
            builder.CloseElement();

            builder.OpenElement(34, "stop");
            builder.AddAttribute(35, "offset", "100%");
            builder.AddAttribute(36, "stop-color", !string.IsNullOrEmpty(StrokeColor) ? StrokeColor : "currentColor");
            builder.AddAttribute(37, "stop-opacity", "0.02");
            builder.CloseElement();

            builder.CloseElement(); // linearGradient
            builder.CloseElement(); // defs

            var bottomY = (Height - padY).ToString("F2", CultureInfo.InvariantCulture);
            var areaData = $"{pathData} L {points.Last().X.ToString("F2", CultureInfo.InvariantCulture)} {bottomY} L {points.First().X.ToString("F2", CultureInfo.InvariantCulture)} {bottomY} Z";

            builder.OpenElement(38, "path");
            builder.AddAttribute(39, "class", "fa-sparkline-area");
            builder.AddAttribute(40, "d", areaData);
            builder.AddAttribute(41, "fill", $"url(#{_gradientId})");
            builder.CloseElement();
        }

        // Line Path
        builder.OpenElement(42, "path");
        builder.AddAttribute(43, "class", "fa-sparkline-line");
        builder.AddAttribute(44, "d", pathData);
        builder.AddAttribute(45, "fill", "none");
        builder.AddAttribute(46, "stroke-width", StrokeWidth.ToString("F1", CultureInfo.InvariantCulture));
        if (!string.IsNullOrEmpty(StrokeColor))
        {
            builder.AddAttribute(47, "stroke", StrokeColor);
        }
        builder.CloseElement();

        // Highlighting dots
        if (ShowMinMax && points.Count > 1)
        {
            var minPoint = points.OrderBy(p => p.Val).First();
            var maxPoint = points.OrderByDescending(p => p.Val).First();

            // Min Point
            builder.OpenElement(48, "circle");
            builder.AddAttribute(49, "class", "fa-sparkline-point fa-sparkline-point-min");
            builder.AddAttribute(50, "cx", minPoint.X.ToString("F2", CultureInfo.InvariantCulture));
            builder.AddAttribute(51, "cy", minPoint.Y.ToString("F2", CultureInfo.InvariantCulture));
            builder.AddAttribute(52, "r", (StrokeWidth * 1.3).ToString("F1", CultureInfo.InvariantCulture));
            builder.CloseElement();

            // Max Point
            builder.OpenElement(53, "circle");
            builder.AddAttribute(54, "class", "fa-sparkline-point fa-sparkline-point-max");
            builder.AddAttribute(55, "cx", maxPoint.X.ToString("F2", CultureInfo.InvariantCulture));
            builder.AddAttribute(56, "cy", maxPoint.Y.ToString("F2", CultureInfo.InvariantCulture));
            builder.AddAttribute(57, "r", (StrokeWidth * 1.3).ToString("F1", CultureInfo.InvariantCulture));
            builder.CloseElement();
        }

        if (ShowLastPoint && points.Count > 0)
        {
            var last = points.Last();
            builder.OpenElement(58, "circle");
            builder.AddAttribute(59, "class", "fa-sparkline-point fa-sparkline-point-last");
            builder.AddAttribute(60, "cx", last.X.ToString("F2", CultureInfo.InvariantCulture));
            builder.AddAttribute(61, "cy", last.Y.ToString("F2", CultureInfo.InvariantCulture));
            builder.AddAttribute(62, "r", (StrokeWidth * 1.5).ToString("F1", CultureInfo.InvariantCulture));
            if (!string.IsNullOrEmpty(StrokeColor))
            {
                builder.AddAttribute(63, "fill", StrokeColor);
            }
            builder.CloseElement();
        }

        builder.CloseElement(); // svg
    }
}
