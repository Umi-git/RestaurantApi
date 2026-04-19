using Microsoft.EntityFrameworkCore;
using RestaurantApi.Data;

namespace RestaurantApi.Reports;

public static class ReportEndpoints
{
    public static RouteGroupBuilder MapReportEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/daily", GetDailyReport).WithName("GetDailyReport");
        return group;
    }

    static async Task<IResult> GetDailyReport(RestaurantDbContext db, string? date)
    {
        DateTime targetDate;

        if (!DateTime.TryParse(date, out targetDate))
            targetDate = DateTime.UtcNow.Date;
        else
            targetDate = targetDate.Date;

        var nextDay = targetDate.AddDays(1);

        var orders = await db.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.MenuItem)
            .Where(o => o.CreatedAt >= targetDate && o.CreatedAt < nextDay)
            .ToListAsync();

        var topItems = orders
            .SelectMany(o => o.OrderItems)
            .GroupBy(oi => new { oi.MenuItemId, oi.MenuItem.Name })
            .Select(g => new
            {
                MenuItemId = g.Key.MenuItemId,
                Name = g.Key.Name,
                TotalQuantity = g.Sum(oi => oi.Quantity)
            })
            .OrderByDescending(x => x.TotalQuantity)
            .Take(3)
            .ToList();

        return Results.Ok(new
        {
            Date = targetDate.ToString("yyyy-MM-dd"),
            OrderCount = orders.Count,
            TotalRevenue = orders.Sum(o => o.TotalAmount),
            TopItems = topItems
        });
    }
}