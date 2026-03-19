import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useActionOpportunities } from '../../../shared/hooks/useActionOpportunities';
import { useLocality } from '../../../features/locality/hooks/useLocality';
import type { ActionOpportunityListResponse } from '../../../api/models';
import { ActionOpportunityCard } from './ActionOpportunityCard';
import { Button } from '../../../shared/components/layout/Button';
import { Select } from '../../../shared/components/forms/Select';
import { Card } from '../../../shared/components/layout/Card';
import { Input } from '../../../shared/components/ui/input';

export function ActionsPage() {
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('all');
  const [sortBy, setSortBy] = useState<'recent' | 'title' | 'participants' | 'progress'>('recent');
  const { currentUrlPath } = useLocality();

  const { data: actionOpportunities = [], isLoading, error, refetch } = useActionOpportunities({
    searchTerm: searchTerm || undefined,
    status: statusFilter,
    sortBy
  });

  const statusOptions = [
    { value: 'all', label: 'All Statuses' },
    { value: 'emerging', label: 'Emerging' },
    { value: 'in-progress', label: 'In Progress' },
    { value: 'completed', label: 'Completed' }
  ];

  const sortOptions = [
    { value: 'recent', label: 'Date Created' },
    { value: 'title', label: 'Title' },
    { value: 'participants', label: 'Most Participants' },
    { value: 'progress', label: 'Progress' }
  ];

  const handleCommit = (actionOpportunityId: string) => {
    // TODO: Implement commitment logic
  };

  const handleWithdraw = (actionOpportunityId: string) => {
    // TODO: Implement withdrawal logic
  };

  if (error) {
    return (
      <div className="space-y-6">
        <div className="flex justify-between items-center">
          <h1 className="text-2xl font-bold text-foreground">Action Opportunities</h1>
        </div>
        
        <Card
          title="Error Loading Action Opportunities"
          content={
            <p className="text-muted-foreground">
              There was an error loading the action opportunities. Please try again.
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
         <h1 className="text-2xl font-bold text-foreground">Action Opportunities</h1>
       </div>

      {/* Filters and Search */}
      <Card
        title="Search and Filter"
        content={
          <div className="flex flex-col sm:flex-row gap-4">
            <div className="flex-1">
              <Input
                type="text"
                placeholder="Search action opportunities by title, description, or parent idea..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
              />
            </div>
            <div className="flex gap-2">
              <Select
                items={statusOptions}
                value={statusFilter}
                onChange={(value) => setStatusFilter(value)}
                placeholder="Filter by status"
              />
              <Select
                items={sortOptions}
                value={sortBy}
                onChange={(value) => setSortBy(value as 'recent' | 'title' | 'participants' | 'progress')}
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
      ) : actionOpportunities.length === 0 ? (
        <Card
          title={
            searchTerm || statusFilter !== 'all' ? 'No action opportunities found' : 'No action opportunities yet'
          }
          content={
            <p className="text-muted-foreground">
              {searchTerm || statusFilter !== 'all' 
                ? 'No action opportunities match your current filters. Try adjusting your search or filters.'
                : 'Be the first to create an action opportunity in your locality.'
              }
            </p>
          }
          footer={
            searchTerm || statusFilter !== 'all' ? (
              <Button 
                onClick={() => {
                  setSearchTerm('');
                  setStatusFilter('all');
                }}
                variant="secondary"
              >
                Clear Filters
              </Button>
                         ) : (
               <Link to={`${currentUrlPath}/ideas`}>
                 <Button 
                   variant="primary"
                   icon="lightbulb"
                 >
                   Browse Ideas to Create Action Opportunities
                 </Button>
               </Link>
             )
          }
        />
      ) : (
        <div className="space-y-4">
          {actionOpportunities.map((actionOpportunity: ActionOpportunityListResponse) => (
            <ActionOpportunityCard 
              key={actionOpportunity.id} 
              actionOpportunity={actionOpportunity}
              onCommit={handleCommit}
              onWithdraw={handleWithdraw}
            />
          ))}
        </div>
      )}


    </div>
  );
} 