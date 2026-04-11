#!/usr/bin/env bash
set -euo pipefail

CONFIGURATION="${1:-Debug}"
OUTPUT_PATH="${2:-src/BindingChaos.Web/api-spec.json}"

echo "Generating OpenAPI specification and API clients..."

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

echo "Project root: $PROJECT_ROOT"

CSPROJ_PATH="$PROJECT_ROOT/src/BindingChaos.Web.Gateway/BindingChaos.Web.Gateway.csproj"

echo "Building Web Gateway API project..."
dotnet build "$CSPROJ_PATH" --configuration "$CONFIGURATION"

OUTPUT_DIR="$(dirname "$PROJECT_ROOT/$OUTPUT_PATH")"
mkdir -p "$OUTPUT_DIR"

NSWAG_VERSION="14.7.0"

echo "Ensuring NSwag CLI..."
dotnet tool update --global NSwag.ConsoleCore --version "$NSWAG_VERSION" 2>/dev/null \
    || dotnet tool install --global NSwag.ConsoleCore --version "$NSWAG_VERSION"

echo "Generating OpenAPI spec via NSwag..."
if command -v cygpath &>/dev/null; then
    NSWAG_PROJECT="$(cygpath -w "$CSPROJ_PATH")"
    NSWAG_OUTPUT="$(cygpath -w "$PROJECT_ROOT/$OUTPUT_PATH")"
else
    NSWAG_PROJECT="$CSPROJ_PATH"
    NSWAG_OUTPUT="$PROJECT_ROOT/$OUTPUT_PATH"
fi
MSYS_NO_PATHCONV=1 nswag aspnetcore2openapi \
    /project:"$NSWAG_PROJECT" \
    /output:"$NSWAG_OUTPUT" \
    /configuration:"$CONFIGURATION" \
    /aspNetCoreEnvironment:Development

FRONTEND_PATH="$PROJECT_ROOT/src/BindingChaos.Web"
if [ ! -d "$FRONTEND_PATH/node_modules" ]; then
    echo "Installing frontend dependencies..."
    npm --prefix "$FRONTEND_PATH" install
fi

echo "Generating TypeScript API clients..."
npm --prefix "$FRONTEND_PATH" run generate-api-from-spec

echo "OpenAPI specification and API clients generation completed successfully!"
