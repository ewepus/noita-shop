namespace noita_shop.net.dto.request;

public record CreateWandRequest
{
    public Guid? Id { get; init; }
    public int MaxCapacity { get; init; }
    public IReadOnlyList<Guid> SpellIds { get; init; } = [];
    public int MaxMana { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
}
