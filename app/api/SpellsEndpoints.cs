using noita_shop.net.dto;
using noita_shop.net.dto.request;
using noita_shop.net.dto.response;
using noita_shop.net.interfaces;

namespace noita_shop.net.api;

public static class SpellsEndpoints
{
    public static RouteGroupBuilder MapSpellsEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/spells").WithTags("Spells");

        group.MapGet("/", async (ISpellService spells, IMapper mapper) =>
            {
                var result = await spells.GetAllAsync();
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить список заклинаний")
            .WithDescription("Возвращает весь каталог заклинаний с типами, стоимостью маны и остатками.")
            .Produces<IEnumerable<SpellResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{spellId:guid}",
                async (Guid spellId, ISpellService spells, IMapper mapper) =>
            {
                var spell = await spells.GetByIdAsync(spellId);
                return spell is null
                    ? Results.NotFound(new ErrorResponse { Message = "Заклинание не найдено." })
                    : Results.Ok(mapper.Map(spell));
            })
            .WithSummary("Получить заклинание по идентификатору")
            .Produces<SpellResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateSpellRequest body, ISpellService spells, IMapper mapper) =>
            {
                try
                {
                    var created = await spells.AddAsync(body);
                    return Results.Created($"/api/spells/{created.Id}", mapper.Map(created));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Добавить заклинание")
            .Produces<SpellResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{spellId:guid}",
                async (Guid spellId, UpdateSpellRequest body, ISpellService spells, IMapper mapper) =>
            {
                try
                {
                    var updated = await spells.UpdateAsync(spellId, body);
                    return updated is null
                        ? Results.NotFound(new ErrorResponse { Message = "Заклинание не найдено." })
                        : Results.Ok(mapper.Map(updated));
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Изменить заклинание")
            .Produces<SpellResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/{spellId:guid}",
                async (Guid spellId, ISpellService spells) =>
            {
                try
                {
                    var deleted = await spells.DeleteAsync(spellId);
                    return deleted
                        ? Results.NoContent()
                        : Results.NotFound(new ErrorResponse { Message = "Заклинание не найдено." });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Удалить заклинание")
            .WithDescription("Нельзя удалить заклинание, если оно фигурирует в покупках или заряжено в палочку.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return api;
    }
}
