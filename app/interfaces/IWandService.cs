using noita_shop.net.dto.request;
using noita_shop.net.model;

namespace noita_shop.net.interfaces;

public interface IWandService
{
    Task<IReadOnlyList<Wand>> GetAllAsync();

    Task<Wand?> GetByIdAsync(Guid id);

    Task<Wand> AddAsync(CreateWandRequest request);

    Task<Wand?> UpdateAsync(Guid id, UpdateWandRequest request);

    Task<bool> DeleteAsync(Guid id);
}
