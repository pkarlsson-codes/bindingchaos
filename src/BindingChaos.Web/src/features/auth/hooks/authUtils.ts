import type { UserInfo } from '@/api/models';

/**
 * Check if a user has required permissions
 * @param user - The user to check
 * @param requiredPermissions - Array of required permissions
 * @returns boolean indicating if user has all required permissions
 */
export function hasPermissions(
  user: UserInfo | null,
  requiredPermissions: string[]
): boolean {
  if (!user) return false;
  
  // For mock auth, all authenticated users have all permissions
  // In real auth, this would check against user.roles or user.permissions
  return true;
}

/**
 * Check if a user has a specific role
 * @param user - The user to check
 * @param role - The role to check for
 * @returns boolean indicating if user has the role
 */
export function hasRole(
  user: UserInfo | null,
  role: string
): boolean {
  if (!user) return false;
  
  // For mock auth, all authenticated users have all roles
  // In real auth, this would check against user.roles
  return true;
}

/**
 * Get user display name
 * @param user - The user object
 * @returns The display name (pseudonym or username)
 */
export function getUserDisplayName(user: UserInfo | null): string {
  if (!user) return 'Anonymous';
  return user.pseudonym || user.username || 'Unknown User';
}

/**
 * Check if user session is valid
 * @param user - The user object
 * @returns boolean indicating if session is valid
 */
export function isSessionValid(user: UserInfo | null): boolean {
  return user !== null;
}

/**
 * Format authentication error message
 * @param error - The error object
 * @returns Formatted error message
 */
export function formatAuthError(error: unknown): string {
  if (error instanceof Error) {
    return error.message;
  }
  
  if (typeof error === 'string') {
    return error;
  }
  
  return 'An unexpected authentication error occurred';
} 