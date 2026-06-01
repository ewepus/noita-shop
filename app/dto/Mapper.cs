using noita_shop.net.dto.response;
using noita_shop.net.model;

namespace noita_shop.net.dto;

public class Mapper : IMapper
{
    public SpellResponse Map(Spell spell) => new()
    {
        Id = spell.Id,
        Name = spell.Name,
        Type = spell.Type,
        ManaCost = spell.ManaCost,
        Price = spell.Price,
        Stock = spell.Stock
    };

    public WandResponse Map(Wand wand) => new()
    {
        Id = wand.Id,
        MaxCapacity = wand.MaxCapacity,
        SpellIds = wand.SpellIds,
        MaxMana = wand.MaxMana,
        Price = wand.Price,
        Stock = wand.Stock
    };

    public WizardResponse Map(Wizard wizard) => new()
    {
        Id = wizard.Id,
        Name = wizard.Name,
        WandInventory = wizard.WandInventory,
        SpellInventory = wizard.SpellInventory
    };

    public PurchaseResponse Map(Purchase purchase) => new()
    {
        Id = purchase.Id,
        WizardId = purchase.WizardId,
        WizardName = purchase.WizardName,
        CreatedAtUtc = purchase.CreatedAtUtc,
        Total = purchase.Total,
        WandIds = purchase.WandIds,
        SpellIds = purchase.SpellIds
    };
}
