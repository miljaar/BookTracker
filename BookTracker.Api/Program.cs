using BookTracker.Api.Application;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IBookRepository, InMemoryBookRepository>();

builder.Services.AddScoped<BookService>();

var app = builder.Build();

app.MapGet("/books", async (BookService service) => Results.Ok(await service.GetAllBooks()));

app.MapPost("/books", async (BookService service, CreateBookRequest request) =>
{
    var response = await service.CreateBook(request);
    return Results.Created($"/books/{response.Id}", response);
});

app.Run();

public partial class Program;