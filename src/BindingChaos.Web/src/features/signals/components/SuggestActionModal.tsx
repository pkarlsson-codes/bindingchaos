import { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import { useOptionalAuth } from '../../auth/contexts/AuthContext';
import { FormModal } from '../../../shared/components/modals/FormModal';
import { toast } from '../../../shared/components/ui/toast';
import { API_CONFIG } from '../../../config/api';
import type { ActionTypeResponse } from '../../../api/models';

const ACTION_LABELS: Record<string, string> = {
  MakeACall: 'Make a call',
  VisitAWebpage: 'Visit a webpage',
};

const DETAILS_MAX_LENGTH = 500;

interface SuggestActionModalProps {
  isOpen: boolean;
  onClose: () => void;
  signalId: string;
}

export function SuggestActionModal({ isOpen, onClose, signalId }: SuggestActionModalProps) {
  const [actionType, setActionType] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');
  const [url, setUrl] = useState('');
  const [details, setDetails] = useState('');

  const apiClient = useApiClient();
  const queryClient = useQueryClient();
  const auth = useOptionalAuth();

  const { data: actionTypes = [] } = useQuery<ActionTypeResponse[]>({
    queryKey: ['actionTypes'],
    queryFn: () =>
      fetch(`${API_CONFIG.baseUrl}/api/v1/action-types`).then((r) => r.json()).then((envelope) => envelope.data ?? envelope),
    staleTime: Infinity,
  });

  const { mutate, isPending } = useMutation({
    mutationFn: () =>
      apiClient.signals.suggestAction({
        signalId,
        suggestActionRequest: {
          actionType,
          phoneNumber: actionType === 'MakeACall' ? phoneNumber : undefined,
          url: actionType === 'VisitAWebpage' ? url : undefined,
          details: details.trim() || undefined,
        },
      }),
    onMutate: async () => {
      await queryClient.cancelQueries({ queryKey: ['signalDetails', signalId] });
      const previous = queryClient.getQueryData(['signalDetails', signalId]);

      const optimisticAction = {
        id: crypto.randomUUID(),
        actionType,
        phoneNumber: actionType === 'MakeACall' ? phoneNumber : null,
        url: actionType === 'VisitAWebpage' ? url : null,
        details: details.trim() || null,
        suggestedByPseudonym: auth?.user?.pseudonym ?? undefined,
        suggestedAt: new Date().toISOString(),
      };

      queryClient.setQueryData(['signalDetails', signalId], (old: any) => {
        if (!old?.data) return old;
        return {
          ...old,
          data: {
            ...old.data,
            suggestedActions: [...(old.data.suggestedActions ?? []), optimisticAction],
          },
        };
      });

      return { previous };
    },
    onSuccess: () => {
      toast.success('Action suggested successfully');
      handleClose();
    },
    onError: (_err, _vars, context: any) => {
      if (context?.previous !== undefined) {
        queryClient.setQueryData(['signalDetails', signalId], context.previous);
      }
      toast.error('Failed to suggest action. Please try again.');
    },
    onSettled: (_data, error) => {
      if (error) {
        queryClient.invalidateQueries({ queryKey: ['signalDetails', signalId] });
      }
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    mutate();
  };

  const handleClose = () => {
    setActionType('');
    setPhoneNumber('');
    setUrl('');
    setDetails('');
    onClose();
  };

  const detailsRemaining = DETAILS_MAX_LENGTH - details.length;
  const isDetailsOverLimit = detailsRemaining < 0;

  const isSubmitDisabled =
    !actionType ||
    (actionType === 'MakeACall' && !phoneNumber.trim()) ||
    (actionType === 'VisitAWebpage' && !url.trim()) ||
    isDetailsOverLimit;

  return (
    <FormModal
      isOpen={isOpen}
      onClose={handleClose}
      title="Suggest an action"
      description="Choose an action type and provide the details others would need to take it."
      onSubmit={handleSubmit}
      submitText="Suggest action"
      loading={isPending}
      disabled={isSubmitDisabled}
      size="md"
    >
      <div className="space-y-4">
        <div className="space-y-1.5">
          <label className="text-sm font-medium" htmlFor="action-type">
            Action type
          </label>
          <select
            id="action-type"
            value={actionType}
            onChange={(e) => {
              setActionType(e.target.value);
              setPhoneNumber('');
              setUrl('');
            }}
            className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
            disabled={isPending}
          >
            <option value="">Select an action type...</option>
            {actionTypes.map((t) => (
              <option key={t.id} value={t.name}>
                {t.name ? (ACTION_LABELS[t.name] ?? t.name) : ''}
              </option>
            ))}
          </select>
        </div>

        {actionType === 'MakeACall' && (
          <div className="space-y-1.5">
            <label className="text-sm font-medium" htmlFor="phone-number">
              Phone number
            </label>
            <input
              id="phone-number"
              type="tel"
              value={phoneNumber}
              onChange={(e) => setPhoneNumber(e.target.value)}
              placeholder="e.g. +1 555-0100"
              className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
              disabled={isPending}
            />
          </div>
        )}

        {actionType === 'VisitAWebpage' && (
          <div className="space-y-1.5">
            <label className="text-sm font-medium" htmlFor="url">
              URL
            </label>
            <input
              id="url"
              type="url"
              value={url}
              onChange={(e) => setUrl(e.target.value)}
              placeholder="https://example.com"
              className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
              disabled={isPending}
            />
          </div>
        )}

        {actionType && (
          <div className="space-y-1">
            <label className="text-sm font-medium" htmlFor="details">
              Details <span className="font-normal text-muted-foreground">(optional)</span>
            </label>
            <textarea
              id="details"
              value={details}
              onChange={(e) => setDetails(e.target.value)}
              placeholder="Any additional context that would help others take this action..."
              className="w-full min-h-[80px] rounded-md border border-input bg-background px-3 py-2 text-sm placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring resize-none"
              maxLength={DETAILS_MAX_LENGTH + 1}
              disabled={isPending}
            />
            <p className={`text-xs text-right ${isDetailsOverLimit ? 'text-destructive' : 'text-muted-foreground'}`}>
              {detailsRemaining} characters remaining
            </p>
          </div>
        )}
      </div>
    </FormModal>
  );
}
