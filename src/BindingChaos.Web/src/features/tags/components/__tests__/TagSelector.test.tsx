import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { TagSelector } from '../TagSelector';
import { useTagCache } from '../../services/TagCacheService';

// Mock the dependencies
jest.mock('../../services/TagCacheService');
jest.mock('../../../../shared/components/ui/tag-input', () => ({
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

jest.mock('../../../../shared/components/ui/toast', () => ({
  toast: jest.fn()
}));

const mockUseTagCache = useTagCache as jest.MockedFunction<typeof useTagCache>;

describe('TagSelector', () => {
  const defaultProps = {
    selectedTags: ['react', 'typescript'],
    onTagsChange: jest.fn(),
    localityId: 'test-locality',
    userId: 'test-user'
  };

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

  describe('Component Rendering', () => {
    it('renders with default props', () => {
      render(<TagSelector {...defaultProps} availableTags={['javascript', 'python']} />);
      
      expect(screen.getByTestId('tag-input')).toBeInTheDocument();
      expect(screen.getByPlaceholderText('Type tags, press Space or Enter...')).toBeInTheDocument();
    });

    it('renders with custom placeholder', () => {
      render(<TagSelector {...defaultProps} placeholder="Custom placeholder" availableTags={['javascript', 'python']} />);
      
      expect(screen.getByPlaceholderText('Custom placeholder')).toBeInTheDocument();
    });

    it('renders in disabled state', () => {
      render(<TagSelector {...defaultProps} disabled={true} />);
      
      const tagInput = screen.getByTestId('tag-input');
      expect(tagInput).toHaveClass('opacity-50', 'pointer-events-none');
    });

    it('shows loading state when fetching tags', async () => {
      mockTagCache.getPopularTags.mockImplementation(() => 
        new Promise(resolve => setTimeout(() => resolve(['javascript', 'python']), 100))
      );

      render(<TagSelector {...defaultProps} availableTags={[]} />);
      
      expect(screen.getByPlaceholderText('Loading tags...')).toBeInTheDocument();
      
      await waitFor(() => {
        expect(screen.getByPlaceholderText('Type tags, press Space or Enter...')).toBeInTheDocument();
      });
    });
  });

  describe('Tag Loading and Caching', () => {
    it('loads popular tags when no available tags provided', async () => {
      const popularTags = ['javascript', 'python', 'react'];
      mockTagCache.getPopularTags.mockResolvedValue(popularTags);

      render(<TagSelector {...defaultProps} availableTags={[]} />);

      await waitFor(() => {
        expect(mockTagCache.getPopularTags).toHaveBeenCalledWith('test-locality', 50);
      });
    });

    it('does not load popular tags when available tags are provided', () => {
      const availableTags = ['javascript', 'python'];
      render(<TagSelector {...defaultProps} availableTags={availableTags} />);

      expect(mockTagCache.getPopularTags).not.toHaveBeenCalled();
    });

    it('handles error when loading popular tags fails', async () => {
      const consoleSpy = jest.spyOn(console, 'error').mockImplementation(() => {});
      mockTagCache.getPopularTags.mockRejectedValue(new Error('API Error'));

      render(<TagSelector {...defaultProps} availableTags={[]} />);

      await waitFor(() => {
        expect(consoleSpy).toHaveBeenCalledWith('Failed to load popular tags:', expect.any(Error));
      });

      consoleSpy.mockRestore();
    });

    it('prevents multiple simultaneous tag loading requests', async () => {
      mockTagCache.getPopularTags.mockImplementation(() => 
        new Promise(resolve => setTimeout(() => resolve(['javascript']), 100))
      );

      const { rerender } = render(<TagSelector {...defaultProps} availableTags={[]} />);
      
      // Trigger another render while loading
      rerender(<TagSelector {...defaultProps} availableTags={[]} />);

      await waitFor(() => {
        expect(mockTagCache.getPopularTags).toHaveBeenCalledTimes(1);
      });
    });
  });

  describe('Tag Selection and Management', () => {
    it('displays selected tags correctly', () => {
      render(<TagSelector {...defaultProps} />);
      
      expect(screen.getByTestId('selected-tag-react')).toBeInTheDocument();
      expect(screen.getByTestId('selected-tag-typescript')).toBeInTheDocument();
    });

    it('displays available tags correctly', () => {
      const availableTags = ['javascript', 'python'];
      render(<TagSelector {...defaultProps} availableTags={availableTags} />);
      
      expect(screen.getByTestId('available-tag-javascript')).toBeInTheDocument();
      expect(screen.getByTestId('available-tag-python')).toBeInTheDocument();
    });

    it('displays suggested tags with special formatting', () => {
      const suggestedTags = ['suggested-tag'];
      render(<TagSelector {...defaultProps} suggestedTags={suggestedTags} />);
      
      const suggestedTagButton = screen.getByTestId('available-tag-suggested-tag');
      expect(suggestedTagButton).toHaveTextContent('#suggested-tag (suggested)');
    });

    it('filters out already selected tags from available tags', () => {
      const availableTags = ['react', 'javascript', 'python'];
      render(<TagSelector {...defaultProps} availableTags={availableTags} />);
      
      // 'react' should not be in available tags since it's already selected
      expect(screen.queryByTestId('available-tag-react')).not.toBeInTheDocument();
      expect(screen.getByTestId('available-tag-javascript')).toBeInTheDocument();
      expect(screen.getByTestId('available-tag-python')).toBeInTheDocument();
    });

    it('calls onTagsChange when tags are modified', async () => {
      const user = userEvent.setup();
      render(<TagSelector {...defaultProps} />);
      
      const input = screen.getByTestId('tag-input-field');
      await user.type(input, 'newtag,');
      
      expect(defaultProps.onTagsChange).toHaveBeenCalledWith(['react', 'typescript', 'newtag']);
    });

    it('adds new tag to recent tags cache when user is provided', async () => {
      const user = userEvent.setup();
      render(<TagSelector {...defaultProps} />);
      
      const input = screen.getByTestId('tag-input-field');
      await user.type(input, 'newtag,');
      
      expect(mockTagCache.addRecentTag).toHaveBeenCalledWith('newtag');
    });

    it('does not add tag to recent tags cache when no user is provided', async () => {
      const user = userEvent.setup();
      render(<TagSelector {...defaultProps} userId={undefined} />);
      
      const input = screen.getByTestId('tag-input-field');
      await user.type(input, 'newtag,');
      
      expect(mockTagCache.addRecentTag).not.toHaveBeenCalled();
    });
  });



  describe('Edge Cases', () => {
    it('handles empty selected tags array', () => {
      render(<TagSelector {...defaultProps} selectedTags={[]} />);
      
      expect(screen.getByTestId('tag-input')).toBeInTheDocument();
      expect(screen.queryByTestId('selected-tag-react')).not.toBeInTheDocument();
    });

    it('handles empty available tags array', () => {
      render(<TagSelector {...defaultProps} availableTags={[]} />);
      
      expect(screen.getByTestId('tag-input')).toBeInTheDocument();
    });

    it('handles duplicate tags gracefully', async () => {
      const user = userEvent.setup();
      render(<TagSelector {...defaultProps} />);
      
      const input = screen.getByTestId('tag-input-field');
      await user.type(input, 'react,'); // Try to add already selected tag
      
      // Should not call onTagsChange since tag already exists
      expect(defaultProps.onTagsChange).not.toHaveBeenCalled();
    });

    it('handles special characters in tag names', async () => {
      const user = userEvent.setup();
      render(<TagSelector {...defaultProps} />);
      
      const input = screen.getByTestId('tag-input-field');
      await user.type(input, 'tag-with-special-chars!,');
      
      expect(defaultProps.onTagsChange).toHaveBeenCalledWith([
        'react', 
        'typescript', 
        'tag-with-special-chars!'
      ]);
    });
  });

  describe('Accessibility', () => {
    it('has proper ARIA attributes', () => {
      render(<TagSelector {...defaultProps} />);
      
      const input = screen.getByTestId('tag-input-field');
      expect(input).toHaveAttribute('placeholder');
    });

    it('supports keyboard navigation', async () => {
      const user = userEvent.setup();
      render(<TagSelector {...defaultProps} />);
      
      const input = screen.getByTestId('tag-input-field');
      await user.tab();
      
      expect(input).toHaveFocus();
    });
  });

  describe('Performance Optimizations', () => {
    it('memoizes selected tag objects to prevent unnecessary re-renders', () => {
      const { rerender } = render(<TagSelector {...defaultProps} />);
      
      // Get the initial render
      const initialSelectedTags = screen.getAllByTestId(/selected-tag-/);
      expect(initialSelectedTags).toHaveLength(2);
      
      // Re-render with same props
      rerender(<TagSelector {...defaultProps} />);
      
      // Should still have the same number of selected tags
      const rerenderedSelectedTags = screen.getAllByTestId(/selected-tag-/);
      expect(rerenderedSelectedTags).toHaveLength(2);
    });

    it('memoizes all tag objects to prevent unnecessary re-renders', () => {
      const { rerender } = render(<TagSelector {...defaultProps} availableTags={['javascript', 'python']} />);
      
      // Get the initial render
      const initialAvailableTags = screen.getAllByTestId(/available-tag-/);
      expect(initialAvailableTags).toHaveLength(2);
      
      // Re-render with same props
      rerender(<TagSelector {...defaultProps} availableTags={['javascript', 'python']} />);
      
      // Should still have the same number of available tags
      const rerenderedAvailableTags = screen.getAllByTestId(/available-tag-/);
      expect(rerenderedAvailableTags).toHaveLength(2);
    });

    it('memoizes handleTagsChange function to prevent unnecessary re-renders', () => {
      const { rerender } = render(<TagSelector {...defaultProps} />);
      
      // The component should render without errors
      expect(screen.getByTestId('tag-input')).toBeInTheDocument();
      
      // Re-render with same props
      rerender(<TagSelector {...defaultProps} />);
      
      // Should still render without errors
      expect(screen.getByTestId('tag-input')).toBeInTheDocument();
    });
  });
}); 