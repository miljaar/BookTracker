using BookTracker.Api.Domain.Actors;
using BookTracker.Api.Domain.Books;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.Domain.Members;

public class MemberPermissionsTests
{
    [Fact]
    public void MemberCanManageOwnAccount()
    {
        var actor = new Actor(42, MemberRole.Member);
        MemberPermissions.EnsureCanManage(actor, 42);
    }

    [Fact]
    public void MemberCannotManageAnotherAccount()
    {
        var actor = new Actor(42, MemberRole.Member);
        Assert.Throws<ForbiddenOperationException>(() =>
            MemberPermissions.EnsureCanManage(actor, 1));
    }
}