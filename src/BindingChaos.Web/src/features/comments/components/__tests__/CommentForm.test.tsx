import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { CommentForm } from '../CommentForm';

jest.mock('../../../../shared/hooks/useApiClient', () => ({
  useApiClient: jest.fn(),
}));

jest.mock('../../../auth', () => ({
  useUser: jest.fn(),
  AuthRequiredButton: ({ children }: { children: React.ReactNode }) => <>{children}</>,
}));

const mockToastSuccess = jest.fn();
const mockToastError = jest.fn();

jest.mock('../../../../shared/components/ui/toast', () => ({
  toast: {
    success: (...args: unknown[]) => mockToastSuccess(...args),
    error: (...args: unknown[]) => mockToastError(...args),
  },
}));

const mockUseApiClient = require('../../../../shared/hooks/useApiClient').useApiClient;
const mockUseUser = require('../../../auth').useUser;

function renderWithQueryClient(ui: React.ReactElement) {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false },
    },
  });

  return render(<QueryClientProvider client={queryClient}>{ui}</QueryClientProvider>);
}

describe('CommentForm', () => {
  const createPost = jest.fn();
  const createReply = jest.fn();

  beforeEach(() => {
    jest.clearAllMocks();

    mockUseUser.mockReturnValue({
      user: { id: 'user-1', pseudonym: 'Test User' },
    });

    mockUseApiClient.mockReturnValue({
      discourse: {
        createPost,
        createReply,
      },
    });

    createPost.mockResolvedValue({ data: 'post-1' });
    createReply.mockResolvedValue({ data: 'reply-1' });
  });

  it('renders comment mode by default', () => {
    renderWithQueryClient(
      <CommentForm entityType="signal" entityId="signal-1" threadId="thread-1" />,
    );

    expect(screen.getByPlaceholderText('Write a comment...')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Post Comment' })).toBeInTheDocument();
  });

  it('renders reply mode when replyingTo is provided', () => {
    renderWithQueryClient(
      <CommentForm
        entityType="signal"
        entityId="signal-1"
        threadId="thread-1"
        replyingTo="post-1"
      />,
    );

    expect(screen.getByPlaceholderText('Write a reply...')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Post Reply' })).toBeInTheDocument();
  });

  it('submits a root post when threadId is present', async () => {
    const user = userEvent.setup();

    renderWithQueryClient(
      <CommentForm entityType="signal" entityId="signal-1" threadId="thread-1" />,
    );

    await user.type(screen.getByPlaceholderText('Write a comment...'), 'Hello world');
    await user.click(screen.getByRole('button', { name: 'Post Comment' }));

    await waitFor(() => {
      expect(createPost).toHaveBeenCalledWith({
        threadId: 'thread-1',
        createPostRequest: { content: 'Hello world' },
      });
    });
  });

  it('submits a reply when replyingTo is provided', async () => {
    const user = userEvent.setup();

    renderWithQueryClient(
      <CommentForm
        entityType="signal"
        entityId="signal-1"
        threadId="thread-1"
        replyingTo="post-1"
      />,
    );

    await user.type(screen.getByPlaceholderText('Write a reply...'), 'Reply text');
    await user.click(screen.getByRole('button', { name: 'Post Reply' }));

    await waitFor(() => {
      expect(createReply).toHaveBeenCalledWith({
        threadId: 'thread-1',
        postId: 'post-1',
        createReplyRequest: { content: 'Reply text' },
      });
    });
  });

  it('calls onCommentAdded and success toast for root post', async () => {
    const user = userEvent.setup();
    const onCommentAdded = jest.fn();

    renderWithQueryClient(
      <CommentForm
        entityType="signal"
        entityId="signal-1"
        threadId="thread-1"
        onCommentAdded={onCommentAdded}
      />,
    );

    await user.type(screen.getByPlaceholderText('Write a comment...'), 'Saved');
    await user.click(screen.getByRole('button', { name: 'Post Comment' }));

    await waitFor(() => {
      expect(onCommentAdded).toHaveBeenCalledWith(
        expect.objectContaining({ id: 'post-1', content: 'Saved' }),
      );
      expect(mockToastSuccess).toHaveBeenCalledWith('Comment posted successfully!');
    });
  });

  it('shows error toast when post submission fails', async () => {
    const user = userEvent.setup();
    createPost.mockRejectedValueOnce(new Error('boom'));

    renderWithQueryClient(
      <CommentForm entityType="signal" entityId="signal-1" threadId="thread-1" />,
    );

    await user.type(screen.getByPlaceholderText('Write a comment...'), 'Oops');
    await user.click(screen.getByRole('button', { name: 'Post Comment' }));

    await waitFor(() => {
      expect(mockToastError).toHaveBeenCalledWith('Failed to post comment. Please try again.');
    });
  });
});
