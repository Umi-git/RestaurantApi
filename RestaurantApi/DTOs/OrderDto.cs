namespace RestaurantApi.DTOs;

public record OrderItemInputDto(int MenuItemId, int Quantity);

public record CreateOrderDto(List<OrderItemInputDto> Items);

public record OrderItemDto(
    int MenuItemId,
    string MenuItemName,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal
);

public record OrderDto(
    int Id,
    DateTime CreatedAt,
    decimal TotalAmount,
    string Status,
    List<OrderItemDto> Items
);

public record OrderSummaryDto(int Id, DateTime CreatedAt, decimal TotalAmount, string Status, int ItemCount);