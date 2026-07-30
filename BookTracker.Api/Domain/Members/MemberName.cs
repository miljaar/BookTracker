namespace BookTracker.Api.Domain.Members;

public sealed record MemberName
{
    public const int MaxLength = 100;
    public string Value { get; }

    public MemberName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Membername is required.");

        if (value.Contains('\0'))
            throw new DomainException("Membername can not contain a null character.");

        var cleaned = value.Trim();

        if (cleaned.Length > MaxLength)
            throw new DomainException($"Membername should not exceed {MaxLength} characters.");

        Value = cleaned;
    }

    public static implicit operator string(MemberName name)
    {
        return name.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}