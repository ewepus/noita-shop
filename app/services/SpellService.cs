using Microsoft.EntityFrameworkCore;
using noita_shop.net.database;
using noita_shop.net.dto.request;
using noita_shop.net.interfaces;
using noita_shop.net.model;

namespace noita_shop.net.services;

public class SpellService(ShopDbContext db) : ISpellService
{
    public async Task<IReadOnlyList<Spell>> GetAllAsync()
        => await db.Spells
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();

    public async Task<Spell?> GetByIdAsync(Guid id)
        => await db.Spells
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Spell> AddAsync(CreateSpellRequest request)
    {
        ValidateSpellFields(request.Name, request.ManaCost, request.Price, request.Stock);

        var id = request.Id ?? Guid.NewGuid();
        if (await db.Spells.AnyAsync(x => x.Id == id))
        {
            throw new InvalidOperationException($"Заклинание с идентификатором {id} уже существует.");
        }

        var entity = new Spell
        {
            Id = id,
            Name = request.Name.Trim(),
            Type = request.Type,
            ManaCost = request.ManaCost,
            Price = request.Price,
            Stock = request.Stock
        };

        db.Spells.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<Spell?> UpdateAsync(Guid id, UpdateSpellRequest request)
    {
        ValidateSpellFields(request.Name, request.ManaCost, request.Price, request.Stock);

        var entity = await db.Spells.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return null;
        }

        entity.Name = request.Name.Trim();
        entity.Type = request.Type;
        entity.ManaCost = request.ManaCost;
        entity.Price = request.Price;
        entity.Stock = request.Stock;
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await db.Spells.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return false;
        }

        var usedInPurchase = await db.Purchases.AnyAsync(p => p.SpellIds.Contains(id));
        if (usedInPurchase)
        {
            throw new InvalidOperationException(
                "Нельзя удалить заклинание: оно указано в одной или нескольких покупках.");
        }

        var usedInWand = await db.Wands.AnyAsync(w => w.SpellIds.Contains(id));
        if (usedInWand)
        {
            throw new InvalidOperationException(
                "Нельзя удалить заклинание: оно заряжено в одну или несколько палочек.");
        }

        db.Spells.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }

    private static void ValidateSpellFields(string name, int manaCost, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Название заклинания не должно быть пустым.");
        }

        if (manaCost < 0)
        {
            throw new ArgumentException("Стоимость маны не может быть отрицательной.");
        }

        if (price < 0)
        {
            throw new ArgumentException("Цена не может быть отрицательной.");
        }

        if (stock < 0)
        {
            throw new ArgumentException("Остаток на складе не может быть отрицательным.");
        }
    }
}
