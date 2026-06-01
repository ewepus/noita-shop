namespace noita_shop.net.dto.request;

public record CreatePurchaseRequest
{
    public Guid WizardId { get; init; }
    public IReadOnlyList<Guid> WandIds { get; init; } = [];
    public IReadOnlyList<Guid> SpellIds { get; init; } = [];
}
