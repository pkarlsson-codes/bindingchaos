import { useState, useCallback, useRef, useEffect } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useApiClient } from './useApiClient';
import { toast } from '../components/ui/toast';

interface UseAffectednessStateProps {
  concernId: string;
  initialAffectedCount: number;
  initialIsAffected: boolean;
}

export function useAffectednessState({
  concernId,
  initialAffectedCount,
  initialIsAffected,
}: UseAffectednessStateProps) {
  const apiClient = useApiClient();
  const queryClient = useQueryClient();

  const [affectedCount, setAffectedCount] = useState(initialAffectedCount);
  const [isAffected, setIsAffected] = useState(initialIsAffected);
  const [isPending, setIsPending] = useState(false);

  const preUpdateRef = useRef({ affectedCount: initialAffectedCount, isAffected: initialIsAffected });
  const isPendingRef = useRef(isPending);
  isPendingRef.current = isPending;

  useEffect(() => {
    if (!isPendingRef.current) {
      setAffectedCount(initialAffectedCount);
      setIsAffected(initialIsAffected);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [initialAffectedCount, initialIsAffected]);

  const declareMutation = useMutation({
    mutationFn: () => apiClient.concerns.declareAffected({ concernId }),
    onMutate: () => {
      preUpdateRef.current = { affectedCount, isAffected };
      setIsPending(true);
      setIsAffected(true);
      setAffectedCount(c => c + 1);
    },
    onSuccess: () => {
      setIsPending(false);
      queryClient.invalidateQueries({ queryKey: ['concerns'] });
    },
    onError: (error: Error) => {
      setAffectedCount(preUpdateRef.current.affectedCount);
      setIsAffected(preUpdateRef.current.isAffected);
      setIsPending(false);
      toast.error('Failed to declare affectedness');
      console.error(error);
    },
  });

  const withdrawMutation = useMutation({
    mutationFn: () => apiClient.concerns.withdrawAffectedness({ concernId }),
    onMutate: () => {
      preUpdateRef.current = { affectedCount, isAffected };
      setIsPending(true);
      setIsAffected(false);
      setAffectedCount(c => Math.max(0, c - 1));
    },
    onSuccess: () => {
      setIsPending(false);
      queryClient.invalidateQueries({ queryKey: ['concerns'] });
    },
    onError: (error: Error) => {
      setAffectedCount(preUpdateRef.current.affectedCount);
      setIsAffected(preUpdateRef.current.isAffected);
      setIsPending(false);
      toast.error('Failed to withdraw affectedness');
      console.error(error);
    },
  });

  const toggle = useCallback(() => {
    if (isPending) return;
    if (isAffected) {
      withdrawMutation.mutate();
    } else {
      declareMutation.mutate();
    }
  }, [isPending, isAffected, declareMutation, withdrawMutation]);

  return { affectedCount, isAffected, isPending, toggle };
}
