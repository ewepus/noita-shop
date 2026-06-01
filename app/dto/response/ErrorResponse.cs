namespace noita_shop.net.dto.response;

public record ErrorResponse
{
    public string Message { get; init; } = string.Empty;
}
