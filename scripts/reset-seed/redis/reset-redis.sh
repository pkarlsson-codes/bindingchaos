# Sourced by reset-area.sh — defines reset_redis.
# Requires ensure_service_running from reset-shared.sh.

reset_redis() {
    echo "[Reset:redis] Flushing Redis data..."
    ensure_service_running "redis" "bindingchaos_redis"
    docker exec -i bindingchaos_redis redis-cli FLUSHALL
    echo "[Reset:redis] Done."
}
