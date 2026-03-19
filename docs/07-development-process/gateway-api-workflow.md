# API Development Workflow

## Overview

Frontend TypeScript types are generated from the Web Gateway's OpenAPI spec for type safety.

## Architecture

- **Web Gateway** (port 4000): BFF using NSwag
- **Core Platform API** (port 5000): Internal API using Swashbuckle  
- **Frontend**: React/TypeScript SPA

## Workflow

### 1. Make API Changes

Edit controllers/models in Web Gateway. Add XML comments for documentation.

### 2. Generate Types

**Recommended:**
```bash
npm run frontend:generate
```

This builds the Web Gateway, generates the OpenAPI spec with NSwag, and creates TypeScript types.

**Manual options:**
- `cd src/BindingChaos.Web && npm run generate-api` - Uses live API at http://localhost:4000
- `cd src/BindingChaos.Web && npm run generate-api-from-spec` - Uses local `api-spec.json`

### 3. Use Generated Types

Import from `src/api/` in frontend components. Generated client uses Fetch API with TypeScript interfaces.

## Configuration

**Generator:** `@openapitools/openapi-generator-cli` with `typescript-fetch`  
**Output:** `src/BindingChaos.Web/src/api/`  
**Base URL:** `http://localhost:4000`

## Troubleshooting

- **Build fails:** Check .NET 10 SDK installed, run `dotnet restore`
- **Type errors:** Verify `api-spec.json` exists and is valid
- **Port conflicts:** Web Gateway (4000), Core API (5000), Frontend (3000)
- **Missing NSwag:** `dotnet tool install --global NSwag.ConsoleCore`

## Quick Commands

```bash
npm run infra:start          # Start databases
npm run gateway:start        # Start Web Gateway
npm run frontend:start       # Start frontend dev server
npm run frontend:generate    # Generate API clients
```

## Related Documentation

- [API Architecture](../02%20Architecture/api-interface/api-architecture.md)
- [OpenAPI Type Generation Decision](../02%20Architecture/decisions/00020%20OpenAPI%20Type%20Generation%20for%20Frontend-Backend%20Type%20Safety.md)