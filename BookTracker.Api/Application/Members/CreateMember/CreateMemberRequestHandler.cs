using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage.Members;

namespace BookTracker.Api.Application.Members.CreateMember;

public class CreateMemberRequestHandler(IMemberRepository repository) : IHandler
{
    public async Task<CreateMemberResponse> Execute(CreateMemberRequest request)
    {
        var member = new Member
        {
            Name = new MemberName(request.Name),
            Email = new MemberEmail(request.Email)
        };
        var savedMember = await repository.AddAsync(member);
        return new CreateMemberResponse
        {
            Id = savedMember.Id,
            Name = savedMember.Name,
            Email = savedMember.Email
        };
    }
}