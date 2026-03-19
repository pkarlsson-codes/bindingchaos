import React, { Component } from 'react';
import type { ErrorInfo, ReactNode } from 'react';
import { Button } from '@/shared/components/layout/Button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/shared/components/ui/card';
import { AUTH_CONFIG } from '../../../config/auth';

interface Props {
  children: ReactNode;
  fallback?: ReactNode;
}

interface State {
  hasError: boolean;
  error?: Error;
}

export class AuthErrorBoundary extends Component<Props, State> {
  public state: State = {
    hasError: false
  };

  public static getDerivedStateFromError(error: Error): State {
    // Update state so the next render will show the fallback UI
    return { hasError: true, error };
  }

  public componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    console.error('Authentication error caught by boundary:', error, errorInfo);
    
    // Log to error reporting service in production
    if (process.env.NODE_ENV === 'production') {
      // TODO: Send to error reporting service
      console.error('Auth error details:', {
        message: error.message,
        stack: error.stack,
        componentStack: errorInfo.componentStack,
        timestamp: new Date().toISOString(),
      });
    }
  }

  private handleRetry = () => {
    this.setState({ hasError: false, error: undefined });
  };

  private handleLogout = () => {
    // Clear any corrupted auth state
    localStorage.removeItem(AUTH_CONFIG.storageKey);
    window.location.reload();
  };

  public render() {
    if (this.state.hasError) {
      if (this.props.fallback) {
        return this.props.fallback;
      }

      return (
        <div className="min-h-screen flex items-center justify-center bg-background">
          <Card className="w-full max-w-md">
            <CardHeader>
              <CardTitle className="text-destructive">Authentication Error</CardTitle>
              <CardDescription>
                Something went wrong with the authentication system
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="text-sm text-muted-foreground">
                {this.state.error?.message || 'An unexpected authentication error occurred'}
              </div>
              
              <div className="flex gap-2">
                <Button onClick={this.handleRetry} variant="outline">
                  Try Again
                </Button>
                <Button onClick={this.handleLogout} variant="danger">
                  Clear Session
                </Button>
              </div>
            </CardContent>
          </Card>
        </div>
      );
    }

    return this.props.children;
  }
} 