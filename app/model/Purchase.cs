namespace noita_shop.net.model;

public class Purchase
{
    public Guid Id { get; set; }

    public Guid WizardId { get; set; }

    public string WizardName { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public decimal Total { get; set; }

    public Guid[] WandIds { get; set; } = [];

    public Guid[] SpellIds { get; set; } = [];
}
