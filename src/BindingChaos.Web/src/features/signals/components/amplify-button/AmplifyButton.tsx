import { useState, useEffect, useRef } from 'react';
import { BoltIcon } from '@heroicons/react/24/outline';
import { Button } from '../../../../shared/components/layout/Button';
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
}

type ConfirmState = 'none' | 'confirm-amplify' | 'confirm-withdraw';

export function AmplifyButton({
  signalId,
  amplifyCount = 0,
  isAmplifiedByCurrentUser = false,
  isOriginator = false,
  size = 'sm',
  className = '',
  onSuccess,
  onError,
}: AmplifyButtonProps) {
  const { amplifyCount: localCount, isAmplified, isPending, amplify, deamplify } = useAmplifyState({
    signalId,
    initialAmplifyCount: amplifyCount,
    initialIsAmplified: isAmplifiedByCurrentUser,
    onAmplifySuccess: onSuccess,
    onDeamplifySuccess: onSuccess,
    onAmplifyError: onError,
    onDeamplifyError: onError,
  });

  const [confirmState, setConfirmState] = useState<ConfirmState>('none');
  const [isHovering, setIsHovering] = useState(false);
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!isPending) setConfirmState('none');
  }, [isPending]);

  const phase = confirmState !== 'none' ? confirmState : isAmplified ? 'amplified' : 'idle';

  useEffect(() => {
    if (confirmState === 'none') return;

    const handleClickOutside = (e: MouseEvent) => {
      if (containerRef.current && !containerRef.current.contains(e.target as Node)) {
        setConfirmState('none');
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [confirmState]);

  const handleClick = (e: React.MouseEvent) => {
    e.stopPropagation();
    if (phase === 'idle') {
      setConfirmState('confirm-amplify');
    } else if (phase === 'confirm-amplify') {
      amplify();
    } else if (phase === 'amplified') {
      setConfirmState('confirm-withdraw');
    } else if (phase === 'confirm-withdraw') {
      deamplify();
    }
  };

  const label = (() => {
    if (phase === 'confirm-amplify') return 'Confirm?';
    if (phase === 'amplified') return isHovering ? 'Withdraw?' : 'Amplified';
    if (phase === 'confirm-withdraw') return 'Confirm?';
    return 'Amplify';
  })();

  const variant = (() => {
    if (phase === 'confirm-withdraw') return 'danger' as const;
    if (phase === 'amplified') return 'outline' as const;
    return 'primary' as const;
  })();

  const extraClassName = phase === 'confirm-amplify'
    ? 'border-green-500 bg-green-600 hover:bg-green-700 text-white'
    : '';

  return (
    <div ref={containerRef} className="relative inline-block">
      <AuthRequiredButton action={isAmplified ? 'deamplify this signal' : 'amplify this signal'}>
        <Button
          onClick={handleClick}
          onMouseEnter={() => setIsHovering(true)}
          onMouseLeave={() => setIsHovering(false)}
          size={size}
          variant={variant}
          disabled={isPending || isOriginator}
          loading={isPending}
          className={`flex items-center gap-2 ${extraClassName} ${className}`}
          aria-label={`${label} signal`}
        >
          <BoltIcon className="w-4 h-4" aria-hidden="true" />
          <span>{label}</span>
          {localCount > 0 && (
            <span className="ml-1 px-1.5 py-0.5 rounded-full text-xs font-medium bg-current/20">
              {localCount}
            </span>
          )}
        </Button>
      </AuthRequiredButton>
    </div>
  );
}
