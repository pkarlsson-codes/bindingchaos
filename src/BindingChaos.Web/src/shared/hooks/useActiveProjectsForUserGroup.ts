import { useInfiniteQuery } from '@tanstack/react-query';
import { useApiClient } from './useApiClient';

export function useActiveProjectsForUserGroup(userGroupId: string) {
  const apiClient = useApiClient();

  return useInfiniteQuery({
    queryKey: ['projects', userGroupId, 'active'],
    queryFn: async ({ pageParam = 1 }) => {
      return await apiClient.projects.getProjectsForUserGroup({
        userGroupId,
        filterStatuses: 'Active',
        pageNumber: pageParam,
        pageSize: 10,
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
