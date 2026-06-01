namespace noita_shop.net.dto.response;

public record WizardResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid[] WandInventory { get; init; } = [];
    public Guid[] SpellInventory { get; init; } = [];
}
