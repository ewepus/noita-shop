using noita_shop.net.dto;
using noita_shop.net.dto.request;
using noita_shop.net.dto.response;
using noita_shop.net.interfaces;

namespace noita_shop.net.api;

public static class WandsEndpoints
{
    public static RouteGroupBuilder MapWandsEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/wands").WithTags("Wands");

        group.MapGet("/", async (IWandService wands, IMapper mapper) =>
            {
                var result = await wands.GetAllAsync();
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить список палочек")
            .WithDescription("Возвращает весь каталог палочек с вместимостью, запасом маны и остатками.")
            .Produces<IEnumerable<WandResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{wandId:guid}",
                async (Guid wandId, IWandService wands, IMapper mapper) =>
            {
                var wand = await wands.GetByIdAsync(wandId);
                return wand is null
                    ? Results.NotFound(new ErrorResponse { Message = "Палочка не найдена." })
                    : Results.Ok(mapper.Map(wand));
            })
            .WithSummary("Получить палочку по идентификатору")
            .Produces<WandResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateWandRequest body, IWandService wands, IMapper mapper) =>
            {
                try
                {
                    var created = await wands.AddAsync(body);
                    return Results.Created($"/api/wands/{created.Id}", mapper.Map(created));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Добавить палочку")
            .WithDescription("SpellIds задаёт начальный упорядоченный список заклинаний в слотах палочки (не более MaxCapacity).")
            .Produces<WandResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{wandId:guid}",
                async (Guid wandId, UpdateWandRequest body, IWandService wands, IMapper mapper) =>
            {
                try
                {
                    var updated = await wands.UpdateAsync(wandId, body);
                    return updated is null
                        ? Results.NotFound(new ErrorResponse { Message = "Палочка не найдена." })
                        : Results.Ok(mapper.Map(updated));
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Изменить палочку")
            .Produces<WandResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/{wandId:guid}",
                async (Guid wandId, IWandService wands) =>
            {
                try
                {
                    var deleted = await wands.DeleteAsync(wandId);
                    return deleted
                        ? Results.NoContent()
                        : Results.NotFound(new ErrorResponse { Message = "Палочка не найдена." });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Удалить палочку")
            .WithDescription("Нельзя удалить палочку, если она фигурирует в покупках.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return api;
    }
}
