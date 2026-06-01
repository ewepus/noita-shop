namespace noita_shop.net.model;

public interface IShopItem
{
    Guid Id { get; }
    string Name { get; }
    decimal Price { get; }
    int Stock { get; set; }
}
