namespace BookTracker.Api.Domain.Books;

public sealed record AuthorName
{
    public const int MaxLength = 100;
    public string Value { get; }

    public AuthorName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Author is required.");

        if (value.Contains('\0'))
            throw new DomainException("Author name can not contain a null character.");

        var cleaned = value.Trim();

        if (cleaned.Length > MaxLength)
            throw new DomainException($"Author cannot be longer than {MaxLength} characters.");

        Value = cleaned;
    }

    public static implicit operator string(AuthorName author)
    {
        return author.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}