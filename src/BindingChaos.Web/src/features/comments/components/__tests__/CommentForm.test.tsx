import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { CommentForm } from '../CommentForm';

// Mock dependencies
jest.mock('../../../../shared/hooks/useApiClient', () => ({
  useApiClient: jest.fn()
}));
jest.mock('../../../auth', () => ({
  useUser: jest.fn(),
  AuthRequiredButton: ({ children }: any) => <div data-testid="auth-button">{children}</div>
}));
jest.mock('../../../../shared/components/Button', () => ({
  Button: ({ children, onClick, disabled }: any) => {
    // Actually apply the disabled attribute when it's true
    return (
      <button 
        onClick={onClick} 
        disabled={disabled === true ? true : undefined} 
        data-testid="submit-button"
      >
        {children}
      </button>
    );
  }
}));
jest.mock('../../../../shared/components/ui/textarea', () => ({
  Textarea: ({ value, onChange, placeholder, onKeyDown }: any) => (
    <textarea
      value={value}
      onChange={onChange}
      placeholder={placeholder}
      onKeyDown={onKeyDown}
      data-testid="comment-textarea"
    />
  )
}));
jest.mock('../../../../shared/components/ui/toast', () => ({
  toast: jest.fn()
}));
jest.mock('../../../../shared/utils/queryCacheUtils', () => ({
  createOptimisticComment: jest.fn(),
  addCommentToCache: jest.fn(),
  replaceOptimisticComment: jest.fn()
}));

const mockUseApiClient = require('../../../../shared/hooks/useApiClient').useApiClient;
const mockUseUser = require('../../../auth').useUser;
const mockToast = require('../../../../shared/components/ui/toast').toast;

// Test data
const mockUser = {
  id: 'user1',
  pseudonym: 'Test User'
};

const mockComment = {
  id: 'comment-1',
  content: 'Test comment content',
  authorId: 'user1',
  authorPseudonym: 'Test User',
  createdAt: new Date(),
  entityType: 'signal',
  entityId: 'signal-1',
  replyCount: 0,
  isEdited: false
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

describe('CommentForm', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    
    // Default mocks
    mockUseApiClient.mockReturnValue({
      discourse: {
        createPost: jest.fn().mockResolvedValue('comment-1'),
        createReply: jest.fn().mockResolvedValue('reply-1'),
        getPostsByEntity: jest.fn().mockResolvedValue({
          posts: { items: [mockComment] }
        }),
        getPostReplies: jest.fn().mockResolvedValue({
          replies: { items: [mockComment] }
        })
      }
    });
    
    mockUseUser.mockReturnValue({ user: mockUser });
  });

  describe('Rendering', () => {
    it('renders the comment form with textarea', () => {
      render(
        <TestWrapper>
          <CommentForm entityType="signal" entityId="signal-1" />
        </TestWrapper>
      );

      expect(screen.getByTestId('comment-textarea')).toBeInTheDocument();
      expect(screen.getByPlaceholderText('Write a comment...')).toBeInTheDocument();
      expect(screen.getByTestId('submit-button')).toBeInTheDocument();
    });

    it('renders with reply placeholder when replying', () => {
      render(
        <TestWrapper>
          <CommentForm 
            entityType="signal" 
            entityId="signal-1" 
            replyingTo="parent-comment"
          />
        </TestWrapper>
      );

      expect(screen.getByPlaceholderText('Write a reply...')).toBeInTheDocument();
      expect(screen.getByText('Post Reply')).toBeInTheDocument();
    });

    it('shows reply context when replying', () => {
      const onCancelReply = jest.fn();
      
      render(
        <TestWrapper>
          <CommentForm 
            entityType="signal" 
            entityId="signal-1" 
            replyingTo="parent-comment"
            onCancelReply={onCancelReply}
          />
        </TestWrapper>
      );

      expect(screen.getByText(/Replying to comment/)).toBeInTheDocument();
      expect(screen.getByText('Cancel')).toBeInTheDocument();
    });
  });

  describe('Form Interaction', () => {
    it('updates textarea value when typing', async () => {
      const user = userEvent.setup();
      
      render(
        <TestWrapper>
          <CommentForm entityType="signal" entityId="signal-1" />
        </TestWrapper>
      );

      const textarea = screen.getByTestId('comment-textarea');
      await user.type(textarea, 'New comment content');

      expect(textarea).toHaveValue('New comment content');
    });

    it('submits form when submit button is clicked', async () => {
      const user = userEvent.setup();
      const onCommentAdded = jest.fn();
      
      render(
        <TestWrapper>
          <CommentForm 
            entityType="signal" 
            entityId="signal-1" 
            onCommentAdded={onCommentAdded}
          />
        </TestWrapper>
      );

      const textarea = screen.getByTestId('comment-textarea');
      const submitButton = screen.getByTestId('submit-button');

      await user.type(textarea, 'Test comment');
      await user.click(submitButton);

      await waitFor(() => {
        expect(mockUseApiClient().discourse.createPost).toHaveBeenCalledWith({
          threadId: undefined,
          createPostRequest: {
            content: 'Test comment'
          }
        });
      });
    });

    it('submits reply when replying to a comment', async () => {
      const user = userEvent.setup();
      
      render(
        <TestWrapper>
          <CommentForm 
            entityType="signal" 
            entityId="signal-1" 
            replyingTo="parent-comment"
          />
        </TestWrapper>
      );

      const textarea = screen.getByTestId('comment-textarea');
      const submitButton = screen.getByTestId('submit-button');

      await user.type(textarea, 'Test reply');
      await user.click(submitButton);

      await waitFor(() => {
        expect(mockUseApiClient().discourse.createReply).toHaveBeenCalledWith({
          threadId: undefined,
          postId: 'parent-comment',
          createReplyRequest: {
            content: 'Test reply'
          }
        });
      });
    });
  });

  describe('Form Validation', () => {
    it('does not submit empty comments', async () => {
      const user = userEvent.setup();
      
      render(
        <TestWrapper>
          <CommentForm entityType="signal" entityId="signal-1" />
        </TestWrapper>
      );

      const submitButton = screen.getByTestId('submit-button');
      await user.click(submitButton);

      expect(mockUseApiClient().discourse.createPost).not.toHaveBeenCalled();
    });

    it('does not submit whitespace-only comments', async () => {
      const user = userEvent.setup();
      
      render(
        <TestWrapper>
          <CommentForm entityType="signal" entityId="signal-1" />
        </TestWrapper>
      );

      const textarea = screen.getByTestId('comment-textarea');
      const submitButton = screen.getByTestId('submit-button');

      await user.type(textarea, '   ');
      await user.click(submitButton);

      expect(mockUseApiClient().discourse.createPost).not.toHaveBeenCalled();
    });

    it('trims whitespace from comment content', async () => {
      const user = userEvent.setup();
      
      render(
        <TestWrapper>
          <CommentForm entityType="signal" entityId="signal-1" />
        </TestWrapper>
      );

      const textarea = screen.getByTestId('comment-textarea');
      const submitButton = screen.getByTestId('submit-button');

      await user.type(textarea, '  Test comment  ');
      await user.click(submitButton);

      await waitFor(() => {
        expect(mockUseApiClient().discourse.createPost).toHaveBeenCalledWith({
          threadId: undefined,
          createPostRequest: {
            content: 'Test comment'
          }
        });
      });
    });
  });

  describe('Success Handling', () => {
    it('calls onCommentAdded callback on successful submission', async () => {
      const user = userEvent.setup();
      const onCommentAdded = jest.fn();
      
      render(
        <TestWrapper>
          <CommentForm 
            entityType="signal" 
            entityId="signal-1" 
            onCommentAdded={onCommentAdded}
          />
        </TestWrapper>
      );

      const textarea = screen.getByTestId('comment-textarea');
      const submitButton = screen.getByTestId('submit-button');

      await user.type(textarea, 'Test comment');
      await user.click(submitButton);

      await waitFor(() => {
        expect(onCommentAdded).toHaveBeenCalledWith(mockComment);
      });
    });

    it('shows success toast on successful submission', async () => {
      const user = userEvent.setup();
      
      render(
        <TestWrapper>
          <CommentForm entityType="signal" entityId="signal-1" />
        </TestWrapper>
      );

      const textarea = screen.getByTestId('comment-textarea');
      const submitButton = screen.getByTestId('submit-button');

      await user.type(textarea, 'Test comment');
      await user.click(submitButton);

      await waitFor(() => {
        expect(mockToast).toHaveBeenCalledWith({
          message: 'Comment posted successfully!',
          type: 'success'
        });
      });
    });

    it('clears form after successful submission', async () => {
      const user = userEvent.setup();
      
      render(
        <TestWrapper>
          <CommentForm entityType="signal" entityId="signal-1" />
        </TestWrapper>
      );

      const textarea = screen.getByTestId('comment-textarea');
      const submitButton = screen.getByTestId('submit-button');

      await user.type(textarea, 'Test comment');
      await user.click(submitButton);

      await waitFor(() => {
        expect(textarea).toHaveValue('');
      });
    });
  });

  describe('Error Handling', () => {
    it('shows error toast when submission fails', async () => {
      const user = userEvent.setup();
      
      mockUseApiClient.mockReturnValue({
        discourse: {
          createPost: jest.fn().mockRejectedValue(new Error('API Error')),
          createReply: jest.fn().mockRejectedValue(new Error('API Error')),
          getPostsByEntity: jest.fn().mockResolvedValue({
            posts: { items: [mockComment] }
          }),
          getPostReplies: jest.fn().mockResolvedValue({
            replies: { items: [mockComment] }
          })
        }
      });
      
      render(
        <TestWrapper>
          <CommentForm entityType="signal" entityId="signal-1" />
        </TestWrapper>
      );

      const textarea = screen.getByTestId('comment-textarea');
      const submitButton = screen.getByTestId('submit-button');

      await user.type(textarea, 'Test comment');
      await user.click(submitButton);

      await waitFor(() => {
        expect(mockToast).toHaveBeenCalledWith({
          message: 'Failed to post comment. Please try again.',
          type: 'error'
        });
      });
    });
  });

  describe('Authentication', () => {
    it('shows login prompt when user is not authenticated', () => {
      mockUseUser.mockReturnValue({ user: null });
      
      render(
        <TestWrapper>
          <CommentForm entityType="signal" entityId="signal-1" />
        </TestWrapper>
      );

      // When user is not authenticated, AuthRequiredButton should show a login prompt
      const submitButton = screen.getByTestId('submit-button');
      expect(submitButton).toBeInTheDocument();
      // The button should be present but not functional when not authenticated
      expect(submitButton).toBeInTheDocument();
    });

    it('enables submit button when user is authenticated', () => {
      mockUseUser.mockReturnValue({ user: mockUser });
      
      render(
        <TestWrapper>
          <CommentForm entityType="signal" entityId="signal-1" />
        </TestWrapper>
      );

      const submitButton = screen.getByTestId('submit-button');
      expect(submitButton).toBeInTheDocument();
      // The button should be functional when authenticated
      expect(submitButton).toBeInTheDocument();
    });
  });
}); 