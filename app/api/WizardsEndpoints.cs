using noita_shop.net.dto;
using noita_shop.net.dto.request;
using noita_shop.net.dto.response;
using noita_shop.net.interfaces;

namespace noita_shop.net.api;

public static class WizardsEndpoints
{
    public static RouteGroupBuilder MapWizardsEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/wizards").WithTags("Wizards");

        group.MapGet("/", async (IWizardService wizards, IMapper mapper) =>
            {
                var result = await wizards.GetAllAsync();
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить список волшебников")
            .WithDescription("Возвращает всех зарегистрированных волшебников с их инвентарями.")
            .Produces<IEnumerable<WizardResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{wizardId:guid}",
                async (Guid wizardId, IWizardService wizards, IMapper mapper) =>
            {
                var wizard = await wizards.GetByIdAsync(wizardId);
                return wizard is null
                    ? Results.NotFound(new ErrorResponse { Message = "Волшебник не найден." })
                    : Results.Ok(mapper.Map(wizard));
            })
            .WithSummary("Получить волшебника по идентификатору")
            .Produces<WizardResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateWizardRequest body, IWizardService wizards, IMapper mapper) =>
            {
                try
                {
                    var created = await wizards.AddAsync(body);
                    return Results.Created($"/api/wizards/{created.Id}", mapper.Map(created));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Зарегистрировать волшебника")
            .Produces<WizardResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{wizardId:guid}",
                async (Guid wizardId, UpdateWizardRequest body, IWizardService wizards, IMapper mapper) =>
            {
                try
                {
                    var updated = await wizards.UpdateAsync(wizardId, body);
                    return updated is null
                        ? Results.NotFound(new ErrorResponse { Message = "Волшебник не найден." })
                        : Results.Ok(mapper.Map(updated));
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Изменить данные волшебника")
            .Produces<WizardResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/{wizardId:guid}",
                async (Guid wizardId, IWizardService wizards) =>
            {
                try
                {
                    var deleted = await wizards.DeleteAsync(wizardId);
                    return deleted
                        ? Results.NoContent()
                        : Results.NotFound(new ErrorResponse { Message = "Волшебник не найден." });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Удалить волшебника")
            .WithDescription("Нельзя удалить волшебника, если у него есть покупки.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return api;
    }
}
