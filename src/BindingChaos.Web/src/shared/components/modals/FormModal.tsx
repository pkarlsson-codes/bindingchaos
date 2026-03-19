import React from 'react';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from '../ui/dialog';
import { Button } from '../layout/Button';

interface FormModalProps {
  isOpen: boolean;
  onClose: () => void;
  title: string;
  description?: string;
  onSubmit: (e: React.FormEvent) => void;
  submitText: string;
  loading?: boolean;
  disabled?: boolean;
  children: React.ReactNode;
  size?: 'sm' | 'md' | 'lg' | 'xl';
}

export function FormModal({
  isOpen,
  onClose,
  title,
  description,
  onSubmit,
  submitText,
  loading = false,
  disabled = false,
  children,
  size = 'md'
}: FormModalProps) {
  const sizeClasses = {
    sm: 'max-w-sm',
    md: 'max-w-md',
    lg: 'max-w-lg',
    xl: 'max-w-xl'
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className={sizeClasses[size]}>
        <DialogHeader>
          <DialogTitle>{title}</DialogTitle>
          {description && <DialogDescription>{description}</DialogDescription>}
        </DialogHeader>
        
        <form onSubmit={onSubmit} className="space-y-4">
          {children}
          
          <DialogFooter>
            <Button
              type="submit"
              variant="primary"
              loading={loading}
              disabled={disabled || loading}
            >
              {submitText}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
} 