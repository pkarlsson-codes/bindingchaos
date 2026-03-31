import { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import { Button } from '../../../shared/components/ui/button';
import { Card } from '../../../shared/components/layout/Card';
import { IdeaDetailsCard } from './IdeaDetailsCard';
import { CommentsCard } from '../../comments';

export function IdeaDetailsPage() {
  const navigate = useNavigate();
  const { ideaId } = useParams<{ ideaId: string }>();
  const apiClient = useApiClient();
  const [showCreateActionOpportunity, setShowCreateActionOpportunity] = useState(false);

  const { data: ideaDetailResponse, isLoading, error } = useQuery({
    queryKey: ['idea', ideaId],
    queryFn: () => apiClient.ideas.getIdea({ ideaId: ideaId! }),
    enabled: !!ideaId,
  });

  const ideaDetail = ideaDetailResponse?.data;

  if (isLoading) {
    return (
      <div className="space-y-6">
        <div className="flex items-center gap-4">
          <Button onClick={() => navigate(`/ideas`)} variant="ghost" size="sm">
            ← Back to Ideas
          </Button>
        </div>
        <Card
          content={
            <div className="animate-pulse">
              <div className="h-8 bg-muted rounded w-1/3 mb-4"></div>
              <div className="h-4 bg-muted rounded w-1/2 mb-2"></div>
              <div className="h-4 bg-muted rounded w-3/4"></div>
            </div>
          }
        />
      </div>
    );
  }

  if (error || !ideaDetail) {
    return (
      <div className="space-y-6">
        <div className="flex items-center gap-4">
          <Button onClick={() => navigate(`/ideas`)} variant="ghost" size="sm">
            ← Back to Ideas
          </Button>
        </div>
        <Card
          title="Error Loading Idea"
          content={
            <div className="text-center">
              <div className="text-destructive text-lg mb-2">Error loading idea</div>
              <p className="text-muted-foreground">
                {error instanceof Error ? error.message : 'Unable to load idea details'}
              </p>
            </div>
          }
        />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <Button onClick={() => navigate(`/ideas`)} variant="ghost" size="sm">
            ← Back to Ideas
          </Button>
          <h1 className="text-2xl font-bold text-foreground">{ideaDetail.idea?.title}</h1>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-1 gap-6">
        <IdeaDetailsCard 
            idea={ideaDetail.idea!}
          />

          <CommentsCard
            entityType="idea"
            entityId={ideaId!}
            allowEditing={true}
          />
      </div>
    </div>
  );
} 