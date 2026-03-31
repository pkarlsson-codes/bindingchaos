import { useParams, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import { SignalDetailsCard } from './SignalDetailsCard';
import { CommentsCard } from '../../comments';
import { LoadingSpinner } from '../../../shared/components/feedback/LoadingSpinner';
import { Button } from '../../../shared/components/ui/button';

export function SignalDetailsPage() {
  const navigate = useNavigate();
  const { signalId } = useParams<{ signalId: string }>();
  const apiClient = useApiClient();
  const {
    data: signalDetailResponse,
    isLoading,
    error,
  } = useQuery({
    queryKey: ['signalDetails', signalId],
    queryFn: async () => {
      const result = await apiClient.signals.getSignalDetails({ signalId: signalId! });
      return result;
    },
    enabled: !!signalId,
  });

  const signalDetail = signalDetailResponse?.data;


  if (isLoading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <LoadingSpinner />
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-destructive mb-4">Error Loading Signal</h1>
          <p className="text-muted-foreground">Failed to load signal details or trend data. Please try again.</p>
        </div>
      </div>
    );
  }

  if (!signalDetail) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-muted-foreground mb-4">Signal Not Found</h1>
          <p className="text-muted-foreground">The requested signal could not be found.</p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Button onClick={() => navigate('/signals')} variant="ghost" size="sm">
          ← Back to Signals
        </Button>
      </div>

      <div className="grid grid-cols-1 gap-6">
        <div className="space-y-6">
          <SignalDetailsCard
            signalDetail={signalDetail}
          />

          <CommentsCard
            entityType="signal"
            entityId={signalDetail.id!}
            allowEditing={true}
          />
        </div>

        <div className="space-y-6">
        </div>
      </div>
    </div>
  );
} 