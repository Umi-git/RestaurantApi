using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RestaurantApi.Data;
using RestaurantApi.DTOs;
using RestaurantApi.Endpoints;
using RestaurantApi.Middleware;
using RestaurantApi.Reports;
using RestaurantApi.Validators;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseSqlite("Data Source=restaurant.db"));

// Validators
builder.Services.AddScoped<IValidator<CreateMenuItemDto>, CreateMenuItemValidator>();
builder.Services.AddScoped<IValidator<CreateOrderDto>, CreateOrderValidator>();

// Error handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();
    db.Database.EnsureCreated();
    DatabaseSeeder.Seed(db);
}

// Middleware
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Restaurant API";
    });
}

// Route groups
var menu = app.MapGroup("/menu").WithTags("Menu");
menu.MapMenuEndpoints();

var orders = app.MapGroup("/orders").WithTags("Orders");
orders.MapOrderEndpoints();

var reports = app.MapGroup("/reports").WithTags("Reports");
reports.MapReportEndpoints();

app.Run();