import { useState, useEffect, memo } from 'react';
import { Button } from '../../../shared/components/layout/Button';
import { Input } from '../../../shared/components/ui/input';
import { Select } from '../../../shared/components/forms/Select';
import { TagSelector } from '../../tags/components/TagSelector';
import { MagnifyingGlassIcon } from '@heroicons/react/20/solid';

export interface FilterState {
  timeRange: string;
  searchTerm: string;
  tags: string[];
  sortBy: string;
}

interface FilterBarProps {
  filters: FilterState;
  onFiltersChange: (filters: FilterState) => void;
  availableTags: string[];
  totalCount: number;
  isLoading?: boolean;
}

export const FilterBar = memo(function FilterBar({ filters, onFiltersChange, availableTags, totalCount, isLoading = false }: FilterBarProps) {
  const hasActiveFilters = filters.timeRange !== 'all' || 
    filters.searchTerm !== '' || 
    filters.tags.length > 0 || 
    filters.sortBy !== 'recent';

  const [isExpanded, setIsExpanded] = useState(hasActiveFilters);
  const [localSearchTerm, setLocalSearchTerm] = useState(filters.searchTerm);

  // Auto-expand when filters become active
  useEffect(() => {
    if (hasActiveFilters && !isExpanded) {
      setIsExpanded(true);
    }
  }, [hasActiveFilters, isExpanded]);

  // Debounced search
  useEffect(() => {
    const timer = setTimeout(() => {
      if (localSearchTerm !== filters.searchTerm) {
        onFiltersChange({ ...filters, searchTerm: localSearchTerm });
      }
    }, 300);

    return () => clearTimeout(timer);
  }, [localSearchTerm, filters, onFiltersChange]);

  const handleFilterChange = (key: keyof FilterState, value: any) => {
    onFiltersChange({ ...filters, [key]: value });
  };

  const clearAllFilters = () => {
    onFiltersChange({
      timeRange: 'all',
      searchTerm: '',
      tags: [],
      sortBy: 'recent'
    });
    setLocalSearchTerm('');
  };

  const timeRangeOptions = [
    { value: 'all', label: 'All Time' },
    { value: '24h', label: 'Last 24 Hours' },
    { value: '7d', label: 'Last 7 Days' },
    { value: '30d', label: 'Last 30 Days' }
  ];

  const sortOptions = [
    { value: 'recent', label: 'Most Recent' },
    { value: 'amplified', label: 'Most Amplified' }
  ];

  return (
    <div className="relative bg-card rounded-lg shadow-sm border border-border p-4 mb-6">
      {/* Loading Overlay */}
      {isLoading && (
        <div className="absolute inset-0 bg-background/50 backdrop-blur-sm rounded-lg flex items-center justify-center z-10">
          <div className="flex items-center gap-2 text-muted-foreground">
            <div className="w-5 h-5 border-2 border-primary border-t-transparent rounded-full animate-spin" />
            <span className="text-sm font-medium">Updating results...</span>
          </div>
        </div>
      )}

      {/* Header */}
      <div className="flex justify-between items-center mb-4">
        <div className="flex items-center gap-4">
          <h3 className="text-lg font-semibold text-foreground">Filters</h3>
          {hasActiveFilters && (
            <span className="bg-secondary text-secondary-foreground text-sm px-2 py-1 rounded-full">
              {totalCount} signals
            </span>
          )}
        </div>
        <div className="flex items-center gap-2">
          {hasActiveFilters && (
            <Button
              variant="secondary"
              size="sm"
              onClick={clearAllFilters}
            >
              Clear All
            </Button>
          )}
          <Button
            variant="secondary"
            size="sm"
            onClick={() => setIsExpanded(!isExpanded)}
          >
            {isExpanded ? 'Hide' : 'Show'} Filters
          </Button>
        </div>
      </div>

      {/* Search Bar - Always Visible */}
      <div className="mb-4">
        <label className="block text-sm font-medium text-foreground mb-2">
          Search
        </label>
        <div className="relative w-full max-w-sm">
          <Input
            type="text"
            placeholder="Search signals..."
            value={localSearchTerm}
            onChange={(e) => setLocalSearchTerm(e.target.value)}
            className="pl-10"
          />
          <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <MagnifyingGlassIcon className="h-5 w-5 text-muted-foreground flex-shrink-0" />
          </div>
        </div>
      </div>

      {/* Expanded Filters */}
      {isExpanded && (
        <div className="space-y-4 pt-4 border-t border-border">
          {/* Time Range Filter */}
          <div>
            <label className="block text-sm font-medium text-foreground mb-2">
              Time Range
            </label>
            <Select
              items={timeRangeOptions}
              value={filters.timeRange}
              onChange={(value) => handleFilterChange('timeRange', value)}
              placeholder="Select time range"
            />
          </div>

          {/* Sort Options */}
          <div>
            <label className="block text-sm font-medium text-foreground mb-2">
              Sort By
            </label>
            <Select
              items={sortOptions}
              value={filters.sortBy}
              onChange={(value) => handleFilterChange('sortBy', value)}
              placeholder="Select sort option"
            />
          </div>

          {/* Tag Filter */}
          {availableTags.length > 0 && (
            <div>
              <label className="block text-sm font-medium text-foreground mb-2">
                Tags
              </label>
              <TagSelector
                selectedTags={filters.tags}
                onTagsChange={(tags) => handleFilterChange('tags', tags)}
                availableTags={availableTags}
                placeholder="Select tags..."
              />
            </div>
          )}
        </div>
      )}
    </div>
  );
});

FilterBar.displayName = 'FilterBar';