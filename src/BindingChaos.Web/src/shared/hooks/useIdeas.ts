import { useQuery } from '@tanstack/react-query';
import { useApiClient } from './useApiClient';
import type { IdeaListItemResponse } from '../../api/models';

interface UseIdeasOptions {
  searchTerm?: string;
  sortBy?: string;
  page?: number;
  pageSize?: number;
}

export function useIdeas(options: UseIdeasOptions = {}) {
  const apiClient = useApiClient();
  const { searchTerm, sortBy, page, pageSize } = options;

  return useQuery({
    queryKey: ['ideas', searchTerm, sortBy, page, pageSize],
    queryFn: async (): Promise<IdeaListItemResponse[]> => {
      const response = await apiClient.ideas.getIdeas({
        pageNumber: page,
        pageSize: pageSize,
        filterSearchTerm: searchTerm,
      });

      return response.data?.ideas?.items || [];
    },
    staleTime: 2 * 60 * 1000,
    retry: 2,
  });
} 