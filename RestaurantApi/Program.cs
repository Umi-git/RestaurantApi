using Microsoft.EntityFrameworkCore;
using RestaurantApi.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseSqlite("Data Source=restaurant.db"));

builder.Services.AddOpenApi();

builder.Services.AddScoped<IValidator<CreateMenuItemDto>, CreateMenuItemValidator>();
builder.Services.AddScoped<IValidator<CreateOrderDto>, CreateOrderValidator>();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();
    db.Database.EnsureCreated();
    DatabaseSeeder.Seed(db);
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Route groups
var menu = app.MapGroup("/menu").WithTags("Menu");
menu.MapMenuEndpoints();

var orders = app.MapGroup("/orders").WithTags("Orders");
orders.MapOrderEndpoints();

app.Run();