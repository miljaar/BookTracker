using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;
using BookTracker.Api.Endpoints;
using BookTracker.Api.Application.Books.GetBookDetails;
using BookTracker.Api.Application.Books.GetBookSummaries;
using BookTracker.Api.Application.Books.DeleteBook;
using BookTracker.Api.Application.Books.UpdateBook;
using BookTracker.Api.Application.Books.CreateBook;
using BookTracker.Api.Seeding;
using BookTracker.Api.Wiring;

var builder = WebApplication.CreateBuilder(args);
builder.AddBookTracker();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors();
app.UseBookTracker();
app.Run();

public partial class Program;