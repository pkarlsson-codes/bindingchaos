import { useQuery } from '@tanstack/react-query';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import type { SocietyResponse } from '../../../api/models';

export function useSociety(societyId: string | undefined) {
  const apiClient = useApiClient();

  return useQuery({
    queryKey: ['society', societyId],
    queryFn: async (): Promise<SocietyResponse> => {
      const response = await apiClient.societies.getSociety({ societyId: societyId! });
      return response.data?.society ?? {};
    },
    enabled: !!societyId,
    staleTime: 2 * 60 * 1000,
  });
}
