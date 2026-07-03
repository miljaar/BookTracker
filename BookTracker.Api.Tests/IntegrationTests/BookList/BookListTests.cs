using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.BookList;
using BookTracker.Api.Domain;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookTracker.Api.Tests.IntegrationTests.BookList;

public class BookListTests
{
    private readonly CustomWebApplicationFactory factory = new();

    [Fact]
    public async Task GetBooksReturnsBooks()
    {
        var writer = factory.GetWriter();
        writer.Seed(db => db.Books.Add(
            new Book
            {
                Title = "Cannery Row",
                Author = "John Steinbeck",
                Year = 1945
            }
        ));

        var client = factory.CreateClient();

        var response = await client.GetAsync("/books");
        var books = await response.Content.ReadFromJsonAsync<List<BookInfo>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(books);
        var bookInfo = Assert.Single(books);
        Assert.Equal("Cannery Row", bookInfo.Title);
        Assert.Equal("John Steinbeck", bookInfo.Author);
    }
}