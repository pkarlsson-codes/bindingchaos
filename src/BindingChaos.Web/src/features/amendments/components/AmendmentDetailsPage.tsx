import { useNavigate, useParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { Card } from '../../../shared/components/layout/Card';
import { Button } from '../../../shared/components/ui/button';
import { Badge } from '../../../shared/components/ui/badge';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import { AmendmentVotingControls } from './AmendmentVotingControls';
import { SupportTrendChart } from './SupportTrendChart';
import { CommentsCard } from '../../comments';
import { SafeHtmlRenderer } from '../../../shared/components/feedback/SafeHtmlRenderer';
import { SupportersList } from './SupportersList';
import { OpponentsList } from './OpponentsList';

export function AmendmentDetailsPage() {
  const navigate = useNavigate();
  const { amendmentId, ideaId } = useParams<{ amendmentId: string; ideaId: string }>();
  const apiClient = useApiClient();

  const { data: amendmentResponse, isLoading: amendmentLoading, error: amendmentError } = useQuery({
    queryKey: ['amendment', ideaId, amendmentId],
    queryFn: () => apiClient.amendments.getAmendmentDetails({ ideaId: ideaId!, amendmentId: amendmentId! }),
    enabled: !!ideaId && !!amendmentId,
  });

  const { data: supportersResponse, isLoading: supportersLoading } = useQuery({
    queryKey: ['amendment-supporters', ideaId, amendmentId],
    queryFn: () => apiClient.amendments.getAmendmentSupporters({ ideaId: ideaId!, amendmentId: amendmentId! }),
    enabled: !!ideaId && !!amendmentId,
  });

  const { data: opponentsResponse, isLoading: opponentsLoading } = useQuery({
    queryKey: ['amendment-opponents', ideaId, amendmentId],
    queryFn: () => apiClient.amendments.getAmendmentOpponents({ ideaId: ideaId!, amendmentId: amendmentId! }),
    enabled: !!ideaId && !!amendmentId,
  });

  const { data: trendResponse, isLoading: trendLoading } = useQuery({
    queryKey: ['amendment-trend', ideaId, amendmentId],
    queryFn: () => apiClient.amendments.getAmendmentTrend({ ideaId: ideaId!, amendmentId: amendmentId! }),
    enabled: !!ideaId && !!amendmentId,
  });

  const amendment = amendmentResponse?.data;
  const supporters = supportersResponse?.data?.items || [];
  const opponents = opponentsResponse?.data?.items || [];
  const trendData = trendResponse?.data?.dataPoints || [];

  const isLoading = amendmentLoading || supportersLoading || opponentsLoading || trendLoading;
  const error = amendmentError;

  if (isLoading) {
    return (
      <div className="space-y-6">
        <div className="flex items-center gap-4">
          <Button 
            onClick={() => navigate(`/ideas/${ideaId}`)} 
            variant="ghost" 
            size="sm"
          >
            ← Back to Idea
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

  if (error || !amendment) {
    return (
      <div className="space-y-6">
        <div className="flex items-center gap-4">
          <Button 
            onClick={() => navigate(`/ideas/${ideaId}`)} 
            variant="ghost" 
            size="sm"
          >
            ← Back to Idea
          </Button>
        </div>
        <Card
          title="Error Loading Amendment"
          content={
            <div className="text-center">
              <div className="text-destructive text-lg mb-2">Error loading amendment</div>
              <p className="text-muted-foreground">
                {error instanceof Error ? error.message : 'Unable to load amendment details'}
              </p>
            </div>
          }
        />
      </div>
    );
  }

  const getStatusBadgeVariant = (status: string) => {
    switch (status) {
      case 'open':
        return 'default';
      case 'approved':
        return 'default';
      case 'rejected':
        return 'destructive';
      case 'withdrawn':
        return 'secondary';
      case 'outdated':
        return 'outline';
      default:
        return 'default';
    }
  };

  return (
    <div className="space-y-6">
      {/* Header with back button */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <Button 
            onClick={() => navigate(`/ideas/${ideaId}`)} 
            variant="ghost" 
            size="sm"
          >
            ← Back to Idea
          </Button>
          <div>
            <h1 className="text-2xl font-bold text-foreground">{amendment.title}</h1>
            <div className="flex items-center gap-2 mt-1">
              <Badge variant={getStatusBadgeVariant(amendment.status || '')}>
                {amendment.status?.toUpperCase()}
              </Badge>
              <span className="text-sm text-muted-foreground">
                by {amendment.proposerPseudonym}
              </span>
            </div>
          </div>
        </div>
        
        {/* Voting Controls */}
        <div className="flex items-center gap-4">
          <AmendmentVotingControls
            amendment={{
              id: amendment.id!,
              supporterCount: amendment.supporterCount || 0,
              opponentCount: amendment.opponentCount || 0,
              proposedByCurrentUser: amendment.proposedByCurrentUser || false,
              supportedByCurrentUser: amendment.supportedByCurrentUser || false,
              opposedByCurrentUser: amendment.opposedByCurrentUser || false,
              isOpen: amendment.status === 'Open' || false,
              status: amendment.status || ''
            }}
            ideaId={ideaId || ''}
            size="lg"
          />
        </div>
      </div>

      {/* Support Trend Chart - Full Width */}
      <SupportTrendChart 
        data={trendData} 
        title="Support Trend" 
        amendmentCreatedAt={amendment.propsedAt}
      />

      {/* Main content - Two column layout */}
      <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
        {/* Left column - Wide (3/4) */}
        <div className="lg:col-span-3 space-y-6">
          {/* Amendment Details */}
          <div className="space-y-6">
            <Card
              title="Proposed Changes"
              content={
                <div className="space-y-4">
                  <div>
                    <h3 className="font-semibold text-lg">New Title</h3>
                    <p className="text-foreground">{amendment.proposedTitle}</p>
                  </div>
                  <div>
                    <h3 className="font-semibold text-lg">New Content</h3>
                    <div className="prose prose-sm max-w-none">
                      <SafeHtmlRenderer html={amendment.proposedBody || ''} />
                    </div>
                  </div>
                </div>
              }
            />
          </div>

          {/* Comments */}
          <CommentsCard 
            entityType="amendment"
            entityId={amendment.id!}
            allowEditing={true}
          />
        </div>

        {/* Right column - Narrow (1/4) */}
        <div className="space-y-6">
          {/* Supporters List */}
          <SupportersList supporters={supporters} />
          
          {/* Opponents List */}
          <OpponentsList opponents={opponents} />
        </div>
      </div>
    </div>
  );
}
