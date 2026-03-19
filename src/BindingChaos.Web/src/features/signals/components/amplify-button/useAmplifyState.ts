import { useState, useCallback, useRef, useEffect } from 'react';
import { useMutation } from '@tanstack/react-query';
import { useApiClient } from '../../../../shared/hooks/useApiClient';

interface UseAmplifyStateProps {
  signalId: string;
  initialAmplifyCount: number;
  initialIsAmplified: boolean;
  reason?: string;
  onSuccess?: () => void;
  onError?: (error: Error) => void;
}

interface AmplifyState {
  amplifyCount: number;
  isAmplified: boolean;
  isPending: boolean;
  showCommentary: boolean;
  showDeamplifyConfirm: boolean;
}

export function useAmplifyState({
  signalId,
  initialAmplifyCount,
  initialIsAmplified,
  reason = 'HighRelevance',
  onSuccess,
  onError
}: UseAmplifyStateProps) {
  const apiClient = useApiClient();

  // Use refs to track the latest state values to avoid stale closures
  const stateRef = useRef<{ amplifyCount: number; isAmplified: boolean }>({
    amplifyCount: initialAmplifyCount,
    isAmplified: initialIsAmplified
  });

  const [state, setState] = useState<AmplifyState>({
    amplifyCount: initialAmplifyCount,
    isAmplified: initialIsAmplified,
    isPending: false,
    showCommentary: false,
    showDeamplifyConfirm: false
  });

  // Update refs whenever state changes
  useEffect(() => {
    stateRef.current = {
      amplifyCount: state.amplifyCount,
      isAmplified: state.isAmplified
    };
  }, [state.amplifyCount, state.isAmplified]);

  // Update state from props (e.g., when query refetches)
  const updateFromProps = useCallback((amplifyCount: number, isAmplified: boolean) => {
    setState(prev => {
      // Don't update if we're in a pending state
      if (prev.isPending) {
        return prev;
      }
      
      // Don't update if the values are the same
      if (prev.amplifyCount === amplifyCount && prev.isAmplified === isAmplified) {
        return prev;
      }
      
      return {
        ...prev,
        amplifyCount,
        isAmplified
      };
    });
  }, []);

  const handleAmplify = useCallback(async (commentary?: string) => {
    try {
      const response = await apiClient.signals.amplifySignal({
        signalId,
        amplifySignalRequest: {
          reason,
          commentary: commentary?.trim() || null
        }
      });
      return response;
    } catch (error) {
      const errorMessage = error instanceof Error ? error : new Error('Failed to amplify signal');
      onError?.(errorMessage);
      throw error;
    }
  }, [apiClient, signalId, reason, onError]);

  const handleDeamplify = useCallback(async () => {
    try {
      const response = await apiClient.signals.deamplifySignal({
        signalId
      });
      return response;
    } catch (error) {
      const errorMessage = error instanceof Error ? error : new Error('Failed to deamplify signal');
      onError?.(errorMessage);
      throw error;
    }
  }, [apiClient, signalId, onError]);

  const amplifySignalMutation = useMutation({
    mutationFn: handleAmplify,
    onMutate: async () => {
      // Optimistic update
      setState(prev => ({
        ...prev,
        isPending: true,
        amplifyCount: prev.amplifyCount + 1,
        isAmplified: true,
        showCommentary: false
      }));
    },
    onSuccess: (response) => {
      if (response.data) {
        // Update with actual server response
        setState(prev => ({
          ...prev,
          isPending: false,
          amplifyCount: response.data?.newAmplifyCount ?? prev.amplifyCount,
          isAmplified: true,
          showCommentary: false
        }));
      }

      onSuccess?.();
    },
    onError: (error: Error) => {
      // Revert optimistic update on error
      setState(prev => ({
        ...prev,
        isPending: false,
        amplifyCount: stateRef.current.amplifyCount,
        isAmplified: stateRef.current.isAmplified,
        showCommentary: false
      }));
      onError?.(error);
    }
  });

  const deamplifySignalMutation = useMutation({
    mutationFn: handleDeamplify,
    onMutate: async () => {
      // Optimistic update
      setState(prev => ({
        ...prev,
        isPending: true,
        amplifyCount: Math.max(0, prev.amplifyCount - 1),
        isAmplified: false,
        showDeamplifyConfirm: false
      }));
    },
    onSuccess: (response) => {
      if (response.data) {
        // Update with actual server response
        setState(prev => ({
          ...prev,
          isPending: false,
          amplifyCount: response.data?.newAmplifyCount ?? prev.amplifyCount,
          isAmplified: false,
          showDeamplifyConfirm: false
        }));
      }

      onSuccess?.();
    },
    onError: (error: Error) => {
      // Revert optimistic update on error
      setState(prev => ({
        ...prev,
        isPending: false,
        amplifyCount: stateRef.current.amplifyCount,
        isAmplified: stateRef.current.isAmplified,
        showDeamplifyConfirm: false
      }));
      onError?.(error);
    }
  });

  const setShowCommentary = useCallback((show: boolean) => {
    setState(prev => ({ ...prev, showCommentary: show }));
  }, []);

  const setShowDeamplifyConfirm = useCallback((show: boolean) => {
    setState(prev => ({ ...prev, showDeamplifyConfirm: show }));
  }, []);

  return {
    // State
    ...state,
    
    // Actions
    setShowCommentary,
    setShowDeamplifyConfirm,
    updateFromProps,
    
    // Mutations
    amplifySignalMutation,
    deamplifySignalMutation
  };
}
