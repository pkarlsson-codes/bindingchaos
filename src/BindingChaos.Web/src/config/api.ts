/**
 * API Configuration
 * Centralized configuration for API endpoints and settings
 */

export const API_CONFIG = {
  baseUrl: import.meta.env.VITE_GATEWAY_URL || 'http://localhost:4000',
  version: 'v1',
  timeout: 30000,
  retryAttempts: 3,
  retryDelay: 1000,
} as const;

/**
 * Get the full API base path including version
 */
export function getApiBasePath(): string {
  return `${API_CONFIG.baseUrl}/api/${API_CONFIG.version}`;
}

/**
 * Get the full URL for a specific endpoint
 */
export function getApiUrl(endpoint: string): string {
  return `${getApiBasePath()}/${endpoint.replace(/^\/+/, '')}`;
} 