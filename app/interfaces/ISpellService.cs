using noita_shop.net.dto.request;
using noita_shop.net.model;

namespace noita_shop.net.interfaces;

public interface ISpellService
{    Task<IReadOnlyList<Spell>> GetAllAsync();

    Task<Spell?> GetByIdAsync(Guid id);

    Task<Spell> AddAsync(CreateSpellRequest request);

    Task<Spell?> UpdateAsync(Guid id, UpdateSpellRequest request);

    Task<bool> DeleteAsync(Guid id);
}
