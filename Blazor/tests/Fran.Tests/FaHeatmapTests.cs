using Bunit;
using Fran.Components;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Fran.Tests;

public class FaHeatmapTests : BunitContext
{
    [Fact]
    public void FaHeatmap_RendersGridAndLegend()
    {
        var cells = new List<FaHeatmapCell>
        {
            new("c1", 0.0, "Day 1"),
            new("c2", 20.0, "Day 2"),
            new("c3", 50.0, "Day 3"),
            new("c4", 80.0, "Day 4"),
            new("c5", 100.0, "Day 5")
        };

        var cut = Render<FaHeatmap>(parameters => parameters
            .Add(p => p.Items, cells)
            .Add(p => p.Columns, 5)
            .Add(p => p.MaxValue, 100.0)
            .Add(p => p.ColorScale, FaHeatmapColorScale.Green)
            .Add(p => p.Label, "Commit Activity")
            .Add(p => p.ShowLegend, true));

        var root = cut.Find(".fa-heatmap-wrapper");
        Assert.NotNull(root);

        var title = cut.Find(".fa-heatmap-title");
        Assert.Equal("Commit Activity", title.TextContent);

        var grid = cut.Find("div.fa-heatmap.fa-heatmap-green");
        Assert.NotNull(grid);
        Assert.Equal("grid", grid.GetAttribute("role"));

        var buttons = cut.FindAll("button.fa-heatmap-cell");
        Assert.Equal(5, buttons.Count);

        // Verify computed levels
        Assert.Contains("fa-heatmap-level-0", buttons[0].ClassName);
        Assert.Contains("fa-heatmap-level-1", buttons[1].ClassName);
        Assert.Contains("fa-heatmap-level-2", buttons[2].ClassName);
        Assert.Contains("fa-heatmap-level-4", buttons[3].ClassName);
        Assert.Contains("fa-heatmap-level-4", buttons[4].ClassName);

        // Verify legend
        var legend = cut.Find(".fa-heatmap-legend");
        Assert.NotNull(legend);
        Assert.Contains("Less", legend.TextContent);
        Assert.Contains("More", legend.TextContent);
    }

    [Fact]
    public void FaHeatmap_ExplicitLevels_RespectedAccurately()
    {
        var cells = new List<FaHeatmapCell>
        {
            new("c1", 10.0, ExplicitLevel: 3),
            new("c2", 90.0, ExplicitLevel: 1)
        };

        var cut = Render<FaHeatmap>(parameters => parameters
            .Add(p => p.Items, cells)
            .Add(p => p.ColorScale, FaHeatmapColorScale.Blue));

        var buttons = cut.FindAll("button.fa-heatmap-cell");
        Assert.Contains("fa-heatmap-level-3", buttons[0].ClassName);
        Assert.Contains("fa-heatmap-level-1", buttons[1].ClassName);
    }
}
