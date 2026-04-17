using RestaurantApi.Models;

namespace RestaurantApi.Data;

public static class DatabaseSeeder
{
    public static void Seed(RestaurantDbContext context)
    {
        if (context.MenuItems.Any()) return;

        var items = new List<MenuItem>
        {
            new() { Name = "Margherita Pizza",    Category = "Pizza",    Price = 12.99m, Description = "Classic tomato and mozzarella" },
            new() { Name = "Pepperoni Pizza",     Category = "Pizza",    Price = 14.99m, Description = "Loaded with pepperoni" },
            new() { Name = "Caesar Salad",        Category = "Salad",    Price = 8.99m,  Description = "Romaine, croutons, parmesan" },
            new() { Name = "Greek Salad",         Category = "Salad",    Price = 9.49m,  Description = "Feta, olives, cucumber" },
            new() { Name = "Spaghetti Bolognese", Category = "Pasta",    Price = 13.99m, Description = "Rich meat sauce" },
            new() { Name = "Penne Arrabbiata",    Category = "Pasta",    Price = 11.99m, Description = "Spicy tomato sauce" },
            new() { Name = "Tiramisu",            Category = "Dessert",  Price = 6.99m,  Description = "Classic Italian dessert" },
            new() { Name = "Panna Cotta",         Category = "Dessert",  Price = 5.99m,  Description = "Vanilla cream with berry coulis" },
            new() { Name = "Sparkling Water",     Category = "Drinks",   Price = 2.99m,  Description = "500ml bottle" },
            new() { Name = "Red Wine",            Category = "Drinks",   Price = 7.99m,  Description = "local area wine" },
        };

        context.MenuItems.AddRange(items);
        context.SaveChanges();
    }
}