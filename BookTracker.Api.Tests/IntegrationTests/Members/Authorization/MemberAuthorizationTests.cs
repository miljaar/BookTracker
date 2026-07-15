using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Application.Members.UpdateMember;

namespace BookTracker.Api.Tests.IntegrationTests.Members.Authorization;

public class MemberAuthorizationTests : IntegrationTest
{
    [Fact]
    public async Task CreateMemberDoesNotRequireAuthentication()
    {
        var request =
            new CreateMemberRequest
            {
                Name = "Grace Hopper",
                Email = "grace@example.com",
                Password = "debugging-moth"
            };

        var response =
            await Client.PostAsJsonAsync(
                "/members",
                request);

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Created);
    }

    [Fact]
    public async Task UpdateMemberRequiresAuthentication()
    {
        var memberId = SeedMember();

        var request =
            new UpdateMemberRequest
            {
                Name = "Ada Byron",
                Email = "ada.byron@example.com"
            };

        var response =
            await Client.PutAsJsonAsync(
                $"/members/{memberId}",
                request);

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteMemberRequiresAuthentication()
    {
        var memberId = SeedMember();

        var response = await Client.DeleteAsync($"/members/{memberId}");
        await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);

        var member = Reader.Query(db => db.Members.Find(memberId));

        Assert.NotNull(member);
    }

    [Fact]
    public async Task MemberCanUpdateOwnAccount()
    {
        var memberId = await AuthenticateAsMember();

        var request = new UpdateMemberRequest
        {
            Name = "Ada Byron",
            Email = "ada.byron@example.com"
        };

        var response = await Client.PutAsJsonAsync($"/members/{memberId}", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task MemberCannotUpdateAnotherMember()
    {
        var currentMemberId =
            await AuthenticateAsMember();

        var otherMemberId = SeedMember(
                "Grace Hopper",
                "grace@example.com");

        var request = new UpdateMemberRequest
        {
            Name = "Changed Name",
            Email = "changed@example.com"
        };

        var response = await Client.PutAsJsonAsync($"/members/{otherMemberId}", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);

        var member = Reader.Query(db => db.Members.Find(otherMemberId));

        Assert.NotNull(member);
        Assert.Equal("Grace Hopper", member.Name.Value);
        Assert.Equal("grace@example.com", member.Email.Value);
    }


    [Fact]
    public async Task MemberCannotDeleteAnotherMember()
    {
        var currentMemberId =
            await AuthenticateAsMember();

        var otherMemberId = SeedMember(
                "Grace Hopper",
                "grace@example.com");

        var response = await Client.DeleteAsync($"/members/{otherMemberId}");

        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);

        var member = Reader.Query(db => db.Members.Find(otherMemberId));

        Assert.NotNull(member);
        Assert.Equal("Grace Hopper", member.Name.Value);
        Assert.Equal("grace@example.com", member.Email.Value);
    }
}
