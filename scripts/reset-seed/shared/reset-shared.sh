# Sourced by reset-area.sh — defines shared docker/postgres helpers.
# POSTGRES_CONTAINER, POSTGRES_DB, POSTGRES_USER, POSTGRES_PASSWORD must be
# exported by the caller before sourcing this file.

invoke_compose() {
    if command -v docker-compose &>/dev/null; then
        docker-compose "$@"
    else
        docker compose "$@"
    fi
}

ensure_postgres() {
    echo "[Reset] Ensuring postgres is running..."

    local exists
    exists=$(docker ps -a --format "{{.Names}}" | grep -x "$POSTGRES_CONTAINER" || true)
    if [ -z "$exists" ]; then
        echo "[Reset] Postgres container '$POSTGRES_CONTAINER' not found. Creating via compose..."
        invoke_compose up -d postgres
    else
        docker start "$POSTGRES_CONTAINER" >/dev/null 2>&1 || true
    fi

    local max_attempts=30
    local attempt
    for ((attempt=1; attempt<=max_attempts; attempt++)); do
        if docker exec -e PGPASSWORD="$POSTGRES_PASSWORD" -i "$POSTGRES_CONTAINER" \
            pg_isready -h localhost -U "$POSTGRES_USER" -d "$POSTGRES_DB" >/dev/null 2>&1; then
            echo "[Reset] Postgres is ready."
            return 0
        fi
        sleep 1
    done

    echo "Postgres did not become ready within $max_attempts seconds." >&2
    exit 1
}

ensure_service_running() {
    local service="$1"
    local container_name="$2"

    local exists
    exists=$(docker ps -a --format "{{.Names}}" | grep -x "$container_name" || true)
    if [ -z "$exists" ]; then
        echo "[Reset] Service '$service' container '$container_name' not found. Creating via compose..."
        invoke_compose up -d "$service" >/dev/null
        return 0
    fi

    local is_running
    is_running=$(docker inspect -f '{{.State.Running}}' "$container_name" 2>/dev/null || echo "false")
    if [ "$is_running" != "true" ]; then
        echo "[Reset] Starting container '$container_name'..."
        docker start "$container_name" >/dev/null
    fi
}

execute_postgres_sql() {
    local database="$1"
    local sql="$2"

    echo "[Reset:sql] db=$database sql=$sql"
    docker exec -e PGPASSWORD="$POSTGRES_PASSWORD" -i "$POSTGRES_CONTAINER" \
        psql -w -h localhost -v ON_ERROR_STOP=1 -U "$POSTGRES_USER" -d "$database" -c "$sql"
}
