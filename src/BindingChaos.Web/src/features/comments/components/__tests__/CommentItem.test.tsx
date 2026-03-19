import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { CommentItem } from '../CommentItem';

// Mock dependencies
jest.mock('../../../../shared/hooks/useApiClient', () => ({
  useApiClient: jest.fn()
}));
jest.mock('../../../auth', () => ({
  useUser: jest.fn()
}));
jest.mock('../../../../shared/components/Button', () => ({
  Button: ({ children, onClick, variant, size, className, 'aria-label': ariaLabel }: any) => (
    <button 
      onClick={onClick} 
      data-testid="button"
      data-variant={variant}
      data-size={size}
      className={className}
      aria-label={ariaLabel}
    >
      {children}
    </button>
  )
}));
jest.mock('../../../../shared/components/LoadingSpinner', () => ({
  LoadingSpinner: ({ size, className }: any) => (
    <div data-testid="loading-spinner" data-size={size} className={className}>
      Loading...
    </div>
  )
}));
jest.mock('../CommentForm', () => ({
  CommentForm: ({ entityType, entityId, replyingTo, onCancelReply, onCommentAdded }: any) => (
    <div data-testid="comment-form">
      <span>Form for {entityType} {entityId}</span>
      {replyingTo && <span>Replying to {replyingTo}</span>}
      <button onClick={onCancelReply}>Cancel Reply</button>
      <button onClick={() => onCommentAdded?.({ id: 'new-reply', content: 'Test reply' })}>
        Submit Reply
      </button>
    </div>
  )
}));
jest.mock('../../../../shared/components/ui/popover', () => ({
  Popover: ({ children }: any) => <div data-testid="popover">{children}</div>,
  PopoverTrigger: ({ children }: any) => <div data-testid="popover-trigger">{children}</div>,
  PopoverContent: ({ children }: any) => <div data-testid="popover-content">{children}</div>
}));

const mockUseApiClient = require('../../../../shared/hooks/useApiClient').useApiClient;
const mockUseUser = require('../../../auth').useUser;

// Test data
const mockUser = {
  id: 'user1',
  pseudonym: 'Test User'
};

const mockComment = {
  id: 'comment-1',
  content: 'This is a test comment',
  authorId: 'user1',
  authorPseudonym: 'Test User',
  createdAt: new Date('2024-01-01T10:00:00Z'),
  entityType: 'signal',
  entityId: 'signal-1',
  replyCount: 0,
  isEdited: false
};

const mockCommentWithReplies = {
  ...mockComment,
  replyCount: 2,
  replies: [
    {
      id: 'reply-1',
      content: 'This is a reply',
      authorId: 'user2',
      authorPseudonym: 'User Two',
      createdAt: new Date('2024-01-01T11:00:00Z'),
      entityType: 'signal',
      entityId: 'signal-1',
      replyCount: 0,
      isEdited: false
    }
  ]
};

// Test wrapper component
const TestWrapper = ({ children }: { children: React.ReactNode }) => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false }
    }
  });

  return (
    <QueryClientProvider client={queryClient}>
      {children}
    </QueryClientProvider>
  );
};

describe('CommentItem', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    
    // Default mocks
    mockUseApiClient.mockReturnValue({
      comments: {
        deleteComment: jest.fn().mockResolvedValue(undefined)
      }
    });
    
    mockUseUser.mockReturnValue({ user: mockUser });
    
    // Mock window.confirm
    global.window.confirm = jest.fn().mockReturnValue(true);
  });

  describe('Rendering', () => {
    it('renders comment content and metadata', () => {
      render(
        <TestWrapper>
          <CommentItem 
            comment={mockComment}
            entityType="signal"
            entityId="signal-1"
          />
        </TestWrapper>
      );

      expect(screen.getByText('This is a test comment')).toBeInTheDocument();
      expect(screen.getByText('Test User')).toBeInTheDocument();
      expect(screen.getByText(/Jan 1, 2024/)).toBeInTheDocument();
    });

    it('shows edited indicator when comment is edited', () => {
      const editedComment = { ...mockComment, isEdited: true };
      
      render(
        <TestWrapper>
          <CommentItem 
            comment={editedComment}
            entityType="signal"
            entityId="signal-1"
          />
        </TestWrapper>
      );

      expect(screen.getByText('(edited)')).toBeInTheDocument();
    });

    it('shows reply indicator for nested comments', () => {
      render(
        <TestWrapper>
          <CommentItem 
            comment={mockComment}
            entityType="signal"
            entityId="signal-1"
            depth={1}
          />
        </TestWrapper>
      );

      expect(screen.getByText('↳ Reply')).toBeInTheDocument();
    });
  });

  describe('Reply Functionality', () => {
    it('shows reply button by default', () => {
      render(
        <TestWrapper>
          <CommentItem 
            comment={mockComment}
            entityType="signal"
            entityId="signal-1"
          />
        </TestWrapper>
      );

      expect(screen.getByText('Reply')).toBeInTheDocument();
    });



    it('shows reply form when reply button is clicked', async () => {
      const user = userEvent.setup();
      
      render(
        <TestWrapper>
          <CommentItem 
            comment={mockComment}
            entityType="signal"
            entityId="signal-1"
          />
        </TestWrapper>
      );

      const replyButton = screen.getByText('Reply');
      await user.click(replyButton);

      expect(screen.getByTestId('comment-form')).toBeInTheDocument();
      expect(screen.getByText('Replying to comment-1')).toBeInTheDocument();
    });

    it('hides reply form when cancel is clicked', async () => {
      const user = userEvent.setup();
      
      render(
        <TestWrapper>
          <CommentItem 
            comment={mockComment}
            entityType="signal"
            entityId="signal-1"
          />
        </TestWrapper>
      );

      const replyButton = screen.getByText('Reply');
      await user.click(replyButton);

      expect(screen.getByTestId('comment-form')).toBeInTheDocument();

      const cancelButton = screen.getByText('Cancel Reply');
      await user.click(cancelButton);

      expect(screen.queryByTestId('comment-form')).not.toBeInTheDocument();
    });
  });

  describe('Delete Functionality', () => {
    it('shows delete option for comment author', () => {
      render(
        <TestWrapper>
          <CommentItem 
            comment={mockComment}
            entityType="signal"
            entityId="signal-1"
            allowEditing={true}
          />
        </TestWrapper>
      );

      // The ellipsis button should be present
      expect(screen.getByLabelText('Comment options')).toBeInTheDocument();
    });

    it('hides delete option for non-author', () => {
      mockUseUser.mockReturnValue({ user: { id: 'user2', pseudonym: 'Other User' } });
      
      render(
        <TestWrapper>
          <CommentItem 
            comment={mockComment}
            entityType="signal"
            entityId="signal-1"
            allowEditing={true}
          />
        </TestWrapper>
      );

      expect(screen.queryByLabelText('Comment options')).not.toBeInTheDocument();
    });

    it('hides delete option when allowEditing is false', () => {
      render(
        <TestWrapper>
          <CommentItem 
            comment={mockComment}
            entityType="signal"
            entityId="signal-1"
            allowEditing={false}
          />
        </TestWrapper>
      );

      expect(screen.queryByLabelText('Comment options')).not.toBeInTheDocument();
    });

    it('deletes comment when confirmed', async () => {
      const user = userEvent.setup();
      const onCommentDeleted = jest.fn();
      
      render(
        <TestWrapper>
          <CommentItem 
            comment={mockComment}
            entityType="signal"
            entityId="signal-1"
            allowEditing={true}
            onCommentDeleted={onCommentDeleted}
          />
        </TestWrapper>
      );

      const optionsButton = screen.getByLabelText('Comment options');
      await user.click(optionsButton);

      const deleteButton = screen.getByText('Delete');
      await user.click(deleteButton);

      await waitFor(() => {
        expect(mockUseApiClient().comments.deleteComment).toHaveBeenCalledWith({ commentId: 'comment-1' });
        expect(onCommentDeleted).toHaveBeenCalledWith('comment-1');
      });
    });
  });

  describe('Reply Display', () => {
    it('shows reply count when replies exist', () => {
      render(
        <TestWrapper>
          <CommentItem 
            comment={mockCommentWithReplies}
            entityType="signal"
            entityId="signal-1"
          />
        </TestWrapper>
      );

      expect(screen.getByText('1 reply')).toBeInTheDocument();
    });

    it('shows load replies button when replies exist but are not loaded', () => {
      const commentWithReplyCount = { ...mockComment, replyCount: 3 };
      
      render(
        <TestWrapper>
          <CommentItem 
            comment={commentWithReplyCount}
            entityType="signal"
            entityId="signal-1"
          />
        </TestWrapper>
      );

      expect(screen.getByText('Load 3 replies')).toBeInTheDocument();
    });

    it('calls onLoadReplies when load button is clicked', async () => {
      const user = userEvent.setup();
      const onLoadReplies = jest.fn();
      
      const commentWithReplyCount = { ...mockComment, replyCount: 3 };
      
      render(
        <TestWrapper>
          <CommentItem 
            comment={commentWithReplyCount}
            entityType="signal"
            entityId="signal-1"
            onLoadReplies={onLoadReplies}
          />
        </TestWrapper>
      );

      const loadButton = screen.getByText('Load 3 replies');
      await user.click(loadButton);

      expect(onLoadReplies).toHaveBeenCalledWith('comment-1');
    });

    it('renders nested replies when loaded', () => {
      render(
        <TestWrapper>
          <CommentItem 
            comment={mockCommentWithReplies}
            entityType="signal"
            entityId="signal-1"
          />
        </TestWrapper>
      );

      expect(screen.getByText('This is a reply')).toBeInTheDocument();
      expect(screen.getByText('User Two')).toBeInTheDocument();
    });
  });

  describe('Date Formatting', () => {
    it('formats date correctly', () => {
      const commentWithDate = {
        ...mockComment,
        createdAt: new Date('2024-12-25T15:30:00Z')
      };
      
      render(
        <TestWrapper>
          <CommentItem 
            comment={commentWithDate}
            entityType="signal"
            entityId="signal-1"
          />
        </TestWrapper>
      );

      expect(screen.getByText(/Dec 25, 2024/)).toBeInTheDocument();
    });

    it('handles undefined date gracefully', () => {
      const commentWithoutDate = { ...mockComment, createdAt: undefined };
      
      render(
        <TestWrapper>
          <CommentItem 
            comment={commentWithoutDate}
            entityType="signal"
            entityId="signal-1"
          />
        </TestWrapper>
      );

      // Should not crash and should not show date
      expect(screen.queryByText(/, \d{1,2}:\d{2} [AP]M$/)).not.toBeInTheDocument();
    });
  });

  describe('Accessibility', () => {
    it('has proper ARIA labels for interactive elements', () => {
      render(
        <TestWrapper>
          <CommentItem 
            comment={mockComment}
            entityType="signal"
            entityId="signal-1"
            allowEditing={true}
          />
        </TestWrapper>
      );

      expect(screen.getByLabelText('Comment options')).toBeInTheDocument();
      expect(screen.getByLabelText('Reply to comment by Test User')).toBeInTheDocument();
    });
  });
}); 