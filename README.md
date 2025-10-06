
# Agenda API (ASP.NET Core 9 + EF Core + SQL Server)

API RESTful para agenda de contatos e agendamentos, construída com ASP.NET Core, Entity Framework Core e Identity (JWT). O projeto utiliza Repository Pattern, FluentValidation e Swagger.

## Recursos
- Endpoints sob o prefixo `/api`
- Autenticação JWT e Identity Roles
- Swagger UI para documentação interativa
- Banco SQL Server com migrações automáticas e seed inicial

## Executar com Docker Compose (recomendado)

Pré-requisitos:
- Docker Desktop instalado e em execução

Passos:
1. Abra um terminal na pasta `agenda-api/WebApis`
2. Execute:
   ```bash
   docker compose up -d --build
   ```

Serviços levantados:
- `sqlserver`: SQL Server 2022 (porta `1433`)
- `webapis`: API .NET (porta host `7000` -> `5000` no container)
- `agenda-web`: Frontend (Nginx servindo build do Vue na porta `3000` do host)
- `nginx`: Reverse proxy público (porta `80` e `443`), encaminhando:
  - `/` → frontend (`agenda-web`)
  - `/api/` → API (`webapis`)
- `ef-migrator`: aplica migrações automaticamente antes de subir a API

URLs úteis:
- Frontend via proxy: `http://localhost/`
- API direta: `http://localhost:7000/api`
- Swagger da API: `http://localhost:7000/swagger`

Variáveis de ambiente principais (definidas no `docker-compose.yml`):
```yaml
DatabaseProvider: SqlServer
ConnectionStrings__SqlServer: Server=sqlserver;Database=agenda;User=sa;Password=84844323sdf!@#%;TrustServerCertificate=True;
ASPNETCORE_ENVIRONMENT: Development
```

### Seed (dados iniciais)
As migrações do EF aplicam um seed com usuário administrador padrão:
- Usuário: `administrador`
- Senha: `administrador8485!@`

Após subir os containers, autentique-se com as credenciais acima e altere a senha imediatamente em ambientes não-desenvolvimento.

## Execução Local (sem Docker)
1. Restaurar dependências:
   ```bash
   dotnet restore
   ```
2. Configurar a string de conexão em `WebApis/appsettings.Development.json` ou via variáveis de ambiente (chave `ConnectionStrings:SqlServer`).
3. Aplicar migrações e criar o banco:
   ```bash
   dotnet ef database update --project Infraestructure/Infraestructure.csproj --startup-project WebApis/WebApis.csproj
   ```
4. Executar a API:
   ```bash
   dotnet run --project WebApis/WebApis.csproj
   ```
5. Acessos:
   - API: `http://localhost:5000/api`
   - Swagger: `http://localhost:5000/swagger`

## Estrutura (resumo)
- `Entities/`: Entidades, DTOs e enums
- `Domain/`: Interfaces e serviços de domínio
- `Infraestructure/`: DbContext, migrações e repositórios
- `WebApis/`: API (controllers, validações, Program.cs, Docker/Nginx)

## Licença
Projeto sob licença MIT. Consulte `LICENSE` para detalhes.
