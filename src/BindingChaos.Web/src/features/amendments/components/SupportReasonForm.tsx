import { useState, useCallback } from 'react';
import { Button } from '../../../shared/components/layout/Button';
import { Icon } from '../../../shared/components/layout/Icon';
import { useClickOutside } from '../../../shared/hooks/useClickOutside';

export interface SupportReasonFormProps {
  onSubmit: (reason: string) => void;
  onCancel: () => void;
  isSubmitting: boolean;
}

export function SupportReasonForm({ 
  onSubmit, 
  onCancel, 
  isSubmitting 
}: SupportReasonFormProps) {
  const [reason, setReason] = useState('');
  const reasonId = 'support-reason-input';
  
  // Handle click outside to close the form (disabled when submitting)
  const formRef = useClickOutside<HTMLDivElement>(onCancel, !isSubmitting);

  const handleSubmit = useCallback(() => {
    if (reason.trim()) {
      onSubmit(reason.trim());
    }
  }, [reason, onSubmit]);

  const handleKeyDown = useCallback((e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && (e.metaKey || e.ctrlKey)) {
      e.preventDefault();
      handleSubmit();
    }
  }, [handleSubmit]);

  return (
    <div 
      ref={formRef}
      className="absolute top-full left-0 mt-2 w-[420px] max-w-[calc(100vw-2rem)] p-4 bg-background border border-border rounded-lg shadow-lg z-10"
    >
      <div className="space-y-3">
        <label htmlFor={reasonId} className="block text-sm font-medium text-foreground">
          Why do you support this amendment?
        </label>
        
        <textarea
          id={reasonId}
          value={reason}
          onChange={(e) => setReason(e.target.value)}
          placeholder="Explain your reasoning for supporting this amendment..."
          className="w-full min-h-[80px] p-3 text-sm border border-border rounded-md bg-background text-foreground placeholder:text-muted-foreground resize-none focus:outline-none focus:ring-2 focus:ring-primary focus:border-transparent"
          disabled={isSubmitting}
          onKeyDown={handleKeyDown}
          maxLength={1000}
        />
        
        <div className="flex flex-wrap justify-between items-center gap-2">
          <span 
            className="text-xs text-muted-foreground"
            aria-live="polite"
          >
            {reason.length}/1000 characters
          </span>
          <div className="flex flex-wrap justify-end gap-2">
            <Button
              onClick={onCancel}
              variant="ghost"
              size="sm"
              disabled={isSubmitting}
            >
              Cancel
            </Button>
            <Button
              onClick={handleSubmit}
              variant="primary"
              size="sm"
              disabled={isSubmitting || !reason.trim()}
              className="flex items-center gap-2"
            >
              <Icon name="thumbs-up" size={16} />
              Support Amendment
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}
