# Frontend Developer Guide

## Quick Start

```bash
npm install
npm run dev
```

The dev server runs on `http://localhost:3000` and proxies API requests to the Web Gateway at `http://localhost:4000`.

## Tech Stack

- **React 18.3.1** + **TypeScript 5.8.3** + **Vite 5.4.12**
- **@tanstack/react-query** - Server state management
- **@headlessui/react** - Accessible UI components
- **@heroicons/react** - Icons
- **Tailwind CSS** - Styling
- **react-router-dom** - Routing

See `package.json` for exact versions.

## Key Architecture

- **BFF Pattern**: Frontend → Web Gateway → Backend APIs
- **Auto-generated API Client**: TypeScript services from OpenAPI spec
- **Frontend-Driven**: You can add new API endpoints with mock models in the Web Gateway

## Where to Find Things

### Routes
- **Current routes**: Check `src/components/App.tsx`
- **Navigation**: See `src/components/Header.tsx`

### API Endpoints
- **Available endpoints**: Check `src/api/services/` (auto-generated)
- **API models**: See `src/api/models/`
- **Gateway endpoints**: Look at `src/BindingChaos.Web.Gateway/Controllers/`

### Components
- **Reusable components**: `src/components/` (Modal, Button, etc.)
- **Page components**: `src/components/` (SignalFeed, IdeasPage, etc.)
- **Custom hooks**: `src/hooks/`

### Styling
- **Global styles**: `src/App.css` and `src/index.css`
- **Component styles**: Tailwind classes + custom CSS
- **Design system**: Use existing Button and Modal components

## Development Workflow

### Adding New API Endpoints

**Important**: The frontend dictates the models! Feel free to add new API endpoints with mock models in the Web Gateway.

1. **Add endpoint to Web Gateway** (`src/BindingChaos.Web.Gateway/Controllers/`)
2. **Generate new OpenAPI spec**: `scripts/generate-openapi.ps1`
3. **Regenerate API client**: `npm run generate-api-from-spec`
4. **Use in frontend**: Import and use the new service

### Common Patterns

**Data Fetching**:
```tsx
import { useApiClient } from '@/hooks/useApiClient';

const { data } = useQuery({
  queryKey: ['my-data'],
  queryFn: () => apiClient.myService.getMyData()
});
```

**Mutations**:
```tsx
const mutation = useMutation({
  mutationFn: (data) => apiClient.myService.createMyData(data),
  onSuccess: () => queryClient.invalidateQueries({ queryKey: ['my-data'] })
});
```

## Configuration

- **API Configuration**: Centralized in `src/config/api.ts`
- **Environment Variables**: Configured in `.env.local` (VITE_API_BASE_URL for API base URL)
- **React Query**: 5-minute stale time, 1 retry (see `src/components/App.tsx`)
- **Vite aliases**: `@/` points to `src/` (see `vite.config.ts`)

### Environment Configuration

The `.env.local` file contains the local development configuration:

```bash
# API Configuration
VITE_API_BASE_URL=http://localhost:4000
```

## Getting Help

- **Examples**: Check existing components for patterns
- **API methods**: Review generated services in `src/api/services/`
- **Gateway endpoints**: Look at Web Gateway controllers
- **Styling**: Follow existing Button/Modal component patterns 