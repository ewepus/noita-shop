namespace noita_shop.net.model;

public class Spell
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public SpellType Type { get; set; }

    public int ManaCost { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }
}
