#!/usr/bin/env bash
set -euo pipefail

CONTAINER_NAME="${1:-bindingchaos_postgres}"
DATABASE="${2:-bindingchaos}"
USER="${3:-postgres}"
SCHEMA="${4:-keycloak}"

echo "[Keycloak DB] Ensuring schema '$SCHEMA' exists in database '$DATABASE' on container '$CONTAINER_NAME'..."

container=$(docker ps -a --format "{{.Names}}" | grep -x "$CONTAINER_NAME" || true)
if [ -z "$container" ]; then
    echo "[Keycloak DB] Postgres container '$CONTAINER_NAME' not found. Starting 'postgres' service via docker-compose..."
    if command -v docker-compose &>/dev/null; then
        docker-compose up -d postgres >/dev/null
    else
        docker compose up -d postgres >/dev/null
    fi
fi

is_running=$(docker inspect -f '{{.State.Running}}' "$CONTAINER_NAME" 2>/dev/null || echo "false")
if [ "$is_running" != "true" ]; then
    echo "[Keycloak DB] Starting container '$CONTAINER_NAME'..."
    docker start "$CONTAINER_NAME" >/dev/null
fi

sql="CREATE SCHEMA IF NOT EXISTS \"${SCHEMA}\" AUTHORIZATION ${USER};"
echo "[Keycloak DB] Executing: $sql"
docker exec -i "$CONTAINER_NAME" psql -U "$USER" -d "$DATABASE" -c "$sql"

echo "[Keycloak DB] Schema '$SCHEMA' ensured."
