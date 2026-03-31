import { useCallback, useRef, useEffect, useState } from 'react';
import { useInfiniteQuery } from '@tanstack/react-query';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import { useCommentReplies } from '../hooks/useCommentReplies';
import { CommentForm } from './CommentForm';
import { CommentItem } from './CommentItem';
import { Card } from '../../../shared/components/layout/Card';
import type { PostViewModel, ReplyViewModel } from '../../../api/models';
import { CursorDirection } from '../../../api/models';

interface CommentsCardProps {
  entityType: 'idea' | 'signal' | 'amendment';
  entityId: string;
  pageSize?: number;
  maxHeight?: string;
  allowEditing?: boolean;
  replyDepth?: number;
  onCommentAdded?: (comment: PostViewModel) => void;
  onCommentDeleted?: (commentId: string) => void;
}

export function CommentsCard({
  entityType,
  entityId,
  pageSize = 20,
  maxHeight,
  allowEditing = true,
  replyDepth = 0,
  onCommentAdded,
  onCommentDeleted
}: CommentsCardProps) {
  const apiClient = useApiClient();
  const { loadReplies, isLoading: isLoadingReplies } = useCommentReplies({ entityType, entityId });
  const observerRef = useRef<IntersectionObserver | null>(null);
  const [loadedReplies, setLoadedReplies] = useState<Record<string, ReplyViewModel[]>>({});
  const [threadId, setThreadId] = useState<string>('');

  const {
    data,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
    isLoading,
    error
  } = useInfiniteQuery({
    queryKey: ['contributions', entityType, entityId],
    initialPageParam: null as string | null,
    queryFn: async ({ pageParam }) => {
      const response = await apiClient.discourse.getPostsByEntity({
        entityType,
        entityId,
        cursor: pageParam,
        limit: pageSize,
        direction: CursorDirection.Forward
      });
      
      if (!threadId && response.data?.threadId) {
        setThreadId(response.data?.threadId);
      }
      
      return {
        contributions: response.data?.posts?.items || [],
        nextCursor: response.data?.posts?.nextCursor || null,
        hasMore: !!response.data?.posts?.nextCursor
      };
    },
    getNextPageParam: (lastPage) => lastPage.nextCursor,
    enabled: !!entityId
  });

  const lastCommentRef = useCallback((node: HTMLDivElement) => {
    if (isLoading) return;
    if (observerRef.current) observerRef.current.disconnect();
    
    observerRef.current = new IntersectionObserver(entries => {
      if (entries[0].isIntersecting && hasNextPage && !isFetchingNextPage) {
        fetchNextPage();
      }
    });
    
    if (node) observerRef.current.observe(node);
  }, [isLoading, hasNextPage, isFetchingNextPage, fetchNextPage]);

  // Cleanup observer on unmount
  useEffect(() => {
    return () => {
      if (observerRef.current) {
        observerRef.current.disconnect();
      }
    };
  }, []);

  const handleLoadReplies = useCallback(async (commentId: string) => {
    if (!threadId) {
      console.error('Thread ID not available for loading replies');
      return;
    }
    
    try {
      const response = await apiClient.discourse.getPostReplies({
        threadId,
        postId: commentId,
        limit: 10,
        direction: CursorDirection.Forward
      });
      
      const replies = response.data?.replies?.items || [];
      setLoadedReplies(prev => ({
        ...prev,
        [commentId]: replies
      }));
    } catch (error) {
      console.error('Failed to load replies:', error);
    }
  }, [apiClient.discourse, threadId]);

  const handleReplyReplaced = useCallback((oldReplyId: string, newReply: ReplyViewModel) => {
    if (newReply.parentPostId) {
      setLoadedReplies(prev => {
        const currentReplies = prev[newReply.parentPostId as string] || [];
        
        if (oldReplyId) {
          return {
            ...prev,
            [newReply.parentPostId as string]: currentReplies.map(reply => 
              reply.id === oldReplyId ? newReply : reply
            )
          };
        } else {
          // Add new reply
          return {
            ...prev,
            [newReply.parentPostId as string]: [...currentReplies, newReply]
          };
        }
      });
    }
  }, []);

  const allComments = data?.pages.flatMap(page => page.contributions) || [];

  if (error) {
    return (
      <Card
        title="Comments"
        content={
          <div className="text-center py-8">
            <p className="text-destructive">Error loading comments</p>
          </div>
        }
      />
    );
  }

  return (
    <Card
      title="Comments"
      content={
        <>
          {/* Add new comment */}
          <CommentForm
            entityType={entityType}
            entityId={entityId}
            threadId={threadId}
            replyDepth={replyDepth}
            onCommentAdded={onCommentAdded}
          />

          {/* Comments list */}
          <div className="space-y-4">
            {isLoading ? (
              <div className="text-center py-8">
                <div className="animate-pulse">
                  <div className="h-4 bg-muted rounded w-3/4 mb-2"></div>
                  <div className="h-4 bg-muted rounded w-1/2"></div>
                </div>
              </div>
            ) : allComments.length === 0 ? (
              <div className="text-center py-8">
                <p className="text-muted-foreground">No comments yet. Be the first to comment!</p>
              </div>
            ) : (
              allComments.map((comment, index) => (
                <div
                  key={comment.id}
                  ref={index === allComments.length - 1 ? lastCommentRef : null}
                >
                  <CommentItem
                    key={comment.id}
                    comment={comment}
                    entityType={entityType}
                    entityId={entityId}
                    threadId={threadId}
                    allowEditing={allowEditing}
                    depth={0}
                    onReply={handleLoadReplies}
                    onCommentDeleted={onCommentDeleted}
                    onLoadReplies={handleLoadReplies}
                    onReplyReplaced={handleReplyReplaced}
                    isLoadingReplies={isLoadingReplies}
                    loadedReplies={loadedReplies[comment.id!] || []}
                  />
                </div>
              ))
            )}
            
            {/* Loading more indicator */}
            {isFetchingNextPage && (
              <div className="text-center py-4">
                <div className="animate-pulse">
                  <div className="h-4 bg-muted rounded w-1/2 mx-auto"></div>
                </div>
              </div>
            )}
          </div>
        </>
      }
    />
  );
} 