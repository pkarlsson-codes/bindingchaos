# Sourced by reset-area.sh — defines reset_keycloak and helpers.
# Requires ensure_postgres, ensure_service_running, execute_postgres_sql from reset-shared.sh.

_configure_keycloak_admin_credentials() {
    docker exec -i "$KEYCLOAK_CONTAINER" /opt/keycloak/bin/kcadm.sh config credentials \
        --server http://localhost:8080 \
        --realm master \
        --user "$KEYCLOAK_ADMIN_USER" \
        --password "$KEYCLOAK_ADMIN_PASSWORD" >/dev/null
}

_reimport_keycloak_realm() {
    echo "[Reset:keycloak] Re-importing realm '$KEYCLOAK_REALM' without container restart..."
    docker exec -i "$KEYCLOAK_CONTAINER" \
        /opt/keycloak/bin/kcadm.sh delete "realms/$KEYCLOAK_REALM" >/dev/null 2>&1 || true
    docker exec -i "$KEYCLOAK_CONTAINER" \
        /opt/keycloak/bin/kcadm.sh create realms \
        -f "/opt/keycloak/data/import/realm-bindingchaos-dev.json" >/dev/null
    echo "[Reset:keycloak] Done."
}

_restart_keycloak_container() {
    echo "[Reset:keycloak] Restarting Keycloak container to apply schema reset..."
    docker restart "$KEYCLOAK_CONTAINER" >/dev/null
}

_wait_keycloak_ready() {
    local max_attempts=180
    local attempt
    for ((attempt=1; attempt<=max_attempts; attempt++)); do
        if curl -sf "http://localhost:7080/health/ready" >/dev/null 2>&1; then
            echo "[Reset:keycloak] Keycloak is ready."
            return 0
        fi
        sleep 1
    done
    echo "Keycloak did not become ready within $max_attempts seconds." >&2
    exit 1
}

reset_keycloak() {
    echo "[Reset:keycloak] Resetting Keycloak schema and realm import state..."
    ensure_postgres
    ensure_service_running "keycloak" "$KEYCLOAK_CONTAINER"

    execute_postgres_sql "$POSTGRES_DB" 'DROP SCHEMA IF EXISTS "keycloak" CASCADE;'

    local keycloak_migration_script
    keycloak_migration_script="$(dirname "${BASH_SOURCE[0]}")/db-migrate-keycloak.sh"
    bash "$keycloak_migration_script"

    _restart_keycloak_container
    _wait_keycloak_ready
    _configure_keycloak_admin_credentials
    _reimport_keycloak_realm
}
