import React from 'react';
import { useAuth } from '../contexts/AuthContext';
import { Button } from '@/shared/components/layout/Button';
import { API_CONFIG } from '@/config/api';

export function AuthStatus() {
  const { user, logout, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div className="flex items-center space-x-2">
        <div className="h-4 w-4 animate-pulse bg-muted rounded"></div>
        <span className="text-sm text-muted-foreground">Loading...</span>
      </div>
    );
  }

  if (!user) {
    return (
      <Button
        variant="ghost"
        size="sm"
        onClick={() => {
          window.location.href = `${API_CONFIG.baseUrl}/auth/login`;
        }}
      >
        Login
      </Button>
    );
  }

  return (
    <div className="flex items-center space-x-2">
      <span className="text-sm text-muted-foreground">
        Logged in as <span className="font-medium text-foreground">{user.pseudonym}</span>
      </span>
      <Button
        variant="ghost"
        size="sm"
        onClick={logout}
      >
        Logout
      </Button>
    </div>
  );
} 