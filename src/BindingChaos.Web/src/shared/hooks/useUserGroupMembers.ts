import { useInfiniteQuery } from '@tanstack/react-query';
import { useApiClient } from './useApiClient';

export function useUserGroupMembers(userGroupId: string) {
  const apiClient = useApiClient();

  return useInfiniteQuery({
    queryKey: ['userGroupMembers', userGroupId],
    queryFn: async ({ pageParam = 1 }) => {
      return await apiClient.userGroups.getUserGroupMembers({
        id: userGroupId,
        pageNumber: pageParam,
        pageSize: 20,
      });
    },
    initialPageParam: 1,
    getNextPageParam: (lastPage) => {
      const data = lastPage?.data;
      if (data?.hasNextPage) {
        return (data.pageNumber ?? 1) + 1;
      }
      return undefined;
    },
    enabled: !!userGroupId,
    staleTime: 2 * 60 * 1000,
    retry: 2,
  });
}
