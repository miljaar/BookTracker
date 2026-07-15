using BookTracker.Api.Domain.Members;
using Microsoft.AspNetCore.Identity;

namespace BookTracker.Api.Tests.IntegrationTests;

public abstract class IntegrationTest : IDisposable
{
    private readonly CustomWebApplicationFactory factory = new();
    protected HttpClient Client { get; }
    protected EfReader Reader { get; }
    protected EfWriter Writer { get; }

    protected IntegrationTest()
    {
        Client = factory.CreateClient();
        Reader = factory.GetReader();
        Writer = factory.GetWriter();
    }
    protected void SeedMember(string password = "analytical-engine")
    {
        var member = new Member
        {
            Name = new MemberName("Ada Lovelace"),
            Email = new MemberEmail("ada@example.com"),
            PasswordHash = string.Empty
        };

        var passwordHasher = new PasswordHasher<Member>();

        member.PasswordHash = passwordHasher.HashPassword(member, password);

        Writer.Seed(db => db.Members.Add(member));
    }

    public void Dispose()
    {
        Client.Dispose();
        factory.Dispose();
    }
}