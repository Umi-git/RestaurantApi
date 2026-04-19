using Microsoft.EntityFrameworkCore;
using FluentValidation;
using RestaurantApi.Data;
using RestaurantApi.DTOs;
using RestaurantApi.Models;

namespace RestaurantApi.Endpoints;

public static class OrderEndpoints
{
    public static RouteGroupBuilder MapOrderEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateOrder).WithName("CreateOrder");
        group.MapGet("/", GetAllOrders).WithName("GetAllOrders");
        group.MapGet("/{id:int}", GetOrderById).WithName("GetOrderById");
        return group;
    }

    static async Task<IResult> CreateOrder(
        CreateOrderDto dto,
        RestaurantDbContext db,
        IValidator<CreateOrderDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());

        // Validate all menu items exist
        var menuItemIds = dto.Items.Select(i => i.MenuItemId).Distinct().ToList();
        var menuItems = await db.MenuItems
            .Where(m => menuItemIds.Contains(m.Id) && m.IsAvailable)
            .ToDictionaryAsync(m => m.Id);

        var missing = menuItemIds.Except(menuItems.Keys).ToList();
        if (missing.Any())
            return Results.BadRequest(new { error = $"Menu items not found or unavailable: {string.Join(", ", missing)}" });

        var order = new Order { CreatedAt = DateTime.UtcNow };

        foreach (var item in dto.Items)
        {
            var menuItem = menuItems[item.MenuItemId];
            order.OrderItems.Add(new OrderItem
            {
                MenuItemId = item.MenuItemId,
                Quantity = item.Quantity,
                UnitPrice = menuItem.Price
            });
        }

        order.TotalAmount = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity);

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        return Results.CreatedAtRoute("GetOrderById", new { id = order.Id }, ToOrderDto(order, menuItems));
    }

    static async Task<IResult> GetAllOrders(RestaurantDbContext db)
    {
        var orders = await db.Orders
            .Include(o => o.OrderItems)
            .Select(o => new OrderSummaryDto(
                o.Id,
                o.CreatedAt,
                o.TotalAmount,
                o.Status,
                o.OrderItems.Sum(oi => oi.Quantity)
            ))
            .ToListAsync();

        return Results.Ok(orders);
    }

    static async Task<IResult> GetOrderById(int id, RestaurantDbContext db)
    {
        var order = await db.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null) return Results.NotFound();

        var menuItems = order.OrderItems.ToDictionary(oi => oi.MenuItemId, oi => oi.MenuItem);
        return Results.Ok(ToOrderDto(order, menuItems));
    }

    static OrderDto ToOrderDto(Order order, Dictionary<int, MenuItem> menuItems) =>
        new(
            order.Id,
            order.CreatedAt,
            order.TotalAmount,
            order.Status,
            order.OrderItems.Select(oi => new OrderItemDto(
                oi.MenuItemId,
                menuItems[oi.MenuItemId].Name,
                oi.Quantity,
                oi.UnitPrice,
                oi.UnitPrice * oi.Quantity
            )).ToList()
        );
}