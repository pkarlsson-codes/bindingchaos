import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import { SignalDetailsCard } from './SignalDetailsCard';
import { AmplificationsTimeline } from './AmplificationsTimeline';
import { AmplificationTrendChart } from './AmplificationTrendChart';
import { SuggestedActionsCard } from './SuggestedActionsCard';
import { SuggestActionModal } from './SuggestActionModal';
import { CommentsCard } from '../../comments';
import { LoadingSpinner } from '../../../shared/components/feedback/LoadingSpinner';
import { ProposeIdeaFromSignalModal } from '../../ideas/components/ProposeIdeaFromSignalModal';
import { Button } from '../../../shared/components/ui/button';

export function SignalDetailsPage() {
  const navigate = useNavigate();
  const { signalId } = useParams<{ signalId: string }>();
  const [isProposeIdeaModalOpen, setIsProposeIdeaModalOpen] = useState(false);
  const [isSuggestActionModalOpen, setIsSuggestActionModalOpen] = useState(false);
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

  // Fetch amplification trend data
  const {
    data: trendResponse,
    isLoading: isTrendLoading,
    error: trendError,
  } = useQuery({
    queryKey: ['signalAmplificationTrend', signalId],
    queryFn: async () => {
      const result = await apiClient.signals.getSignalAmplificationTrend({ signalId: signalId! });
      return result;
    },
    enabled: !!signalId,
  });

  const signalDetail = signalDetailResponse?.data;
  const trendData = trendResponse?.data;





  const handleProposeIdea = () => {
    setIsProposeIdeaModalOpen(true);
  };

  const handleSuggestAction = () => {
    setIsSuggestActionModalOpen(true);
  };

  if (isLoading || isTrendLoading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <LoadingSpinner />
      </div>
    );
  }

  if (error || trendError) {
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
      {/* Back navigation */}
      <div className="flex items-center gap-4">
        <Button onClick={() => navigate('/signals')} variant="ghost" size="sm">
          ← Back to Signals
        </Button>
      </div>

      {/* Main content - Two column layout */}
      <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
        {/* Left column - Wide (3/4) */}
        <div className="lg:col-span-3 space-y-6">
          {/* Signal details card */}
          <SignalDetailsCard
            signalDetail={signalDetail}
            onProposeIdea={handleProposeIdea}
          />

          {/* Suggested actions */}
          <SuggestedActionsCard
            suggestedActions={signalDetail.suggestedActions || []}
            onSuggestAction={handleSuggestAction}
          />

          {/* Comments */}
          <CommentsCard
            entityType="signal"
            entityId={signalDetail.id!}
            allowEditing={true}
          />
        </div>

        {/* Right column - Narrow (1/4) */}
        <div className="space-y-6">
          {/* Amplification trend chart */}
          <AmplificationTrendChart
            data={trendData?.dataPoints || []}
            title="Amplification Trend"
            signalCreatedAt={signalDetail?.createdAt}
          />

          <AmplificationsTimeline
            dataPoints={trendData?.dataPoints || []}
            amplifications={signalDetail.amplifications || []}
            title="Active Amplifications"
          />
        </div>
      </div>

      {/* Propose Idea Modal */}
      {signalDetail && (
        <ProposeIdeaFromSignalModal
          isOpen={isProposeIdeaModalOpen}
          onClose={() => setIsProposeIdeaModalOpen(false)}
          signal={signalDetail}
        />
      )}

      {/* Suggest Action Modal */}
      {signalDetail && (
        <SuggestActionModal
          isOpen={isSuggestActionModalOpen}
          onClose={() => setIsSuggestActionModalOpen(false)}
          signalId={signalDetail.id!}
        />
      )}
    </div>
  );
} 