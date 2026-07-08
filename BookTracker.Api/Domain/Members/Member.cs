namespace BookTracker.Api.Domain.Members;

public class Members
{
    public int Id { get; set; }
    public required MemberName Name { get; set; }
    public required MemberEmail Email { get; set; }
}