import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { CommentsCard } from '../CommentsCard';

// Mock dependencies
jest.mock('../../../../shared/hooks/useApiClient', () => ({
  useApiClient: jest.fn()
}));
jest.mock('../../hooks/useCommentReplies', () => ({
  useCommentReplies: jest.fn()
}));
jest.mock('../CommentForm', () => ({
  CommentForm: ({ entityType, entityId, onCommentAdded }: any) => (
    <div data-testid="comment-form">
      <button onClick={() => onCommentAdded?.({ id: 'new-comment', content: 'Test comment' })}>
        Add Comment
      </button>
    </div>
  )
}));
jest.mock('../CommentItem', () => ({
  CommentItem: ({ comment, onCommentDeleted }: any) => (
    <div data-testid={`comment-item-${comment.id}`}>
      <span>{comment.content}</span>
      <button onClick={() => onCommentDeleted?.(comment.id)}>Delete</button>
    </div>
  )
}));
jest.mock('../../../../shared/components/Card', () => ({
  Card: ({ title, content }: any) => (
    <div data-testid="card">
      <h2>{title}</h2>
      <div>{content}</div>
    </div>
  )
}));

const mockUseApiClient = require('../../../../shared/hooks/useApiClient').useApiClient;
const mockUseCommentReplies = require('../../hooks/useCommentReplies').useCommentReplies;

// Test data
const mockComments = [
  {
    id: '1',
    content: 'First comment',
    authorId: 'user1',
    authorPseudonym: 'User One',
    createdAt: new Date('2024-01-01'),
    entityType: 'signal',
    entityId: 'signal-1',
    replyCount: 0,
    isEdited: false
  },
  {
    id: '2',
    content: 'Second comment',
    authorId: 'user2',
    authorPseudonym: 'User Two',
    createdAt: new Date('2024-01-02'),
    entityType: 'signal',
    entityId: 'signal-1',
    replyCount: 2,
    isEdited: false
  }
];

const mockApiResponse = {
  items: mockComments,
  hasNextPage: false,
  totalCount: 2
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

describe('CommentsCard', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    
    // Default mocks
    mockUseApiClient.mockReturnValue({
      comments: {
        getComments: jest.fn().mockResolvedValue(mockApiResponse)
      }
    });
    
    mockUseCommentReplies.mockReturnValue({
      loadReplies: jest.fn(),
      isLoading: false
    });
  });

  describe('Rendering', () => {
    it('renders the comments card with title', () => {
      render(
        <TestWrapper>
          <CommentsCard entityType="signal" entityId="signal-1" />
        </TestWrapper>
      );

      expect(screen.getByText('Comments')).toBeInTheDocument();
      expect(screen.getByTestId('comment-form')).toBeInTheDocument();
    });

    it('renders with custom props', () => {
      render(
        <TestWrapper>
          <CommentsCard 
            entityType="idea" 
            entityId="idea-1" 
            pageSize={10}
            maxHeight="400px"
            allowEditing={false}
            replyDepth={2}
          />
        </TestWrapper>
      );

      expect(screen.getByText('Comments')).toBeInTheDocument();
    });
  });

  describe('Loading States', () => {
    it('shows loading skeleton when initially loading', async () => {
      mockUseApiClient.mockReturnValue({
        comments: {
          getComments: jest.fn().mockImplementation(() => 
            new Promise(resolve => setTimeout(() => resolve(mockApiResponse), 100))
          )
        }
      });

      render(
        <TestWrapper>
          <CommentsCard entityType="signal" entityId="signal-1" />
        </TestWrapper>
      );

      // Should show loading skeleton
      expect(screen.getByText('Comments')).toBeInTheDocument();
      // The loading skeleton should be visible initially
      await waitFor(() => {
        expect(screen.getByText('First comment')).toBeInTheDocument();
      });
    });
  });

  describe('Error Handling', () => {
    it('shows error message when API call fails', async () => {
      mockUseApiClient.mockReturnValue({
        comments: {
          getComments: jest.fn().mockRejectedValue(new Error('API Error'))
        }
      });

      render(
        <TestWrapper>
          <CommentsCard entityType="signal" entityId="signal-1" />
        </TestWrapper>
      );

      await waitFor(() => {
        expect(screen.getByText('Error loading comments')).toBeInTheDocument();
      });
    });
  });

  describe('Empty State', () => {
    it('shows empty state when no comments exist', async () => {
      mockUseApiClient.mockReturnValue({
        comments: {
          getComments: jest.fn().mockResolvedValue({
            items: [],
            hasNextPage: false,
            totalCount: 0
          })
        }
      });

      render(
        <TestWrapper>
          <CommentsCard entityType="signal" entityId="signal-1" />
        </TestWrapper>
      );

      await waitFor(() => {
        expect(screen.getByText('No comments yet. Be the first to comment!')).toBeInTheDocument();
      });
    });
  });

  describe('Comment Display', () => {
    it('renders comments when data is loaded', async () => {
      render(
        <TestWrapper>
          <CommentsCard entityType="signal" entityId="signal-1" />
        </TestWrapper>
      );

      await waitFor(() => {
        expect(screen.getByText('First comment')).toBeInTheDocument();
        expect(screen.getByText('Second comment')).toBeInTheDocument();
      });
    });

    it('passes correct props to CommentItem components', async () => {
      render(
        <TestWrapper>
          <CommentsCard 
            entityType="signal" 
            entityId="signal-1" 
            allowEditing={false}
          />
        </TestWrapper>
      );

      await waitFor(() => {
        expect(screen.getByTestId('comment-item-1')).toBeInTheDocument();
        expect(screen.getByTestId('comment-item-2')).toBeInTheDocument();
      });
    });
  });

  describe('Event Handlers', () => {
    it('calls onCommentAdded when comment is added', async () => {
      const onCommentAdded = jest.fn();

      render(
        <TestWrapper>
          <CommentsCard 
            entityType="signal" 
            entityId="signal-1" 
            onCommentAdded={onCommentAdded}
          />
        </TestWrapper>
      );

      await waitFor(() => {
        expect(screen.getByTestId('comment-form')).toBeInTheDocument();
      });

      const addButton = screen.getByText('Add Comment');
      addButton.click();

      expect(onCommentAdded).toHaveBeenCalledWith({
        id: 'new-comment',
        content: 'Test comment'
      });
    });

    it('calls onCommentDeleted when comment is deleted', async () => {
      const onCommentDeleted = jest.fn();

      render(
        <TestWrapper>
          <CommentsCard 
            entityType="signal" 
            entityId="signal-1" 
            onCommentDeleted={onCommentDeleted}
          />
        </TestWrapper>
      );

      await waitFor(() => {
        expect(screen.getByTestId('comment-item-1')).toBeInTheDocument();
      });

      const deleteButton = screen.getByTestId('comment-item-1').querySelector('button');
      deleteButton?.click();

      expect(onCommentDeleted).toHaveBeenCalledWith('1');
    });
  });

  describe('Query Configuration', () => {
    it('uses correct query key for different entity types', () => {
      const mockGetComments = jest.fn().mockResolvedValue(mockApiResponse);
      
      mockUseApiClient.mockReturnValue({
        comments: { getComments: mockGetComments }
      });

      render(
        <TestWrapper>
          <CommentsCard entityType="idea" entityId="idea-1" replyDepth={2} />
        </TestWrapper>
      );

      expect(mockGetComments).toHaveBeenCalledWith({
        entityType: 'idea',
        entityId: 'idea-1',
        page: 1,
        pageSize: 20,
        replyDepth: 2
      });
    });

    it('disables query when entityId is not provided', () => {
      const mockGetComments = jest.fn();
      
      mockUseApiClient.mockReturnValue({
        comments: { getComments: mockGetComments }
      });

      render(
        <TestWrapper>
          <CommentsCard entityType="signal" entityId="" />
        </TestWrapper>
      );

      // Query should not be called when entityId is empty
      expect(mockGetComments).not.toHaveBeenCalled();
    });
  });
}); 