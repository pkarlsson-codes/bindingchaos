import { useQuery } from '@tanstack/react-query';
import { useApiClient } from './useApiClient';
import type { UserGroupListItemResponse } from '../../api/models';

export function useUserGroups(commonsId: string) {
  const apiClient = useApiClient();

  return useQuery({
    queryKey: ['userGroups', commonsId],
    queryFn: async (): Promise<UserGroupListItemResponse[]> => {
      const response = await apiClient.userGroups.getUserGroupsForCommons({ commonsId });
      return response.data?.items || [];
    },
    enabled: !!commonsId,
    staleTime: 2 * 60 * 1000,
    retry: 2,
  });
}
