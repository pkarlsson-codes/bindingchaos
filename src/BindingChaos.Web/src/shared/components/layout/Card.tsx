import React from 'react';
import {
  Card as ShadCard,
  CardAction,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card"
import { cn } from "@/shared/lib/utils"

interface CardProps {
  className?: string;
  // Header props
  title?: React.ReactNode;
  description?: React.ReactNode;
  headerAction?: React.ReactNode;
  // Content props
  content?: React.ReactNode;
  // Footer props
  footer?: React.ReactNode;
  footerClassName?: string;
}

export function Card({ 
  className,
  title,
  description,
  headerAction,
  content,
  footer,
  footerClassName
}: CardProps) {
  return (
    <ShadCard className={className}>
      {(title || description || headerAction) && (
        <CardHeader>
          {title && <CardTitle>{title}</CardTitle>}
          {description && <CardDescription>{description}</CardDescription>}
          {headerAction && <CardAction>{headerAction}</CardAction>}
        </CardHeader>
      )}
      
      {content && <CardContent>{content}</CardContent>}
      
      {footer && <CardFooter className={footerClassName}>{footer}</CardFooter>}
    </ShadCard>
  );
}

// Export all the shadcn/ui Card sub-components for direct use
export {
  CardAction,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
};
