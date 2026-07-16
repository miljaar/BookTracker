using BookTracker.Api.Application.Members;
using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Application.Members.DeleteMember;
using BookTracker.Api.Application.Members.GetMemberDetails;
using BookTracker.Api.Application.Members.GetMemberSummaries;
using BookTracker.Api.Application.Members.UpdateMember;
using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Security;
using System.Security.Claims;

namespace BookTracker.Api.Endpoints.Members;

public static class MemberEndpoints
{
    public static IEndpointRouteBuilder MapMemberEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/members", GetMemberSummaries)
            .RequireAuthorization(AuthorizationPolicies.ManageMembers);
        app.MapGet("/members/{id:int}", GetMemberDetails)
            .RequireAuthorization(AuthorizationPolicies.ManageMembers);

        app.MapPost("/members", CreateMember);

        app.MapPut("/members/{id:int}", UpdateMember)
            .RequireAuthorization();
        app.MapDelete("/members/{id:int}", DeleteMember)
            .RequireAuthorization();

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

    public static async Task<IResult> CreateMember(
        CreateMemberRequest request,
        CreateMemberCommandHandler handler)
    {
        try
        {
            var member = await handler.Execute(request);
            return Results.Created($"/members/{member.Id}", member);
        }
        catch (MemberEmailAlreadyExistsException ex)
        {
            return Results.Conflict(new { error = ex.Message });
        }
        catch (DomainException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    public static async Task<IResult> UpdateMember(
        int id,
        UpdateMemberRequest request,
        ClaimsPrincipal user,
        UpdateMemberRequestHandler handler)
    {
        if (!CanManageMember(user, id))
            return Results.Forbid();

        try
        {
            var updated = await handler.Execute(id, request);
            if (!updated)
                return Results.NotFound();
            return Results.NoContent();
        }
        catch (MemberEmailAlreadyExistsException ex)
        {
            return Results.Conflict(new { error = ex.Message });
        }
        catch (DomainException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    public static async Task<IResult> DeleteMember(
        int id,
        ClaimsPrincipal user,
        DeleteMemberHandler handler)
    {
        if (!CanManageMember(user, id))
            return Results.Forbid();

        var deleted = await handler.Execute(id);

        if (!deleted)
            return Results.NotFound();

        return Results.NoContent();
    }

    private static bool CanManageMember(ClaimsPrincipal user, int memberId)
    {
        if (user.IsInRole(nameof(MemberRole.Administrator)))
            return true;

        var claim = user.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(claim, out var currentMemberId) && currentMemberId == memberId;
    }
}