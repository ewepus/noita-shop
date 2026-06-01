namespace noita_shop.net.model;

public class Wand
{
    public Guid Id { get; set; }

    public int MaxCapacity { get; set; }

    public Guid[] SpellIds { get; set; } = [];

    public int MaxMana { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }
}
