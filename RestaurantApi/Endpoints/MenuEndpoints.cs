using Microsoft.EntityFrameworkCore;
using FluentValidation;
using RestaurantApi.Data;
using RestaurantApi.DTOs;
using RestaurantApi.Models;

namespace RestaurantApi.Endpoints;

public static class MenuEndpoints
{
    public static RouteGroupBuilder MapMenuEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllMenuItems).WithName("GetMenuItems");
        group.MapGet("/{id:int}", GetMenuItemById).WithName("GetMenuItemById");
        group.MapPost("/", CreateMenuItem).WithName("CreateMenuItem");
        group.MapPut("/{id:int}", UpdateMenuItem).WithName("UpdateMenuItem");
        group.MapDelete("/{id:int}", DeleteMenuItem).WithName("DeleteMenuItem");
        return group;
    }

    static async Task<IResult> GetAllMenuItems(
        RestaurantDbContext db,
        string? q,
        string? category,
        decimal? minPrice,
        decimal? maxPrice)
    {
        var query = db.MenuItems.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(m => m.Name.Contains(q));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(m => m.Category == category);

        if (minPrice.HasValue)
            query = query.Where(m => m.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(m => m.Price <= maxPrice.Value);

        var items = await query
            .Select(m => new MenuItemDto(m.Id, m.Name, m.Description, m.Category, m.Price, m.IsAvailable))
            .ToListAsync();

        return Results.Ok(items);
    }

    static async Task<IResult> GetMenuItemById(int id, RestaurantDbContext db)
    {
        var item = await db.MenuItems.FindAsync(id);
        return item is null
            ? Results.NotFound()
            : Results.Ok(new MenuItemDto(item.Id, item.Name, item.Description, item.Category, item.Price, item.IsAvailable));
    }

    static async Task<IResult> CreateMenuItem(
        CreateMenuItemDto dto,
        RestaurantDbContext db,
        IValidator<CreateMenuItemDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());

        var item = new MenuItem
        {
            Name = dto.Name,
            Description = dto.Description,
            Category = dto.Category,
            Price = dto.Price
        };

        db.MenuItems.Add(item);
        await db.SaveChangesAsync();

        return Results.CreatedAtRoute("GetMenuItemById", new { id = item.Id },
            new MenuItemDto(item.Id, item.Name, item.Description, item.Category, item.Price, item.IsAvailable));
    }

    static async Task<IResult> UpdateMenuItem(int id, UpdateMenuItemDto dto, RestaurantDbContext db)
    {
        var item = await db.MenuItems.FindAsync(id);
        if (item is null) return Results.NotFound();

        item.Name = dto.Name;
        item.Description = dto.Description;
        item.Category = dto.Category;
        item.Price = dto.Price;
        item.IsAvailable = dto.IsAvailable;

        await db.SaveChangesAsync();
        return Results.Ok(new MenuItemDto(item.Id, item.Name, item.Description, item.Category, item.Price, item.IsAvailable));
    }

    static async Task<IResult> DeleteMenuItem(int id, RestaurantDbContext db)
    {
        var item = await db.MenuItems.FindAsync(id);
        if (item is null) return Results.NotFound();

        db.MenuItems.Remove(item);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
}