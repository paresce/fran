using Bunit;
using Fran.Components;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Fran.Tests;

public class FaSparklineTests : BunitContext
{
    [Fact]
    public void FaSparkline_WithData_RendersSvgWithPointsAndLine()
    {
        var data = new double[] { 10.0, 25.0, 15.0, 40.0, 30.0, 50.0 };

        var cut = Render<FaSparkline>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Width, 140.0)
            .Add(p => p.Height, 36.0)
            .Add(p => p.StrokeColor, "#38bdf8")
            .Add(p => p.Label, "Stock Trend"));

        var svg = cut.Find("svg.fa-sparkline");
        Assert.NotNull(svg);
        Assert.Equal("img", svg.GetAttribute("role"));
        Assert.Equal("Stock Trend", svg.GetAttribute("aria-label"));
        Assert.Equal("140", svg.GetAttribute("width"));
        Assert.Equal("36", svg.GetAttribute("height"));

        // Verify line path exists
        var line = cut.Find("path.fa-sparkline-line");
        Assert.NotNull(line);
        Assert.StartsWith("M", line.GetAttribute("d"));
        Assert.Equal("#38bdf8", line.GetAttribute("stroke"));

        // Verify area path exists (default Type = Area)
        var area = cut.Find("path.fa-sparkline-area");
        Assert.NotNull(area);
        Assert.Contains("Z", area.GetAttribute("d"));

        // Verify min/max/last points
        var minPoint = cut.Find("circle.fa-sparkline-point-min");
        Assert.NotNull(minPoint);

        var maxPoint = cut.Find("circle.fa-sparkline-point-max");
        Assert.NotNull(maxPoint);

        var lastPoint = cut.Find("circle.fa-sparkline-point-last");
        Assert.NotNull(lastPoint);
    }

    [Fact]
    public void FaSparkline_BarType_RendersRectElements()
    {
        var data = new double[] { 12.0, 18.0, 30.0, 22.0 };

        var cut = Render<FaSparkline>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Type, FaSparklineType.Bar));

        var rects = cut.FindAll("rect.fa-sparkline-bar");
        Assert.Equal(4, rects.Count);
    }

    [Fact]
    public void FaSparkline_EmptyData_RendersFallbackLine()
    {
        var cut = Render<FaSparkline>(parameters => parameters
            .Add(p => p.Data, Array.Empty<double>()));

        var emptyLine = cut.Find("line.fa-sparkline-empty");
        Assert.NotNull(emptyLine);
    }
}
