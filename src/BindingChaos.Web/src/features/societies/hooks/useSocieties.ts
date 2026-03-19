import { useQuery } from '@tanstack/react-query';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import type { SocietyListItemResponse } from '../../../api/models';

interface UseSocietiesOptions {
  page?: number;
  pageSize?: number;
  filterTag?: string;
}

export function useSocieties(options: UseSocietiesOptions = {}) {
  const apiClient = useApiClient();
  const { page = 1, pageSize = 20, filterTag } = options;

  return useQuery({
    queryKey: ['societies', page, pageSize, filterTag],
    queryFn: async (): Promise<{ items: SocietyListItemResponse[]; totalCount: number }> => {
      const response = await apiClient.societies.getSocieties({
        pageNumber: page,
        pageSize,
        filterTag: filterTag ?? null,
      });
      const paged = response.data?.societies;
      return {
        items: paged?.items ?? [],
        totalCount: paged?.totalCount ?? 0,
      };
    },
    staleTime: 2 * 60 * 1000,
  });
}
