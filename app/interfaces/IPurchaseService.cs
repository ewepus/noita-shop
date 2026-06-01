using noita_shop.net.dto.request;
using noita_shop.net.model;

namespace noita_shop.net.interfaces;

public interface IPurchaseService
{
    Task<IReadOnlyList<Purchase>> GetAllAsync();

    Task<Purchase?> GetByIdAsync(Guid id);

    Task<Purchase> CreateAsync(CreatePurchaseRequest request);

    Task<Purchase?> UpdateAsync(Guid id, CreatePurchaseRequest request);

    Task<bool> DeleteAsync(Guid id);
}
