using BookTracker.Api.Domain.Members;
using BookTracker.Api.Security;
using BookTracker.Api.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Application.Auth.Login;

public class LoginCommandHandler(
    AppDbContext dbContext,
    IPasswordHasher<Member> passwordHasher,
    JwtTokenGenerator tokenGenerator,
    ILogger<LoginCommandHandler> logger) : IHandler
{
    public async Task<LoginResponse?> Execute(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return null;

        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        var member = await dbContext.Members
            .AsNoTracking()
            .SingleOrDefaultAsync(member => (string)member.Email == email);

        if (member is null)
            return null;

        logger.LogInformation("Check password for login");
        var verification = passwordHasher.VerifyHashedPassword(
            member,
            member.PasswordHash,
            request.Password
            );

        var copyMember = member;
        copyMember.PasswordHash = String.Empty;
        var newPasswordHash = passwordHasher.HashPassword(copyMember, request.Password);
        logger.LogInformation($"hash '{member.PasswordHash}', new hash: '{newPasswordHash}'");

        if (verification == PasswordVerificationResult.Failed)
        {
            logger.LogInformation($"Check failed for member '{member.Email}'");
            return null;
        }
        return tokenGenerator.Generate(member);
    }
}