import { SignalCard } from './SignalCard';
import { FilterBar } from './FilterBar';
import { useSignalFilters } from '../../../shared/hooks/useSignalFilters';
import { Button } from '../../../shared/components/layout/Button';
import { CreateSignalModal } from './CreateSignalModal';
import { Card } from '../../../shared/components/layout/Card';
import { useState, useMemo, useEffect } from 'react';
import { useInfiniteSignals } from '../../../shared/hooks/useInfiniteSignals';
import { useIntersectionObserver } from '../../../shared/hooks/useIntersectionObserver';
import { BackToTop } from '../../../shared/components/ui/BackToTop';

export function SignalFeed() {
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);

  const { filters, updateFilters } = useSignalFilters();

  // Use infinite query hook
  const {
    data,
    isLoading,
    error,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
    refetch,
  } = useInfiniteSignals({ filters });

  // Flatten all pages into a single array of signals
  const signals = useMemo(() => {
    if (!data?.pages) return [];
    return data.pages.flatMap((page: any) => page?.data?.signals?.items || []);
  }, [data?.pages]);

  // Get total count from the first page
  const totalCount = useMemo(() => {
    return data?.pages?.[0]?.data?.signals?.totalCount || 0;
  }, [data?.pages]);

  // Get available tags from the first page
  const availableTags = useMemo(() => {
    return data?.pages?.[0]?.data?.availableTags || [];
  }, [data?.pages]);

  // Get loaded count (may be less than total when more pages exist)
  const loadedCount = signals.length;

  // Intersection observer for infinite scroll
  const { ref: scrollTriggerRef, isIntersecting } = useIntersectionObserver({
    threshold: 0.5,
    rootMargin: '200px',
    enabled: hasNextPage && !isFetchingNextPage,
  });

  // Fetch next page when scroll trigger is visible
  useEffect(() => {
    if (isIntersecting && hasNextPage && !isFetchingNextPage) {
      fetchNextPage();
    }
  }, [isIntersecting, hasNextPage, isFetchingNextPage, fetchNextPage]);

  if (isLoading) {
    return (
      <div className="space-y-6">
        <div className="flex justify-between items-center">
          <h1 className="text-2xl font-bold text-foreground">Signal Feed</h1>
          <Button 
            variant="primary"
            disabled
          >
            Create Signal
          </Button>
        </div>
        
        <div className="space-y-4">
          {[...Array(3)].map((_, i) => (
            <div key={i} className="bg-background rounded-lg shadow p-6 animate-pulse">
              <div className="h-4 bg-muted rounded w-3/4 mb-2"></div>
              <div className="h-3 bg-muted rounded w-1/2 mb-4"></div>
              <div className="h-3 bg-muted rounded w-full mb-2"></div>
              <div className="h-3 bg-muted rounded w-2/3"></div>
            </div>
          ))}
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="space-y-6">
        <div className="flex justify-between items-center">
          <h1 className="text-2xl font-bold text-foreground">Signal Feed</h1>
          <Button 
            variant="primary"
            onClick={() => setIsCreateModalOpen(true)}
            icon="sparkles"
          >
            Create Signal
          </Button>
        </div>
        
        <Card
          title="Error Loading Signals"
          content={
            <div className="text-center">
              <p className="text-muted-foreground mb-4">Unable to load signals from the server.</p>
            </div>
          }
          footer={
            <Button 
              onClick={() => refetch()}
              variant="primary"
            >
              Try Again
            </Button>
          }
        />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-foreground">Signal Feed</h1>
        <Button 
          variant="primary"
          onClick={() => setIsCreateModalOpen(true)}
          icon="sparkles"
        >
          Create Signal
        </Button>
      </div>

      {/* Filter Bar */}
      <FilterBar
        filters={filters}
        onFiltersChange={updateFilters}
        availableTags={availableTags}
        totalCount={totalCount}
        isLoading={isFetchingNextPage || isLoading}
      />

      {/* Pagination metadata - show when loaded count differs from total */}
      {loadedCount > 0 && totalCount > loadedCount && (
        <div className="text-sm text-muted-foreground text-center py-2">
          Showing {loadedCount} of {totalCount} signals
        </div>
      )}

      {/* Signals List */}
      <div>
        {signals.length === 0 ? (
            <Card
              title={
                filters.searchTerm
                  ? `No signals about "${filters.searchTerm}" yet`
                  : filters.tags.length > 0 || filters.timeRange !== 'all'
                  ? 'No signals match your filters'
                  : 'No signals yet'
              }
              content={
                <p className="text-muted-foreground">
                  {filters.searchTerm
                    ? 'Be the first to create a signal about this topic! Share what you know or what concerns you.'
                    : filters.tags.length > 0 || filters.timeRange !== 'all'
                    ? 'Try adjusting your filters to see more signals.'
                    : 'Be the first to create a signal in your locality.'
                  }
                </p>
              }
              footer={
                <Button 
                  onClick={() => setIsCreateModalOpen(true)}
                  variant="primary"
                  icon="sparkles"
                >
                  {filters.searchTerm ? 'Create Signal' : 'Create First Signal'}
                </Button>
              }
            />
          ) : (
            <div className="space-y-4">
              {signals.map((signal: any, index: number) => (
                <SignalCard key={signal.id || index} signal={signal} />
              ))}

              {/* Infinite scroll trigger */}
              {hasNextPage && (
                <div ref={scrollTriggerRef} className="py-8 flex justify-center">
                  {isFetchingNextPage && (
                    <div className="flex items-center gap-2 text-muted-foreground">
                      <div className="w-5 h-5 border-2 border-primary border-t-transparent rounded-full animate-spin" />
                      <span>Loading more signals...</span>
                    </div>
                  )}
                </div>
              )}

              {/* End of results message */}
              {!hasNextPage && signals.length > 0 && (
                <div className="py-8 text-center text-muted-foreground">
                  <p>You've reached the end of the feed</p>
                  <p className="text-sm mt-1">Showing all {totalCount} signals</p>
                </div>
              )}
            </div>
          )}
      </div>

      <CreateSignalModal 
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
      />

      {/* Back to top button - shows after scrolling down */}
      <BackToTop threshold={600} />
    </div>
  );
}