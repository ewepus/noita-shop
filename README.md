# noita-shop

Магазин заклинаний на `ASP.NET Core Minimal API`.

---

## 1) Что это за проект

`noita-shop` демонстрирует базовый backend магазина в стиле игры Noita:

- каталог **заклинаний** (Spell): тип, стоимость маны, цена, остаток;
- каталог **палочек** (Wand): вместимость, упорядоченный список заклинаний в слотах, запас маны, цена, остаток;
- **волшебники** (Wizard): покупатели с инвентарём — до 4 палочек и до 20 заклинаний;
- **покупки** (Purchase): списание со склада, пополнение инвентаря волшебника, откат при удалении;
- выгрузка текстового **чека** по покупке.

---

## 2) Технологии

- `.NET 10`
- `ASP.NET Core Minimal API`
- `Entity Framework Core`
- `Npgsql` (PostgreSQL provider)
- `Swagger / OpenAPI` (Swashbuckle)

---

## 3) Модели домена

### Spell
| Поле | Тип | Описание |
|---|---|---|
| Id | Guid | Идентификатор |
| Name | string | Название |
| Type | SpellType | `Projectile` / `Utility` / `ProjectileModifier` |
| ManaCost | int | Стоимость маны |
| Price | decimal | Цена в магазине |
| Stock | int | Остаток на складе |

### Wand
| Поле | Тип | Описание |
|---|---|---|
| Id | Guid | Идентификатор |
| MaxCapacity | int | Максимум заклинаний в слотах |
| SpellIds | Guid[] | Упорядоченный список заклинаний (≤ MaxCapacity) |
| MaxMana | int | Максимальный запас маны |
| Price | decimal | Цена в магазине |
| Stock | int | Остаток на складе |

### Wizard
| Поле | Тип | Описание |
|---|---|---|
| Id | Guid | Идентификатор |
| Name | string | Имя волшебника |
| WandInventory | Guid[] | Палочки в инвентаре (не более **4**) |
| SpellInventory | Guid[] | Заклинания в инвентаре (не более **20**) |

---

## 4) Структура проекта

```
noita-shop/
├── NoitaShop.sln
├── app/
│   ├── NoitaShop.csproj
│   ├── Program.cs
│   ├── api/               — endpoint-модули
│   │   ├── SpellsEndpoints.cs
│   │   ├── WandsEndpoints.cs
│   │   ├── WizardsEndpoints.cs
│   │   └── PurchasesEndpoints.cs
│   ├── database/
│   │   └── ShopDbContext.cs
│   ├── dto/
│   │   ├── IMapper.cs / Mapper.cs
│   │   ├── request/       — входные DTO
│   │   └── response/      — выходные DTO
│   ├── interfaces/        — контракты сервисов
│   ├── model/             — доменные сущности
│   └── services/          — бизнес-логика
└── out/                   — примеры ответов API
```

---

## 5) Конфигурация PostgreSQL

Строки подключения (в каталоге `app`):

- `appsettings.json` → база `noita_shop_db`
- `appsettings.Development.json` → база `noita_shop_db_dev`

По умолчанию: `localhost:5432`, пользователь `postgres`, пароль `postgres`.

В режиме **Development** при старте вызывается `EnsureCreated` (таблицы создаются автоматически).

---

## 6) Как запустить

```bash
# из корня репозитория
dotnet run --project app/NoitaShop.csproj

# или из каталога app
cd app && dotnet run

# сборка всего решения
dotnet build NoitaShop.sln
```

---

## 7) Swagger

После запуска: `http://localhost:5209/swagger`

---

## 8) Основные endpoint-ы

**Заклинания** (`/api/spells`): `GET` список / по id, `POST`, `PUT`, `DELETE`  
> Нельзя удалить заклинание, если оно фигурирует в покупках или заряжено в палочку.

**Палочки** (`/api/wands`): `GET` список / по id, `POST`, `PUT`, `DELETE`  
> При создании/обновлении: `SpellIds.Count ≤ MaxCapacity`, все id должны существовать.  
> Нельзя удалить палочку, если она фигурирует в покупках.

**Волшебники** (`/api/wizards`): `GET` список / по id, `POST`, `PUT`, `DELETE`  
> Инвентарь обновляется автоматически при покупках. Нельзя удалить, если есть покупки.

**Покупки** (`/api/purchases`): `GET` список / по id, `POST`, `PUT`, `DELETE`, `GET .../receipt`  
> `POST` / `PUT`: списывает товары со склада, добавляет в инвентарь волшебника.  
> `DELETE`: возвращает товары на склад, убирает из инвентаря.  
> Лимиты инвентаря волшебника проверяются при каждой покупке.

---

## 9) Пример создания покупки

`POST /api/purchases`

```json
{
  "wizardId": "cccccccc-cccc-cccc-cccc-cccccccccc01",
  "wandIds": [
    "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb01"
  ],
  "spellIds": [
    "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1",
    "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"
  ]
}
```

Ответ `201 Created`:

```json
{
  "id": "dddddddd-dddd-dddd-dddd-dddddddddd01",
  "wizardId": "cccccccc-cccc-cccc-cccc-cccccccccc01",
  "wizardName": "Nolla",
  "createdAtUtc": "2026-05-04T13:49:19.258749Z",
  "total": 1670.0,
  "wandIds": ["bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb01"],
  "spellIds": [
    "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1",
    "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"
  ]
}
```

---

## 10) Быстрая проверка API

Файл `app/noita-shop.net.http` содержит готовые запросы для IDE (VS Code REST Client, Rider).
