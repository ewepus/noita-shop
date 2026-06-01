namespace noita_shop.net.dto.response;

public record WandResponse
{
    public Guid Id { get; init; }
    public int MaxCapacity { get; init; }
    public Guid[] SpellIds { get; init; } = [];
    public int MaxMana { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
}
