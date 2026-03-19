import React, { useState } from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { TagSelector } from '../components/TagSelector';
import { TagBag } from '../components/TagBag';
import { useTagCache } from '../services/TagCacheService';

// Mock the dependencies
jest.mock('../services/TagCacheService');
jest.mock('../../../shared/components/ui/tag-input', () => ({
  TagInput: ({ tags, setTags, allTags, placeholder, className }: any) => {
    const [inputValue, setInputValue] = React.useState('');
    
    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
      const value = e.target.value;
      setInputValue(value);
      
      if (value.includes(',')) {
        const newTag = value.replace(',', '').trim();
        if (newTag && !tags.some((t: any) => t.value === newTag)) {
          setTags([...tags, { label: newTag, value: newTag }]);
        }
        setInputValue('');
      }
    };
    
    return (
      <div data-testid="tag-input" className={className}>
        <input 
          data-testid="tag-input-field"
          placeholder={placeholder}
          value={inputValue}
          onChange={handleInputChange}
        />
      <div data-testid="selected-tags">
        {tags.map((tag: any, index: number) => (
          <span key={index} data-testid={`selected-tag-${tag.value}`}>
            {tag.label}
            <button 
              onClick={() => setTags(tags.filter((t: any) => t.value !== tag.value))}
              data-testid={`remove-tag-${tag.value}`}
            >
              ×
            </button>
          </span>
        ))}
      </div>
      <div data-testid="available-tags">
        {allTags.map((tag: any, index: number) => (
          <button 
            key={index}
            onClick={() => {
              if (!tags.some((t: any) => t.value === tag.value)) {
                setTags([...tags, tag]);
              }
            }}
            data-testid={`available-tag-${tag.value}`}
          >
            {tag.label}
          </button>
        ))}
      </div>
    </div>
  );
  }
}));

jest.mock('../../../shared/components/ui/toast', () => ({
  toast: jest.fn()
}));

const mockUseTagCache = useTagCache as jest.MockedFunction<typeof useTagCache>;

// Test component that uses both TagSelector and TagBag
function TestTagIntegration() {
  const [selectedTags, setSelectedTags] = useState<string[]>(['react', 'typescript']);
  const [availableTags] = useState<string[]>(['javascript', 'python', 'nodejs']);

  return (
    <div>
      <h2>Tag Integration Test</h2>
      <TagSelector
        selectedTags={selectedTags}
        onTagsChange={setSelectedTags}
        availableTags={availableTags}
        localityId="test-locality"
        userId="test-user"
      />
      <div data-testid="tag-display">
        <h3>Selected Tags:</h3>
        <TagBag tags={selectedTags} maxTags={3} />
      </div>
    </div>
  );
}

describe('Tag Components Integration', () => {
  const mockTagCache = {
    getPopularTags: jest.fn(),
    addRecentTag: jest.fn(),
    searchTags: jest.fn(),
    getRecentTags: jest.fn(),
    clearCache: jest.fn(),
    getCachedTags: jest.fn()
  };

  beforeEach(() => {
    jest.clearAllMocks();
    mockUseTagCache.mockReturnValue(mockTagCache);
  });

  describe('TagSelector and TagBag Integration', () => {
    it('displays selected tags in both components', () => {
      render(<TestTagIntegration />);
      
      // Check TagSelector shows selected tags
      expect(screen.getByTestId('selected-tag-react')).toBeInTheDocument();
      expect(screen.getByTestId('selected-tag-typescript')).toBeInTheDocument();
      
      // Check TagBag shows selected tags
      expect(screen.getAllByText('react')).toHaveLength(2); // One in TagSelector, one in TagBag
      expect(screen.getAllByText('typescript')).toHaveLength(2); // One in TagSelector, one in TagBag
    });

    it('updates TagBag when tags are added via TagSelector', async () => {
      const user = userEvent.setup();
      render(<TestTagIntegration />);
      
      // Add a new tag
      const input = screen.getByTestId('tag-input-field');
      await user.type(input, 'javascript,');
      
      // Check that TagBag updates to show the new tag
      await waitFor(() => {
        expect(screen.getAllByText('javascript')).toHaveLength(2); // One in TagSelector, one in TagBag
      });
      
      // Verify all three tags are now displayed
      expect(screen.getAllByText('react')).toHaveLength(2); // One in TagSelector, one in TagBag
      expect(screen.getAllByText('typescript')).toHaveLength(2); // One in TagSelector, one in TagBag
      expect(screen.getAllByText('javascript')).toHaveLength(2); // One in TagSelector, one in TagBag
    });

    it('updates TagBag when tags are removed via TagSelector', async () => {
      const user = userEvent.setup();
      render(<TestTagIntegration />);
      
      // Remove a tag
      const removeButton = screen.getByTestId('remove-tag-react');
      await user.click(removeButton);
      
      // Check that TagBag updates to hide the removed tag
      await waitFor(() => {
        expect(screen.queryByText('react')).not.toBeInTheDocument();
      });
      
      // Verify only typescript remains
      expect(screen.getAllByText('typescript')).toHaveLength(2); // One in TagSelector, one in TagBag
    });

    it('respects maxTags limit in TagBag when tags are added', async () => {
      const user = userEvent.setup();
      render(<TestTagIntegration />);
      
      // Add two more tags to exceed the maxTags limit of 3
      const input = screen.getByTestId('tag-input-field');
      await user.type(input, 'javascript,python,');
      
      // Check that TagBag shows the limit and "more" indicator
      await waitFor(() => {
        expect(screen.getByText(/\+.*more/)).toBeInTheDocument();
      });
      
      // Verify only 3 tags are displayed
      const badges = screen.getAllByText(/\+.*more/);
      expect(badges).toHaveLength(1); // 1 "more" badge
      expect(screen.getByText('+1 more')).toBeInTheDocument();
    });
  });

  describe('Tag Cache Integration', () => {
    it('loads popular tags when no available tags are provided', async () => {
      const popularTags = ['vue', 'angular', 'svelte'];
      mockTagCache.getPopularTags.mockResolvedValue(popularTags);
      
      render(
        <TagSelector
          selectedTags={[]}
          onTagsChange={jest.fn()}
          availableTags={[]}
          localityId="test-locality"
          userId="test-user"
        />
      );
      
      await waitFor(() => {
        expect(mockTagCache.getPopularTags).toHaveBeenCalledWith('test-locality', 50);
      });
    });

    it('adds new tags to recent cache when user is provided', async () => {
      const user = userEvent.setup();
      render(<TestTagIntegration />);
      
      const input = screen.getByTestId('tag-input-field');
      await user.type(input, 'newtag,');
      
      expect(mockTagCache.addRecentTag).toHaveBeenCalledWith('newtag');
    });
  });

  describe('Error Handling Integration', () => {
    it('handles API errors gracefully in TagSelector', async () => {
      const consoleSpy = jest.spyOn(console, 'error').mockImplementation(() => {});
      mockTagCache.getPopularTags.mockRejectedValue(new Error('API Error'));
      
      render(
        <TagSelector
          selectedTags={[]}
          onTagsChange={jest.fn()}
          availableTags={[]}
          localityId="test-locality"
          userId="test-user"
        />
      );
      
      await waitFor(() => {
        expect(consoleSpy).toHaveBeenCalledWith('Failed to load popular tags:', expect.any(Error));
      });
      
      consoleSpy.mockRestore();
    });

    it('continues to function when cache service fails', async () => {
      const user = userEvent.setup();
      mockTagCache.getPopularTags.mockRejectedValue(new Error('Cache Error'));
      
      render(<TestTagIntegration />);
      
      // Should still be able to add tags manually
      const input = screen.getByTestId('tag-input-field');
      await user.type(input, 'manual-tag,');
      
      await waitFor(() => {
        expect(screen.getByTestId('selected-tag-manual-tag')).toBeInTheDocument();
      });
    });
  });

  describe('Performance Integration', () => {
    it('handles large numbers of tags efficiently', async () => {
      const manyTags = Array.from({ length: 100 }, (_, i) => `tag-${i}`);
      const user = userEvent.setup();
      
      render(
        <div>
          <TagSelector
            selectedTags={manyTags.slice(0, 10)}
            onTagsChange={jest.fn()}
            availableTags={manyTags}
            localityId="test-locality"
            userId="test-user"
          />
          <TagBag tags={manyTags} maxTags={5} />
        </div>
      );
      
      // Should render without performance issues
      expect(screen.getByTestId('available-tag-tag-10')).toBeInTheDocument();
      expect(screen.getByText('+95 more')).toBeInTheDocument();
    });

    it('maintains responsive UI during tag operations', async () => {
      const user = userEvent.setup();
      render(<TestTagIntegration />);
      
      // Rapidly add multiple tags
      const input = screen.getByTestId('tag-input-field');
      await user.type(input, 'tag1,tag2,tag3,tag4,tag5,');
      
      // UI should remain responsive
      await waitFor(() => {
        expect(screen.getByTestId('selected-tag-tag1')).toBeInTheDocument();
      });
    });
  });

  describe('Accessibility Integration', () => {
    it('maintains proper focus management between components', async () => {
      const user = userEvent.setup();
      render(<TestTagIntegration />);
      
      const input = screen.getByTestId('tag-input-field');
      await user.tab();
      
      expect(input).toHaveFocus();
    });

    it('provides proper screen reader support', () => {
      render(<TestTagIntegration />);
      
      // Check that both components provide meaningful content
      expect(screen.getByText('Selected Tags:')).toBeInTheDocument();
      expect(screen.getAllByText('react')).toHaveLength(2); // One in TagSelector, one in TagBag
      expect(screen.getAllByText('typescript')).toHaveLength(2); // One in TagSelector, one in TagBag
    });
  });

  describe('State Synchronization', () => {
    it('maintains consistent state between components', async () => {
      const user = userEvent.setup();
      render(<TestTagIntegration />);
      
      // Verify initial state
      expect(screen.getAllByText('react')).toHaveLength(2); // One in TagSelector, one in TagBag
      expect(screen.getAllByText('typescript')).toHaveLength(2); // One in TagSelector, one in TagBag
      
      // Add a tag
      const input = screen.getByTestId('tag-input-field');
      await user.type(input, 'javascript,');
      
      // Verify both components show the same state
      await waitFor(() => {
        const tagBag = screen.getByTestId('tag-display');
        expect(tagBag).toHaveTextContent('react');
        expect(tagBag).toHaveTextContent('typescript');
        expect(tagBag).toHaveTextContent('javascript');
      });
    });

    it('handles concurrent tag operations correctly', async () => {
      const user = userEvent.setup();
      render(<TestTagIntegration />);
      
      const input = screen.getByTestId('tag-input-field');
      
      // Simulate rapid tag additions
      await user.type(input, 'tag1,');
      await user.type(input, 'tag2,');
      await user.type(input, 'tag3,');
      
      // Verify all tags are added correctly
      await waitFor(() => {
        expect(screen.getByTestId('selected-tag-tag1')).toBeInTheDocument();
        expect(screen.getByTestId('selected-tag-tag2')).toBeInTheDocument();
        expect(screen.getByTestId('selected-tag-tag3')).toBeInTheDocument();
      });
    });
  });
}); 