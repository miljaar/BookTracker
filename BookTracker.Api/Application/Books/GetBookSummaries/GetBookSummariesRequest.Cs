namespace BookTracker.Api.Application.Books.GetBookSummaries
{
    public class GetBookSummariesRequest
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? Search { get; set; }

    }
}