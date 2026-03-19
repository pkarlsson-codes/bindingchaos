# Port Guidelines

This document defines the port allocation strategy for the BindingChaos platform.

## Port Ranges

### Frontend Applications
- **React Web App**: Port 3000
- **Future frontend apps**: Port 3001, 3002, etc.

### Gateway Services
- **Web Gateway (BFF)**: Port 4000
- **Future gateways**: Port 4001, 4002, etc.

### Backend APIs
- **Core Platform API**: Port 5000
- **Future backend APIs**: Port 5001, 5002, etc.

## Current Configuration

| Service | Port | Description |
|---------|------|-------------|
| BindingChaos.Web | 3000 | React frontend application |
| BindingChaos.Web.Gateway | 4000 | Backend for Frontend (BFF) gateway |
| BindingChaos.CorePlatform.API | 5000 | Core backend API |

## Infrastructure Services (Docker)

| Service | Port | Description |
|---------|------|-------------|
| PostgreSQL | 5432 | Primary database |
| Redis | 6379 | Token storage and caching |
| RabbitMQ | 5672 / 15672 | Message broker / management UI |
| Keycloak | 7080 | Identity provider |
| MinIO | 9000 / 9001 | Object storage / console UI |
| Seq | 5341 | Structured log server — admin/admin (Development only) |

## Development URLs

- **Frontend**: http://localhost:3000
- **Web Gateway**: http://localhost:4000
- **Core API**: http://localhost:5000
- **Seq (log UI)**: http://localhost:5341

## Notes

- Frontend proxies API requests to the Web Gateway
- Web Gateway forwards requests to appropriate backend APIs
- All services use HTTP in development (HTTPS ports are available but not used by default) 