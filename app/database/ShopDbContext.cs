using Microsoft.EntityFrameworkCore;
using noita_shop.net.model;

namespace noita_shop.net.database;

public class ShopDbContext(DbContextOptions<ShopDbContext> options) : DbContext(options)
{
    public DbSet<Spell> Spells { get; set; }
    public DbSet<Wand> Wands { get; set; }
    public DbSet<Wizard> Wizards { get; set; }
    public DbSet<Purchase> Purchases { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Spell>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Type).IsRequired();
            entity.Property(x => x.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Wand>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Price).HasPrecision(18, 2);
            entity.Property(x => x.SpellIds)
                .HasColumnType("uuid[]")
                .IsRequired();
        });

        modelBuilder.Entity<Wizard>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.WandInventory)
                .HasColumnType("uuid[]")
                .IsRequired();
            entity.Property(x => x.SpellInventory)
                .HasColumnType("uuid[]")
                .IsRequired();
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.WizardName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Total).HasPrecision(18, 2);
            entity.Property(x => x.WandIds)
                .HasColumnType("uuid[]")
                .IsRequired();
            entity.Property(x => x.SpellIds)
                .HasColumnType("uuid[]")
                .IsRequired();
        });
    }
}
