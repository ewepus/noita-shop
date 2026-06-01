namespace noita_shop.net.dto.response;

public record PurchaseResponse
{
    public Guid Id { get; init; }
    public Guid WizardId { get; init; }
    public string WizardName { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public decimal Total { get; init; }
    public Guid[] WandIds { get; init; } = [];
    public Guid[] SpellIds { get; init; } = [];
}
