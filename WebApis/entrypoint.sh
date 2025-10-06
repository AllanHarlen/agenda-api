#!/usr/bin/env sh
set -eu

echo "Iniciando aplicação..."
exec dotnet WebApis.dll
