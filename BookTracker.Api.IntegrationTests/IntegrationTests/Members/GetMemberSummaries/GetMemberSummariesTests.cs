using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application;
using BookTracker.Api.Application.Members.GetMemberSummaries;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.IntegrationTests.Members.GetMemberSummaries;

[Collection(PostgreSqlCollection.Name)]
public class GetMemberSummariesTests(PostgreSqlFixture database) : IntegrationTest(database)
{
    [Fact]
    public async Task GetMemberSummariesReturnsMemberSummaries()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        var response = await Client.GetAsync("/members");
        var result = await response.ReadJsonAs<PagedResult<MemberSummary>>(HttpStatusCode.OK);

        var memberSummary = Assert.Single(result.Items);
        Assert.Equal("Ada Lovelace", memberSummary.Name);
        Assert.Equal("ada@example.com", memberSummary.Email);
        Assert.True(memberSummary.Id != 0);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesReturnsRequestedPage()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        Writer.Seed(db =>
        {
            db.Members.AddRange(
                new Member
                {
                    Name = new MemberName("Dimitri De Tremmerie"),
                    Email = new MemberEmail("ddt@brt.be")
                },
                new Member
                {
                    Name = new MemberName("Vlad"),
                    Email = new MemberEmail("vlad@vrt.be")
                },
                new Member
                {
                    Name = new MemberName("Maya de Bij"),
                    Email = new MemberEmail("maya@studio100.be")
                });
        });

        var result = await Client.GetFromJsonAsync<PagedResult<MemberSummary>>("/members?page=3&pageSize=1");

        Assert.NotNull(result);

        var member = Assert.Single(result.Items);

        Assert.Equal("Vlad", member.Name);
        Assert.Equal(3, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(4, result.TotalItems);
        Assert.Equal(4, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesCanSearchByName()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        Writer.Seed(db =>
        {
            db.Members.AddRange(
                new Member
                {
                    Name = new MemberName("Dimitri De Tremmerie"),
                    Email = new MemberEmail("ddt@brt.be")
                },
                new Member
                {
                    Name = new MemberName("Vlad"),
                    Email = new MemberEmail("vlad@vrt.be")
                });
        });

        var response = await Client.GetAsync("/members?search=dimi");

        var result = await response.ReadJsonAs<PagedResult<MemberSummary>>(HttpStatusCode.OK);

        var member = Assert.Single(result.Items);

        Assert.Equal("Dimitri De Tremmerie", member.Name);
        Assert.Equal("ddt@brt.be", member.Email);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesCanSearchByEmail()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        Writer.Seed(db =>
        {
            db.Members.AddRange(
                new Member
                {
                    Name = new MemberName("Dimitri De Tremmerie"),
                    Email = new MemberEmail("ddt@brt.be")
                },
                new Member
                {
                    Name = new MemberName("Vlad"),
                    Email = new MemberEmail("vlad@vrt.be")
                });
        });

        var response = await Client.GetAsync("/members?search=vlad@");

        var result = await response.ReadJsonAs<PagedResult<MemberSummary>>(HttpStatusCode.OK);

        var member = Assert.Single(result.Items);

        Assert.Equal("vlad@vrt.be", member.Email);
        Assert.Equal("Vlad", member.Name);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }
}