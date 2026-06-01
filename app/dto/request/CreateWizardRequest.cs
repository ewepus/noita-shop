namespace noita_shop.net.dto.request;

public record CreateWizardRequest
{
    public Guid? Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
