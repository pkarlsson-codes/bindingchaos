import { useInfiniteQuery } from '@tanstack/react-query';
import { useApiClient } from './useApiClient';
import type { FilterState } from '../../features/signals/components/FilterBar';
import { SortDirection } from '@/api/models';

function getSortField(sortBy: string): string {
  switch (sortBy) {
    case 'recent':
      return 'capturedAt';
    case 'amplified':
      return 'amplificationCount';
    default:
      return 'capturedAt';
  }
}

interface UseInfiniteSignalsOptions {
  filters: FilterState;
  pageSize?: number;
}

export function useInfiniteSignals({ filters, pageSize = 20 }: UseInfiniteSignalsOptions) {
  const apiClient = useApiClient();

  return useInfiniteQuery({
    queryKey: ['signals', 'infinite', filters],
    queryFn: async ({ pageParam = 1 }) => {
      const requestParams = {
        pageNumber: pageParam,
        pageSize: pageSize,
        filterTimeRange: filters.timeRange !== 'all' ? filters.timeRange : undefined,
        filterSearchTerm: filters.searchTerm || undefined,
        filterTags: filters.tags.filter(tag => tag && tag.trim() !== '').length > 0 
          ? filters.tags.filter(tag => tag && tag.trim() !== '') 
          : undefined,
        sort: filters.sortBy !== 'recent' ? [{ 
          field: getSortField(filters.sortBy), 
          direction: SortDirection.Desc
        }] : undefined
      };

      const response = await apiClient.signals.getSignals(requestParams);
      return response;
    },
    initialPageParam: 1,
    getNextPageParam: (lastPage) => {
      const signals = lastPage?.data?.signals;
      if (!signals) return undefined;

      if (signals.hasNextPage) {
        return (signals.pageNumber || 1) + 1;
      }
      return undefined;
    },
    staleTime: 5 * 60 * 1000, // 5 minutes
    placeholderData: (previousData) => previousData, // Keep previous data while fetching
  });
}
