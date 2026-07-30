using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Actors;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Application.Members.GetMemberSummaries;

public class GetMemberSummariesQueryHandler(AppDbContext dbContext) : IHandler
{
    private const int DefaultPage = 1;
    private const int defaultPageSize = 10;
    private const int MinPageSize = 1;
    private const int MaxPageSize = 50;

    public async Task<PagedResult<MemberSummary>> Execute(
        Actor actor,
        GetMemberSummariesRequest request)
    {
        MemberPermissions.EnsureCanViewDirectory(actor);

        var page = Math.Max(1, request.Page ?? DefaultPage);
        var pageSize = Math.Clamp(request.PageSize ?? defaultPageSize, MinPageSize, MaxPageSize);

        var query = dbContext.Members.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            if (request.Search.Contains('\0'))
                throw new DomainException("Invalid search term.");

            var search = request.Search
                .Trim()
                .Replace("\\", "\\\\")
                .Replace("%", "\\%")
                .Replace("_", "\\_");

            search = $"%{search}%";
            query = query.Where(member =>
                EF.Functions.ILike((string)member.Name, search) ||
                EF.Functions.ILike((string)member.Email, search));
        }

        var totalMembers = await query.CountAsync();

        var members = await query
            .OrderBy(member => member.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(member =>
                new MemberSummary
                {
                    Id = member.Id,
                    Name = member.Name,
                    Email = member.Email
                })
            .ToListAsync();

        return new PagedResult<MemberSummary>
        {
            Items = members,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalMembers,
            TotalPages = (int)Math.Ceiling(totalMembers / (double)pageSize)
        };
    }

}