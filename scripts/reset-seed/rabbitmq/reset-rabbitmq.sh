# Sourced by reset-area.sh — defines reset_rabbitmq.
# Requires ensure_service_running from reset-shared.sh.

reset_rabbitmq() {
    echo "[Reset:rabbitmq] Purging RabbitMQ queues in-place..."
    ensure_service_running "rabbitmq" "bindingchaos_rabbitmq"

    local queues
    queues=$(docker exec -i bindingchaos_rabbitmq rabbitmqctl list_queues -q name)

    local queue
    while IFS= read -r queue; do
        [ -z "$queue" ] && continue
        echo "[Reset:rabbitmq] Purging queue '$queue'..."
        docker exec -i bindingchaos_rabbitmq rabbitmqctl purge_queue "$queue" >/dev/null
    done <<< "$queues"

    echo "[Reset:rabbitmq] Done."
}
