using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application;
using BookTracker.Api.Application.Members.GetMemberSummaries;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.GetMemberSummaries;

public class GetMemberSummariesTests : IntegrationTest
{
    [Fact]
    public async Task GetMemberSummariesReturnsMemberSummaries()
    {
        var member = new Member
        {
            Name = new MemberName("Dimitri De Tremmerie"),
            Email = new MemberEmail("ddt@brt.be")
        };

        Writer.Seed(context => context.Members.Add(member));

        var response = await Client.GetAsync("/members");
        var result = await response.ReadJsonAs<PagedResult<MemberSummary>>(HttpStatusCode.OK);

        var memberSummary = Assert.Single(result.Items);
        Assert.Equal("Dimitri De Tremmerie", memberSummary.Name);
        Assert.Equal("ddt@brt.be", memberSummary.Email);
        Assert.True(memberSummary.Id != 0);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesReturnsRequestedPage()
    {
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

        var result = await Client.GetFromJsonAsync<PagedResult<MemberSummary>>("/members?page=2&pageSize=1");

        Assert.NotNull(result);

        var member = Assert.Single(result.Items);

        Assert.Equal("Vlad", member.Name);
        Assert.Equal(2, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(3, result.TotalItems);
        Assert.Equal(3, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesCanSearchByName()
    {
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