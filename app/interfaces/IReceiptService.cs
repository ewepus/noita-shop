using noita_shop.net.model;

namespace noita_shop.net.interfaces;
public interface IReceiptService
{
    ReceiptFile BuildReceipt(Purchase purchase);
}
