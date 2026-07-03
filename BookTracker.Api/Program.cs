using BookTracker.Api.Application;
using BookTracker.Api.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IBookRepository, InMemoryBookRepository>();

builder.Services.AddScoped<BookService>();

var app = builder.Build();

app.MapGet("/books", async (BookService service) => Results.Ok(await service.GetAllBooks()));

app.Run();

public partial class Program;