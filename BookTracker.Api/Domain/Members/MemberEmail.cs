namespace BookTracker.Api.Domain.Members;

public sealed record MemberEmail
{
    public const int MaxLength = 200;
    public string Value { get; }

    public MemberEmail(string value)
    {
        var cleaned = value.Trim();

        if (string.IsNullOrWhiteSpace(cleaned))
            throw new DomainException("Memberemail is required");

        if (!cleaned.Contains('@'))
            throw new DomainException("Memberemail should contain a valid email");

        if (cleaned.Length > MaxLength)
            throw new DomainException($"Memberemail should not exceed {MaxLength} characters.");

        Value = cleaned;
    }

    public static implicit operator string(MemberEmail email)
    {
        return email.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}