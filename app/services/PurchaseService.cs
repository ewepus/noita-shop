using Microsoft.EntityFrameworkCore;
using noita_shop.net.database;
using noita_shop.net.dto.request;
using noita_shop.net.interfaces;
using noita_shop.net.model;

namespace noita_shop.net.services;

public class PurchaseService(ShopDbContext db, WizardService wizardService) : IPurchaseService
{
    public async Task<IReadOnlyList<Purchase>> GetAllAsync()
        => await db.Purchases
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

    public async Task<Purchase?> GetByIdAsync(Guid id)
        => await db.Purchases
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Purchase> CreateAsync(CreatePurchaseRequest request)
    {
        if (request.WandIds.Count == 0 && request.SpellIds.Count == 0)
        {
            throw new ArgumentException("Покупка должна содержать хотя бы одну палочку или одно заклинание.");
        }

        var wizard = await db.Wizards.FirstOrDefaultAsync(x => x.Id == request.WizardId);
        if (wizard is null)
        {
            throw new InvalidOperationException("Волшебник не найден.");
        }

        var (_, wandTotal) = await ReserveItemsAsync(db.Wands, request.WandIds, "Палочка");
        var (_, spellTotal) = await ReserveItemsAsync(db.Spells, request.SpellIds, "Заклинание");

        wizardService.AddWandsToInventory(wizard, request.WandIds);
        wizardService.AddSpellsToInventory(wizard, request.SpellIds);

        var purchase = new Purchase
        {
            Id = Guid.NewGuid(),
            WizardId = wizard.Id,
            WizardName = wizard.Name,
            CreatedAtUtc = DateTime.UtcNow,
            Total = wandTotal + spellTotal,
            WandIds = request.WandIds.ToArray(),
            SpellIds = request.SpellIds.ToArray()
        };

        db.Purchases.Add(purchase);
        await db.SaveChangesAsync();
        return purchase;
    }

    public async Task<Purchase?> UpdateAsync(Guid id, CreatePurchaseRequest request)
    {
        if (request.WandIds.Count == 0 && request.SpellIds.Count == 0)
        {
            throw new ArgumentException("Покупка должна содержать хотя бы одну палочку или одно заклинание.");
        }

        var purchase = await db.Purchases.FirstOrDefaultAsync(x => x.Id == id);
        if (purchase is null)
        {
            return null;
        }

        var oldWizard = await db.Wizards.FirstAsync(x => x.Id == purchase.WizardId);
        wizardService.RemoveWandsFromInventory(oldWizard, purchase.WandIds);
        wizardService.RemoveSpellsFromInventory(oldWizard, purchase.SpellIds);
        await RestoreStockAsync(db.Wands, purchase.WandIds);
        await RestoreStockAsync(db.Spells, purchase.SpellIds);

        var newWizard = await db.Wizards.FirstOrDefaultAsync(x => x.Id == request.WizardId);
        if (newWizard is null)
        {
            throw new InvalidOperationException("Волшебник не найден.");
        }

        var (_, wandTotal) = await ReserveItemsAsync(db.Wands, request.WandIds, "Палочка");
        var (_, spellTotal) = await ReserveItemsAsync(db.Spells, request.SpellIds, "Заклинание");

        wizardService.AddWandsToInventory(newWizard, request.WandIds);
        wizardService.AddSpellsToInventory(newWizard, request.SpellIds);

        purchase.WizardId = newWizard.Id;
        purchase.WizardName = newWizard.Name;
        purchase.Total = wandTotal + spellTotal;
        purchase.WandIds = request.WandIds.ToArray();
        purchase.SpellIds = request.SpellIds.ToArray();

        await db.SaveChangesAsync();
        return purchase;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var purchase = await db.Purchases.FirstOrDefaultAsync(x => x.Id == id);
        if (purchase is null)
        {
            return false;
        }

        var wizard = await db.Wizards.FirstAsync(x => x.Id == purchase.WizardId);
        wizardService.RemoveWandsFromInventory(wizard, purchase.WandIds);
        wizardService.RemoveSpellsFromInventory(wizard, purchase.SpellIds);
        await RestoreStockAsync(db.Wands, purchase.WandIds);
        await RestoreStockAsync(db.Spells, purchase.SpellIds);

        db.Purchases.Remove(purchase);
        await db.SaveChangesAsync();
        return true;
    }

    private static async Task<(List<T> items, decimal total)> ReserveItemsAsync<T>(
        DbSet<T> set,
        IReadOnlyList<Guid> ids,
        string entityLabel)
        where T : class, IShopItem
    {
        if (ids.Count == 0)
        {
            return ([], 0m);
        }

        var grouped = ids.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        var items = await set
            .Where(x => grouped.Keys.Contains(x.Id))
            .ToListAsync();

        decimal total = 0m;
        foreach (var (itemId, qty) in grouped)
        {
            var item = items.FirstOrDefault(x => x.Id == itemId);
            if (item is null)
            {
                throw new InvalidOperationException($"{entityLabel} {itemId} не найден(а).");
            }

            if (item.Stock < qty)
            {
                throw new InvalidOperationException(
                    $"Недостаточный остаток для \"{item.Name}\". Осталось: {item.Stock}.");
            }

            item.Stock -= qty;
            total += item.Price * qty;
        }

        return (items, total);
    }

    private static async Task RestoreStockAsync<T>(
        DbSet<T> set,
        Guid[] ids)
        where T : class, IShopItem
    {
        if (ids.Length == 0)
        {
            return;
        }

        var distinct = ids.Distinct().ToList();
        var items = await set
            .Where(x => distinct.Contains(x.Id))
            .ToListAsync();

        foreach (var itemId in ids)
        {
            var item = items.First(x => x.Id == itemId);
            item.Stock += 1;
        }
    }
}
