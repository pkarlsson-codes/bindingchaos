import { useState, useCallback } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useApiClient } from '../../../shared/hooks/useApiClient';

export type UserVoteState = 'none' | 'supporting' | 'opposing';

interface AmendmentVotingInitialState {
  supporterCount: number;
  opponentCount: number;
  userVoteState: UserVoteState;
  createdByCurrentUser: boolean;
  isOpen: boolean;
}

interface UseAmendmentVotingProps {
  amendmentId: string;
  ideaId: string;
  initialState: AmendmentVotingInitialState;
  onSuccess?: () => void;
  onError?: (error: Error) => void;
}

interface AmendmentVotingState {
  supporterCount: number;
  opponentCount: number;
  userVoteState: UserVoteState;
  isPending: boolean;
}

interface AmendmentVotingActions {
  support: (reason: string) => void;
  oppose: (reason: string) => void;
  withdrawSupport: () => void;
  withdrawOpposition: () => void;
}

export function useAmendmentVoting({
  amendmentId,
  ideaId,
  initialState,
  onSuccess,
  onError
}: UseAmendmentVotingProps) {
  const apiClient = useApiClient();
  const queryClient = useQueryClient();

  const [state, setState] = useState<AmendmentVotingState>({
    supporterCount: initialState.supporterCount,
    opponentCount: initialState.opponentCount,
    userVoteState: initialState.userVoteState,
    isPending: false
  });

  const canVote = !initialState.createdByCurrentUser && initialState.isOpen;

  const supportMutation = useMutation({
    mutationFn: async (reason: string) => {
      if (!canVote) {
        throw new Error('Cannot vote on your own amendment or amendment is not open');
      }
      
      const response = await apiClient.amendments.supportAmendment({
        ideaId,
        amendmentId,
        supportAmendmentRequest: { reason: reason.trim() }
      });
      return response;
    },
    onMutate: async () => {
      setState(prev => ({
        ...prev,
        supporterCount: prev.supporterCount + 1,
        userVoteState: 'supporting',
        isPending: true
      }));
      await queryClient.cancelQueries({ queryKey: ['amendments', ideaId] });
    },
    onSuccess: (response) => {
      setState(prev => ({
        ...prev,
        supporterCount: response.supporterCount ?? prev.supporterCount,
        opponentCount: response.opponentCount ?? prev.opponentCount,
        isPending: false
      }));
      queryClient.invalidateQueries({ queryKey: ['amendments', ideaId] });
      onSuccess?.();
    },
    onError: (error) => {
      setState(prev => ({
        ...prev,
        supporterCount: Math.max(0, prev.supporterCount - 1),
        userVoteState: 'none',
        isPending: false
      }));
      const errorMessage = error instanceof Error ? error : new Error('Failed to support amendment');
      onError?.(errorMessage);
    }
  });

  const opposeMutation = useMutation({
    mutationFn: async (reason: string) => {
      if (!canVote) {
        throw new Error('Cannot vote on your own amendment or amendment is not open');
      }
      
      const response = await apiClient.amendments.opposeAmendment({
        ideaId,
        amendmentId,
        opposeAmendmentRequest: { reason: reason.trim() }
      });
      return response;
    },
    onMutate: async () => {
      setState(prev => ({
        ...prev,
        opponentCount: prev.opponentCount + 1,
        userVoteState: 'opposing',
        isPending: true
      }));
      await queryClient.cancelQueries({ queryKey: ['amendments', ideaId] });
    },
    onSuccess: (response) => {
      setState(prev => ({
        ...prev,
        supporterCount: response.supporterCount ?? prev.supporterCount,
        opponentCount: response.opponentCount ?? prev.opponentCount,
        isPending: false
      }));
      queryClient.invalidateQueries({ queryKey: ['amendments', ideaId] });
      onSuccess?.();
    },
    onError: (error) => {
      setState(prev => ({
        ...prev,
        opponentCount: Math.max(0, prev.opponentCount - 1),
        userVoteState: 'none',
        isPending: false
      }));
      const errorMessage = error instanceof Error ? error : new Error('Failed to oppose amendment');
      onError?.(errorMessage);
    }
  });

  const withdrawSupportMutation = useMutation({
    mutationFn: async () => {
      const response = await apiClient.amendments.withdrawSupport({
        ideaId,
        amendmentId
      });
      return response;
    },
    onMutate: async () => {
      setState(prev => ({
        ...prev,
        supporterCount: Math.max(0, prev.supporterCount - 1),
        userVoteState: 'none',
        isPending: true
      }));
      await queryClient.cancelQueries({ queryKey: ['amendments', ideaId] });
    },
    onSuccess: (/*response*/) => {
      setState(prev => ({
        ...prev,
        // TODO: We're no longer getting this data:
        // supporterCount: response.supporterCount ?? prev.supporterCount,
        // opponentCount: response.opponentCount ?? prev.opponentCount,
        isPending: false
      }));
      queryClient.invalidateQueries({ queryKey: ['amendments', ideaId] });
      onSuccess?.();
    },
    onError: (error) => {
      setState(prev => ({
        ...prev,
        supporterCount: prev.supporterCount + 1,
        userVoteState: 'supporting',
        isPending: false
      }));
      const errorMessage = error instanceof Error ? error : new Error('Failed to withdraw support');
      onError?.(errorMessage);
    }
  });

  const withdrawOppositionMutation = useMutation({
    mutationFn: async () => {
      const response = await apiClient.amendments.withdrawOpposition({
        ideaId,
        amendmentId,
        body: {}
      });
      return response;
    },
    onMutate: async () => {
      setState(prev => ({
        ...prev,
        opponentCount: Math.max(0, prev.opponentCount - 1),
        userVoteState: 'none',
        isPending: true
      }));
      await queryClient.cancelQueries({ queryKey: ['amendments', ideaId] });
    },
    onSuccess: (response) => {
      setState(prev => ({
        ...prev,
        supporterCount: response.supporterCount ?? prev.supporterCount,
        opponentCount: response.opponentCount ?? prev.opponentCount,
        isPending: false
      }));
      queryClient.invalidateQueries({ queryKey: ['amendments', ideaId] });
      onSuccess?.();
    },
    onError: (error) => {
      setState(prev => ({
        ...prev,
        opponentCount: prev.opponentCount + 1,
        userVoteState: 'opposing',
        isPending: false
      }));
      const errorMessage = error instanceof Error ? error : new Error('Failed to withdraw opposition');
      onError?.(errorMessage);
    }
  });

  const actions: AmendmentVotingActions = {
    support: useCallback((reason: string) => {
      if (state.userVoteState === 'opposing') {
        onError?.(new Error('Withdraw your opposition before supporting this amendment.'));
        return;
      }

      supportMutation.mutate(reason);
    }, [supportMutation, state.userVoteState, onError]),
    
    oppose: useCallback((reason: string) => {
      if (state.userVoteState === 'supporting') {
        onError?.(new Error('Withdraw your support before opposing this amendment.'));
        return;
      }

      opposeMutation.mutate(reason);
    }, [opposeMutation, state.userVoteState, onError]),
    
    withdrawSupport: useCallback(() => {
      withdrawSupportMutation.mutate();
    }, [withdrawSupportMutation]),
    
    withdrawOpposition: useCallback(() => {
      withdrawOppositionMutation.mutate();
    }, [withdrawOppositionMutation])
  };

  return {
    supporterCount: state.supporterCount,
    opponentCount: state.opponentCount,
    userVoteState: state.userVoteState,
    isPending: state.isPending,
    canVote,
    actions
  };
}
