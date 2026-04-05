import { useState } from 'react';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import { useUser } from '../../auth';
import { Button } from '@/shared/components/layout/Button';
import { LoadingSpinner } from '@/shared/components/feedback/LoadingSpinner';
import { CommentForm } from './CommentForm';
import type { PostViewModel, ReplyViewModel } from '@/api/models';
import { CursorDirection } from '@/api/models';

interface CommentItemProps {
  comment: PostViewModel;
  entityType: 'idea' | 'signal' | 'amendment';
  entityId: string;
  threadId?: string;
  allowEditing?: boolean;
  depth?: number;
  onReply?: (commentId: string) => void;
  onCommentDeleted?: (commentId: string) => void;
  onLoadReplies?: (commentId: string) => void;
  onReplyReplaced?: (oldReplyId: string, newReply: ReplyViewModel) => void;
  isLoadingReplies?: boolean;
  loadedReplies?: ReplyViewModel[];
}

// Separate component for rendering individual replies with their own reply functionality
function ReplyItem({
  reply,
  entityType,
  entityId,
  threadId,
  allowEditing,
  depth,
  onReplyReplaced,
  onLoadReplies,
  isLoadingReplies
}: {
  reply: ReplyViewModel;
  entityType: 'idea' | 'signal' | 'amendment';
  entityId: string;
  threadId?: string;
  allowEditing?: boolean;
  depth: number;
  onReplyReplaced?: (oldReplyId: string, newReply: ReplyViewModel) => void;
  onLoadReplies?: (commentId: string) => void;
  isLoadingReplies?: boolean;
}) {
  const apiClient = useApiClient();
  const { user } = useUser();
  const [isReplying, setIsReplying] = useState(false);
  const [loadedNestedReplies, setLoadedNestedReplies] = useState<ReplyViewModel[]>([]);
  const [isLoadingNestedReplies, setIsLoadingNestedReplies] = useState(false);

  const formatDate = (date: Date | undefined) => {
    if (!date) return '';
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  // Calculate indentation based on depth
  const getNestingStyles = () => {
    return {
      container: "ml-6 border-l-2 border-primary/20 pl-4 py-2 bg-primary/5 rounded-r-md",
      content: "flex-1"
    };
  };

  const nestingStyles = getNestingStyles();

  const handleLoadNestedReplies = async () => {
    if (!threadId) return;
    
    setIsLoadingNestedReplies(true);
    try {
      // Make the API call directly for nested replies
      const response = await apiClient.discourse.getPostReplies({
        threadId,
        postId: reply.id!,
        limit: 10,
        direction: CursorDirection.Forward
      });
      
      const nestedReplies = response.data?.replies?.items || [];
      setLoadedNestedReplies(nestedReplies);
    } catch (error) {
      console.error('Failed to load nested replies:', error);
    } finally {
      setIsLoadingNestedReplies(false);
    }
  };

  const handleNestedReplyReplaced = (oldReplyId: string, newReply: ReplyViewModel) => {
    if (newReply.parentPostId === reply.id) {
      setLoadedNestedReplies(prev => {
        return oldReplyId ? 
          prev.map(r => r.id === oldReplyId ? newReply : r) :
          [...prev, newReply];
      });
      
      // Also propagate the change up to parent components
      if (onReplyReplaced) {
        onReplyReplaced(oldReplyId, newReply);
      }
    }
  };

  return (
    <div className={nestingStyles.container}>
      <div className="text-xs text-primary font-medium mb-1">
        ↳ Reply
      </div>
      <div className="flex items-start gap-3">
        <div className={nestingStyles.content}>
          <div className="flex items-center gap-2 mb-1">
            <span className="font-medium text-sm">{reply.authorPseudonym}</span>
            <span className="text-xs text-muted-foreground">{formatDate(reply.createdAt)}</span>
          </div>
          
          <p className="text-foreground whitespace-pre-wrap">{reply.content}</p>
          
          <div className="flex items-center justify-between mt-2">
            <div className="flex items-center gap-4">
              <Button
                onClick={() => setIsReplying(true)}
                variant="ghost"
                size="sm"
                className="text-sm p-0 h-auto"
                aria-label={`Reply to reply by ${reply.authorPseudonym}`}
              >
                Reply
              </Button>
            </div>
          </div>
          
          {/* Inline reply form for replies */}
          {isReplying && (
            <div className="mt-3">
              <CommentForm
                entityType={entityType}
                entityId={entityId}
                threadId={threadId}
                replyingTo={reply.id}
                replyDepth={depth + 1}
                onCancelReply={() => setIsReplying(false)}
                onCommentAdded={() => {
                  setIsReplying(false);
                }}
                onReplyReplaced={handleNestedReplyReplaced}
              />
            </div>
          )}
          
          {/* Nested reply count and load replies button */}
          {((reply.replyCount || 0) > 0 || loadedNestedReplies.length > 0) && (
            <div className="mt-4 space-y-3 border-t border-border pt-3">
              {loadedNestedReplies.length > 0 ? (
                // Show loaded nested replies
                <>
                  <div className="text-xs text-muted-foreground font-medium mb-2">
                    {loadedNestedReplies.length} {loadedNestedReplies.length === 1 ? 'reply' : 'replies'}
                  </div>
                  {loadedNestedReplies.map((nestedReply: ReplyViewModel) => (
                    <ReplyItem
                      key={nestedReply.id}
                      reply={nestedReply}
                      entityType={entityType}
                      entityId={entityId}
                      threadId={threadId}
                      allowEditing={allowEditing}
                      depth={depth + 1}
                      onReplyReplaced={handleNestedReplyReplaced}
                      onLoadReplies={onLoadReplies}
                      isLoadingReplies={isLoadingNestedReplies}
                    />
                  ))}
                  
                  {/* Show load more nested replies button if there are more to load */}
                  {(reply.replyCount || 0) > loadedNestedReplies.length && (
                    <div className="text-center mt-3">
                      <Button
                        onClick={handleLoadNestedReplies}
                        variant="ghost"
                        size="sm"
                        className="text-sm text-primary hover:text-primary/80"
                      >
                        Load {(reply.replyCount || 0) - loadedNestedReplies.length} {((reply.replyCount || 0) - loadedNestedReplies.length) === 1 ? 'more reply' : 'more replies'}
                      </Button>
                    </div>
                  )}
                </>
              ) : (
                // Show load nested replies button or loading state
                <div className="text-center">
                  {isLoadingNestedReplies ? (
                    <div className="flex items-center justify-center text-sm text-muted-foreground">
                      <LoadingSpinner size="sm" className="mr-2" />
                      Loading replies...
                    </div>
                  ) : (
                    <Button
                      onClick={handleLoadNestedReplies}
                      variant="ghost"
                      size="sm"
                      className="text-sm text-primary hover:text-primary/80"
                    >
                      Load {reply.replyCount || 0} {(reply.replyCount || 0) === 1 ? 'reply' : 'replies'}
                    </Button>
                  )}
                </div>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

export function CommentItem({
  comment,
  entityType,
  entityId,
  threadId,
  allowEditing = true,
  depth = 0,
  onReply,
  onCommentDeleted,
  onLoadReplies,
  onReplyReplaced,
  isLoadingReplies = false,
  loadedReplies = []
}: CommentItemProps) {
  const { user } = useUser();
  const [isReplying, setIsReplying] = useState(false);

  const formatDate = (date: Date | undefined) => {
    if (!date) return '';
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  // Calculate indentation and visual styling based on depth
  const getNestingStyles = () => {
    if (depth === 0) {
      return {
        container: "border-b border-border pb-4 last:border-b-0",
        content: "flex-1"
      };
    }
    
    // Each nested level gets a fixed indentation relative to its parent
    return {
      container: "ml-6 border-l-2 border-primary/20 pl-4 py-2 bg-primary/5 rounded-r-md",
      content: "flex-1"
    };
  };

  const nestingStyles = getNestingStyles();

  return (
    <div className={nestingStyles.container}>
      {depth > 0 && (
        <div className="text-xs text-primary font-medium mb-1">
          ↳ Reply
        </div>
      )}
      <div className="flex items-start gap-3">
        <div className={nestingStyles.content}>
          <div className="flex items-center gap-2 mb-1">
            <span className="font-medium text-sm">{comment.authorPseudonym}</span>
            <span className="text-xs text-muted-foreground">{formatDate(comment.createdAt)}</span>
          </div>
          
          <p className="text-foreground whitespace-pre-wrap">{comment.content}</p>
          
          <div className="flex items-center justify-between mt-2">
            <div className="flex items-center gap-4">
              <Button
                onClick={() => setIsReplying(true)}
                variant="ghost"
                size="sm"
                className="text-sm p-0 h-auto"
                aria-label={`Reply to comment by ${comment.authorPseudonym}`}
              >
                Reply
              </Button>
            </div>
          </div>
          
          {/* Inline reply form */}
          {isReplying && (
            <div className="mt-3">
              <CommentForm
                entityType={entityType}
                entityId={entityId}
                threadId={threadId}
                replyingTo={comment.id}
                replyDepth={depth + 1}
                onCancelReply={() => setIsReplying(false)}
                onCommentAdded={() => {
                  setIsReplying(false);
                  // Don't trigger onReply for replies - let the optimistic update handle it
                }}
                onReplyReplaced={onReplyReplaced}
              />
            </div>
          )}
          
          {/* Reply count and load replies button */}
          {(comment.replyCount || 0) > 0 && (
            <div className="mt-4 space-y-3 border-t border-border pt-3">
              {loadedReplies.length > 0 ? (
                // Show loaded replies using the ReplyItem component
                <>
                  <div className="text-xs text-muted-foreground font-medium mb-2">
                    {loadedReplies.length} {loadedReplies.length === 1 ? 'reply' : 'replies'}
                  </div>
                  {loadedReplies.map((reply: ReplyViewModel) => (
                    <ReplyItem
                      key={reply.id}
                      reply={reply}
                      entityType={entityType}
                      entityId={entityId}
                      threadId={threadId}
                      allowEditing={allowEditing}
                      depth={depth + 1}
                      onReplyReplaced={onReplyReplaced}
                      onLoadReplies={onLoadReplies}
                      isLoadingReplies={isLoadingReplies}
                    />
                  ))}
                  
                  {/* Show load more replies button if there are more replies to load */}
                  {(comment.replyCount || 0) > loadedReplies.length && (
                    <div className="text-center mt-3">
                      <Button
                        onClick={() => onLoadReplies?.(comment.id!)}
                        variant="ghost"
                        size="sm"
                        className="text-sm text-primary hover:text-primary/80"
                      >
                        Load {(comment.replyCount || 0) - loadedReplies.length} {((comment.replyCount || 0) - loadedReplies.length) === 1 ? 'more reply' : 'more replies'}
                      </Button>
                    </div>
                  )}
                </>
              ) : (
                // Show load replies button or loading state
                <div className="text-center">
                  {isLoadingReplies ? (
                    <div className="flex items-center justify-center text-sm text-muted-foreground">
                      <LoadingSpinner size="sm" className="mr-2" />
                      Loading replies...
                    </div>
                  ) : (
                    <Button
                      onClick={() => onLoadReplies?.(comment.id!)}
                      variant="ghost"
                      size="sm"
                      className="text-sm text-primary hover:text-primary/80"
                    >
                      Load {comment.replyCount || 0} {(comment.replyCount || 0) === 1 ? 'reply' : 'replies'}
                    </Button>
                  )}
                </div>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
} 