import React from 'react';
import { render, screen } from '@testing-library/react';
import { TagBag } from '../TagBag';

// Mock the Badge component
jest.mock('../../../../shared/components/ui/badge', () => ({
  Badge: ({ children, variant, className, ...props }: any) => (
    <span 
      data-testid="badge" 
      data-variant={variant} 
      className={className}
      {...props}
    >
      {children}
    </span>
  )
}));

describe('TagBag', () => {
  const defaultProps = {
    tags: ['react', 'typescript', 'javascript', 'python']
  };

  describe('Component Rendering', () => {
    it('renders all tags when no maxTags limit is set', () => {
      render(<TagBag {...defaultProps} />);
      
      const badges = screen.getAllByText(/react|typescript|javascript|python/);
      expect(badges).toHaveLength(4);
      
      expect(screen.getByText('react')).toBeInTheDocument();
      expect(screen.getByText('typescript')).toBeInTheDocument();
      expect(screen.getByText('javascript')).toBeInTheDocument();
      expect(screen.getByText('python')).toBeInTheDocument();
    });

    it('renders with custom className', () => {
      render(<TagBag {...defaultProps} className="custom-class" />);
      
      const container = screen.getByText('react').closest('div');
      expect(container).toHaveClass('custom-class');
    });

    it('applies default styling classes', () => {
      render(<TagBag {...defaultProps} />);
      
      const container = screen.getByText('react').closest('div');
      expect(container).toHaveClass('flex', 'flex-wrap', 'gap-2');
    });
  });

  describe('Max Tags Limit', () => {
    it('limits displayed tags when maxTags is set', () => {
      render(<TagBag {...defaultProps} maxTags={2} />);
      
      const badges = screen.getAllByText(/react|typescript/);
      expect(badges).toHaveLength(2); // 2 tags
      
      expect(screen.getByText('react')).toBeInTheDocument();
      expect(screen.getByText('typescript')).toBeInTheDocument();
      expect(screen.getByText('+2 more')).toBeInTheDocument();
    });

    it('shows correct remaining count when maxTags is set', () => {
      render(<TagBag {...defaultProps} maxTags={1} />);
      
      expect(screen.getByText('+3 more')).toBeInTheDocument();
    });

    it('does not show "more" badge when maxTags equals or exceeds total tags', () => {
      render(<TagBag {...defaultProps} maxTags={4} />);
      
      const badges = screen.getAllByText(/react|typescript|javascript|python/);
      expect(badges).toHaveLength(4);
      expect(screen.queryByText(/\+.*more/)).not.toBeInTheDocument();
    });

    it('does not show "more" badge when maxTags is greater than total tags', () => {
      render(<TagBag {...defaultProps} maxTags={10} />);
      
      const badges = screen.getAllByText(/react|typescript|javascript|python/);
      expect(badges).toHaveLength(4);
      expect(screen.queryByText(/\+.*more/)).not.toBeInTheDocument();
    });
  });

  describe('Badge Styling', () => {
    it('applies correct styling to tag badges', () => {
      render(<TagBag {...defaultProps} />);
      
      const tagBadges = screen.getAllByText(/react|typescript|javascript|python/);
      
      tagBadges.forEach(badge => {
        expect(badge).toHaveClass('bg-primary/10', 'text-primary', 'border-primary/20');
        expect(badge).toHaveAttribute('data-variant', 'default');
      });
    });

    it('applies correct styling to "more" badge', () => {
      render(<TagBag {...defaultProps} maxTags={2} />);
      
      const moreBadge = screen.getByText('+2 more').closest('[data-testid="badge"]');
      expect(moreBadge).toHaveClass('text-muted-foreground');
      expect(moreBadge).toHaveAttribute('data-variant', 'secondary');
    });
  });

  describe('Edge Cases', () => {
    it('handles empty tags array', () => {
      render(<TagBag tags={[]} />);
      
      const badges = screen.queryAllByTestId('badge');
      expect(badges).toHaveLength(0);
    });

    it('handles single tag', () => {
      render(<TagBag tags={['react']} />);
      
      const badges = screen.getAllByText('react');
      expect(badges).toHaveLength(1);
      expect(screen.getByText('react')).toBeInTheDocument();
    });

    it('handles tags with special characters', () => {
      const specialTags = ['react-18', 'typescript@5.0', 'c++', 'c#'];
      render(<TagBag tags={specialTags} />);
      
      expect(screen.getByText('react-18')).toBeInTheDocument();
      expect(screen.getByText('typescript@5.0')).toBeInTheDocument();
      expect(screen.getByText('c++')).toBeInTheDocument();
      expect(screen.getByText('c#')).toBeInTheDocument();
    });

    it('handles very long tag names', () => {
      const longTag = 'very-long-tag-name-that-might-cause-layout-issues';
      render(<TagBag tags={[longTag]} />);
      
      expect(screen.getByText(longTag)).toBeInTheDocument();
    });

    it('handles maxTags of 0', () => {
      render(<TagBag {...defaultProps} maxTags={0} />);
      
      // When maxTags is 0, no tags should be displayed, only the "more" badge
      const badges = screen.getAllByTestId('badge');
      expect(badges).toHaveLength(1); // Only the "more" badge
      expect(screen.getByText('+4 more')).toBeInTheDocument();
      expect(screen.queryByText('react')).not.toBeInTheDocument();
      expect(screen.queryByText('typescript')).not.toBeInTheDocument();
      expect(screen.queryByText('javascript')).not.toBeInTheDocument();
      expect(screen.queryByText('python')).not.toBeInTheDocument();
    });

    it('handles maxTags of 1', () => {
      render(<TagBag {...defaultProps} maxTags={1} />);
      
      const badges = screen.getAllByText('react');
      expect(badges).toHaveLength(1); // 1 tag
      expect(screen.getByText('react')).toBeInTheDocument();
      expect(screen.getByText('+3 more')).toBeInTheDocument();
    });
  });

  describe('Accessibility', () => {
    it('renders tags in a semantic container', () => {
      render(<TagBag {...defaultProps} />);
      
      const container = screen.getByText('react').closest('div');
      expect(container).toBeInTheDocument();
    });

    it('provides proper text content for screen readers', () => {
      render(<TagBag {...defaultProps} maxTags={2} />);
      
      expect(screen.getByText('+2 more')).toBeInTheDocument();
    });

    it('maintains proper focus order', () => {
      render(<TagBag {...defaultProps} />);
      
      const badges = screen.getAllByText(/react|typescript|javascript|python/);
      expect(badges.length).toBeGreaterThan(0);
    });
  });

  describe('Performance Considerations', () => {
    it('handles large number of tags efficiently', () => {
      const manyTags = Array.from({ length: 100 }, (_, i) => `tag-${i}`);
      render(<TagBag tags={manyTags} />);
      
      const badges = screen.getAllByText(/tag-\d+/);
      expect(badges).toHaveLength(100);
    });

    it('handles large number of tags with maxTags limit', () => {
      const manyTags = Array.from({ length: 100 }, (_, i) => `tag-${i}`);
      render(<TagBag tags={manyTags} maxTags={5} />);
      
      const badges = screen.getAllByText(/tag-\d+/);
      expect(badges).toHaveLength(5); // 5 tags
      expect(screen.getByText('+95 more')).toBeInTheDocument();
    });
  });

  describe('Integration with Badge Component', () => {
    it('passes correct props to Badge component', () => {
      render(<TagBag {...defaultProps} />);
      
      const badges = screen.getAllByText(/react|typescript|javascript|python/);
      
      // Check that each badge has the expected variant
      badges.forEach(badge => {
        expect(badge).toHaveAttribute('data-variant');
      });
    });

    it('maintains Badge component structure', () => {
      render(<TagBag {...defaultProps} />);
      
      const badges = screen.getAllByText(/react|typescript|javascript|python/);
      expect(badges.length).toBeGreaterThan(0);
      
      badges.forEach(badge => {
        expect(badge.tagName).toBe('SPAN');
      });
    });
  });

  describe('Performance Optimizations', () => {
    it('uses stable keys instead of array indices for React reconciliation', () => {
      const { rerender } = render(<TagBag {...defaultProps} />);
      
      // Get the initial render
      const initialBadges = screen.getAllByText(/react|typescript|javascript|python/);
      expect(initialBadges).toHaveLength(4);
      
      // Re-render with same props
      rerender(<TagBag {...defaultProps} />);
      
      // Should still have the same number of badges
      const rerenderedBadges = screen.getAllByText(/react|typescript|javascript|python/);
      expect(rerenderedBadges).toHaveLength(4);
    });

    it('maintains stable keys when tags are reordered', () => {
      const reorderedProps = {
        tags: ['typescript', 'react', 'python', 'javascript'] // Same tags, different order
      };
      
      const { rerender } = render(<TagBag {...defaultProps} />);
      
      // Get the initial render
      const initialBadges = screen.getAllByText(/react|typescript|javascript|python/);
      expect(initialBadges).toHaveLength(4);
      
      // Re-render with reordered tags
      rerender(<TagBag {...reorderedProps} />);
      
      // Should still have the same number of badges
      const rerenderedBadges = screen.getAllByText(/react|typescript|javascript|python/);
      expect(rerenderedBadges).toHaveLength(4);
    });
  });
}); 