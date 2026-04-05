import { useQuery } from '@tanstack/react-query';
import { useApiClient } from './useApiClient';
import type { ProjectResponse } from '../../api/models';

export function useProject(projectId: string) {
  const apiClient = useApiClient();

  return useQuery({
    queryKey: ['project', projectId],
    queryFn: async (): Promise<ProjectResponse | undefined> => {
      const response = await apiClient.projects.getProject({ projectId });
      return response.data ?? undefined;
    },
    enabled: !!projectId,
    staleTime: 2 * 60 * 1000,
    retry: 2,
  });
}
