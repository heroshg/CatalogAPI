# CatalogAPI

Microsserviço responsável pelo catálogo de jogos e pelo fluxo de compra da plataforma FiapCloudGames.

## Responsabilidades
- CRUD de jogos
- Biblioteca de jogos por usuário (GameLicense)
- Iniciar fluxo de compra publicando `OrderPlacedEvent`
- Adicionar jogo à biblioteca ao receber `PaymentProcessedEvent` com status `Approved`

## Fluxo de Compra
```
POST /api/games/purchase
       ↓
CatalogAPI publica OrderPlacedEvent
       ↓
PaymentsAPI processa e publica PaymentProcessedEvent
       ↓
CatalogAPI consome → cria GameLicense se Approved
```

## Eventos publicados
| Evento | Quando |
|--------|--------|
| `OrderPlacedEvent` | Ao iniciar uma compra |

## Eventos consumidos
| Evento | Ação |
|--------|------|
| `PaymentProcessedEvent` | Cria GameLicense se status = Approved |

## Variáveis de Ambiente

| Variável | Descrição |
|----------|-----------|
| `ConnectionStrings__Catalog` | Connection string PostgreSQL |
| `Jwt__Key` | Chave secreta JWT (deve ser a mesma do UsersAPI) |
| `Jwt__Issuer` | Issuer do JWT |
| `Jwt__Audience` | Audience do JWT |
| `RabbitMQ__Host` | Host do RabbitMQ |
| `RabbitMQ__Username` | Usuário RabbitMQ |
| `RabbitMQ__Password` | Senha RabbitMQ |

## Endpoints

| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| POST | /api/games | Admin | Registrar jogo |
| GET | /api/games | Auth | Listar jogos |
| GET | /api/games/library/{userId} | Auth | Biblioteca do usuário |
| POST | /api/games/purchase | Auth | Iniciar compra |
