import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { CommentsCard } from '../CommentsCard';

jest.mock('../../../../shared/hooks/useApiClient', () => ({
  useApiClient: jest.fn(),
}));

jest.mock('../../hooks/useCommentReplies', () => ({
  useCommentReplies: jest.fn(() => ({
    loadReplies: jest.fn(),
    isLoading: false,
  })),
}));

jest.mock('../CommentForm', () => ({
  CommentForm: () => <div data-testid="comment-form">Comment form</div>,
}));

jest.mock('../CommentItem', () => ({
  CommentItem: ({ comment }: { comment: { id?: string; content?: string } }) => (
    <div data-testid={`comment-item-${comment.id}`}>{comment.content}</div>
  ),
}));

const mockUseApiClient = require('../../../../shared/hooks/useApiClient').useApiClient;

function renderWithQueryClient(ui: React.ReactElement) {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false },
    },
  });

  return render(<QueryClientProvider client={queryClient}>{ui}</QueryClientProvider>);
}

describe('CommentsCard', () => {
  const getPostsByEntity = jest.fn();

  beforeEach(() => {
    jest.clearAllMocks();

    mockUseApiClient.mockReturnValue({
      discourse: {
        getPostsByEntity,
        getPostReplies: jest.fn(),
      },
    });

    getPostsByEntity.mockResolvedValue({
      data: {
        threadId: 'thread-1',
        posts: {
          items: [
            {
              id: 'c1',
              content: 'First comment',
              authorPseudonym: 'User One',
              createdAt: new Date('2024-01-01T10:00:00Z'),
              replyCount: 0,
            },
          ],
          nextCursor: null,
        },
      },
    });
  });

  it('renders title and comment form', () => {
    renderWithQueryClient(<CommentsCard entityType="signal" entityId="signal-1" />);

    expect(screen.getByText('Comments')).toBeInTheDocument();
    expect(screen.getByTestId('comment-form')).toBeInTheDocument();
  });

  it('renders comments loaded from API', async () => {
    renderWithQueryClient(<CommentsCard entityType="signal" entityId="signal-1" />);

    await waitFor(() => {
      expect(screen.getByTestId('comment-item-c1')).toBeInTheDocument();
      expect(screen.getByText('First comment')).toBeInTheDocument();
    });
  });

  it('renders empty state when no comments exist', async () => {
    getPostsByEntity.mockResolvedValueOnce({
      data: {
        threadId: 'thread-1',
        posts: {
          items: [],
          nextCursor: null,
        },
      },
    });

    renderWithQueryClient(<CommentsCard entityType="signal" entityId="signal-1" />);

    await waitFor(() => {
      expect(screen.getByText('No comments yet. Be the first to comment!')).toBeInTheDocument();
    });
  });

  it('renders error state when API call fails', async () => {
    getPostsByEntity.mockRejectedValueOnce(new Error('boom'));

    renderWithQueryClient(<CommentsCard entityType="signal" entityId="signal-1" />);

    await waitFor(() => {
      expect(screen.getByText('Error loading comments')).toBeInTheDocument();
    });
  });
});
