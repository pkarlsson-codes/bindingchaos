import { useQuery } from '@tanstack/react-query';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import type { SocietyMemberResponse } from '../../../api/models';

interface UseSocietyMembersOptions {
  page?: number;
  pageSize?: number;
}

export function useSocietyMembers(societyId: string | undefined, options: UseSocietyMembersOptions = {}) {
  const apiClient = useApiClient();
  const { page = 1, pageSize = 20 } = options;

  return useQuery({
    queryKey: ['society-members', societyId, page, pageSize],
    queryFn: async (): Promise<{ items: SocietyMemberResponse[]; totalCount: number }> => {
      const response = await apiClient.societies.getSocietyMembers({
        societyId: societyId!,
        pageNumber: page,
        pageSize,
      });
      const paged = response.data;
      return {
        items: paged?.items ?? [],
        totalCount: paged?.totalCount ?? 0,
      };
    },
    enabled: !!societyId,
    staleTime: 2 * 60 * 1000,
  });
}
