using System.Net;
using BookTracker.Api.Application.Books.GetBookDetails;
using BookTracker.Api.Domain.Books;

namespace BookTracker.Api.IntegrationTests.Books.GetBookDetails;

[Collection(PostgreSqlCollection.Name)]
public class GetBookDetailsTests(PostgreSqlFixture database) : IntegrationTest(database)
{
    [Fact]
    public async Task GetBookDetailsReturnsBookDetail()
    {
        Writer.Seed(db =>
            db.Books.Add(
                new Book
                {
                    Title = new BookTitle("Dune"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1965
                }
            )
        );

        var response = await Client.GetAsync("/books/1");
        var book = await response.ReadJsonAs<GetBookDetailsResponse>(HttpStatusCode.OK);

        Assert.Equal(1, book.Id);
        Assert.Equal("Dune", book.Title);
        Assert.Equal("Frank Herbert", book.Author);
        Assert.Equal(1965, book.Year);
    }

    [Fact]
    public async Task GetBookDetailsReturnsNotFoundWhenBookDoesNotExist()
    {
        var response = await Client.GetAsync("/books/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}