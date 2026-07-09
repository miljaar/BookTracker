using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Storage.Members;

public class EFMemberRepository(AppDbContext dbContext) : IMemberRepository
{
    public async Task<Member> AddAsync(Member member)
    {
        dbContext.Members.Add(member);
        await dbContext.SaveChangesAsync();
        return member;
    }

    public async Task<bool> UpdateAsync(Member member)
    {
        var updateMember = await dbContext.Members.FindAsync(member.Id);
        if (updateMember is null)
            return false;

        updateMember.Email = member.Email;
        updateMember.Name = member.Name;

        await dbContext.SaveChangesAsync();
        return true;

    }

    public async Task<bool> DeleteAsync(int id)
    {
        var member = await dbContext.Members.FindAsync(id);
        if (member is null)
            return false;

        dbContext.Members.Remove(member);
        await dbContext.SaveChangesAsync();
        return true;
    }
}