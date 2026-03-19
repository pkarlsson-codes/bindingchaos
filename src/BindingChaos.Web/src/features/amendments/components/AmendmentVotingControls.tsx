import { useState, useCallback } from 'react';
import { Button } from '../../../shared/components/layout/Button';
import { Icon } from '../../../shared/components/layout/Icon';
import { ToggleGroup, ToggleGroupItem } from '../../../shared/components/ui/ui/toggle-group';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '../../../shared/components/ui/dialog';
import { AuthRequiredButton } from '../../auth';
import { useAmendmentVoting } from './useAmendmentVoting';
import type { UserVoteState } from './useAmendmentVoting';
import { SupportReasonForm } from './SupportReasonForm';
import { OpposeReasonForm } from './OpposeReasonForm';

export interface AmendmentVotingControlsProps {
  amendment: {
    id: string;
    supporterCount: number;
    opponentCount: number;
    proposedByCurrentUser: boolean;
    supportedByCurrentUser: boolean;
    opposedByCurrentUser: boolean;
    isOpen: boolean;
    status: string;
  };
  ideaId: string;
  size?: 'sm' | 'md' | 'lg';
  className?: string;
  onSuccess?: () => void;
  onError?: (error: Error) => void;
}

export function AmendmentVotingControls({ 
  amendment, 
  ideaId,
  size = 'sm',
  className = '',
  onSuccess,
  onError
}: AmendmentVotingControlsProps) {
  const [showSupportReasonForm, setShowSupportReasonForm] = useState(false);
  const [showOpposeReasonForm, setShowOpposeReasonForm] = useState(false);
  const [showWithdrawModal, setShowWithdrawModal] = useState(false);
  const [pendingWithdrawalVoteState, setPendingWithdrawalVoteState] = useState<UserVoteState>('none');
  
  const getInitialUserVoteState = (): UserVoteState => {
    if (amendment.supportedByCurrentUser) return 'supporting';
    if (amendment.opposedByCurrentUser) return 'opposing';
    return 'none';
  };

  const {
    supporterCount,
    opponentCount,
    userVoteState,
    isPending,
    canVote,
    actions
  } = useAmendmentVoting({
    amendmentId: amendment.id,
    ideaId,
    initialState: {
      supporterCount: amendment.supporterCount,
      opponentCount: amendment.opponentCount,
      userVoteState: getInitialUserVoteState(),
      createdByCurrentUser: amendment.proposedByCurrentUser,
      isOpen: amendment.isOpen
    },
    onSuccess,
    onError
  });

  const handleVoteSelection = useCallback((voteType: string) => {
    if (isPending) {
      return;
    }

    if (voteType === 'support') {
      setShowOpposeReasonForm(false);
      setShowSupportReasonForm(true);
    } else if (voteType === 'oppose') {
      setShowSupportReasonForm(false);
      setShowOpposeReasonForm(true);
    }
  }, [isPending]);

  const handleWithdrawVote = useCallback((e: React.MouseEvent) => {
    e.stopPropagation();

    if (isPending) {
      return;
    }

    if (userVoteState === 'supporting' || userVoteState === 'opposing') {
      setPendingWithdrawalVoteState(userVoteState);
      setShowWithdrawModal(true);
    }

  }, [userVoteState, isPending]);

  const handleConfirmWithdraw = useCallback(() => {
    if (pendingWithdrawalVoteState === 'supporting') {
      actions.withdrawSupport();
    } else if (pendingWithdrawalVoteState === 'opposing') {
      actions.withdrawOpposition();
    }

    setShowWithdrawModal(false);
    setPendingWithdrawalVoteState('none');
  }, [pendingWithdrawalVoteState, actions]);

  const handleCancelWithdraw = useCallback(() => {
    if (isPending) {
      return;
    }

    setShowWithdrawModal(false);
    setPendingWithdrawalVoteState('none');
  }, [isPending]);

  const handleSupportReasonSubmit = useCallback((reason: string) => {
    actions.support(reason);
    setShowSupportReasonForm(false);
  }, [actions]);

  const handleOpposeReasonSubmit = useCallback((reason: string) => {
    actions.oppose(reason);
    setShowOpposeReasonForm(false);
  }, [actions]);

  const handleSupportReasonCancel = useCallback(() => {
    setShowSupportReasonForm(false);
  }, []);

  const handleOpposeReasonCancel = useCallback(() => {
    setShowOpposeReasonForm(false);
  }, []);

  const isSupported = userVoteState === 'supporting';
  const isOpposed = userVoteState === 'opposing';



  // If user can't vote, show disabled state with appropriate message
  if (!canVote) {
    let message = '';
    let icon: 'clock' | 'users' | 'x' = 'clock';
    
    if (amendment.proposedByCurrentUser) {
      message = 'You cannot vote on your own amendment';
      icon = 'users';
    } else if (!amendment.isOpen) {
      message = 'Voting is closed for this amendment';
      icon = 'clock';
    } else {
      message = 'Voting is not available';
      icon = 'x';
    }

    return (
      <div className="relative">
        <Button
          variant="ghost"
          disabled
          size={size}
          className={`opacity-50 cursor-not-allowed ${className}`}
          aria-label={message}
        >
          <Icon name={icon} className="w-4 h-4 mr-2" />
          <span className="text-sm">{message}</span>
        </Button>
      </div>
    );
  }

  // If user has voted, show single button with withdraw option
  if (isSupported || isOpposed) {
    return (
      <div className="relative">
        <AuthRequiredButton action={`withdraw ${isSupported ? 'support for' : 'opposition to'} this amendment`}>
          <Button
            onClick={handleWithdrawVote}
            size={size}
            variant={isSupported ? "primary" : "danger"}
            disabled={isPending}
            className={`flex items-center gap-2 ${className} ${
              isSupported 
                ? 'bg-green-600 hover:bg-green-700 text-white border-green-600' 
                : 'bg-red-600 hover:bg-red-700 text-white border-red-600'
            }`}
            aria-label={`Withdraw ${isSupported ? 'support for' : 'opposition to'} amendment`}
          >
            <Icon name={isSupported ? "check" : "x"} className="w-4 h-4" />
            <span>{isSupported ? 'Supported' : 'Opposed'}</span>
          </Button>
        </AuthRequiredButton>

        {/* Support Reason Form */}
        {showSupportReasonForm && (
          <SupportReasonForm
            onSubmit={handleSupportReasonSubmit}
            onCancel={handleSupportReasonCancel}
            isSubmitting={isPending}
          />
        )}

        {/* Oppose Reason Form */}
        {showOpposeReasonForm && (
          <OpposeReasonForm
            onSubmit={handleOpposeReasonSubmit}
            onCancel={handleOpposeReasonCancel}
            isSubmitting={isPending}
          />
        )}

        <Dialog open={showWithdrawModal} onOpenChange={(open) => !open && handleCancelWithdraw()}>
          <DialogContent className="sm:max-w-md">
            <DialogHeader>
              <DialogTitle>
                {pendingWithdrawalVoteState === 'supporting' ? 'Withdraw Support?' : 'Withdraw Opposition?'}
              </DialogTitle>
              <DialogDescription>
                {pendingWithdrawalVoteState === 'supporting'
                  ? 'Do you want to withdraw your support for this amendment?'
                  : 'Do you want to withdraw your opposition to this amendment?'}
              </DialogDescription>
            </DialogHeader>
            <DialogFooter>
              <Button
                type="button"
                variant="ghost"
                onClick={handleCancelWithdraw}
                disabled={isPending}
              >
                Cancel
              </Button>
              <Button
                type="button"
                variant={pendingWithdrawalVoteState === 'supporting' ? 'primary' : 'danger'}
                onClick={handleConfirmWithdraw}
                loading={isPending}
              >
                {pendingWithdrawalVoteState === 'supporting' ? 'Withdraw Support' : 'Withdraw Opposition'}
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </div>
    );
  }

  // If user can vote and hasn't voted, show split button
  return (
    <div className="relative">
      <AuthRequiredButton action="vote on this amendment">
        <div className={className}>
                     <ToggleGroup type="single" variant="outline" size={size === 'sm' ? 'sm' : size === 'lg' ? 'lg' : 'default'}>
             <ToggleGroupItem
               value="support"
               onClick={() => handleVoteSelection('support')}
               className="hover:bg-green-100 hover:text-green-800 data-[state=on]:bg-green-600 data-[state=on]:text-white"
             >
               Support{supporterCount > 0 ? ` (${supporterCount})` : ''}
             </ToggleGroupItem>
             <ToggleGroupItem
               value="oppose"
               onClick={() => handleVoteSelection('oppose')}
               className="hover:bg-red-100 hover:text-red-800 data-[state=on]:bg-red-600 data-[state=on]:text-white"
             >
               Oppose{opponentCount > 0 ? ` (${opponentCount})` : ''}
             </ToggleGroupItem>
           </ToggleGroup>
        </div>
      </AuthRequiredButton>

      {/* Support Reason Form */}
      {showSupportReasonForm && (
        <SupportReasonForm
          onSubmit={handleSupportReasonSubmit}
          onCancel={handleSupportReasonCancel}
          isSubmitting={isPending}
        />
      )}

      {/* Oppose Reason Form */}
      {showOpposeReasonForm && (
        <OpposeReasonForm
          onSubmit={handleOpposeReasonSubmit}
          onCancel={handleOpposeReasonCancel}
          isSubmitting={isPending}
        />
      )}
    </div>
  );
}
