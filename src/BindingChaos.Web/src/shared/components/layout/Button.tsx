import React from 'react';
import { Button as ShadcnButton } from '../ui/button';
import { Icon } from './Icon';
import type { IconName } from './Icon';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'danger' | 'ghost' | 'outline';
  size?: 'sm' | 'md' | 'lg';
  loading?: boolean;
  icon?: IconName | React.ReactNode;
  children: React.ReactNode;
}

export function Button({ 
  variant = 'primary', 
  size = 'md', 
  loading = false,
  icon,
  children,
  className = '',
  disabled,
  ...props 
}: ButtonProps) {
  // Map old variants to shadcn/ui variants
  const variantMap = {
    primary: 'default' as const,
    secondary: 'secondary' as const,
    danger: 'destructive' as const,
    ghost: 'ghost' as const,
    outline: 'outline' as const
  };

  // Map old sizes to shadcn/ui sizes
  const sizeMap = {
    sm: 'sm' as const,
    md: 'default' as const,
    lg: 'lg' as const
  };

  return (
    <ShadcnButton
      variant={variantMap[variant]}
      size={sizeMap[size]}
      className={className}
      disabled={disabled || loading}
      {...props}
    >
      {loading && (
        <svg className="animate-spin -ml-1 mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24">
          <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
          <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
        </svg>
      )}
      {!loading && icon && (
        <span className="mr-2">
          {typeof icon === 'string' ? <Icon name={icon as IconName} /> : icon}
        </span>
      )}
      {children}
    </ShadcnButton>
  );
} 