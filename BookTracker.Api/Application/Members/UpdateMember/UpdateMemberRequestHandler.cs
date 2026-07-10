using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage.Members;

namespace BookTracker.Api.Application.Members.UpdateMember;

public class UpdateMemberRequestHandler(IMemberRepository repository) : IHandler
{
    public async Task<bool> Execute(int id, UpdateMemberRequest request)
    {
        return await repository.UpdateAsync(
            new Member
            {
                Id = id,
                Name = new MemberName(request.Name),
                Email = new MemberEmail(request.Email)
            }
        );
    }
}