function Invoke-ResetRedis {
    Write-Host "[Reset:redis] Flushing Redis data..."
    Ensure-ServiceRunning -Service "redis" -ContainerName "bindingchaos_redis"

    & docker exec -i bindingchaos_redis redis-cli FLUSHALL
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to flush Redis."
    }

    Write-Host "[Reset:redis] Done."
}