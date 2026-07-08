using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;
namespace BookTracker.Api.Tests.Domain.Members;

public class MemberNameTests
{
    [Fact]
    public void MemberNameAcceptValidName()
    {
        var member = new MemberName("Maarten Verbeke");
        Assert.Equal("Maarten Verbeke", member.Value);
    }

    [Fact]
    public void MemberNameTrimsName()
    {
        var member = new MemberName("    Maarten Verbeke   ");
        Assert.Equal("Maarten Verbeke", member.Value);
    }

    [Fact]
    public void MemberNameRejectsWhitespaces()
    {
        var ex = Assert.Throws<DomainException>(() => new MemberName("    "));
        Assert.Equal("Membername is required", ex.Message);
    }

    [Fact]
    public void MemberNameRejectsNameLongerThan100Characters()
    {
        var ex = Assert.Throws<DomainException>(() => new MemberName(new string('e', 101)));
        Assert.Equal("Membername should not exceed 100 characters.", ex.Message);
    }
}