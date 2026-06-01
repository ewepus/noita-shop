using System.Text;
using noita_shop.net.interfaces;
using noita_shop.net.model;

namespace noita_shop.net.services;

public class ReceiptService : IReceiptService
{
    public ReceiptFile BuildReceipt(Purchase purchase)
    {
        var builder = new StringBuilder();
        builder.AppendLine("NOITA SHOP.NET");
        builder.AppendLine("Кассовый чек");
        builder.AppendLine(new string('-', 40));
        builder.AppendLine($"Покупка: {purchase.Id}");
        builder.AppendLine($"Дата (UTC): {purchase.CreatedAtUtc:yyyy-MM-dd HH:mm:ss}");
        builder.AppendLine($"Волшебник: {purchase.WizardName} ({purchase.WizardId})");
        builder.AppendLine(new string('-', 40));

        if (purchase.WandIds.Length > 0)
        {
            builder.AppendLine("Палочки (id):");
            foreach (var wandId in purchase.WandIds)
            {
                builder.AppendLine($"- {wandId}");
            }
        }

        if (purchase.SpellIds.Length > 0)
        {
            builder.AppendLine("Заклинания (id):");
            foreach (var spellId in purchase.SpellIds)
            {
                builder.AppendLine($"- {spellId}");
            }
        }

        builder.AppendLine(new string('-', 40));
        builder.AppendLine($"ИТОГО: {purchase.Total:F2} GOLD");

        return new ReceiptFile
        {
            FileName = $"receipt-{purchase.Id}.txt",
            ContentType = "text/plain; charset=utf-8",
            Content = Encoding.UTF8.GetBytes(builder.ToString())
        };
    }
}
