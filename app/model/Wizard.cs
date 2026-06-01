namespace noita_shop.net.model;

public class Wizard
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public Guid[] WandInventory { get; set; } = [];

    public Guid[] SpellInventory { get; set; } = [];
}
