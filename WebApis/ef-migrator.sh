#!/usr/bin/env sh
# Script de migração do EF Core (compatível com /bin/sh e containers diversos)
# - Evita uso de 'pipefail' (não suportado em sh)
# - Usa apenas construções POSIX (while no lugar de {1..30})

set -eu

echo "==> dotnet --info"
dotnet --info

# Garante dotnet-ef disponível (instala globalmente se necessário)
export PATH="$PATH:/root/.dotnet/tools"
if ! command -v dotnet-ef >/dev/null 2>&1; then
  echo "==> Instalando dotnet-ef globalmente..."
  dotnet tool install -g dotnet-ef || dotnet tool update -g dotnet-ef || true
fi

echo "==> dotnet ef --version"
dotnet ef --version || true

# Determina o provider e o contexto EF a utilizar
PROVIDER="${DatabaseProvider:-PostgreSQL}"
PROVIDER_LC=$(echo "$PROVIDER" | tr '[:upper:]' '[:lower:]')
if [ "$PROVIDER_LC" = "postgresql" ]; then
  EF_CONTEXT="Infraestructure.Configuration.PostgreSqlContext"
else
  # Contexto padrão do SQL Server
  EF_CONTEXT="Infraestructure.Configuration.ContextBase"
fi

echo "==> Provider: $PROVIDER | EF Context: $EF_CONTEXT"

echo "==> Restore e build dos projetos"
dotnet restore Infraestructure/Infraestructure.csproj
dotnet restore WebApis/WebApis.csproj
# Build de Release para garantir que as deps estejam preparadas
dotnet build WebApis/WebApis.csproj -c Release --nologo

# Tenta aplicar migrações até o DB ficar pronto (máx 30 tentativas)
i=1
while [ "$i" -le 30 ]; do
  echo "==> Aplicando migrações (tentativa $i)..."
  if dotnet ef database update \
      --project Infraestructure/Infraestructure.csproj \
      --startup-project WebApis/WebApis.csproj \
      --context "$EF_CONTEXT"; then
    echo "==> Migrações aplicadas com sucesso."
    exit 0
  fi
  echo "==> Falha ao aplicar migrações; aguardando 5s..."
  sleep 5
  i=$((i+1))
done

echo "==> Falha ao aplicar migrações após múltiplas tentativas."
exit 1