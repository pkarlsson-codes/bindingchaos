import { useParams, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { useEmergingPatterns } from '../../../shared/hooks/useEmergingPatterns';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import { Card } from '../../../shared/components/layout/Card';
import { Button } from '../../../shared/components/layout/Button';
import { Badge } from '../../../shared/components/ui/badge';
import type { SignalDetailViewModel } from '../../../api/models';

function useSignalDetails(signalId: string) {
  const apiClient = useApiClient();
  return useQuery({
    queryKey: ['signals', signalId],
    queryFn: async (): Promise<SignalDetailViewModel | null> => {
      const response = await apiClient.signals.getSignalDetails({ signalId });
      return response.data ?? null;
    },
    staleTime: 5 * 60 * 1000,
  });
}

function SignalRow({ signalId }: { signalId: string }) {
  const navigate = useNavigate();
  const { data: signal, isLoading } = useSignalDetails(signalId);

  if (isLoading) {
    return (
      <div className="p-4 border rounded-lg animate-pulse">
        <div className="h-4 bg-muted rounded w-1/3 mb-2"></div>
        <div className="h-3 bg-muted rounded w-2/3"></div>
      </div>
    );
  }

  if (!signal) {
    return null;
  }

  return (
    <div
      className="p-4 border rounded-lg hover:bg-muted/50 cursor-pointer transition-colors"
      onClick={() => navigate(`/signals/${signalId}`)}
    >
      <p className="font-medium text-foreground">{signal.title}</p>
      {signal.description && (
        <p className="text-sm text-muted-foreground mt-1 line-clamp-2">{signal.description}</p>
      )}
    </div>
  );
}

export function EmergingPatternDetailsPage() {
  const { clusterLabel } = useParams<{ clusterLabel: string }>();
  const navigate = useNavigate();
  const { data: patterns = [], isLoading, error } = useEmergingPatterns();

  const clusterLabelNum = clusterLabel !== undefined ? parseInt(clusterLabel, 10) : NaN;
  const pattern = patterns.find(p => p.clusterLabel === clusterLabelNum);

  const isNoise = clusterLabelNum === -1;
  const title = isNoise ? 'Unclustered Signals' : `Pattern #${clusterLabelNum}`;

  if (error) {
    return (
      <div className="space-y-6">
        <h1 className="text-2xl font-bold text-foreground">{title}</h1>
        <Card
          title="Error Loading Pattern"
          content={<p className="text-muted-foreground">There was an error loading this pattern. Please try again.</p>}
          footer={<Button onClick={() => navigate('/patterns')} variant="secondary">Back to Patterns</Button>}
        />
      </div>
    );
  }

  if (!isLoading && !pattern) {
    return (
      <div className="space-y-6">
        <h1 className="text-2xl font-bold text-foreground">Pattern not found</h1>
        <Card
          title="Not Found"
          content={<p className="text-muted-foreground">This pattern does not exist or has not been identified yet.</p>}
          footer={<Button onClick={() => navigate('/patterns')} variant="secondary">Back to Patterns</Button>}
        />
      </div>
    );
  }

  const signalIds = pattern?.signalIds ?? [];

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Button onClick={() => navigate('/patterns')} variant="secondary">← Back</Button>
        <h1 className="text-2xl font-bold text-foreground">{title}</h1>
      </div>

      {isLoading ? (
        <div className="space-y-3">
          {[1, 2, 3].map(i => (
            <div key={i} className="p-4 border rounded-lg animate-pulse">
              <div className="h-4 bg-muted rounded w-1/3 mb-2"></div>
              <div className="h-3 bg-muted rounded w-2/3"></div>
            </div>
          ))}
        </div>
      ) : (
        <>
          {pattern?.keywords && pattern.keywords.length > 0 && (
            <div className="flex gap-2 flex-wrap">
              {pattern.keywords.map(kw => (
                <Badge key={kw} variant="secondary">{kw}</Badge>
              ))}
            </div>
          )}
          {signalIds.length === 0 ? (
            <Card
              title="No signals"
              content={<p className="text-muted-foreground">This pattern has no signals assigned to it.</p>}
            />
          ) : (
            <div className="space-y-3">
              <p className="text-sm text-muted-foreground">{signalIds.length} signal{signalIds.length !== 1 ? 's' : ''}</p>
              {signalIds.map(id => (
                <SignalRow key={id} signalId={id} />
              ))}
            </div>
          )}
        </>
      )}
    </div>
  );
}
