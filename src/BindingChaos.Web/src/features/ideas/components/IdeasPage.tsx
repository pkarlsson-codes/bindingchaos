import { useState } from 'react';
import { useIdeas } from '../../../shared/hooks/useIdeas';
import type { IdeaListItemResponse } from '../../../api/models';
import { IdeaCard } from './IdeaCard';
import { Button } from '../../../shared/components/layout/Button';
import { Select } from '../../../shared/components/forms/Select';
import { Card } from '../../../shared/components/layout/Card';
import { Input } from '../../../shared/components/ui/input';

export function IdeasPage() {
  const [searchTerm, setSearchTerm] = useState('');
  const [sortBy, setSortBy] = useState<'recent' | 'title' | 'contributors' | 'amendments'>('recent');

  const { data: ideas = [], isLoading, error, refetch } = useIdeas({
    searchTerm: searchTerm || undefined,
    sortBy
  });

  const sortOptions = [
    { value: 'recent', label: 'Date Created' },
    { value: 'title', label: 'Title' },
    { value: 'contributors', label: 'Most Contributors' },
    { value: 'amendments', label: 'Most Amendments' }
  ];

  if (error) {
    return (
      <div className="space-y-6">
        <div className="flex justify-between items-center">
          <h1 className="text-2xl font-bold text-foreground">Ideas</h1>
        </div>
        
        <Card
          title="Error Loading Ideas"
          content={
            <p className="text-muted-foreground">
              There was an error loading the ideas. Please try again.
            </p>
          }
          footer={
            <Button onClick={() => refetch()} variant="secondary">
              Retry
            </Button>
          }
        />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-foreground">Ideas</h1>
      </div>

      {/* Filters and Search */}
      <Card
        title="Search and Filter"
        content={
          <div className="flex flex-col sm:flex-row gap-4">
            <div className="flex-1">
              <Input
                type="text"
                placeholder="Search ideas by title, description, or tags..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
              />
            </div>
            <div>
              <Select
                items={sortOptions}
                value={sortBy}
                onChange={(value) => setSortBy(value as 'recent' | 'title' | 'contributors' | 'amendments')}
                placeholder="Sort by"
              />
            </div>
          </div>
        }
      />

      {/* Content */}
      {isLoading ? (
        <div className="space-y-4">
          <Card
            content={
              <div className="animate-pulse">
                <div className="h-4 bg-muted rounded w-3/4 mb-2"></div>
                <div className="h-3 bg-muted rounded w-1/2 mb-2"></div>
                <div className="h-3 bg-muted rounded w-full"></div>
              </div>
            }
          />
          <Card
            content={
              <div className="animate-pulse">
                <div className="h-4 bg-muted rounded w-2/3 mb-2"></div>
                <div className="h-3 bg-muted rounded w-3/4 mb-2"></div>
                <div className="h-3 bg-muted rounded w-1/2"></div>
              </div>
            }
          />
          <Card
            content={
              <div className="animate-pulse">
                <div className="h-4 bg-muted rounded w-1/2 mb-2"></div>
                <div className="h-3 bg-muted rounded w-full mb-2"></div>
                <div className="h-3 bg-muted rounded w-2/3"></div>
              </div>
            }
          />
        </div>
      ) : ideas.length === 0 ? (
        <Card
          title={searchTerm ? 'No ideas found' : 'No ideas yet'}
          content={
            <p className="text-muted-foreground">
              {searchTerm 
                ? 'No ideas match your current search. Try adjusting your search term.'
                : 'Be the first to create an idea in your locality.'
              }
            </p>
          }
          footer={
            searchTerm ? (
              <Button 
                onClick={() => setSearchTerm('')}
                variant="secondary"
              >
                Clear Search
              </Button>
            ) : (
              <Button variant="primary">
                Create First Idea
              </Button>
            )
          }
        />
      ) : (
        <div className="space-y-4">
          {ideas.map((idea: IdeaListItemResponse) => (
            <IdeaCard key={idea.id} idea={idea as any} />
          ))}
        </div>
      )}
    </div>
  );
} 