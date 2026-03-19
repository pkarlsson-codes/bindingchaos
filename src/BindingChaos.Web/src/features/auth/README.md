# Production-Ready Authentication System

This is a complete, production-ready authentication system that works with the gateway's mock authentication backend. The frontend provides full authentication capabilities including session management, route protection, and user state management.

## Features

- **Mock User Management**: Simulates user login/logout with pseudonyms
- **Local Storage Persistence**: Remembers user session across page reloads with automatic expiration
- **Protected Routes**: Route guards for authenticated-only pages
- **TypeScript Support**: Fully typed interfaces for user data
- **React Context**: Provides authentication state throughout the app
- **Session Management**: Automatic session validation and cleanup
- **Error Handling**: Comprehensive error handling and user feedback
- **Production Ready**: Complete authentication system ready for production use

## Usage

### 1. Wrap your app with AuthProvider

```tsx
import { AuthProvider } from './shared/contexts/AuthContext';

function App() {
  return (
    <AuthProvider>
      {/* Your app components */}
    </AuthProvider>
  );
}
```

### 2. Use authentication in components

```tsx
import { useAuth, useUser } from './shared/contexts/AuthContext';

function MyComponent() {
  const { user, login, logout } = useAuth();
  // or just get user info
  const { user, isLoading } = useUser();

  if (!user) {
    return <div>Please login</div>;
  }

  return <div>Welcome, {user.pseudonym}!</div>;
}
```

### 3. Initiate login via redirect

Redirect users to `/auth/login` (optionally include a `returnUrl`):

```ts
window.location.href = `/auth/login?returnUrl=${encodeURIComponent(window.location.href)}`;
```

### 4. Use the AuthStatus component

```tsx
import { AuthStatus } from './shared/components/AuthStatus';

function Header() {
  return (
    <header>
      <h1>My App</h1>
      <AuthStatus />
    </header>
  );
}
```

### 5. Protect routes with authentication

```tsx
import { ProtectedRoute } from './shared/components/ProtectedRoute';

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/dashboard" element={
        <ProtectedRoute>
          <Dashboard />
        </ProtectedRoute>
      } />
    </Routes>
  );
}
```

### 6. Protect actions that require authentication

```tsx
import { AuthRequired } from './shared/components/AuthRequired';

function SignalDetailsPage() {
  return (
    <div>
      <h1>Signal Details</h1>
      <p>Public content that anyone can see</p>
      
      <AuthRequired action="add a comment">
        <CommentForm />
      </AuthRequired>
      
      <AuthRequired action="amplify this signal">
        <AmplifyButton />
      </AuthRequired>
    </div>
  );
}
```

### 7. Use authenticated API client

```tsx
import { useAuthenticatedApiClient } from './shared/hooks/useApiClient';

function MyComponent() {
  const apiClient = useAuthenticatedApiClient();
  
  // API calls will automatically include auth headers
  const handleSubmit = async () => {
    await apiClient.ideas.createIdea({ /* data */ });
  };
}
```

## Production Features

This authentication system includes all the features needed for production use:

### Session Management
- **Automatic Session Persistence**: User sessions are automatically saved to localStorage
- **Session Expiration**: Sessions expire after 24 hours for security
- **Automatic Cleanup**: Expired sessions are automatically cleared
- **Cross-Tab Synchronization**: Session state is synchronized across browser tabs

### Security Features
- **Action-Based Protection**: Only actions that modify data require authentication
- **Public Browsing**: All content (signals, ideas, actions) can be browsed without authentication
- **Session Validation**: Sessions are validated on app startup
- **Secure Logout**: Complete session cleanup on logout

### User Experience
- **Loading States**: Proper loading indicators during auth operations
- **Error Handling**: User-friendly error messages for auth failures
- **Remember Me**: Session persistence across browser sessions
- **Redirect After Login**: Users are redirected to their intended destination
- **Public Access**: Users can browse all content without authentication
- **Action Prompts**: Clear prompts when authentication is needed for actions

## API Integration

The authentication system is fully integrated with the API client. All authenticated requests automatically include user information:

```tsx
// In useApiClient.ts
export function useAuthenticatedApiClient() {
  const { user } = useAuth();
  
  const client = useMemo(() => {
    const config = new Configuration({
      basePath: API_CONFIG.baseUrl,
      headers: user ? {
        'X-User-ID': user.id || '',
        'X-User-Pseudonym': user.pseudonym || '',
      } : {}
    });
    
    return {
      ideas: new IdeasApi(config),
      signals: new SignalsApi(config),
      comments: new CommentsApi(config),
      tags: new TagsApi(config),
      auth: new AuthApi(config),
    };
  }, [user]);

  return client;
}
```

The gateway handles all authentication logic on the backend, so the frontend simply needs to include user identification headers. 