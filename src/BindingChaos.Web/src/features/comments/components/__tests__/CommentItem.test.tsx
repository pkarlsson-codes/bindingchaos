import React from 'react';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { CommentItem } from '../CommentItem';

jest.mock('../../../../shared/hooks/useApiClient', () => ({
  useApiClient: jest.fn(() => ({
    discourse: {
      getPostReplies: jest.fn().mockResolvedValue({
        data: { replies: { items: [] } },
      }),
    },
  })),
}));

jest.mock('../../../auth', () => ({
  useUser: jest.fn(() => ({ user: { id: 'user-1', pseudonym: 'Test User' } })),
}));

jest.mock('../CommentForm', () => ({
  CommentForm: ({ replyingTo }: { replyingTo?: string }) => (
    <div data-testid="comment-form">Replying to {replyingTo}</div>
  ),
}));

const baseComment = {
  id: 'comment-1',
  content: 'This is a comment',
  authorId: 'user-1',
  authorPseudonym: 'Test User',
  createdAt: new Date('2024-01-01T10:00:00Z'),
  replyCount: 2,
  hasReplies: true,
};

describe('CommentItem', () => {
  it('renders comment content and author', () => {
    render(
      <CommentItem
        comment={baseComment}
        entityType="signal"
        entityId="signal-1"
      />,
    );

    expect(screen.getByText('This is a comment')).toBeInTheDocument();
    expect(screen.getByText('Test User')).toBeInTheDocument();
  });

  it('shows inline reply form when Reply is clicked', async () => {
    const user = userEvent.setup();

    render(
      <CommentItem
        comment={baseComment}
        entityType="signal"
        entityId="signal-1"
      />,
    );

    await user.click(screen.getByRole('button', { name: /Reply to comment by/i }));

    expect(screen.getByTestId('comment-form')).toBeInTheDocument();
    expect(screen.getByText('Replying to comment-1')).toBeInTheDocument();
  });

  it('calls onLoadReplies when load replies is clicked', async () => {
    const user = userEvent.setup();
    const onLoadReplies = jest.fn();

    render(
      <CommentItem
        comment={baseComment}
        entityType="signal"
        entityId="signal-1"
        onLoadReplies={onLoadReplies}
      />,
    );

    await user.click(screen.getByRole('button', { name: 'Load 2 replies' }));

    expect(onLoadReplies).toHaveBeenCalledWith('comment-1');
  });
});
