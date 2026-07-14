using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Domain.Members;
using Microsoft.AspNetCore.Identity;

namespace BookTracker.Api.Tests.IntegrationTests.Members.CreateMember;

public class CreateMemberTests : IntegrationTest
{
    [Fact]
    public async Task PostMemberCreatesMember()
    {
        var request = new CreateMemberRequest
        {
            Name = "Thomas Verbeke",
            Email = "thomas@verbeke.com",
            Password = "somepassword"
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
        Assert.NotEqual("somepassword", member.PasswordHash);

        var passwordHasher = new PasswordHasher<Member>();
        var result = passwordHasher.VerifyHashedPassword(member, member.PasswordHash, "somepassword");
        Assert.Equal(PasswordVerificationResult.Success, result);
    }


    [Fact]
    public async Task PostMemberReturnsBadRequestOnEmptyName()
    {
        var request = new CreateMemberRequest
        {
            Name = "    ",
            Email = "thomas@verbeke.com",
            Password = "somepassword"
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
            Email = "thomasverbeke.com",
            Password = "somepassword"
        };

        var response = await Client.PostAsJsonAsync("/members", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostMemberReturnsBadRequestWithEmptyPassword()
    {
        var request = new CreateMemberRequest
        {
            Name = "Thomas Verbeke",
            Email = "thomas@verbeke.com",
            Password = ""
        };

        var response = await Client.PostAsJsonAsync("/members", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest, "Password is required.");
    }

    [Fact]
    public async Task PostMemberReturnsBadRequestWithShortPassword()
    {
        var request = new CreateMemberRequest
        {
            Name = "Thomas Verbeke",
            Email = "thomas@verbeke.com",
            Password = "shorty"
        };

        var response = await Client.PostAsJsonAsync("/members", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest, "Password must contain at least 8 characters.");
    }

    [Fact]
    public async Task PostMemberReturnsConflictWithDoubleEmail()
    {
        var request = new CreateMemberRequest
        {
            Name = "Thomas Verbeke",
            Email = "thomas@verbeke.com",
            Password = "ValidPassword"
        };

        var response = await Client.PostAsJsonAsync("/members", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.Created);
        var request2 = new CreateMemberRequest
        {
            Name = "Guide Verkammen",
            Email = "Thomas@Verbeke.com",
            Password = "ValidPassword"
        };

        var response2 = await Client.PostAsJsonAsync("/members", request2);
        await response2.ShouldHaveStatusCode(HttpStatusCode.Conflict, "A member with this email already exists.");
    }
}