# Novvi API

E-commerce simples (Usuários, Endereços, Funcionários, Produtos, Pedidos e Pagamentos) construído em
.NET 10 com Clean Architecture, para os Checkpoints (CP2 → CP4) da FIAP.

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
Sugestão de conteúdo a anexar depois de rodar localmente:
- Print do Swagger UI com os endpoints documentados.
- Trecho JSON de `/health` saudável e de `/health` com o banco indisponível (503).
- Trecho de log (console) de um `POST /api/pedidos` com `TraceId`.
- Saída do `dotnet test` (ou print do Test Explorer).
