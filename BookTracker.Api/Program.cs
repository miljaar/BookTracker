using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;
using BookTracker.Api.Endpoints;
using BookTracker.Api.Application.BookList;
using BookTracker.Api.Application.GetBookById;
using BookTracker.Api.Application.DeleteBook;
using BookTracker.Api.Application.UpdateBook;
using BookTracker.Api.Application.CreateBook;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("BookTracker"));
});

builder.Services.AddScoped<IBookRepository, EfBookRepository>();

builder.Services.AddScoped<GetBookListQuery>();
builder.Services.AddScoped<GetBookByIdQuery>();

builder.Services.AddScoped<DeleteBookCommandHandler>();
builder.Services.AddScoped<UpdateBookCommandHandler>();
builder.Services.AddScoped<CreateBookCommandHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
    }
}

app.MapBookEndpoints();

app.Run();

public partial class Program;