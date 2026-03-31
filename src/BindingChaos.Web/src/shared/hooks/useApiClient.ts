import { useMemo } from 'react';
import {
  IdeasApi, SignalsApi, DiscourseApi, TagsApi, AuthApi, SocietiesApi,
  EmergingPatternsApi, Configuration
} from '@/api';
import { API_CONFIG } from '@/config/api';
import { useOptionalAuth } from '../../features/auth/contexts/AuthContext';

function getCookie(name: string): string | undefined {
  if (typeof document === 'undefined') return undefined;
  const value = `; ${document.cookie}`;
  const parts = value.split(`; ${name}=`);
  if (parts.length === 2) {
    const raw = parts.pop()!.split(';').shift();
    return raw ? decodeURIComponent(raw) : undefined;
  }
  return undefined;
}

function withCsrfMiddleware(config: Configuration): Configuration {
  const csrfMiddleware = {
    async pre(ctx: { url: string; init: RequestInit }) {
      const method = ctx.init.method?.toString().toUpperCase();
      if (method && method !== 'GET' && method !== 'HEAD' && method !== 'OPTIONS') {
        const token = getCookie('bc_csrf');
        if (token) {
          let headers: Headers;
          if (ctx.init.headers instanceof Headers) {
            headers = ctx.init.headers;
          } else if (Array.isArray(ctx.init.headers)) {
            headers = new Headers(ctx.init.headers);
          } else {
            headers = new Headers(ctx.init.headers as Record<string, string> | undefined);
          }
          headers.set('X-CSRF-Token', token);
          return { url: ctx.url, init: { ...ctx.init, headers } };
        }
      }
      return { url: ctx.url, init: ctx.init };
    }
  };

  return new Configuration({
    basePath: config.basePath,
    credentials: config.credentials,
    headers: config.headers,
    middleware: [...config.middleware, csrfMiddleware]
  });
}

/**
 * Custom hook to create and configure the API client
 * This is the single source of truth for API client configuration
 */
export function useApiClient() {
  const auth = useOptionalAuth();
  const user = auth?.user ?? null;

  const client = useMemo(() => {
    const baseHeaders: Record<string, string> = {};
    if (user) {
      baseHeaders['X-User-ID'] = user.id || '';
      baseHeaders['X-User-Pseudonym'] = user.pseudonym || '';
    }

    const baseConfig = new Configuration({
      basePath: API_CONFIG.baseUrl,
      credentials: 'include',
      headers: baseHeaders
    });
    const config = withCsrfMiddleware(baseConfig);

    return {
      ideas: new IdeasApi(config),
      signals: new SignalsApi(config),
      discourse: new DiscourseApi(config),
      tags: new TagsApi(config),
      auth: new AuthApi(config),
      societies: new SocietiesApi(config),
      emergingPatterns: new EmergingPatternsApi(config),
    };
  }, [user]);

  return client;
}