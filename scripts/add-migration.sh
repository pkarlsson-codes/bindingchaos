#!/usr/bin/env bash
set -euo pipefail

CONTEXT_PROJECT_PATH="${1:?Usage: add-migration.sh <ContextProjectPath> <DbContextName> <MigrationName>}"
DB_CONTEXT_NAME="${2:?Missing DbContextName}"
MIGRATION_NAME="${3:?Missing MigrationName}"

echo "Adding EF Core migration '$MIGRATION_NAME'..."
echo "Context project: $CONTEXT_PROJECT_PATH"
echo "DbContext: $DB_CONTEXT_NAME"

dotnet ef migrations add "$MIGRATION_NAME" \
    --project "$CONTEXT_PROJECT_PATH" \
    --context "$DB_CONTEXT_NAME" \
    --output-dir Infrastructure/Persistence/Migrations
