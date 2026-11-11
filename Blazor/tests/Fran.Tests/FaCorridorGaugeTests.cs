using Bunit;
using Fran.Components;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Fran.Tests;

public class FaCorridorGaugeTests : BunitContext
{
    [Fact]
    public void FaCorridorGauge_Default_RendersTrackAndOptimalSegment()
    {
        var cut = Render<FaCorridorGauge>(parameters => parameters
            .Add(p => p.Value, 0.5)
            .Add(p => p.Min, 0.0)
            .Add(p => p.Max, 1.0)
            .Add(p => p.OptimalLower, 0.3)
            .Add(p => p.OptimalUpper, 0.7)
            .Add(p => p.Label, "Propulsion Window"));

        var root = cut.Find(".fa-corridor-gauge");
        Assert.NotNull(root);
        Assert.Equal("meter", root.GetAttribute("role"));
        Assert.Equal("0.5", root.GetAttribute("aria-valuenow"));

        var label = cut.Find(".fa-corridor-label");
        Assert.Equal("Propulsion Window", label.TextContent);

        var badge = cut.Find(".fa-corridor-badge-optimal");
        Assert.NotNull(badge);
        Assert.Contains("Optimal", badge.TextContent);

        var marker = cut.Find(".fa-corridor-marker");
        Assert.Contains("left:50.0%", marker.GetAttribute("style"));
    }

    [Fact]
    public void FaCorridorGauge_BelowOptimalFloor_ShowsSubOptimalWarning()
    {
        var cut = Render<FaCorridorGauge>(parameters => parameters
            .Add(p => p.Value, 0.15)
            .Add(p => p.Min, 0.0)
            .Add(p => p.Max, 1.0)
            .Add(p => p.OptimalLower, 0.25)
            .Add(p => p.OptimalUpper, 0.40)
            .Add(p => p.Unit, "St"));

        var badge = cut.Find(".fa-corridor-badge-warning");
        Assert.NotNull(badge);
        Assert.Contains("Below Floor", badge.TextContent);
        Assert.Contains("St", badge.TextContent);
    }

    [Fact]
    public void FaCorridorGauge_AboveOptimalCeiling_ShowsExcessiveAlert()
    {
        var cut = Render<FaCorridorGauge>(parameters => parameters
            .Add(p => p.Value, 0.55)
            .Add(p => p.Min, 0.0)
            .Add(p => p.Max, 1.0)
            .Add(p => p.OptimalLower, 0.25)
            .Add(p => p.OptimalUpper, 0.40));

        var badge = cut.Find(".fa-corridor-badge-alert");
        Assert.NotNull(badge);
        Assert.Contains("Above Ceiling", badge.TextContent);
    }
}
