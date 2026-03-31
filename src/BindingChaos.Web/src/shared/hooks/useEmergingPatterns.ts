import { useQuery } from '@tanstack/react-query';
import { useApiClient } from './useApiClient';
import type { EmergingPatternResponse } from '../../api/models';

export function useEmergingPatterns() {
  const apiClient = useApiClient();

  return useQuery({
    queryKey: ['emerging-patterns'],
    queryFn: async (): Promise<EmergingPatternResponse[]> => {
      const response = await apiClient.emergingPatterns.getEmergingPatterns();
      return response.data?.patterns || [];
    },
    staleTime: 2 * 60 * 1000,
    retry: 2,
  });
}
