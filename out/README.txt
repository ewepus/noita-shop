Демонстрационные ответы HTTP API (сняты при работающем сервисе и PostgreSQL).

Порядок запросов при записи этих файлов:
  1. GET  /                              → GET_root.json
  2. GET  /api/spells                    → GET_api_spells.json
  3. GET  /api/wands                     → GET_api_wands.json
  4. GET  /api/wizards                   → GET_api_wizards.json
  5. POST /api/purchases                 → тело запроса в POST_api_purchases_request.json,
                                          ответ в POST_api_purchases_response.json
  6. GET  /api/purchases                 → GET_api_purchases.json
  7. GET  /api/purchases/{id}            → (тот же id, что в ответе POST) — аналог response
  8. GET  /api/purchases/{id}/receipt    → GET_api_purchases_id_receipt.txt
  9. GET  /api/wands  (после покупки)   → GET_api_wands_after_purchase.json  (остатки -1)
 10. GET  /api/spells (после покупки)   → GET_api_spells_after_purchase.json (остатки -1)
 11. GET  /api/wizards (после покупки)  → GET_api_wizards_after_purchase.json (инвентарь пополнен)
 12. GET  .../wizards/{несущ. id}       → GET_api_wizards_not_found.json (404)
 13. GET  .../purchases/{несущ. id}     → GET_api_purchases_not_found.json (404)

Коды ответов: успешные GET — 200, POST создания покупки — 201, «не найдено» — 404,
ошибки валидации/бизнес-логики — 400.
