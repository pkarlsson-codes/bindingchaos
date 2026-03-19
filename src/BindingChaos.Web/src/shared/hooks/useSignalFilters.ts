import { useState, useEffect, useCallback } from 'react';
import { useSearchParams } from 'react-router-dom';
import type { FilterState } from '../../features/signals/components/FilterBar';

const DEFAULT_FILTERS: FilterState = {
  timeRange: 'all',
  searchTerm: '',
  tags: [],
  sortBy: 'recent'
};

export function useSignalFilters() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [filters, setFilters] = useState<FilterState>(() => {
    return {
      timeRange: searchParams.get('timeRange') || DEFAULT_FILTERS.timeRange,
      searchTerm: searchParams.get('searchTerm') || DEFAULT_FILTERS.searchTerm,
      tags: searchParams.get('tags')?.split(',').filter(Boolean) || DEFAULT_FILTERS.tags,
      sortBy: searchParams.get('sortBy') || DEFAULT_FILTERS.sortBy
    };
  });

  const updateURL = useCallback((newFilters: FilterState) => {
    const params = new URLSearchParams();

    if (newFilters.timeRange !== DEFAULT_FILTERS.timeRange) {
      params.set('timeRange', newFilters.timeRange);
    }
    if (newFilters.searchTerm !== DEFAULT_FILTERS.searchTerm) {
      params.set('searchTerm', newFilters.searchTerm);
    }
    if (newFilters.tags.length > 0) {
      params.set('tags', newFilters.tags.join(','));
    }
    if (newFilters.sortBy !== DEFAULT_FILTERS.sortBy) {
      params.set('sortBy', newFilters.sortBy);
    }

    setSearchParams(params, { replace: true });
  }, [setSearchParams]);

  const updateFilters = useCallback((newFilters: FilterState) => {
    setFilters(newFilters);
    updateURL(newFilters);
  }, [updateURL]);

  const resetFilters = useCallback(() => {
    setFilters(DEFAULT_FILTERS);
    setSearchParams({}, { replace: true });
  }, [setSearchParams]);

  useEffect(() => {
    const urlFilters: FilterState = {
      timeRange: searchParams.get('timeRange') || DEFAULT_FILTERS.timeRange,
      searchTerm: searchParams.get('searchTerm') || DEFAULT_FILTERS.searchTerm,
      tags: searchParams.get('tags')?.split(',').filter(Boolean) || DEFAULT_FILTERS.tags,
      sortBy: searchParams.get('sortBy') || DEFAULT_FILTERS.sortBy
    };

    setFilters(urlFilters);
  }, [searchParams]);

  return {
    filters,
    updateFilters,
    resetFilters
  };
} 