using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.UpdateMember;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.UpdateMember;

public class UpdateMemberTests : IntegrationTest
{
    [Fact]
    public async Task UpdateMemberUpdatesAMember()
    {
        Writer.Seed(db => db.Members.Add(
            new Member
            {
                Name = new MemberName("Dimitri De Tremmerie"),
                Email = new MemberEmail("ddt@brt.be")
            }
        ));

        var memberUpdated = new UpdateMemberRequest
        {
            Name = "Tom",
            Email = "tom@tom.tom"
        };

        var response = await Client.PutAsJsonAsync("/members/1", memberUpdated);
        var result = response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var memberCheck = Reader.Query(context => context.Members.Find(1));
        Assert.NotNull(memberCheck);
        Assert.Equal("Tom", memberCheck.Name);
        Assert.Equal("tom@tom.tom", memberCheck.Email);
    }

    [Fact]
    public async Task UpdateMemberReturnsNotFoundForUnknownId()
    {
        Writer.Seed(db => db.Members.Add(
            new Member
            {
                Name = new MemberName("Dimitri De Tremmerie"),
                Email = new MemberEmail("ddt@brt.be")
            }
        ));

        var memberUpdated = new UpdateMemberRequest
        {
            Name = "Tom",
            Email = "tom@tom.tom"
        };

        var response = await Client.PutAsJsonAsync("/members/999", memberUpdated);
        var result = response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateMemberReturnsNotFoundForInvalidEmail()
    {
        Writer.Seed(db => db.Members.Add(
            new Member
            {
                Name = new MemberName("Dimitri De Tremmerie"),
                Email = new MemberEmail("ddt@brt.be")
            }
        ));

        var memberUpdated = new UpdateMemberRequest
        {
            Name = "Tom",
            Email = "tomtom.tom"
        };

        var response = await Client.PutAsJsonAsync("/members/999", memberUpdated);
        var result = response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }
}