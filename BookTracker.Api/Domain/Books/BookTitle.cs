namespace BookTracker.Api.Domain.Books;

public sealed record BookTitle
{
    public const int MaxLength = 100;
    public string Value { get; }
    public BookTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Title is required.");

        if (value.Contains('\0'))
            throw new DomainException("Title can not contain a null character.");

        var cleaned = value.Trim();

        if (cleaned.Length > MaxLength)
            throw new DomainException($"Title cannot be longer than {MaxLength} characters.");

        Value = cleaned;
    }

    public static implicit operator string(BookTitle title)
    {
        return title.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}