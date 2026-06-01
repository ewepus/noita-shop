using Microsoft.EntityFrameworkCore;
using noita_shop.net.database;
using noita_shop.net.dto.request;
using noita_shop.net.interfaces;
using noita_shop.net.model;

namespace noita_shop.net.services;

public class WizardService(ShopDbContext db) : IWizardService
{
    private const int MaxWands = 4;
    private const int MaxSpells = 20;

    public async Task<IReadOnlyList<Wizard>> GetAllAsync()
        => await db.Wizards
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();

    public async Task<Wizard?> GetByIdAsync(Guid id)
        => await db.Wizards
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Wizard> AddAsync(CreateWizardRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Имя волшебника не должно быть пустым.");
        }

        var id = request.Id ?? Guid.NewGuid();
        if (await db.Wizards.AnyAsync(x => x.Id == id))
        {
            throw new InvalidOperationException($"Волшебник с идентификатором {id} уже существует.");
        }

        var entity = new Wizard
        {
            Id = id,
            Name = request.Name.Trim(),
            WandInventory = [],
            SpellInventory = []
        };

        db.Wizards.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<Wizard?> UpdateAsync(Guid id, UpdateWizardRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Имя волшебника не должно быть пустым.");
        }

        var entity = await db.Wizards.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return null;
        }

        entity.Name = request.Name.Trim();
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await db.Wizards.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return false;
        }

        var hasPurchases = await db.Purchases.AnyAsync(p => p.WizardId == id);
        if (hasPurchases)
        {
            throw new InvalidOperationException("Нельзя удалить волшебника: есть связанные покупки.");
        }

        db.Wizards.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }

    public void AddWandsToInventory(Wizard wizard, IEnumerable<Guid> wandIds)
    {
        var updated = wizard.WandInventory.Concat(wandIds).ToArray();
        if (updated.Length > MaxWands)
        {
            throw new InvalidOperationException(
                $"Инвентарь палочек переполнен: максимум {MaxWands}, попытка добавить до {updated.Length}.");
        }

        wizard.WandInventory = updated;
    }

    public void AddSpellsToInventory(Wizard wizard, IEnumerable<Guid> spellIds)
    {
        var updated = wizard.SpellInventory.Concat(spellIds).ToArray();
        if (updated.Length > MaxSpells)
        {
            throw new InvalidOperationException(
                $"Инвентарь заклинаний переполнен: максимум {MaxSpells}, попытка добавить до {updated.Length}.");
        }

        wizard.SpellInventory = updated;
    }

    public void RemoveWandsFromInventory(Wizard wizard, Guid[] wandIds)
    {
        var list = wizard.WandInventory.ToList();
        foreach (var id in wandIds)
        {
            list.Remove(id);
        }

        wizard.WandInventory = list.ToArray();
    }

    public void RemoveSpellsFromInventory(Wizard wizard, Guid[] spellIds)
    {
        var list = wizard.SpellInventory.ToList();
        foreach (var id in spellIds)
        {
            list.Remove(id);
        }

        wizard.SpellInventory = list.ToArray();
    }
}
