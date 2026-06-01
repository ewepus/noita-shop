using Microsoft.EntityFrameworkCore;
using noita_shop.net.database;
using noita_shop.net.dto.request;
using noita_shop.net.interfaces;
using noita_shop.net.model;

namespace noita_shop.net.services;

public class WandService(ShopDbContext db) : IWandService
{
    public async Task<IReadOnlyList<Wand>> GetAllAsync()
        => await db.Wands
            .AsNoTracking()
            .OrderBy(x => x.MaxMana)
            .ToListAsync();

    public async Task<Wand?> GetByIdAsync(Guid id)
        => await db.Wands
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Wand> AddAsync(CreateWandRequest request)
    {
        ValidateWandFields(request.MaxCapacity, request.MaxMana, request.Price, request.Stock);

        if (request.SpellIds.Count > request.MaxCapacity)
        {
            throw new ArgumentException(
                $"Количество заклинаний ({request.SpellIds.Count}) превышает вместимость палочки ({request.MaxCapacity}).");
        }

        var id = request.Id ?? Guid.NewGuid();
        if (await db.Wands.AnyAsync(x => x.Id == id))
        {
            throw new InvalidOperationException($"Палочка с идентификатором {id} уже существует.");
        }

        await ValidateSpellIdsExistAsync(request.SpellIds);

        var entity = new Wand
        {
            Id = id,
            MaxCapacity = request.MaxCapacity,
            SpellIds = request.SpellIds.ToArray(),
            MaxMana = request.MaxMana,
            Price = request.Price,
            Stock = request.Stock
        };

        db.Wands.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<Wand?> UpdateAsync(Guid id, UpdateWandRequest request)
    {
        ValidateWandFields(request.MaxCapacity, request.MaxMana, request.Price, request.Stock);

        if (request.SpellIds.Count > request.MaxCapacity)
        {
            throw new ArgumentException(
                $"Количество заклинаний ({request.SpellIds.Count}) превышает вместимость палочки ({request.MaxCapacity}).");
        }

        var entity = await db.Wands.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return null;
        }

        await ValidateSpellIdsExistAsync(request.SpellIds);

        entity.MaxCapacity = request.MaxCapacity;
        entity.SpellIds = request.SpellIds.ToArray();
        entity.MaxMana = request.MaxMana;
        entity.Price = request.Price;
        entity.Stock = request.Stock;
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await db.Wands.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return false;
        }

        var usedInPurchase = await db.Purchases.AnyAsync(p => p.WandIds.Contains(id));
        if (usedInPurchase)
        {
            throw new InvalidOperationException(
                "Нельзя удалить палочку: она указана в одной или нескольких покупках.");
        }

        db.Wands.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }

    private async Task ValidateSpellIdsExistAsync(IReadOnlyList<Guid> spellIds)
    {
        if (spellIds.Count == 0)
        {
            return;
        }

        var distinct = spellIds.Distinct().ToList();
        var found = await db.Spells
            .Where(x => distinct.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync();

        var missing = distinct.Except(found).ToList();
        if (missing.Count > 0)
        {
            throw new InvalidOperationException(
                $"Заклинания не найдены: {string.Join(", ", missing)}.");
        }
    }

    private static void ValidateWandFields(int maxCapacity, int maxMana, decimal price, int stock)
    {
        if (maxCapacity <= 0)
        {
            throw new ArgumentException("Вместимость палочки должна быть больше нуля.");
        }

        if (maxMana <= 0)
        {
            throw new ArgumentException("Максимальный запас маны должен быть больше нуля.");
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
