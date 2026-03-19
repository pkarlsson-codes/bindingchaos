import { Link } from 'react-router-dom';
import { useLocality } from '../../../features/locality/hooks/useLocality';
import { Card } from '../../../shared/components/layout/Card';
import { Button } from '../../../shared/components/layout/Button';
import { Icon } from '../../../shared/components/layout/Icon';
import { Badge } from '../../../shared/components/ui/badge';
import type { ActionOpportunityListResponse } from '../../../api/models';

interface ActionOpportunityCardProps {
  actionOpportunity: ActionOpportunityListResponse;
  onCommit?: (actionOpportunityId: string) => void;
  onWithdraw?: (actionOpportunityId: string) => void;
}

export function ActionOpportunityCard({ 
  actionOpportunity, 
  onCommit, 
  onWithdraw 
}: ActionOpportunityCardProps) {
  const { currentUrlPath } = useLocality();
  const getStatusColor = (status: string | null | undefined) => {
    switch (status) {
      case 'emerging':
        return 'bg-blue-100 text-blue-800';
      case 'in-progress':
        return 'bg-yellow-100 text-yellow-800';
      case 'completed':
        return 'bg-green-100 text-green-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const getStatusIcon = (status: string | null | undefined) => {
    switch (status) {
      case 'emerging':
        return 'sparkles';
      case 'in-progress':
        return 'clock';
      case 'completed':
        return 'thumbs-up';
      default:
        return 'edit';
    }
  };

  return (
    <Card
      className="hover:shadow-md transition-shadow cursor-pointer"
      content={
        <div className="space-y-4">
          {/* Header */}
          <div className="flex items-start justify-between">
            <div className="flex-1 min-w-0">
              <Link 
                to={`${currentUrlPath}/actions/${actionOpportunity.id}`}
                className="block hover:text-primary transition-colors"
              >
                <h3 className="text-lg font-semibold text-foreground truncate">
                  {actionOpportunity.title}
                </h3>
              </Link>
              <p className="text-sm text-muted-foreground mt-1 line-clamp-2">
                {actionOpportunity.description}
              </p>
            </div>
            <div className="flex items-center gap-2 ml-4">
              <Badge className={getStatusColor(actionOpportunity.status)}>
                <Icon name={getStatusIcon(actionOpportunity.status)} className="w-3 h-3 mr-1" />
                {actionOpportunity.status?.replace('-', ' ') || 'Unknown'}
              </Badge>
            </div>
          </div>

          {/* Progress and Stats */}
          <div className="flex items-center justify-between text-sm text-muted-foreground">
            <div className="flex items-center gap-4">
              <div className="flex items-center gap-1">
                <Icon name="users" className="w-4 h-4" />
                <span>{actionOpportunity.participantCount} participants</span>
              </div>
              <div className="flex items-center gap-1">
                <Icon name="trending-up" className="w-4 h-4" />
                <span>{actionOpportunity.progressPercentage}% complete</span>
              </div>
            </div>
            <div className="text-xs">
              {actionOpportunity.createdAt ? new Date(actionOpportunity.createdAt).toLocaleDateString() : 'Unknown date'}
            </div>
          </div>

          {/* Parent Idea Link */}
          <div className="flex items-center gap-2 text-sm">
            <Icon name="lightbulb" className="w-4 h-4 text-primary" />
            <span className="text-muted-foreground">From idea:</span>
            <Link 
              to={`${currentUrlPath}/ideas/${actionOpportunity.parentIdeaId}`}
              className="text-primary hover:underline truncate"
            >
              {actionOpportunity.parentIdeaTitle}
            </Link>
          </div>

          {/* Action Buttons */}
          <div className="flex items-center justify-between pt-2 border-t">
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
              <Icon name="users" className="w-4 h-4" />
              <span>by {actionOpportunity.creatorName || 'Anonymous'}</span>
            </div>
            
            <div className="flex gap-2">
              {actionOpportunity.isCommitted ? (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => actionOpportunity.id && onWithdraw?.(actionOpportunity.id)}
                  icon="user-minus"
                >
                  Withdraw
                </Button>
              ) : (
                <Button
                  variant="primary"
                  size="sm"
                  onClick={() => actionOpportunity.id && onCommit?.(actionOpportunity.id)}
                  icon="user-plus"
                >
                  Join
                </Button>
              )}
              
              <Link to={`${currentUrlPath}/actions/${actionOpportunity.id}`}>
                <Button variant="ghost" size="sm" icon="arrow-right">
                  View Details
                </Button>
              </Link>
            </div>
          </div>
        </div>
      }
    />
  );
} 