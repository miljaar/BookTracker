using System.Net;
using BookTracker.Api.Application;
using BookTracker.Api.Application.Members;
using BookTracker.Api.Application.Members.GetMemberDetails;
using BookTracker.Api.Application.Members.GetMemberSummaries;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.GetMemberDetails;

[Collection(PostgreSqlCollection.Name)]
public class GetMemberDetailsTests(PostgreSqlFixture database) : IntegrationTest(database)
{
    [Fact]
    public async Task GetMemberDetailsReturnDetails()
    {
        var memberId = await AuthenticateAsMember(MemberRole.Administrator);

        var response = await Client.GetAsync($"/members/{memberId}");
        var memberDetail = await response.ReadJsonAs<GetMemberDetailsResponse>(HttpStatusCode.OK);

        Assert.Equal("Ada Lovelace", memberDetail.Name);
        Assert.Equal("ada@example.com", memberDetail.Email);
        Assert.True(memberDetail.Id != 0);
    }

    [Fact]
    public async Task GetMemberDetailsReturnNotFoundForInvalidId()
    {
        var memberId = await AuthenticateAsMember(MemberRole.Administrator);

        var response = await Client.GetAsync($"/members/{memberId + 99}");

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }
}