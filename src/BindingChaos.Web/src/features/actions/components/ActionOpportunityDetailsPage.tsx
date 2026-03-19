import { useParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import { useLocality } from '../../../features/locality/hooks/useLocality';
import { Card } from '../../../shared/components/layout/Card';
import { Button } from '../../../shared/components/layout/Button';
import { SafeHtmlRenderer } from '../../../shared/components/feedback/SafeHtmlRenderer';
import { Icon } from '../../../shared/components/layout/Icon';
import { Badge } from '../../../shared/components/ui/badge';
import { Link } from 'react-router-dom';

export function ActionOpportunityDetailsPage() {
  const { actionOpportunityId } = useParams<{ actionOpportunityId: string }>();
  const { currentUrlPath } = useLocality();
  const apiClient = useApiClient();

  const { data: actionOpportunity, isLoading, error } = useQuery({
    queryKey: ['actionOpportunity', actionOpportunityId],
    queryFn: async () => {
      if (!actionOpportunityId) throw new Error('Action opportunity ID is required');
      const response = await apiClient.actionOpportunities.getActionOpportunityDetails({
        actionOpportunityId
      });
      return response.data;
    },
    enabled: !!actionOpportunityId,
  });

  if (isLoading) {
    return (
      <div className="space-y-6">
        <div className="animate-pulse">
          <div className="h-8 bg-muted rounded w-1/3 mb-4"></div>
          <div className="h-4 bg-muted rounded w-1/2 mb-2"></div>
          <div className="h-4 bg-muted rounded w-3/4"></div>
        </div>
      </div>
    );
  }

  if (error || !actionOpportunity) {
    return (
      <div className="space-y-6">
        <Card
          title="Error Loading Action Opportunity"
          content={
            <p className="text-muted-foreground">
              There was an error loading the action opportunity. Please try again.
            </p>
          }
          footer={
            <Button onClick={() => window.location.reload()} variant="secondary">
              Retry
            </Button>
          }
        />
      </div>
    );
  }

  const getStatusColor = (status: string | null | undefined) => {
    switch (status) {
      case 'emerging':
        return 'bg-blue-100 text-blue-800 border-blue-200 dark:bg-blue-900 dark:text-blue-200 dark:border-blue-800';
      case 'in-progress':
        return 'bg-yellow-100 text-yellow-800 border-yellow-200 dark:bg-yellow-900 dark:text-yellow-200 dark:border-yellow-800';
      case 'completed':
        return 'bg-green-100 text-green-800 border-green-200 dark:bg-green-900 dark:text-green-200 dark:border-green-800';
      default:
        return 'bg-muted text-muted-foreground border-border';
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString();
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex justify-between items-start">
        <div className="flex-1">
          <div className="flex items-center gap-3 mb-2">
            <Link to={`${currentUrlPath}/actions`} className="text-muted-foreground hover:text-foreground">
              <Icon name="arrow-left" className="w-5 h-5" />
            </Link>
            <h1 className="text-2xl font-bold text-foreground">{actionOpportunity.title}</h1>
          </div>
          <div className="flex items-center gap-4 text-sm text-muted-foreground">
            <span>Created {actionOpportunity.createdAt ? formatDate(actionOpportunity.createdAt) : 'Unknown date'}</span>
            <span>by {actionOpportunity.creatorName || 'Anonymous'}</span>
          </div>
        </div>
        <Badge className={`text-xs px-2 py-1 rounded-full border ${getStatusColor(actionOpportunity.status)}`}>
          {actionOpportunity.status?.replace('-', ' ') || 'Unknown'}
        </Badge>
      </div>

      {/* Main Content */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Left Column - Main Content */}
        <div className="lg:col-span-2 space-y-6">
          {/* Description */}
          <Card
            title="Description"
            content={
              <SafeHtmlRenderer 
                html={actionOpportunity.description || ''} 
                className="text-muted-foreground leading-relaxed prose prose-sm max-w-none"
              />
            }
          />

          {/* Progress */}
          <Card
            title="Progress"
            content={
              <div className="space-y-4">
                <div className="flex items-center justify-between">
                  <span className="text-sm font-medium">Progress</span>
                  <span className="text-sm text-muted-foreground">{actionOpportunity.progressPercentage}%</span>
                </div>
                <div className="w-full bg-muted rounded-full h-2">
                  <div 
                    className="bg-primary h-2 rounded-full transition-all duration-300"
                    style={{ width: `${actionOpportunity.progressPercentage}%` }}
                  ></div>
                </div>
              </div>
            }
          />
        </div>

        {/* Right Column - Sidebar */}
        <div className="space-y-6">
          {/* Parent Idea */}
          <Card
            title="Parent Idea"
            content={
              <div className="space-y-2">
                <Link 
                  to={`${currentUrlPath}/ideas/${actionOpportunity.parentIdeaId}`}
                  className="text-primary hover:underline font-medium"
                >
                  {actionOpportunity.parentIdeaTitle}
                </Link>
                <p className="text-sm text-muted-foreground">
                  This action opportunity was created from an idea.
                </p>
              </div>
            }
          />

          {/* Participants */}
          <Card
            title="Participants"
            content={
              <div className="space-y-2">
                <div className="flex items-center gap-2">
                  <Icon name="users" className="w-4 h-4 text-muted-foreground" />
                  <span className="text-sm font-medium">{actionOpportunity.participantCount} participants</span>
                </div>
                {actionOpportunity.isCommitted && (
                  <div className="flex items-center gap-2 text-sm text-green-600">
                    <Icon name="thumbs-up" className="w-4 h-4" />
                    <span>You are committed ({actionOpportunity.userCommitmentType})</span>
                  </div>
                )}
              </div>
            }
            footer={
              <div className="flex gap-2">
                {actionOpportunity.isCommitted ? (
                  <Button variant="outline" size="sm" icon="user-minus">
                    Withdraw
                  </Button>
                ) : (
                  <Button variant="primary" size="sm" icon="user-plus">
                    Join
                  </Button>
                )}
              </div>
            }
          />

          {/* Actions */}
          <Card
            title="Actions"
            content={
              <div className="space-y-2">
                <Button variant="outline" size="sm" className="w-full justify-start" icon="edit">
                  Update Progress
                </Button>
                <Button variant="outline" size="sm" className="w-full justify-start" icon="message-circle">
                  Send Message
                </Button>
              </div>
            }
          />
        </div>
      </div>
    </div>
  );
} 