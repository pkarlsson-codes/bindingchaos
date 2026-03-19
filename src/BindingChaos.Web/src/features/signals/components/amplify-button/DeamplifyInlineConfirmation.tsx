import { useCallback, useEffect, useRef } from 'react';

interface DeamplifyInlineConfirmationProps {
  onCancel: () => void;
  isSubmitting?: boolean;
}

export function DeamplifyInlineConfirmation({ 
  onCancel, 
  isSubmitting = false 
}: DeamplifyInlineConfirmationProps) {
  const timeoutRef = useRef<NodeJS.Timeout>();

  // Auto-cancel after 10 seconds
  useEffect(() => {
    if (!isSubmitting) {
      timeoutRef.current = setTimeout(() => {
        onCancel();
      }, 10000);
    }

    return () => {
      if (timeoutRef.current) {
        clearTimeout(timeoutRef.current);
      }
    };
  }, [onCancel, isSubmitting]);

  // Handle clicks outside to cancel
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      // Check if click is outside the button area
      const target = event.target as Element;
      if (!target.closest('[data-amplify-button]')) {
        onCancel();
      }
    };

    // Add event listener with a small delay to avoid immediate closure
    const timeoutId = setTimeout(() => {
      document.addEventListener('mousedown', handleClickOutside);
    }, 100);

    return () => {
      clearTimeout(timeoutId);
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [onCancel]);

  return null; // No visible UI, just handles auto-cancellation
} 