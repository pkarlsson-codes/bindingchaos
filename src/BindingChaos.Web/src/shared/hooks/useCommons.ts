import { useQuery } from '@tanstack/react-query';
import { useApiClient } from './useApiClient';
import type { CommonsListItemResponse } from '../../api/models';

export function useCommons() {
  const apiClient = useApiClient();

  return useQuery({
    queryKey: ['commons'],
    queryFn: async (): Promise<CommonsListItemResponse[]> => {
      const response = await apiClient.commons.getCommons({});
      return response.data?.items || [];
    },
    staleTime: 2 * 60 * 1000,
    retry: 2,
  });
}
