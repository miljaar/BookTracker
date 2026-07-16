using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage.Members;
using Microsoft.AspNetCore.Identity;

namespace BookTracker.Api.Application.Members.CreateMember;

public class CreateMemberCommandHandler(
    IMemberRepository repository,
    IPasswordHasher<Member> passwordHasher) : IHandler
{
    public async Task<CreateMemberResponse> Execute(CreateMemberRequest request)
    {
        var name = new MemberName(request.Name);
        var email = new MemberEmail(request.Email);

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new DomainException("Password is required.");

        if (request.Password.Length < 8)
            throw new DomainException("Password must contain at least 8 characters.");

        if (await repository.EmailExistsAsync(email))
            throw new MemberEmailAlreadyExistsException();

        var member = new Member
        {
            Name = name,
            Email = email,
            Role = MemberRole.Member
        };

        member.PasswordHash = passwordHasher.HashPassword(member, request.Password);

        var savedMember = await repository.AddAsync(member);
        return new CreateMemberResponse
        {
            Id = savedMember.Id,
            Name = savedMember.Name,
            Email = savedMember.Email
        };
    }
}