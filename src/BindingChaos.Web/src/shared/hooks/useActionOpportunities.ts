import { useQuery } from '@tanstack/react-query';
import { useApiClient } from './useApiClient';
import { useLocality } from '../../features/locality/hooks/useLocality';
import type { ActionOpportunityListResponse } from '../../api/models';

interface UseActionOpportunitiesOptions {
  searchTerm?: string;
  status?: string;
  sortBy?: string;
  page?: number;
  pageSize?: number;
}

export function useActionOpportunities(options: UseActionOpportunitiesOptions = {}) {
  const apiClient = useApiClient();
  const { currentLocalityId } = useLocality();
  const { searchTerm, status, sortBy, page, pageSize } = options;

  return useQuery({
    queryKey: ['actionOpportunities', currentLocalityId, searchTerm, status, sortBy, page, pageSize],
    queryFn: async (): Promise<ActionOpportunityListResponse[]> => {
      const response = await apiClient.actionOpportunities.getActionOpportunities({
        searchTerm,
        status: status === 'all' ? undefined : status,
        sortBy,
        page,
        pageSize
        // Note: filterLocalityId is not supported by this API - locality filtering is handled automatically by the backend
      });
      return response.actionOpportunities?.items || [];
    },
    staleTime: 2 * 60 * 1000, // 2 minutes
    retry: 2,
  });
} 