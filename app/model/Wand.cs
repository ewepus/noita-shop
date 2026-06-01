namespace noita_shop.net.model;

public class Wand : IShopItem
{
    public Guid Id { get; set; }

    public int MaxCapacity { get; set; }

    public Guid[] SpellIds { get; set; } = [];

    public int MaxMana { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string Name => $"Wand (cap={MaxCapacity}, mana={MaxMana})";
}
