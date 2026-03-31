import { useState, useCallback, useRef, useEffect } from 'react';
import { useMutation } from '@tanstack/react-query';
import { useApiClient } from '../../../../shared/hooks/useApiClient';
import { toast } from '../../../../shared/components/ui/toast';

interface UseAmplifyStateProps {
  signalId: string;
  initialAmplifyCount: number;
  initialIsAmplified: boolean;
  onAmplifySuccess?: () => void;
  onDeamplifySuccess?: () => void;
  onAmplifyError?: (error: Error) => void;
  onDeamplifyError?: (error: Error) => void;
}

export function useAmplifyState({
  signalId,
  initialAmplifyCount,
  initialIsAmplified,
  onAmplifySuccess,
  onDeamplifySuccess,
  onAmplifyError,
  onDeamplifyError,
}: UseAmplifyStateProps) {
  const apiClient = useApiClient();

  const [amplifyCount, setAmplifyCount] = useState(initialAmplifyCount);
  const [isAmplified, setIsAmplified] = useState(initialIsAmplified);
  const [isPending, setIsPending] = useState(false);

  // Track pre-mutation state for rollback
  const preUpdateRef = useRef({ amplifyCount: initialAmplifyCount, isAmplified: initialIsAmplified });

  // Read isPending via ref so the props-sync effect below doesn't re-trigger
  // when isPending flips false after a mutation (which would overwrite the optimistic update).
  const isPendingRef = useRef(isPending);
  isPendingRef.current = isPending;

  // Sync from props when the parent query provides fresh data. Intentionally
  // omits isPending from deps — we check the ref to guard against running
  // mid-mutation, but we don't want the transition from pending→settled to
  // trigger this sync before the parent has refetched.
  useEffect(() => {
    if (!isPendingRef.current) {
      setAmplifyCount(initialAmplifyCount);
      setIsAmplified(initialIsAmplified);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [initialAmplifyCount, initialIsAmplified]);

  const amplifyMutation = useMutation({
    mutationFn: () =>
      apiClient.signals.amplifySignal({ signalId }),
    onMutate: () => {
      preUpdateRef.current = { amplifyCount, isAmplified };
      setIsPending(true);
      setIsAmplified(true);
      setAmplifyCount(c => c + 1);
    },
    onSuccess: response => {
      const serverCount = response.data?.newAmplifyCount;
      if (serverCount !== undefined && serverCount !== null) {
        setAmplifyCount(serverCount);
      }
      setIsPending(false);
      toast.success('Signal amplified');
      onAmplifySuccess?.();
    },
    onError: (error: Error) => {
      setAmplifyCount(preUpdateRef.current.amplifyCount);
      setIsAmplified(preUpdateRef.current.isAmplified);
      setIsPending(false);
      onAmplifyError?.(error);
    },
  });

  const deamplifyMutation = useMutation({
    mutationFn: () => apiClient.signals.deamplifySignal({ signalId }),
    onMutate: () => {
      preUpdateRef.current = { amplifyCount, isAmplified };
      setIsPending(true);
      setIsAmplified(false);
      setAmplifyCount(c => Math.max(0, c - 1));
    },
    onSuccess: response => {
      const serverCount = response.data?.newAmplifyCount;
      if (serverCount !== undefined && serverCount !== null) {
        setAmplifyCount(serverCount);
      }
      setIsPending(false);
      onDeamplifySuccess?.();
    },
    onError: (error: Error) => {
      setAmplifyCount(preUpdateRef.current.amplifyCount);
      setIsAmplified(preUpdateRef.current.isAmplified);
      setIsPending(false);
      onDeamplifyError?.(error);
    },
  });

  const amplify = useCallback(() => amplifyMutation.mutate(), [amplifyMutation]);
  const deamplify = useCallback(() => deamplifyMutation.mutate(), [deamplifyMutation]);

  return { amplifyCount, isAmplified, isPending, amplify, deamplify };
}
