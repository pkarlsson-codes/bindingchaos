import { useQuery } from '@tanstack/react-query';
import { useApiClient } from './useApiClient';
import type { UserGroupListItemResponse } from '../../api/models';

export function useMyUserGroups() {
  const apiClient = useApiClient();

  return useQuery({
    queryKey: ['myUserGroups'],
    queryFn: async (): Promise<UserGroupListItemResponse[]> => {
      const response = await apiClient.userGroups.getMyUserGroups();
      return response.data ?? [];
    },
    staleTime: 2 * 60 * 1000,
    retry: 2,
  });
}
