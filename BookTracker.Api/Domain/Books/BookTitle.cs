namespace BookTracker.Api.Domain
{
    public sealed record BookTitle
    {
        public const int MaxLength = 100;
        public string Value { get; }
        public BookTitle(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Title is required.");

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
}