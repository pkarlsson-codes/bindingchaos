import { useQuery } from '@tanstack/react-query';
import { useApiClient } from './useApiClient';
import type { UserGroupDetailResponse } from '../../api/models';

export function useUserGroup(userGroupId: string) {
  const apiClient = useApiClient();

  return useQuery({
    queryKey: ['userGroup', userGroupId],
    queryFn: async (): Promise<UserGroupDetailResponse | undefined> => {
      const response = await apiClient.userGroups.getUserGroupDetail({ id: userGroupId });
      return response.data ?? undefined;
    },
    enabled: !!userGroupId,
    staleTime: 2 * 60 * 1000,
    retry: 2,
  });
}
