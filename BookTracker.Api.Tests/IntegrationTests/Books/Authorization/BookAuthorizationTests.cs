using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Books.CreateBook;
using BookTracker.Api.Application.Books.UpdateBook;
using BookTracker.Api.Domain.Books;

namespace BookTracker.Api.Tests.IntegrationTests.Books.Authorization;

public class BookAuthorizationTests : IntegrationTest
{
    [Fact]
    public async Task CreateBookRequiresAuthentication()
    {
        var request = new CreateBookRequest
        {
            Title = "Dune",
            Author = "Frank Herbert",
            Year = 1965
        };

        var response = await Client.PostAsJsonAsync("/books", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);

        var count = Reader.Query(db => db.Books.Count());
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task UpdateBookRequiresAuthentication()
    {
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

        var request =
                    new UpdateBookRequest
                    {
                        Title = "The Shining",
                        Author = "Carl Moerman",
                        Year = 1969
                    };

        var response = await Client.PutAsJsonAsync("/books/1", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);

        var book = Reader.Query(db => db.Books.Find(1));

        Assert.NotNull(book);
        Assert.Equal(1965, book.Year);
        Assert.Equal("Frank Herbert", book.Author.Value);
        Assert.Equal("Dune", book.Title.Value);
    }

    [Fact]
    public async Task DeleteBookRequiresAuthentication()
    {
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

        var response = await Client.DeleteAsync("/books/1");
        await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);

        var book = Reader.Query(db => db.Books.Find(1));

        Assert.NotNull(book);
        Assert.Equal(1965, book.Year);
        Assert.Equal("Frank Herbert", book.Author.Value);
        Assert.Equal("Dune", book.Title.Value);
    }

    [Fact]
    public async Task GetBooksDoesNotRequireAuthentication()
    {
        var response = await Client.GetAsync("/books");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetBookDetailsDoesNotRequireAuthentication()
    {
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

        var response = await Client.GetAsync("/books/1");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegularMemberCannotCreateBook()
    {
        await AuthenticateAsMember();

        var request = new CreateBookRequest
        {
            Title = "Dune",
            Author = "Frank Herbert",
            Year = 1965
        };

        var response = await Client.PostAsJsonAsync("/books", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);

        var count = Reader.Query(db => db.Books.Count());

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task RegularMemberCannotUpdateBook()
    {
        await AuthenticateAsMember();

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

        var request = new CreateBookRequest
        {
            Title = "Somewhere",
            Author = "Fance writer",
            Year = 2026
        };

        var response = await Client.PutAsJsonAsync("/books/1", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);

        var book = Reader.Query(db => db.Books.Find(1));

        Assert.NotNull(book);
        Assert.Equal(1965, book.Year);
        Assert.Equal("Frank Herbert", book.Author.Value);
        Assert.Equal("Dune", book.Title.Value);
    }

    [Fact]
    public async Task RegularMemberCannotDeleteBook()
    {
        await AuthenticateAsMember();

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

        var response = await Client.DeleteAsync("/books/1");

        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);

        var count = Reader.Query(db => db.Books.Count());

        Assert.Equal(1, count);
    }
}