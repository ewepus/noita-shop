using Microsoft.EntityFrameworkCore;
using noita_shop.net.database;
using noita_shop.net.model;

namespace noita_shop.net.database;

public static class DataSeeder
{
    public static async Task SeedAsync(ShopDbContext db)
    {
        await SeedSpellsAsync(db);
        await SeedWandsAsync(db);
        await SeedWizardsAsync(db);
    }

    private static async Task SeedSpellsAsync(ShopDbContext db)
    {
        if (await db.Spells.AnyAsync())
        {
            return;
        }

        var spells = new[]
        {
            new Spell
            {
                Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                Name = "Spark Bolt",
                Type = SpellType.Projectile,
                ManaCost = 10,
                Price = 150m,
                Stock = 30
            },
            new Spell
            {
                Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                Name = "Chainsaw",
                Type = SpellType.ProjectileModifier,
                ManaCost = 20,
                Price = 320m,
                Stock = 15
            },
            new Spell
            {
                Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"),
                Name = "Teleport",
                Type = SpellType.Utility,
                ManaCost = 50,
                Price = 800m,
                Stock = 5
            }
        };

        db.Spells.AddRange(spells);
        await db.SaveChangesAsync();
    }

    private static async Task SeedWandsAsync(ShopDbContext db)
    {
        if (await db.Wands.AnyAsync())
        {
            return;
        }

        var wands = new[]
        {
            new Wand
            {
                Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb01"),
                MaxCapacity = 5,
                SpellIds =
                [
                    new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                    new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1")
                ],
                MaxMana = 200,
                Price = 1200m,
                Stock = 3
            },
            new Wand
            {
                Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb02"),
                MaxCapacity = 10,
                SpellIds = [],
                MaxMana = 500,
                Price = 3500m,
                Stock = 1
            }
        };

        db.Wands.AddRange(wands);
        await db.SaveChangesAsync();
    }

    private static async Task SeedWizardsAsync(ShopDbContext db)
    {
        if (await db.Wizards.AnyAsync())
        {
            return;
        }

        var wizards = new[]
        {
            new Wizard
            {
                Id = new Guid("cccccccc-cccc-cccc-cccc-cccccccccc01"),
                Name = "Nolla",
                WandInventory = [],
                SpellInventory = []
            },
            new Wizard
            {
                Id = new Guid("cccccccc-cccc-cccc-cccc-cccccccccc02"),
                Name = "Hämis",
                WandInventory = [],
                SpellInventory = []
            }
        };

        db.Wizards.AddRange(wizards);
        await db.SaveChangesAsync();
    }
}
