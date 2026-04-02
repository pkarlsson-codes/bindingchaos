import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import type { CreateSocietyRequest, SocietyInviteLinkViewResponse } from '../../../api/models';

export function useMySocietyIds() {
  const apiClient = useApiClient();
  return useQuery({
    queryKey: ['societies', 'memberships', 'me'],
    queryFn: async (): Promise<string[]> => {
      const response = await apiClient.societies.getMySocietyIds();
      return response.data ?? [];
    },
    staleTime: 60 * 1000,
  });
}

export function useCreateSociety() {
  const apiClient = useApiClient();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateSocietyRequest) =>
      apiClient.societies.createSociety({ createSocietyRequest: request }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['societies'] });
    },
  });
}

export function useJoinSociety(societyId: string) {
  const apiClient = useApiClient();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (socialContractId: string) =>
      apiClient.societies.joinSociety({
        societyId,
        joinSocietyRequest: { socialContractId },
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['society', societyId] });
      queryClient.invalidateQueries({ queryKey: ['society-members', societyId] });
      queryClient.invalidateQueries({ queryKey: ['societies'] });
      queryClient.invalidateQueries({ queryKey: ['societies', 'memberships', 'me'] });
    },
  });
}

export function useLeaveSociety(societyId: string) {
  const apiClient = useApiClient();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: () => apiClient.societies.leaveSociety({ societyId }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['society', societyId] });
      queryClient.invalidateQueries({ queryKey: ['society-members', societyId] });
      queryClient.invalidateQueries({ queryKey: ['societies'] });
      queryClient.invalidateQueries({ queryKey: ['societies', 'memberships', 'me'] });
    },
  });
}

export function useMySocietyInviteLinks(societyId: string) {
  const apiClient = useApiClient();
  return useQuery({
    queryKey: ['society-invite-links', societyId],
    queryFn: async (): Promise<SocietyInviteLinkViewResponse[]> => {
      const response = await apiClient.societies.getMySocietyInviteLinks({ societyId });
      return response.data ?? [];
    },
  });
}

export function useCreateSocietyInviteLink(societyId: string) {
  const apiClient = useApiClient();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (note: string | undefined) =>
      apiClient.societies.createSocietyInviteLink({
        societyId,
        createSocietyInviteLinkRequest: { note: note ?? null },
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['society-invite-links', societyId] });
    },
  });
}
