import { renderHook, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import React from 'react';
import { useCommentReplies } from '../useCommentReplies';

// Mock dependencies
jest.mock('../../../../shared/hooks/useApiClient', () => ({
  useApiClient: jest.fn()
}));

const mockUseApiClient = require('../../../../shared/hooks/useApiClient').useApiClient;

// Test data
const mockReplies = [
  {
    id: 'reply-1',
    content: 'First reply',
    authorId: 'user2',
    authorPseudonym: 'User Two',
    createdAt: new Date('2024-01-01T11:00:00Z'),
    entityType: 'signal',
    entityId: 'signal-1',
    replyCount: 0,
    isEdited: false
  },
  {
    id: 'reply-2',
    content: 'Second reply',
    authorId: 'user3',
    authorPseudonym: 'User Three',
    createdAt: new Date('2024-01-01T12:00:00Z'),
    entityType: 'signal',
    entityId: 'signal-1',
    replyCount: 0,
    isEdited: false
  }
];

// Create a wrapper with QueryClient
function createTestQueryClient() {
  return new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false }
    }
  });
}

function createWrapper() {
  const testQueryClient = createTestQueryClient();
  return ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={testQueryClient}>{children}</QueryClientProvider>
  );
}

describe('useCommentReplies', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    
    // Default mocks
    mockUseApiClient.mockReturnValue({
      discourse: {
        getPostReplies: jest.fn().mockResolvedValue({
          replies: {
            items: mockReplies,
            nextCursor: null,
            previousCursor: null,
            pageSize: 5,
            requestId: 'test-request-id',
            timestamp: new Date()
          }
        })
      }
    });
  });

  describe('Hook Initialization', () => {
    it('returns hook with expected properties', () => {
      const { result } = renderHook(
        () => useCommentReplies({ entityType: 'signal', entityId: 'signal-1' }),
        { wrapper: createWrapper() }
      );

      expect(result.current).toHaveProperty('loadReplies');
      expect(result.current).toHaveProperty('isLoading');
      expect(result.current).toHaveProperty('error');
      expect(typeof result.current.loadReplies).toBe('function');
      expect(typeof result.current.isLoading).toBe('boolean');
    });

    it('initializes with correct default values', () => {
      const { result } = renderHook(
        () => useCommentReplies({ entityType: 'signal', entityId: 'signal-1' }),
        { wrapper: createWrapper() }
      );

      expect(result.current.isLoading).toBe(false);
      expect(result.current.error).toBeNull();
    });
  });

  describe('Loading Replies', () => {
    it('calls API with correct parameters', async () => {
      const { result } = renderHook(
        () => useCommentReplies({ entityType: 'signal', entityId: 'signal-1' }),
        { wrapper: createWrapper() }
      );

      result.current.loadReplies('comment-1', 'thread-1', 2);

      await waitFor(() => {
        expect(mockUseApiClient().discourse.getPostReplies).toHaveBeenCalledWith({
          threadId: 'thread-1',
          postId: 'comment-1',
          limit: 2,
          direction: 'Forward'
        });
      });
    });

    it('uses default replyDepth of 1 when not specified', async () => {
      const { result } = renderHook(
        () => useCommentReplies({ entityType: 'signal', entityId: 'signal-1' }),
        { wrapper: createWrapper() }
      );

      result.current.loadReplies('comment-1', 'thread-1');

      await waitFor(() => {
        expect(mockUseApiClient().discourse.getPostReplies).toHaveBeenCalledWith({
          threadId: 'thread-1',
          postId: 'comment-1',
          limit: 5,
          direction: 'Forward'
        });
      });
    });

    it('sets loading state during API call', async () => {
      mockUseApiClient.mockReturnValue({
        discourse: {
          getPostReplies: jest.fn().mockImplementation(() => 
            new Promise(resolve => setTimeout(() => resolve({
              replies: {
                items: mockReplies,
                nextCursor: null,
                previousCursor: null,
                pageSize: 5,
                requestId: 'test-request-id',
                timestamp: new Date()
              }
            }), 100))
          )
        }
      });

      const { result } = renderHook(
        () => useCommentReplies({ entityType: 'signal', entityId: 'signal-1' }),
        { wrapper: createWrapper() }
      );

      result.current.loadReplies('comment-1', 'thread-1');

      // Should be loading after calling loadReplies
      await waitFor(() => {
        expect(result.current.isLoading).toBe(true);
      });
    });
  });

  describe('Error Handling', () => {
    it('handles API errors gracefully', async () => {
      const error = new Error('API Error');
      mockUseApiClient.mockReturnValue({
        discourse: {
          getPostReplies: jest.fn().mockRejectedValue(error)
        }
      });

      const { result } = renderHook(
        () => useCommentReplies({ entityType: 'signal', entityId: 'signal-1' }),
        { wrapper: createWrapper() }
      );

      result.current.loadReplies('comment-1', 'thread-1');

      await waitFor(() => {
        expect(result.current.error).toBe(error);
      });
    });

    it('resets loading state on error', async () => {
      const error = new Error('API Error');
      mockUseApiClient.mockReturnValue({
        discourse: {
          getPostReplies: jest.fn().mockRejectedValue(error)
        }
      });

      const { result } = renderHook(
        () => useCommentReplies({ entityType: 'signal', entityId: 'signal-1' }),
        { wrapper: createWrapper() }
      );

      result.current.loadReplies('comment-1', 'thread-1');

      await waitFor(() => {
        // Should not be loading after error
        expect(result.current.isLoading).toBe(false);
        expect(result.current.error).toBe(error);
      });
    });
  });

  describe('Success Handling', () => {
    it('resets loading state on success', async () => {
      const { result } = renderHook(
        () => useCommentReplies({ entityType: 'signal', entityId: 'signal-1' }),
        { wrapper: createWrapper() }
      );

      result.current.loadReplies('comment-1', 'thread-1');

      await waitFor(() => {
        // Should not be loading after success
        expect(result.current.isLoading).toBe(false);
      });
    });

    it('clears error state on success', async () => {
      // First set up an error state
      const error = new Error('API Error');
      mockUseApiClient.mockReturnValue({
        discourse: {
          getPostReplies: jest.fn()
            .mockRejectedValueOnce(error)
            .mockResolvedValueOnce({
              replies: {
                items: mockReplies,
                nextCursor: null,
                previousCursor: null,
                pageSize: 5,
                requestId: 'test-request-id',
                timestamp: new Date()
              }
            })
        }
      });

      const { result } = renderHook(
        () => useCommentReplies({ entityType: 'signal', entityId: 'signal-1' }),
        { wrapper: createWrapper() }
      );

      // First call should fail
      result.current.loadReplies('comment-1', 'thread-1');

      await waitFor(() => {
        expect(result.current.error).toBe(error);
      });

      // Second call should succeed
      result.current.loadReplies('comment-1', 'thread-1');

      await waitFor(() => {
        expect(result.current.error).toBeNull();
      });
    });
  });

  describe('Multiple Calls', () => {
    it('handles multiple loadReplies calls correctly', async () => {
      const { result } = renderHook(
        () => useCommentReplies({ entityType: 'signal', entityId: 'signal-1' }),
        { wrapper: createWrapper() }
      );

      result.current.loadReplies('comment-1', 'thread-1');
      result.current.loadReplies('comment-2', 'thread-1');

      await waitFor(() => {
        expect(mockUseApiClient().discourse.getPostReplies).toHaveBeenCalledTimes(2);
        expect(mockUseApiClient().discourse.getPostReplies).toHaveBeenNthCalledWith(1, {
          threadId: 'thread-1',
          postId: 'comment-1',
          limit: 5,
          direction: 'Forward'
        });
        expect(mockUseApiClient().discourse.getPostReplies).toHaveBeenNthCalledWith(2, {
          threadId: 'thread-1',
          postId: 'comment-2',
          limit: 5,
          direction: 'Forward'
        });
      });
    });
  });

  describe('Edge Cases', () => {
    it('handles empty replies array', async () => {
      mockUseApiClient.mockReturnValue({
        discourse: {
          getPostReplies: jest.fn().mockResolvedValue({
            replies: {
              items: [],
              nextCursor: null,
              previousCursor: null,
              pageSize: 5,
              requestId: 'test-request-id',
              timestamp: new Date()
            }
          })
        }
      });

      const { result } = renderHook(
        () => useCommentReplies({ entityType: 'signal', entityId: 'signal-1' }),
        { wrapper: createWrapper() }
      );

      result.current.loadReplies('comment-1', 'thread-1');

      await waitFor(() => {
        expect(mockUseApiClient().discourse.getPostReplies).toHaveBeenCalledWith({
          threadId: 'thread-1',
          postId: 'comment-1',
          limit: 5,
          direction: 'Forward'
        });
      });
    });

    it('handles different entity types with different IDs', async () => {
      const { result } = renderHook(
        () => useCommentReplies({ entityType: 'amendment', entityId: 'amendment-1' }),
        { wrapper: createWrapper() }
      );

      result.current.loadReplies('comment-1', 'thread-1');

      await waitFor(() => {
        expect(mockUseApiClient().discourse.getPostReplies).toHaveBeenCalledWith({
          threadId: 'thread-1',
          postId: 'comment-1',
          limit: 5,
          direction: 'Forward'
        });
      });
    });
  });
}); 