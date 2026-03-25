#!/usr/bin/env bash
set -euo pipefail

AREA="${1:?Usage: reset-area.sh <app|keycloak|redis|rabbitmq|all>}"

export POSTGRES_CONTAINER="${POSTGRES_CONTAINER:-bindingchaos_postgres}"
export POSTGRES_DB="${POSTGRES_DB:-bindingchaos}"
export POSTGRES_USER="${POSTGRES_USER:-postgres}"
export POSTGRES_PASSWORD="${POSTGRES_PASSWORD:-postgres}"
export KEYCLOAK_CONTAINER="${KEYCLOAK_CONTAINER:-bindingchaos_keycloak}"
export KEYCLOAK_ADMIN_USER="${KEYCLOAK_ADMIN_USER:-admin}"
export KEYCLOAK_ADMIN_PASSWORD="${KEYCLOAK_ADMIN_PASSWORD:-admin}"
export KEYCLOAK_REALM="${KEYCLOAK_REALM:-bindingchaos-dev}"

MODULE_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

# shellcheck source=../shared/reset-shared.sh
. "$MODULE_ROOT/shared/reset-shared.sh"
# shellcheck source=../app/reset-app.sh
. "$MODULE_ROOT/app/reset-app.sh"
# shellcheck source=../keycloak/reset-keycloak.sh
. "$MODULE_ROOT/keycloak/reset-keycloak.sh"
# shellcheck source=../redis/reset-redis.sh
. "$MODULE_ROOT/redis/reset-redis.sh"
# shellcheck source=../rabbitmq/reset-rabbitmq.sh
. "$MODULE_ROOT/rabbitmq/reset-rabbitmq.sh"

case "$AREA" in
    app)      reset_app_data ;;
    keycloak) reset_keycloak ;;
    redis)    reset_redis ;;
    rabbitmq) reset_rabbitmq ;;
    all)
        reset_app_data
        reset_keycloak
        reset_redis
        reset_rabbitmq
        ;;
    *)
        echo "Invalid area: '$AREA'. Must be one of: app, keycloak, redis, rabbitmq, all" >&2
        exit 1
        ;;
esac

echo "[Reset] Completed area '$AREA'."
