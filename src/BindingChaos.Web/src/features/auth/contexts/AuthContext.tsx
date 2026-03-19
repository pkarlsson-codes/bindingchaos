import React, { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';
import { useApiClient } from '@/shared/hooks/useApiClient';
import { useAuthStorage } from '../hooks/useAuthStorage';
import { broadcastLogout } from '../hooks/useAuthListener';
import { AUTH_CONFIG, getSessionExpiryTime } from '../../../config/auth';
import { API_CONFIG } from '@/config/api';
import type { UserInfo } from '@/api/models';

interface AuthContextType {
  user: UserInfo | null;
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
  isLoading: boolean;
  checkAuth: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<UserInfo | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const apiClient = useApiClient();
  const { saveAuthData, getAuthData, clearAuthData } = useAuthStorage();
  
  // Set up cross-tab authentication listener using storage events
  useEffect(() => {
    const handleStorageChange = (event: StorageEvent) => {
      if (event.key === AUTH_CONFIG.storageKey) {
        // Another tab updated the auth state, refresh our state
        checkAuth();
      }
    };

    window.addEventListener('storage', handleStorageChange);
    return () => {
      window.removeEventListener('storage', handleStorageChange);
    };
  }, []);

  // Set up session refresh
  useEffect(() => {
    if (!user) return;

    const interval = setInterval(async () => {
      try {
        // Only validate session if we have cached auth data
        const cachedAuth = getAuthData();
        if (cachedAuth) {
          const response = await apiClient.auth.getCurrentUser();
          if (response.success && response.user) {
            // Session is still valid, update user data
            setUser(response.user);
            const authData = {
              user: {
                id: response.user.id || '',
                username: response.user.username || '',
                pseudonym: response.user.pseudonym || '',
                email: response.user.email || undefined,
              },
              expiresAt: getSessionExpiryTime(),
            };
            saveAuthData(authData);
          } else {
            // Session expired, clear user
            setUser(null);
            clearAuthData();
          }
        }
      } catch (error: any) {
        // Handle 401 errors gracefully (session expired)
        if (error?.status === 401 || error?.response?.status === 401) {
          setUser(null);
          clearAuthData();
          return;
        }
        console.error('Session validation failed:', error);
      }
    }, 5 * 60 * 1000); // 5 minutes

    return () => clearInterval(interval);
  }, [user]);
  

  const checkAuth = async () => {
    try {
      // First check localStorage for cached auth data
      const cachedAuth = getAuthData();
      if (cachedAuth) {
        setUser(cachedAuth.user);
        setIsLoading(false);
        return;
      }

      // No cached data: attempt one-time server-side hydration from session
      try {
        const response = await apiClient.auth.getCurrentUser();
        if (response.success && response.user) {
          setUser(response.user);
          const authData = {
            user: {
              id: response.user.id || '',
              username: response.user.username || '',
              pseudonym: response.user.pseudonym || '',
              email: response.user.email || undefined,
            },
            expiresAt: getSessionExpiryTime(),
          };
          saveAuthData(authData);
          setIsLoading(false);
          return;
        }
      } catch {
        // ignore and treat as anonymous
      }

      setUser(null);
      setIsLoading(false);
    } catch (error: any) {
      console.error('Failed to check authentication:', error);
      setUser(null);
      setIsLoading(false);
    }
  };

  useEffect(() => {
    // Check authentication status on mount
    checkAuth().finally(() => {
      setIsLoading(false);
    });
  }, []);

  const login = async (username: string, password: string) => {
    setIsLoading(true);

    try {
      // For OIDC redirect flow, hydrate from the server-side session via /api/v1/Auth/me
      const response = await apiClient.auth.getCurrentUser();
      if (response.success && response.user) {
        setUser(response.user);
        const authData = {
          user: {
            id: response.user.id || '',
            username: response.user.username || '',
            pseudonym: response.user.pseudonym || '',
            email: response.user.email || undefined,
          },
          expiresAt: getSessionExpiryTime(),
        };
        saveAuthData(authData);
      } else {
        throw new Error('Login failed');
      }
    } catch (error) {
      console.error('Login failed:', error);
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  const logout = () => {
    clearAuthData();
    broadcastLogout();
    window.location.href = `${API_CONFIG.baseUrl}/auth/logout`;
  };

  const value: AuthContextType = {
    user,
    login,
    logout,
    isLoading,
    checkAuth
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}

// Hook for components that need user info but don't need auth actions
export function useUser() {
  const { user, isLoading } = useAuth();
  return { user, isLoading };
} 

// Optional hook: returns undefined when no provider is mounted
// Useful for components that can gracefully degrade without auth
export function useOptionalAuth() {
  return useContext(AuthContext);
}