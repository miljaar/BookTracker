using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.BookList;
using BookTracker.Api.Domain;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookTracker.Api.Tests.IntegrationTests.BookList;

public class BookListTests : IntegrationTest
{

    [Fact]
    public async Task GetBooksReturnsBooks()
    {
        Writer.Seed(db => db.Books.Add(
            new Book
            {
                Title = "Cannery Row",
                Author = "John Steinbeck",
                Year = 1945
            }
        ));

        var response = await Client.GetAsync("/books");
        var books = await response.Content.ReadFromJsonAsync<List<BookInfo>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(books);
        var bookInfo = Assert.Single(books);
        Assert.Equal("Cannery Row", bookInfo.Title);
        Assert.Equal("John Steinbeck", bookInfo.Author);
    }
}