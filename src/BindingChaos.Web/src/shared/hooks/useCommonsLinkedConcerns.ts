import { useQuery } from '@tanstack/react-query';
import { useApiClient } from './useApiClient';
import type { ConcernListItemResponse } from '../../api/models';

export function useCommonsLinkedConcerns(commonsId: string) {
  const apiClient = useApiClient();

  return useQuery({
    queryKey: ['commonsLinkedConcerns', commonsId],
    queryFn: async (): Promise<ConcernListItemResponse[]> => {
      const response = await apiClient.commons.getConcernsForCommons({ commonsId });
      return response.data || [];
    },
    enabled: !!commonsId,
    staleTime: 2 * 60 * 1000,
    retry: 2,
  });
}
