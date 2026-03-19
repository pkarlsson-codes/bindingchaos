/**
 * Authentication Configuration
 * Centralized configuration for authentication settings
 */

export const AUTH_CONFIG = {
  sessionTimeout: 24 * 60 * 60 * 1000,
  sessionCheckInterval: 5 * 60 * 1000,
  sessionWarningThreshold: 10 * 60 * 1000,
  storageKey: 'bindingchaos_auth',
  maxLoginAttempts: 5,
  lockoutDuration: 15 * 60 * 1000,
  showSessionWarning: true,
  autoRefreshSession: true,
  messages: {
    loginFailed: 'Login failed. Please check your credentials and try again.',
    sessionExpired: 'Your session has expired. Please log in again.',
    networkError: 'Network error. Please check your connection and try again.',
    serverError: 'Server error. Please try again later.',
    tooManyAttempts: 'Too many login attempts. Please try again later.',
    invalidCredentials: 'Invalid username or password.',
  },
} as const;

/**
 * Get session expiry time from now
 */
export function getSessionExpiryTime(): number {
  return Date.now() + AUTH_CONFIG.sessionTimeout;
}

/**
 * Check if a timestamp is expired
 */
export function isExpired(timestamp: number): boolean {
  return Date.now() > timestamp;
}

/**
 * Get time until expiry in milliseconds
 */
export function getTimeUntilExpiry(expiryTime: number): number {
  return Math.max(0, expiryTime - Date.now());
} 