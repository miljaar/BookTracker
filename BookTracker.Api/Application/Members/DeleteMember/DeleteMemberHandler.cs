using BookTracker.Api.Storage.Members;

namespace BookTracker.Api.Application.Members.DeleteMember;

public class DeleteMemberHandler(IMemberRepository repository) : IHandler
{
    public async Task<bool> Execute(int id)
    {
        return await repository.DeleteAsync(id);
    }
}