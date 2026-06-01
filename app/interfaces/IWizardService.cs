using noita_shop.net.dto.request;
using noita_shop.net.model;

namespace noita_shop.net.interfaces;

public interface IWizardService
{
    Task<IReadOnlyList<Wizard>> GetAllAsync();

    Task<Wizard?> GetByIdAsync(Guid id);

    Task<Wizard> AddAsync(CreateWizardRequest request);

    Task<Wizard?> UpdateAsync(Guid id, UpdateWizardRequest request);

    Task<bool> DeleteAsync(Guid id);
}
