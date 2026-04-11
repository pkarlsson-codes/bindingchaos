import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import type { ProjectInquiryResponse } from '../../../api/models';

export function useProjectInquiries(projectId: string) {
  const apiClient = useApiClient();

  return useQuery({
    queryKey: ['project-inquiries', projectId],
    queryFn: async (): Promise<ProjectInquiryResponse[]> => {
      const response = await apiClient.projects.getProjectInquiries({ projectId, pageSize: 50 });
      return response.data?.items ?? [];
    },
    enabled: !!projectId,
    staleTime: 30 * 1000,
  });
}

export function useProjectContestationStatus(projectId: string) {
  const apiClient = useApiClient();

  return useQuery({
    queryKey: ['project-contestation-status', projectId],
    queryFn: async () => {
      const response = await apiClient.projects.getProjectContestationStatus({ projectId });
      return response.data ?? null;
    },
    enabled: !!projectId,
    staleTime: 30 * 1000,
  });
}

export function useProjectInquiryMutations(projectId: string) {
  const apiClient = useApiClient();
  const queryClient = useQueryClient();

  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey: ['project-inquiries', projectId] });
    queryClient.invalidateQueries({ queryKey: ['project-contestation-status', projectId] });
  };

  const raiseInquiry = useMutation({
    mutationFn: (body: string) =>
      apiClient.projects.raiseProjectInquiry({ projectId, raiseProjectInquiryRequest: { body } }),
    onSuccess: invalidate,
  });

  const respond = useMutation({
    mutationFn: ({ inquiryId, response }: { inquiryId: string; response: string }) =>
      apiClient.projects.respondToProjectInquiry({
        projectId,
        inquiryId,
        respondToProjectInquiryRequest: { response },
      }),
    onSuccess: invalidate,
  });

  const resolve = useMutation({
    mutationFn: (inquiryId: string) =>
      apiClient.projects.resolveProjectInquiry({ projectId, inquiryId }),
    onSuccess: invalidate,
  });

  const update = useMutation({
    mutationFn: ({ inquiryId, newBody }: { inquiryId: string; newBody: string }) =>
      apiClient.projects.updateProjectInquiry({
        projectId,
        inquiryId,
        updateProjectInquiryRequest: { newBody },
      }),
    onSuccess: invalidate,
  });

  const reopen = useMutation({
    mutationFn: ({ inquiryId, newBody }: { inquiryId: string; newBody?: string }) =>
      apiClient.projects.reopenProjectInquiry({
        projectId,
        inquiryId,
        updateProjectInquiryRequest: newBody ? { newBody } : undefined,
      }),
    onSuccess: invalidate,
  });

  return { raiseInquiry, respond, resolve, update, reopen };
}
