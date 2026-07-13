using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;
namespace BookTracker.Api.Tests.Domain.Members;

public class MemberEmailTests
{
    [Fact]
    public void MemberEmailAcceptValidName()
    {
        var member = new MemberEmail("valid@email.com");
        Assert.Equal("valid@email.com", member.Value);
    }

    [Fact]
    public void MemberEmailTrimsName()
    {
        var member = new MemberEmail("    valid@email.com   ");
        Assert.Equal("valid@email.com", member.Value);
    }

    [Fact]
    public void MemberEmailRejectsWhitespaces()
    {
        var ex = Assert.Throws<DomainException>(() => new MemberEmail("    "));
        Assert.Equal("Memberemail is required.", ex.Message);
    }

    [Fact]
    public void MemberEmailRejectsNull()
    {
        var exception = Assert.Throws<DomainException>(() => new MemberEmail(null!));

        Assert.Equal("Memberemail is required.", exception.Message);
    }

    [Fact]
    public void MemberEmailRejectsInvalidMail()
    {
        var ex = Assert.Throws<DomainException>(() => new MemberEmail("novalidemail.com"));
        Assert.Equal("Memberemail should contain a valid email.", ex.Message);
    }

    [Fact]
    public void MemberEmailRejectsNameLongerThan200Characters()
    {
        var ex = Assert.Throws<DomainException>(() => new MemberEmail(new string('@', 201)));
        Assert.Equal("Memberemail should not exceed 200 characters.", ex.Message);
    }
}