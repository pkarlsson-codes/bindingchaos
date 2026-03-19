import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import type { ReplyViewModel } from '../../../api/models';
import { CursorDirection } from '../../../api/models';

interface UseCommentRepliesProps {
  entityType: 'idea' | 'signal' | 'amendment';
  entityId: string;
}

export function useCommentReplies({ entityType, entityId }: UseCommentRepliesProps) {
  const apiClient = useApiClient();
  const queryClient = useQueryClient();

  const loadRepliesMutation = useMutation({
    mutationFn: async ({ postId, threadId, limit = 5 }: { postId: string; threadId: string; limit?: number }) => {
      const response = await apiClient.discourse.getPostReplies({
        threadId,
        postId,
        limit,
        direction: CursorDirection.Forward
      });
      return { postId, replies: response.replies?.items || [] };
    },
    onSuccess: ({ postId, replies }) => {
      // Invalidate the contributions query to refresh the reply count
      queryClient.invalidateQueries({ queryKey: ['contributions', entityType, entityId] });
    }
  });

  const loadReplies = (postId: string, threadId: string, limit: number = 5) => {
    loadRepliesMutation.mutate({ postId, threadId, limit });
  };

  return {
    loadReplies,
    isLoading: loadRepliesMutation.isPending,
    error: loadRepliesMutation.error
  };
} 