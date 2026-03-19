import { useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import { useUser, AuthRequiredButton } from '../../auth';
import { Button } from '@/shared/components/layout/Button';
import { LoadingSpinner } from '@/shared/components/feedback/LoadingSpinner';
import { Textarea } from '@/shared/components/ui/textarea';
import { toast } from '@/shared/components/ui/toast';

import type { CreatePostRequest, CreateReplyRequest, PostViewModel, ReplyViewModel } from '@/api/models';

interface CommentFormProps {
  entityType: 'idea' | 'signal' | 'amendment';
  entityId: string;
  threadId?: string;
  replyingTo?: string | null;
  replyDepth?: number;
  onCancelReply?: () => void;
  onCommentAdded?: (comment: PostViewModel) => void;
  onReplyReplaced?: (oldReplyId: string, newReply: ReplyViewModel) => void;
}

export function CommentForm({
  entityType,
  entityId,
  threadId,
  replyingTo,
  replyDepth = 0,
  onCancelReply,
  onCommentAdded,
  onReplyReplaced
}: CommentFormProps) {
  const apiClient = useApiClient();
  const queryClient = useQueryClient();
  const { user } = useUser();
  const [newComment, setNewComment] = useState('');



  const addCommentMutation = useMutation({
    mutationFn: async (comment: CreatePostRequest | CreateReplyRequest) => {
      if (!threadId) {
        throw new Error('Thread ID is required to create a post');
      }
      
      if (replyingTo) {
        // Create a reply
        const response = await apiClient.discourse.createReply({
          threadId,
          postId: replyingTo,
          createReplyRequest: comment as CreateReplyRequest
        });
        return response;
      } else {
        // Create a root post
        const response = await apiClient.discourse.createPost({
          threadId,
          createPostRequest: comment as CreatePostRequest
        });
        return response;
      }
    },
    onSuccess: (contributionId) => {
      // Create the complete comment object using form data + returned ID
      if (replyingTo) {
        // For replies, create a reply object
        const reply: ReplyViewModel = {
          id: contributionId,
          content: newComment.trim(),
          authorId: user?.id || 'anonymous',
          authorPseudonym: user?.pseudonym || 'Anonymous',
          createdAt: new Date(),
          parentPostId: replyingTo,
          replyCount: 0,
          hasMoreReplies: false
        };
        
        // Add the reply to the loaded replies state
        if (onReplyReplaced) {
          onReplyReplaced('', reply); // Empty string as old ID since we're not replacing
        }
        
        // Also update the parent post's reply count in the main comments list
        if (replyingTo) {
          queryClient.setQueryData(['contributions', entityType, entityId], (old: any) => {
            if (!old || !old.pages) return old;
            
            return {
              ...old,
              pages: old.pages.map((page: any) => ({
                ...page,
                contributions: page.contributions.map((contribution: any) => {
                  if (contribution.id === replyingTo) {
                    return {
                      ...contribution,
                      replyCount: (contribution.replyCount || 0) + 1,
                      hasReplies: true
                    };
                  }
                  return contribution;
                })
              }))
            };
          });
        }
      } else {
        // For root posts, create a post object
        const post: PostViewModel = {
          id: contributionId,
          content: newComment.trim(),
          authorId: user?.id || 'anonymous',
          authorPseudonym: user?.pseudonym || 'Anonymous',
          createdAt: new Date(),
          replyCount: 0,
          hasReplies: false,
          lastReplyAt: undefined
        };
        
        // Add the post to the main comments list
        queryClient.setQueryData(['contributions', entityType, entityId], (old: any) => {
          if (!old || !old.pages) return old;
          
          return {
            ...old,
            pages: old.pages.map((page: any, index: number) => {
              if (index === 0) {
                return {
                  ...page,
                  contributions: [post, ...page.contributions]
                };
              }
              return page;
            })
          };
        });
        
        // Call the callback
        onCommentAdded?.(post);
      }
      
      // Clear form and reply state
      setNewComment('');
      if (replyingTo) {
        onCancelReply?.();
      }
      
      toast.success(replyingTo ? 'Reply posted successfully!' : 'Comment posted successfully!');
    },
    onError: (err) => {
      toast.error('Failed to post comment. Please try again.');
    }
  });

  const handleSubmitComment = () => {
    if (!newComment.trim()) return;
    
    const request = {
      content: newComment.trim()
    };
    
    addCommentMutation.mutate(request);
  };

  return (
    <div className="mb-6">
      <div className="flex gap-2">
        <Textarea
          value={newComment}
          onChange={(e) => setNewComment(e.target.value)}
          placeholder={replyingTo ? "Write a reply..." : "Write a comment..."}
          className="flex-1 resize-none"
          rows={3}
          disabled={addCommentMutation.isPending}
          onKeyDown={(e) => {
            if (e.key === 'Enter' && (e.metaKey || e.ctrlKey)) {
              handleSubmitComment();
            }
          }}
        />
      </div>
      <div className="flex justify-between items-center mt-2">
        <div className="text-sm text-muted-foreground">
          {replyingTo && (
            <span>
              Replying to comment • 
              <button 
                onClick={onCancelReply}
                disabled={addCommentMutation.isPending}
                className="text-primary hover:underline ml-1 disabled:opacity-50"
              >
                Cancel
              </button>
            </span>
          )}
        </div>
        <AuthRequiredButton action="post a comment">
          <Button
            onClick={handleSubmitComment}
            disabled={user ? (!newComment.trim() || addCommentMutation.isPending) : false}
          >
            {addCommentMutation.isPending ? (
              <>
                <LoadingSpinner size="sm" className="mr-2" />
                {replyingTo ? 'Posting Reply...' : 'Posting Comment...'}
              </>
            ) : (
              replyingTo ? 'Post Reply' : 'Post Comment'
            )}
          </Button>
        </AuthRequiredButton>
      </div>
    </div>
  );
} 