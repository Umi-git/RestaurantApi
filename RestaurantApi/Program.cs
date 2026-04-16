using Microsoft.EntityFrameworkCore;
using RestaurantApi.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseSqlite("Data Source=restaurant.db"));

builder.Services.AddOpenApi();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.Run();