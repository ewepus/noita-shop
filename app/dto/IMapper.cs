using noita_shop.net.dto.response;
using noita_shop.net.model;

namespace noita_shop.net.dto;

public interface IMapper
{
    SpellResponse Map(Spell spell);

    WandResponse Map(Wand wand);

    WizardResponse Map(Wizard wizard);

    PurchaseResponse Map(Purchase purchase);
}
