function Invoke-ResetRabbitMq {
    Write-Host "[Reset:rabbitmq] Purging RabbitMQ queues in-place..."
    Ensure-ServiceRunning -Service "rabbitmq" -ContainerName "bindingchaos_rabbitmq"

    $queues = & docker exec -i bindingchaos_rabbitmq rabbitmqctl list_queues -q name
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to list RabbitMQ queues."
    }

    foreach ($queue in $queues) {
        if ([string]::IsNullOrWhiteSpace($queue)) {
            continue
        }

        Write-Host "[Reset:rabbitmq] Purging queue '$queue'..."
        & docker exec -i bindingchaos_rabbitmq rabbitmqctl purge_queue $queue | Out-Null
        if ($LASTEXITCODE -ne 0) {
            throw "Failed to purge RabbitMQ queue '$queue'."
        }
    }

    Write-Host "[Reset:rabbitmq] Done."
}