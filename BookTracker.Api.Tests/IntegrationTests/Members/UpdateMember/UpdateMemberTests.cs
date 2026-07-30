using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.UpdateMember;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.UpdateMember;

[Collection(PostgreSqlCollection.Name)]
public class UpdateMemberTests(PostgreSqlFixture database) : IntegrationTest(database)
{
    [Fact]
    public async Task UpdateMemberUpdatesAMember()
    {
        var memberId = await AuthenticateAsMember();

        var memberUpdated = new UpdateMemberRequest
        {
            Name = "Tom",
            Email = "tom@tom.tom"
        };

        var response = await Client.PutAsJsonAsync($"/members/{memberId}", memberUpdated);
        var result = response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var memberCheck = Reader.Query(context => context.Members.Find(memberId));
        Assert.NotNull(memberCheck);
        Assert.Equal("Tom", memberCheck.Name);
        Assert.Equal("tom@tom.tom", memberCheck.Email);
    }

    [Fact]
    public async Task UpdateMemberReturnsForbiddenForUnknownId()
    {
        var memberId = await AuthenticateAsMember();

        var memberUpdated = new UpdateMemberRequest
        {
            Name = "Tom",
            Email = "tom@tom.tom"
        };

        var response = await Client.PutAsJsonAsync($"/members/{memberId + 999}", memberUpdated);
        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateMemberReturnsNotFoundForInvalidEmail()
    {
        var memberId = await AuthenticateAsMember();

        var memberUpdated = new UpdateMemberRequest
        {
            Name = "Tom",
            Email = "tomtom.tom"
        };

        var response = await Client.PutAsJsonAsync($"/members/{memberId}", memberUpdated);
        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateMemberCantUseExistingEmail()
    {
        var memberId = await AuthenticateAsMember();

        Writer.Seed(db => db.Members.Add(
            new Member
            {
                Name = new MemberName("Els soens"),
                Email = new MemberEmail("els@dings.be")
            }
        ));

        var memberUpdated = new UpdateMemberRequest
        {
            Name = "Tom",
            Email = "els@dings.be"
        };

        var response = await Client.PutAsJsonAsync($"/members/{memberId}", memberUpdated);
        await response.ShouldHaveStatusCode(HttpStatusCode.Conflict, "A member with this email already exists.");
    }

}