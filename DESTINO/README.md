# Novvi API

E-commerce simples (Usuários, Endereços, Funcionários, Produtos, Pedidos e Pagamentos) construído em
.NET 10 com Clean Architecture, para os Checkpoints (CP2 → CP5) da FIAP.

## Integrantes
- Mathaus — RM 564146
- Vinicius Luis — RM 563340
- Luan peixoto — RM 562258

## Domínio
Loja com Usuários (que possuem Endereços e um histórico de Pedidos), Funcionários (que atendem
Pedidos), Produtos (N:N com Pedido) e Pagamento (1:1 com Pedido, tipo Cartão ou Pix).

## Stack
- .NET 10 / ASP.NET Core Web API
- Entity Framework Core 9 + **Oracle** (`Oracle.EntityFrameworkCore`)
- Swagger / Swashbuckle
- xUnit + Moq

## Arquitetura (Clean Architecture)
```
Novvi.Domain          -> Entidades, regras de negócio, exceptions de domínio
Novvi.Application     -> DTOs, interfaces (repositórios/serviços), serviços de aplicação
Novvi.Infrastructure   -> DbContext, configurações EF, repositórios (genérico + específicos)
Novvi.Api             -> Controllers, Swagger, GlobalExceptionHandler, Health Checks, Program.cs
Novvi.Domain.Tests      -> Testes de unidade do Domain (sem mock)
Novvi.Application.Tests -> Testes de unidade da Application (com mock de repositório)
```

## Como executar

### 1. Configurar a connection string
Edite `Novvi.Api/appsettings.json` (ou `appsettings.Development.json` / variável de ambiente /
`dotnet user-secrets`) com as credenciais do Oracle do laboratório FIAP:

```json
"ConnectionStrings": {
  "Novvi_Context_Oracle": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=oracle.fiap.com.br:1521/ORCL;"
}
```

### 2. Gerar e aplicar as migrations (CP2)
O repositório já contém todo o mapeamento (DbContext + `IEntityTypeConfiguration<T>`), mas as
migrations precisam ser geradas com o SDK/CLI do `dotnet ef` na sua máquina (com acesso ao Oracle):

```bash
dotnet tool install --global dotnet-ef   # se ainda não tiver
dotnet restore

dotnet ef migrations add InitialCreate \
  --project Novvi.Infrastructure \
  --startup-project Novvi.Api

dotnet ef database update \
  --project Novvi.Infrastructure \
  --startup-project Novvi.Api
```

### 3. Rodar a API
```bash
dotnet run --project Novvi.Api
```
- Swagger: `https://localhost:<porta>/swagger`
- Health check: `https://localhost:<porta>/health`

### 4. Rodar os testes
```bash
dotnet test
```
Deve rodar `Novvi.Domain.Tests` (regras de negócio, sem mock) e `Novvi.Application.Tests`
(serviços de aplicação, com mock de repositório via Moq) — todos verdes.

## Endpoints principais
| Recurso | Rotas |
|---|---|
| Produtos | `GET/POST /api/produtos`, `GET/PUT/DELETE /api/produtos/{id}` |
| Funcionários | `GET/POST /api/funcionarios`, `GET/PUT/DELETE /api/funcionarios/{id}` |
| Usuários | `GET/POST /api/usuarios`, `GET/PUT/DELETE /api/usuarios/{id}`, `POST /api/usuarios/{id}/enderecos` |
| Pedidos | `GET/POST /api/pedidos`, `GET/DELETE /api/pedidos/{id}`, `POST /api/pedidos/{id}/pagamento` |

Exemplos de requisição prontos em `Novvi.Api/Novvi.Api.http`.

## Repositório genérico (CP3)
`IRepository<T>` (em `Novvi.Application/Interfaces/Repositories`) define o contrato padrão
(`Add`, `GetById`, `GetAll`, `ExistsById`, `Update`, `Delete`), implementado por `Repository<T>`
via EF Core na Infrastructure e registrado em `services.AddScoped(typeof(IRepository<>), typeof(Repository<>))`.

- **Uso direto do genérico**: `ProdutoService` e `FuncionarioService` usam `IRepository<T>` sem
  repositório específico (CRUD simples).
- **Convivência com específico**: `Usuario` e `Pedido` precisam de consultas com `Include`
  (endereços, produtos, pagamento), então `IUsuarioRepository`/`IPedidoRepository` estendem
  `IRepository<T>` e reaproveitam sua implementação (`UsuarioRepository : Repository<Usuario>`).
- `Delete` é lógico (soft delete via `EntidadeBase.Ativo`), preservando o histórico de pedidos.

## Tratamento global de erros (CP3)
`GlobalExceptionHandler` (`Novvi.Api/Exceptions`) implementa `IExceptionHandler` e converte toda
exceção não tratada em `ProblemDetails` (`application/problem+json`):

| Exceção | Status HTTP |
|---|---|
| `Novvi.Domain.Exceptions.NotFoundException` | 404 |
| `Novvi.Domain.Exceptions.DomainException` | 400 |
| `ArgumentException` / `ArgumentNullException` | 400 |
| `KeyNotFoundException` | 404 |
| `InvalidOperationException` | 409 |
| Qualquer outra | 500 (mensagem genérica em produção) |

Em ambiente de `Development` o `Detail` traz a mensagem real e o `traceId`; em produção, apenas
uma mensagem genérica é exposta (sem stack trace).

## Health checks (CP4)
`GET /health` retorna um único relatório JSON com todos os checks registrados:
- `self`: processo no ar.
- `database`: `AddDbContextCheck<NovviContext>()` (abordagem A recomendada pelo enunciado),
  valida conexão com o Oracle via o próprio `DbContext`.

Status HTTP: `Healthy`/`Degraded` → 200, `Unhealthy` → 503 (comportamento padrão do
`Microsoft.Extensions.Diagnostics.HealthChecks`). Para simular falha, basta usar uma connection
string inválida em `appsettings.Development.json` local (não versionada) e observar `/health`
retornar 503 com o check `database` como `Unhealthy`.

## Observabilidade (CP4)
- `PedidosController.Post` (fluxo de escrita) loga início e sucesso da criação do pedido com
  propriedades nomeadas e `HttpContext.TraceIdentifier`.
- `PedidoService.Create` também loga (nível Application) com propriedades nomeadas
  (`IdUsuario`, `IdFuncionario`, quantidade de produtos).
- `GlobalExceptionHandler` loga toda exceção não tratada em nível `Error`, incluindo o mesmo
  `TraceId` da requisição.

## Testes (CP4)
- **`Novvi.Domain.Tests`**: regras de negócio reais do domínio, sem mock, com `[Fact]` (caminho
  feliz) e `[Theory]`/`[InlineData]` (caminhos de erro), seguindo AAA e a convenção
  `Metodo_Cenario_ResultadoEsperado`.
- **`Novvi.Application.Tests`**: serviços de aplicação com repositórios mockados via Moq.
  Cobre o cenário de dependência ausente (ex.: usuário/funcionário inexistente) lançando a
  exceção mapeada no CP3 **sem** persistir (`Times.Never`), e o caminho feliz persistindo uma
  única vez (`Times.Once`).

## Evidências (`/docs`)
- `headers-v1-query.txt`, `headers-v1-header.txt`, `headers-sem-versao.txt`: headers `api-supported-versions: 2.0` e `api-deprecated-versions: 1.0`
- `versao-nao-suportada-400.txt`: versão inexistente (9.9) responde 400
- `swagger-v1.0.json`, `swagger-v2.0.json`, `swagger-v1-descricao.txt`, `swagger-paths.txt`: Swagger por versão, v1 marcada como deprecada
- `swagger-grupos.png`: print do Swagger com os dois grupos (v1.0 DEPRECADA e v2.0)
- `400-page.txt`, `400-pagesize.txt`: 400 de page e pageSize inválidos (`application/problem+json`)
- `rate-limit-tentativas.txt`, `429-retry-after.txt`: estouro do POST /api/pedidos (429, Retry-After, corpo JSON)
- `health-apos-429.txt`: /health logo depois do 429 (nunca 429; 503 aqui porque o Oracle estava indisponível)
- `dotnet-test.txt`: saída do dotnet test
- `nota-oracle.txt`: explica que, com o Oracle indisponível, os GETs com dados (v1 array, v2 envelope, páginas 1 e 2, página vazia) não puderam ser capturados nesta rodada


---

# CP5 — Versionamento, Paginação e Rate Limit

Recurso escolhido: **Pedidos** (o que mais cresce no domínio). Os demais recursos (Produtos, Usuários,
Funcionários) seguem iguais e estão marcados com `[ApiVersionNeutral]`, então continuam chamáveis
(e visíveis nos dois grupos do Swagger) sem informar versão.

## URLs principais (dev: `http://localhost:5236`)
| O quê | URL |
|---|---|
| Swagger | `/swagger` (seletor com **v1.0 (DEPRECADA)** e **v2.0**) |
| Health | `GET /health` |
| Listagem v1 (deprecada, array) | `GET /api/pedidos?api-version=1.0` · header `X-Api-Version: 1.0` · `GET /api/v1/pedidos` |
| Listagem v2 (envelope paginado) | `GET /api/pedidos` · `GET /api/pedidos?api-version=2.0` · `GET /api/v2/pedidos?page=1&pageSize=20` |

## Como informar a versão
1. Query string: `?api-version=1.0`
2. Header: `X-Api-Version: 1.0`
3. (Extra) Segmento de URL: `/api/v1/pedidos`, `/api/v2/pedidos`
4. **Sem versão → cai na 2.0** (`AssumeDefaultVersionWhenUnspecified = true`, `DefaultApiVersion = 2.0`)

As respostas trazem `api-supported-versions: 2.0` e `api-deprecated-versions: 1.0` (a biblioteca lista a versão deprecada apenas no segundo header).
Os dois contratos chamam o **mesmo** `IPedidoService`: `GetAll()` (v1) e `GetPaged()` (v2); não há service duplicado.

**Escrita:** `GET /{id}`, `POST`, `POST /{id}/pagamento` e `DELETE` não têm `[MapToApiVersion]`, então valem para
**1.0 e 2.0**. A correção pode chamar `POST /api/pedidos` sem versão (cai na 2.0) ou com `?api-version=1.0`.

## Paginação (só na v2)
| Parâmetro | Padrão | Regra |
|---|---|---|
| `page` | 1 | inteiro ≥ 1 |
| `pageSize` | 20 | inteiro de 1 a **100** (teto) |

- Fora da faixa → **400** `application/problem+json` com a regra explícita no `detail`.
- Página além do total → **200** com `items: []`.
- Corpo 200: `{ "page", "pageSize", "totalItems", "totalPages", "items", "hasPrevious", "hasNext" }`; `totalPages = ceil(totalItems / pageSize)`.
- O corte é no banco: `Count` + `OrderBy(CriadoEm).ThenBy(Id)` + `Skip` + `Take` no `IQueryable`, depois `ToList()`
  (`PedidoRepository.GetPagedCompleto`; `IRepository<T>.GetPaged` genérico também disponível).
- A v1 **não pagina**: preserva a lista antiga.
- Validação na Application (`Paginacao.Validar` → `PaginacaoInvalidaException` → 400 no `GlobalExceptionHandler`).

## Rate limit (fixed window, nativo)
| Endpoint | Política | Limite | Janela |
|---|---|---|---|
| `POST /api/pedidos` | `pedidos-escrita` | **10 requisições** | **1 minuto** por IP |
| `GET /api/pedidos` (v2) | `listagem-v2` | 60 requisições | 1 minuto por IP |

Estouro → **429** com header `Retry-After` (segundos) e corpo `application/problem+json` (`status: 429`).
`GET /health` **não** entra no teto (`DisableRateLimiting`). Pipeline: `UseExceptionHandler` → `UseRateLimiter` → `MapControllers`.
Não implementado (itens "recomendados"): headers `X-RateLimit-*` nas respostas que passam.

## Testes (CP5)
`dotnet test` roda os testes do CP4 + `PedidoPaginacaoTests` (`[Theory]` para page/pageSize inválidos, `[Fact]`/`[Theory]` para o intervalo válido e `totalPages`).

## Mapeamento de exceções (acrescentado no CP5)
| Exceção | Status HTTP |
|---|---|
| `PaginacaoInvalidaException` | 400 (mensagem da regra visível em qualquer ambiente) |

(demais linhas da tabela do CP3 permanecem.)
