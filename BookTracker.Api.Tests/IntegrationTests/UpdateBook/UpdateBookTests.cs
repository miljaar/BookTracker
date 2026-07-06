using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.UpdateBook;
using BookTracker.Api.Domain;

namespace BookTracker.Api.Tests.IntegrationTests.UpdateBook;

public class UpdateBookTests : IntegrationTest
{
    [Fact]
    public async Task PutBookUpdatesBook()
    {
        Writer.Seed(db =>
        {
            db.Books.Add(
                new Book
                {
                    Title = "Dune",
                    Author = "Frank Herbert",
                    Year = 1965
                });
        });

        var request =
            new UpdateBookRequest
            {
                Title = "Dune Messiah",
                Author = "Frank Herbert",
                Year = 1969
            };

        var response = await Client.PutAsJsonAsync("/books/1", request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var book = Reader.Query(db => db.Books.Find(1));

        Assert.NotNull(book);
        Assert.Equal(1969, book.Year);
        Assert.Equal("Frank Herbert", book.Author);
        Assert.Equal("Dune Messiah", book.Title);
        Assert.Equal(1, book.Id);
    }

    [Fact]
    public async Task PutBookReturnsNotFoundWhenBookDoesNotExist()
    {
        var request =
            new UpdateBookRequest
            {
                Title = "Unknown Book",
                Author = "Unknown Author",
                Year = 2000
            };

        var response = await Client.PutAsJsonAsync("/books/9999", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}