import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { Button } from '@/shared/components/layout/Button';
import { Input } from '@/shared/components/ui/input';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/shared/components/ui/card';

export function LoginPage() {
  const { login, isLoading, user } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [pseudonym, setPseudonym] = useState('');
  const [error, setError] = useState('');

  // Redirect if already authenticated
  useEffect(() => {
    if (user) {
      const from = (location.state as any)?.from?.pathname || '/';
      navigate(from, { replace: true });
    }
  }, [user, navigate, location]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (!pseudonym.trim()) {
      setError('Please enter a username');
      return;
    }

    try {
      // For mock purposes, use the pseudonym as both username and password
      await login(pseudonym.trim(), 'password');
      const from = (location.state as any)?.from?.pathname || '/';
      navigate(from, { replace: true });
    } catch (error) {
      setError('Failed to login. Please try again.');
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background">
      <Card className="w-full max-w-md">
        <CardHeader className="space-y-1">
          <CardTitle className="text-2xl text-center">Welcome to BindingChaos</CardTitle>
          <CardDescription className="text-center">
            Enter your username to continue
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <label htmlFor="pseudonym" className="text-sm font-medium">
                Username
              </label>
              <Input
                id="pseudonym"
                type="text"
                value={pseudonym}
                onChange={(e) => setPseudonym(e.target.value)}
                placeholder="Enter your username"
                disabled={isLoading}
                autoFocus
                required
              />
            </div>
            
            {error && (
              <p className="text-sm text-destructive">{error}</p>
            )}
            
                          <Button
                type="submit"
                className="w-full"
                loading={isLoading}
                disabled={!pseudonym.trim()}
              >
                Sign In
              </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  );
} 