namespace BookTracker.Api.Application.Members.GetMemberSummaries;

public class GetMemberSummariesRequest
{
    public string? Search { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }

}