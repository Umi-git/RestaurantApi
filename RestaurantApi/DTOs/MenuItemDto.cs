namespace RestaurantApi.DTOs;

public record MenuItemDto(
    int Id,
    string Name,
    string Description,
    string Category,
    decimal Price,
    bool IsAvailable
);

public record CreateMenuItemDto(
    string Name,
    string Description,
    string Category,
    decimal Price
);

public record UpdateMenuItemDto(
    string Name,
    string Description,
    string Category,
    decimal Price,
    bool IsAvailable
);