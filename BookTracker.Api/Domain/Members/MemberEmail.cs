namespace BookTracker.Api.Domain.Members;

public sealed record MemberEmail
{
    public const int MaxLength = 200;
    public string Value { get; }

    public MemberEmail(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Memberemail is required.");

        if (value.Contains('\0'))
            throw new DomainException("Memberemail can not contain a null character.");

        var cleaned = value.Trim().ToLowerInvariant();

        if (!cleaned.Contains('@'))
            throw new DomainException("Memberemail should contain a valid email.");

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