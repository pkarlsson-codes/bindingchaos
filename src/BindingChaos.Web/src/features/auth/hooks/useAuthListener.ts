import { useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { AUTH_CONFIG } from '../../../config/auth';

export function useAuthListener() {
  const { checkAuth } = useAuth();

  useEffect(() => {
    const handleStorageChange = (event: StorageEvent) => {
      if (event.key === AUTH_CONFIG.storageKey) {
        // Another tab updated the auth state, refresh our state
        checkAuth();
      }
    };

    // Listen for storage changes (cross-tab)
    window.addEventListener('storage', handleStorageChange);

    return () => {
      window.removeEventListener('storage', handleStorageChange);
    };
  }, [checkAuth]);
}

// Simple broadcast function for logout events
export function broadcastLogout() {
  localStorage.removeItem(AUTH_CONFIG.storageKey);
} 