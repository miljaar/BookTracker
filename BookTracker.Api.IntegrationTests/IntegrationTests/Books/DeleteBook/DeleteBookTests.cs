using System.Net;
using BookTracker.Api.Domain.Books;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.IntegrationTests.Books.DeleteBook;

[Collection(PostgreSqlCollection.Name)]
public class DeleteBookTests(PostgreSqlFixture database) : IntegrationTest(database)
{

    [Fact]
    public async Task DeleteBookRemovesBook()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

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
        await AuthenticateAsMember(MemberRole.Administrator);

        var response = await Client.DeleteAsync("/books/9999");

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }
}