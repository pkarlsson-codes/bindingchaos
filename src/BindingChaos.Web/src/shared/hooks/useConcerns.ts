import { useQuery } from '@tanstack/react-query';
import { useApiClient } from './useApiClient';
import type { ConcernListItemResponse } from '../../api/models';

export function useConcerns() {
  const apiClient = useApiClient();

  return useQuery({
    queryKey: ['concerns'],
    queryFn: async (): Promise<ConcernListItemResponse[]> => {
      const response = await apiClient.concerns.getConcerns({});
      return response.data?.items || [];
    },
    staleTime: 2 * 60 * 1000,
    retry: 2,
  });
}
