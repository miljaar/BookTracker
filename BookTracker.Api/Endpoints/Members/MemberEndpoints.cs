using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Application.Members.GetMemberDetails;
using BookTracker.Api.Application.Members.GetMemberSummaries;
using BookTracker.Api.Application.Members.UpdateMember;
using BookTracker.Api.Domain;

namespace BookTracker.Api.Endpoints.Members;

public static class MemberEndpoints
{
    public static IEndpointRouteBuilder MapMemberEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/members", GetMemberSummaries);
        app.MapGet("/members/{id:int}", GetMemberDetails);
        app.MapPost("/members", CreateMember);
        app.MapPut("/members/{id:int}", UpdateMember);
        return app;
    }

    public static async Task<IResult> GetMemberSummaries([AsParameters] GetMemberSummariesRequest request, GetMemberSummariesQueryHandler query)
    {
        return Results.Ok(await query.Execute(request));
    }

    public static async Task<IResult> GetMemberDetails(int id, GetMemberDetailsQueryHandler query)
    {
        var member = await query.Execute(id);
        if (member is null)
            return Results.NotFound();
        return Results.Ok(member);
    }

    public static async Task<IResult> CreateMember(CreateMemberRequest request, CreateMemberRequestHandler handler)
    {
        try
        {
            var member = await handler.Execute(request);
            return Results.Created($"/members/{member.Id}", member);
        }
        catch (DomainException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    public static async Task<IResult> UpdateMember(
        int id,
        UpdateMemberRequest request,
        UpdateMemberRequestHandler handler)
    {
        try
        {
            var updated = await handler.Execute(id, request);
            if (!updated)
                return Results.NotFound();
            return Results.NoContent();
        }
        catch (DomainException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }
}