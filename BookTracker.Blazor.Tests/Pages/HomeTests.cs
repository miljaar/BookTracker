using BookTracker.Blazor.Pages;
using Bunit;

public class HomeTests : BunitContext
{
    [Fact]
    public void HidesAuthorsWhenToggleIsClicked()
    {
        var cut = Render<Home>();

        cut.Find("button").Click();

        Assert.DoesNotContain("Frank Herbert", cut.Markup);
        Assert.DoesNotContain("Raymond Chandler", cut.Markup);
    }
}
