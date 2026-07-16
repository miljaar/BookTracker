namespace BookTracker.Api.Domain.Books;

public class ForbiddenOperationException(string message) : Exception(message);