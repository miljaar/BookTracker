using System.Net;
using BookTracker.Api.Application;
using BookTracker.Api.Application.Members;
using BookTracker.Api.Application.Members.GetMemberDetails;
using BookTracker.Api.Application.Members.GetMemberSummaries;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.GetMemberDetails;

public class GetMemberDetailsTests : IntegrationTest
{
    [Fact]
    public async Task GetMemberDetailsReturnDetails()
    {
        Writer.Seed(db => db.Members.Add(
            new Member
            {
                Name = new MemberName("Dimitri De Tremmerie"),
                Email = new MemberEmail("ddt@brt.be")
            }
        ));
        var response = await Client.GetAsync("/members/1");
        var memberDetail = await response.ReadJsonAs<GetMemberDetailsResponse>(HttpStatusCode.OK);

        Assert.Equal("Dimitri De Tremmerie", memberDetail.Name);
        Assert.Equal("ddt@brt.be", memberDetail.Email);
        Assert.True(memberDetail.Id != 0);
    }

    [Fact]
    public async Task GetMemberDetailsReturnNotFoundForInvalidId()
    {
        Writer.Seed(db => db.Members.Add(
            new Member
            {
                Name = new MemberName("Dimitri De Tremmerie"),
                Email = new MemberEmail("ddt@brt.be")
            }
        ));
        var response = await Client.GetAsync("/members/99");

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }
}