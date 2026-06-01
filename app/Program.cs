using Microsoft.EntityFrameworkCore;
using noita_shop.net.api;
using noita_shop.net.database;
using noita_shop.net.dto;
using noita_shop.net.interfaces;
using noita_shop.net.services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Noita Shop API",
        Version = "v1",
        Description = "API магазина заклинаний: заклинания, палочки, покупки, инвентарь, чек."
    });
});

var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("Не найдена строка подключения ConnectionStrings:Postgres.");

builder.Services.AddDbContext<ShopDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<ISpellService, SpellService>();
builder.Services.AddScoped<IWandService, WandService>();
builder.Services.AddScoped<WizardService>();
builder.Services.AddScoped<IWizardService>(sp => sp.GetRequiredService<WizardService>());
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<IMapper, Mapper>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
        await db.Database.EnsureCreatedAsync();
        await DataSeeder.SeedAsync(db);
    }

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Noita Shop API v1");
        options.RoutePrefix = "swagger";
    });
}

var api = app.MapGroup("/api");
api.MapSpellsEndpoints();
api.MapWandsEndpoints();
api.MapWizardsEndpoints();
api.MapPurchasesEndpoints();

app.MapGet("/", () => Results.Ok(new
{
    message = "Noita Shop API работает. Откройте /api/spells, /api/wands, /api/wizards, /api/purchases."
}));

await app.RunAsync();
