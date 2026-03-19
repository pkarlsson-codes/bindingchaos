import { useCallback, useMemo, useEffect } from 'react';
import { Button } from '../../../../shared/components/layout/Button';
import { BoltIcon } from '@heroicons/react/24/outline';
import { AmplifyCommentaryForm } from './AmplifyCommentaryForm';
import { DeamplifyInlineConfirmation } from './DeamplifyInlineConfirmation';
import { AuthRequiredButton } from '../../../../features/auth';
import { useAmplifyState } from './useAmplifyState';

export interface AmplifyButtonProps {
  signalId: string;
  amplifyCount?: number;
  isAmplifiedByCurrentUser?: boolean;
  isOriginator?: boolean;
  size?: 'sm' | 'md' | 'lg';
  className?: string;
  onSuccess?: () => void;
  onError?: (error: Error) => void;
  reason?: string;
}



export function AmplifyButton({ 
  signalId, 
  amplifyCount = 0, 
  isAmplifiedByCurrentUser = false,
  isOriginator = false,
  size = 'sm',
  className = '',
  onSuccess,
  onError,
  reason = 'HighRelevance'
}: AmplifyButtonProps) {
  
  const {
    // State from custom hook
    amplifyCount: localAmplifyCount,
    isAmplified: localIsAmplified,
    isPending,
    showCommentary,
    showDeamplifyConfirm,
    
    // Actions
    setShowCommentary,
    setShowDeamplifyConfirm,
    updateFromProps,
    
    // Mutations
    amplifySignalMutation,
    deamplifySignalMutation
  } = useAmplifyState({
    signalId,
    initialAmplifyCount: amplifyCount,
    initialIsAmplified: isAmplifiedByCurrentUser,
    reason,
    onSuccess,
    onError
  });

  // Update state when props change (e.g., from query refetch)
  useEffect(() => {
    updateFromProps(amplifyCount, isAmplifiedByCurrentUser);
  }, [amplifyCount, isAmplifiedByCurrentUser, updateFromProps]);

  const handleAmplifyClick = useCallback((e: React.MouseEvent) => {
    e.stopPropagation();
    
    if (localIsAmplified) {
      if (showDeamplifyConfirm) {
        // Execute deamplify
        deamplifySignalMutation.mutate();
      } else {
        // Show deamplify confirmation
        setShowDeamplifyConfirm(true);
      }
    } else if (showCommentary) {
      amplifySignalMutation.mutate('');
    } else {
      setShowCommentary(true);
    }
  }, [localIsAmplified, showCommentary, showDeamplifyConfirm, amplifySignalMutation, deamplifySignalMutation, setShowCommentary, setShowDeamplifyConfirm]);

  const handleCommentarySubmit = useCallback((commentary: string) => {
    amplifySignalMutation.mutate(commentary);
  }, [amplifySignalMutation]);

  const handleCommentaryCancel = useCallback(() => {
    setShowCommentary(false);
  }, [setShowCommentary]);



  const handleDeamplifyCancel = useCallback(() => {
    setShowDeamplifyConfirm(false);
  }, [setShowDeamplifyConfirm]);

  // Memoized button content
  const buttonContent = useMemo(() => {
    return (
      <>
        {localIsAmplified ? (
          <>
            <BoltIcon className="w-4 h-4" aria-hidden="true" />
            <span>{showDeamplifyConfirm ? 'Confirm' : 'Deamplify'}</span>
          </>
        ) : (
          <>
            <BoltIcon className="w-4 h-4" aria-hidden="true" />
            <span>Amplify</span>
          </>
        )}
        {localAmplifyCount > 0 && (
          <span 
            className={`ml-1 px-1.5 py-0.5 rounded-full text-xs font-medium ${
              isPending 
                ? 'bg-primary-foreground/10 animate-pulse' 
                : 'bg-primary-foreground/20'
            }`}
            aria-label={`${localAmplifyCount} amplifications`}
          >
            {isPending ? '...' : localAmplifyCount}
          </span>
        )}
      </>
    );
  }, [localIsAmplified, showDeamplifyConfirm, localAmplifyCount, isPending]);

  return (
    <div className="relative">
      <AuthRequiredButton action={localIsAmplified ? "deamplify this signal" : "amplify this signal"}>
        <Button
          onClick={handleAmplifyClick}
          size={size}
          variant={localIsAmplified && showDeamplifyConfirm ? "danger" : localIsAmplified ? "outline" : "primary"}
          disabled={isPending || isOriginator}
          className={`flex items-center gap-2 ${className}`}
          aria-label={`${localIsAmplified ? (showDeamplifyConfirm ? 'Confirm deamplify' : 'Deamplify') : 'Amplify'} signal${localAmplifyCount > 0 ? ` (${localAmplifyCount} amplifications)` : ''}`}
          aria-expanded={showCommentary || showDeamplifyConfirm}
          aria-haspopup="dialog"
          data-amplify-button
        >
          {buttonContent}
        </Button>
      </AuthRequiredButton>

      {showCommentary && (
        <AmplifyCommentaryForm
          signalId={signalId}
          onSubmit={handleCommentarySubmit}
          onCancel={handleCommentaryCancel}
          isSubmitting={isPending}
        />
      )}

      {showDeamplifyConfirm && (
        <DeamplifyInlineConfirmation
          onCancel={handleDeamplifyCancel}
          isSubmitting={isPending}
        />
      )}
    </div>
  );
} 