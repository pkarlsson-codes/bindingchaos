import { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import { Button } from '../../../shared/components/ui/button';
import { Card } from '../../../shared/components/layout/Card';
import type { IdeaDetailViewModel, AmendmentsListItemResponse } from '../../../api/models';
import { IdeaDetailsCard } from './IdeaDetailsCard';
import { CommentsCard } from '../../comments';
import { AmendmentsCard } from './AmendmentsCard';
import { SupportStatisticsCard } from './SupportStatisticsCard';
import { SupportHistoryCard } from './SupportHistoryCard';
import { AuthRequiredButton } from '../../auth';
import { CreateActionOpportunityModal } from '../../actions/components/CreateActionOpportunityModal';

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



  // TODO: Calculate support/opposition totals from amendments data
  // For now, we'll set these to 0 and update the calculation when we have the data
  const totalSupporters = 0;
  const totalOpponents = 0;

  return (
    <div className="space-y-6">
      {/* Header with back button */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <Button onClick={() => navigate(`/ideas`)} variant="ghost" size="sm">
            ← Back to Ideas
          </Button>
          <h1 className="text-2xl font-bold text-foreground">{ideaDetail.idea?.title}</h1>
        </div>
        <div className="flex gap-2">
          <AuthRequiredButton action="create an action opportunity">
            <Button onClick={() => setShowCreateActionOpportunity(true)}>
              Create Action Opportunity
            </Button>
          </AuthRequiredButton>
          <AuthRequiredButton action="propose an amendment">
            <Button onClick={() => navigate(`/ideas/${ideaId}/amendments/new`)}>
              Propose Amendment
            </Button>
          </AuthRequiredButton>
        </div>
      </div>

      {/* Main content - Two column layout */}
      <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
        {/* Left column - Wide (3/4) */}
        <div className="lg:col-span-3 space-y-6">
          {/* Idea details card */}
          <IdeaDetailsCard 
            idea={ideaDetail.idea!}
          />

          {/* Comment threads */}
          <CommentsCard
            entityType="idea"
            entityId={ideaId!}
            allowEditing={true}
          />
        </div>

        {/* Right column - Narrow (1/4) */}
        <div className="space-y-6">
          {/* Amendments list */}
          <AmendmentsCard />

          {/* Support/opposition statistics */}
          <SupportStatisticsCard 
            totalSupporters={totalSupporters}
            totalOpponents={totalOpponents}
          />

          {/* Historical support graph placeholder */}
          <SupportHistoryCard />
        </div>
      </div>

      {/* Create Action Opportunity Modal */}
      {ideaDetail && (
        <CreateActionOpportunityModal
          isOpen={showCreateActionOpportunity}
          onClose={() => setShowCreateActionOpportunity(false)}
          idea={ideaDetail}
        />
      )}
    </div>
  );
} 