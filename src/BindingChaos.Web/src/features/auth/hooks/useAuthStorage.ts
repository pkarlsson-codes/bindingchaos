import { useCallback } from 'react';
import { AUTH_CONFIG } from '../../../config/auth';

interface AuthStorageData {
  user: {
    id: string;
    username: string;
    pseudonym: string;
    email?: string;
  };
  sessionId?: string;
  expiresAt?: number;
}

export function useAuthStorage() {
  const saveAuthData = useCallback((data: AuthStorageData) => {
    try {
      localStorage.setItem(AUTH_CONFIG.storageKey, JSON.stringify(data));
    } catch (error) {
      console.error('Failed to save auth data to localStorage:', error);
    }
  }, []);

  const getAuthData = useCallback((): AuthStorageData | null => {
    try {
      const stored = localStorage.getItem(AUTH_CONFIG.storageKey);
      if (!stored) return null;

      const data: AuthStorageData = JSON.parse(stored);
      
      // Check if session has expired
      if (data.expiresAt && Date.now() > data.expiresAt) {
        clearAuthData();
        return null;
      }

      return data;
    } catch (error) {
      console.error('Failed to get auth data from localStorage:', error);
      return null;
    }
  }, []);

  const clearAuthData = useCallback(() => {
    try {
      localStorage.removeItem(AUTH_CONFIG.storageKey);
    } catch (error) {
      console.error('Failed to clear auth data from localStorage:', error);
    }
  }, []);

  const isAuthenticated = useCallback((): boolean => {
    const data = getAuthData();
    return data !== null;
  }, [getAuthData]);

  return {
    saveAuthData,
    getAuthData,
    clearAuthData,
    isAuthenticated,
  };
} 