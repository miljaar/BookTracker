using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Books.GetBookDetails;
using BookTracker.Api.Application.Books.UpdateBook;
using BookTracker.Api.Domain.Books;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.IntegrationTests.Books.UpdateBook;

[Collection(PostgreSqlCollection.Name)]
public class UpdateBookTests(PostgreSqlFixture database) : IntegrationTest(database)
{
    [Fact]
    public async Task PutBookUpdatesBook()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        Writer.Seed(db =>
        {
            db.Books.Add(
                new Book
                {
                    Title = new BookTitle("Dune"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1965
                });
        });

        var version = Reader.Query(db =>
            db.Books
                .Where(book => book.Id == 1)
                .Select(book => book.Version)
                .Single());

        var request =
            new UpdateBookRequest
            {
                Title = "Dune Messiah",
                Author = "Frank Herbert",
                Year = 1969,
                Version = version
            };

        var response = await Client.PutAsJsonAsync("/books/1", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var book = Reader.Query(db => db.Books.Find(1));

        Assert.NotNull(book);
        Assert.Equal(1969, book.Year);
        Assert.Equal("Frank Herbert", book.Author.Value);
        Assert.Equal("Dune Messiah", book.Title.Value);
        Assert.Equal(1, book.Id);
        Assert.NotEqual(version, book.Version);
    }

    [Fact]
    public async Task PutBookReturnsNotFoundWhenBookDoesNotExist()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        var request =
            new UpdateBookRequest
            {
                Title = "Unknown Book",
                Author = "Unknown Author",
                Year = 2000,
                Version = Guid.NewGuid()
            };

        var response = await Client.PutAsJsonAsync("/books/9999", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PutBookReturnsConflictForStaleVersion()
    {
        await AuthenticateAsMember(MemberRole.Administrator);
        Writer.Seed(db =>
        {
            db.Books.Add(
                new Book
                {
                    Title = new BookTitle("Dune"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1965
                });
        });

        var firstResponse = await Client.GetAsync("/books/1");

        var firstRead =
            await firstResponse.ReadJsonAs<GetBookDetailsResponse>(
                HttpStatusCode.OK);

        var secondResponse = await Client.GetAsync("/books/1");

        var secondRead =
            await secondResponse.ReadJsonAs<GetBookDetailsResponse>(
                HttpStatusCode.OK);

        var firstUpdate =
            new UpdateBookRequest
            {
                Title = "Dune: Special Edition",
                Author = firstRead.Author,
                Year = firstRead.Year,
                Version = firstRead.Version
            };

        var firstUpdateResponse =
            await Client.PutAsJsonAsync("/books/1", firstUpdate);

        await firstUpdateResponse.ShouldHaveStatusCode(
            HttpStatusCode.NoContent);

        var staleUpdate =
            new UpdateBookRequest
            {
                Title = secondRead.Title,
                Author = secondRead.Author,
                Year = 1966,
                Version = secondRead.Version
            };

        var staleUpdateResponse =
            await Client.PutAsJsonAsync("/books/1", staleUpdate);

        await staleUpdateResponse.ShouldHaveStatusCode(
            HttpStatusCode.Conflict);

        var book = Reader.Query(db => db.Books.Find(1));

        Assert.NotNull(book);
        Assert.Equal("Dune: Special Edition", book.Title.Value);
        Assert.Equal(1965, book.Year);
    }
}