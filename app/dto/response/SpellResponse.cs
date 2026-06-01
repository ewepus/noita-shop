using noita_shop.net.model;

namespace noita_shop.net.dto.response;

public record SpellResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public SpellType Type { get; init; }
    public int ManaCost { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
}
