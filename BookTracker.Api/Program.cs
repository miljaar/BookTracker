using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;
using BookTracker.Api.Endpoints;
using BookTracker.Api.Application.GetBookDetails;
using BookTracker.Api.Application.GetBookSummaries;
using BookTracker.Api.Application.DeleteBook;
using BookTracker.Api.Application.UpdateBook;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Seeding;
using BookTracker.Api.Wiring;

var builder = WebApplication.CreateBuilder(args);
builder.AddBookTracker();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseBookTracker();
app.UseCors();
app.Run();

public partial class Program;