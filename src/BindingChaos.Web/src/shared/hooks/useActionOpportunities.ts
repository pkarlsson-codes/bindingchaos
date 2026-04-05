import { useQuery } from '@tanstack/react-query';

interface UseActionOpportunitiesOptions {
  searchTerm?: string;
  status?: string;
  sortBy?: string;
  page?: number;
  pageSize?: number;
}

export function useActionOpportunities(options: UseActionOpportunitiesOptions = {}) {
  const { searchTerm, status, sortBy, page, pageSize } = options;

  return useQuery({
    queryKey: ['actionOpportunities', searchTerm, status, sortBy, page, pageSize],
    queryFn: async (): Promise<unknown[]> => {
      // Legacy hook retained for compatibility while Action Opportunities API is absent.
      return [];
    },
    staleTime: 2 * 60 * 1000, // 2 minutes
    retry: 2,
  });
} 