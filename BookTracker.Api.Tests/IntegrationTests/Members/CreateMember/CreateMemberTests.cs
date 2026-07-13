using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.CreateMember;

public class CreateMemberTests : IntegrationTest
{
    [Fact]
    public async Task PostMemberCreatesMember()
    {
        var request = new CreateMemberRequest
        {
            Name = "Thomas Verbeke",
            Email = "thomas@verbeke.com"
        };

        var response = await Client.PostAsJsonAsync("/members", request);
        var created = await response.ReadJsonAs<CreateMemberResponse>(HttpStatusCode.Created);

        Assert.True(created.Id > 0);
        Assert.Equal("Thomas Verbeke", created.Name);
        Assert.Equal("thomas@verbeke.com", created.Email);

        var member = Reader.Query(context => context.Find<Member>(created.Id));

        Assert.NotNull(member);
        Assert.Equal("Thomas Verbeke", member.Name);
        Assert.Equal("thomas@verbeke.com", member.Email);
    }


    [Fact]
    public async Task PostMemberReturnsBadRequestOnEmptyName()
    {
        var request = new CreateMemberRequest
        {
            Name = "    ",
            Email = "thomas@verbeke.com"
        };

        var response = await Client.PostAsJsonAsync("/members", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostMemberReturnsBadRequestOnInvalidEmail()
    {
        var request = new CreateMemberRequest
        {
            Name = "Thomas Verbeke",
            Email = "thomasverbeke.com"
        };

        var response = await Client.PostAsJsonAsync("/members", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }
}