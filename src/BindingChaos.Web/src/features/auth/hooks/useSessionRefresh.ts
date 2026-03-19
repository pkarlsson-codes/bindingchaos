import { useEffect, useRef } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useAuthStorage } from './useAuthStorage';

export function useSessionRefresh() {
  const { user, checkAuth } = useAuth();
  const { getAuthData } = useAuthStorage();
  const intervalRef = useRef<NodeJS.Timeout>();

  useEffect(() => {
    if (!user) {
      if (intervalRef.current) {
        clearInterval(intervalRef.current);
      }
      return;
    }

    // Simple session validation every 5 minutes
    intervalRef.current = setInterval(async () => {
      try {
        await checkAuth();
      } catch (error) {
        console.error('Session validation failed:', error);
      }
    }, 5 * 60 * 1000); // 5 minutes

    return () => {
      if (intervalRef.current) {
        clearInterval(intervalRef.current);
      }
    };
  }, [user, checkAuth]);
} 