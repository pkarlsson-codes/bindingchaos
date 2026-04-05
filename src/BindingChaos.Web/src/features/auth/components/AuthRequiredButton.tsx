import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useOptionalAuth } from '../contexts/AuthContext';

interface AuthRequiredButtonProps {
  children: React.ReactElement;
  fallback?: React.ReactNode;
  action?: string;
  className?: string;
}

export function AuthRequiredButton({
  children,
  fallback,
  action = "perform this action",
  className = ""
}: AuthRequiredButtonProps) {
  const auth = useOptionalAuth();
  const user = auth?.user ?? null;
  const navigate = useNavigate();
  const [isHovered, setIsHovered] = useState(false);

  const handleAuthRequiredClick = () => {
    navigate('/login');
  };

  // If user is authenticated, render the original children as-is
  if (user) {
    return children;
  }

  // If fallback is provided, render it instead
  if (fallback) {
    return <>{fallback}</>;
  }

  // Extract visual props from children to render a single button that matches appearance
  const { onClick: _onClick, onMouseDown: _onMouseDown, onKeyDown: _onKeyDown, ...visualProps } = children.props as Record<string, unknown>;
  const childContent = (children.props as { children?: React.ReactNode }).children;

  return React.createElement(
    children.type as React.ElementType,
    {
      ...visualProps,
      className: [visualProps.className as string, 'opacity-75 hover:opacity-100', className].filter(Boolean).join(' '),
      onClick: handleAuthRequiredClick,
      onMouseEnter: () => setIsHovered(true),
      onMouseLeave: () => setIsHovered(false),
      'aria-label': `${action} - requires login`,
      title: 'Requires login',
    },
    isHovered ? 'Login to continue' : childContent
  );
}
