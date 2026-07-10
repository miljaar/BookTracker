using BookTracker.Api.Application.Members.GetMemberDetails;
using BookTracker.Api.Application.Members.GetMemberSummaries;

namespace BookTracker.Api.Endpoints.Members;

public static class MemberEndpoints
{
    public static IEndpointRouteBuilder MapMemberEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/members", GetMemberSummaries);
        app.MapGet("/members/{id:int}", GetMemberDetails);
        return app;
    }

    public static async Task<IResult> GetMemberSummaries([AsParameters] GetMemberSummariesRequest request, GetMemberSummariesQueryHandler query)
    {
        return Results.Ok(await query.Execute(request));
    }

    public static async Task<IResult> GetMemberDetails(int id, GetMemberDetailsQueryHandler query)
    {
        var member = query.Execute(id);
        if (member is null)
            return Results.NotFound();
        return Results.Ok(member);
    }
}