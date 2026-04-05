import { useQuery } from '@tanstack/react-query';
import { useApiClient } from './useApiClient';
import type { ProjectListItemResponse } from '../../api/models';

export function useProjectsForUserGroup(userGroupId: string) {
  const apiClient = useApiClient();

  return useQuery({
    queryKey: ['projects', userGroupId],
    queryFn: async (): Promise<ProjectListItemResponse[]> => {
      const response = await apiClient.projects.getProjectsForUserGroup({ userGroupId });
      return response.data?.items || [];
    },
    enabled: !!userGroupId,
    staleTime: 2 * 60 * 1000,
    retry: 2,
  });
}
