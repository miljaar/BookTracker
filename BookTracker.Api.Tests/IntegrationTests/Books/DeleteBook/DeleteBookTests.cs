using System.Net;
using BookTracker.Api.Domain;

namespace BookTracker.Api.Tests.IntegrationTests.Books.DeleteBook;

public class DeleteBookTests : IntegrationTest
{

    [Fact]
    public async Task DeleteBookRemovesBook()
    {
        await AuthenticateAsMember();

        Writer.Seed(db =>
        {
            db.Books.Add(
                new Book
                {
                    Id = 1,
                    Title = new BookTitle("Dune"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1965
                });
        });

        var response = await Client.DeleteAsync("/books/1");

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var book = Reader.Query(db => db.Books.Find(1));

        Assert.Null(book);
    }

    [Fact]
    public async Task DeleteBookReturnsNotFoundWhenBookDoesNotExist()
    {
        await AuthenticateAsMember();

        var response = await Client.DeleteAsync("/books/9999");

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }
}