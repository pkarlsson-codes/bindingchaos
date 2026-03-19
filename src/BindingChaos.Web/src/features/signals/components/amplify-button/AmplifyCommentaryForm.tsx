import { useState, useCallback, useMemo, useEffect, useRef } from 'react';
import { Button } from '../../../../shared/components/layout/Button';
import { 
  BoltIcon, 
  ChatBubbleLeftIcon,
  XMarkIcon
} from '@heroicons/react/24/outline';

interface AmplifyCommentaryFormProps {
  signalId: string;
  onSubmit: (commentary: string) => void;
  onCancel: () => void;
  isSubmitting: boolean;
}

export function AmplifyCommentaryForm({ 
  signalId, 
  onSubmit, 
  onCancel, 
  isSubmitting 
}: AmplifyCommentaryFormProps) {
  const [commentary, setCommentary] = useState('');
  const formRef = useRef<HTMLDivElement>(null);

  // Handle clicks outside the form to close it
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (formRef.current && !formRef.current.contains(event.target as Node)) {
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

  const handleSubmit = useCallback((e: React.MouseEvent) => {
    e.stopPropagation();
    onSubmit(commentary);
  }, [onSubmit, commentary]);

  const handleCancel = useCallback((e: React.MouseEvent) => {
    e.stopPropagation();
    onCancel();
  }, [onCancel]);

  const handleKeyDown = useCallback((e: React.KeyboardEvent) => {
    if (e.key === 'Escape') {
      onCancel();
    }
  }, [onCancel]);

  // Memoized commentary form ID for accessibility
  const commentaryId = useMemo(() => `commentary-${signalId}`, [signalId]);

  return (
    <div 
      ref={formRef}
      className="absolute top-0 right-0 z-10 bg-background border border-border rounded-lg shadow-lg p-4 w-80"
      role="dialog"
      aria-labelledby={`${commentaryId}-label`}
      aria-describedby={`${commentaryId}-description`}
      onKeyDown={handleKeyDown}
    >
      <div className="flex items-center justify-between mb-3">
        <label 
          id={`${commentaryId}-label`}
          htmlFor={commentaryId} 
          className="text-sm font-medium flex items-center gap-2"
        >
          <ChatBubbleLeftIcon className="w-4 h-4" aria-hidden="true" />
          Add commentary (optional)
        </label>
        <button
          onClick={handleCancel}
          className="text-muted-foreground hover:text-foreground transition-colors"
          aria-label="Close commentary form"
        >
          <XMarkIcon className="w-4 h-4" aria-hidden="true" />
        </button>
      </div>
      
      <div id={`${commentaryId}-description`} className="sr-only">
        Enter your commentary for amplifying this signal. Press Escape to cancel.
      </div>
      
      <textarea
        id={commentaryId}
        value={commentary}
        onChange={(e) => setCommentary(e.target.value)}
        onClick={(e) => e.stopPropagation()}
        placeholder="Why are you amplifying this signal?"
        className="w-full px-3 py-2 text-sm border border-border rounded-md focus:outline-none focus:ring-2 focus:ring-ring focus:border-ring resize-none"
        rows={3}
        maxLength={500}
        aria-describedby={`${commentaryId}-char-count`}
      />
      
      <div className="flex justify-between items-center mt-3">
        <span 
          id={`${commentaryId}-char-count`}
          className="text-xs text-muted-foreground"
          aria-live="polite"
        >
          {commentary.length}/500 characters
        </span>
        <Button
          onClick={handleSubmit}
          variant="primary"
          size="sm"
          disabled={isSubmitting}
          className="flex items-center gap-2"
        >
          <BoltIcon className="w-4 h-4" aria-hidden="true" />
          Amplify
        </Button>
      </div>
    </div>
  );
} 