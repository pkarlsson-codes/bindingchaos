import { useNavigate } from 'react-router-dom';
import { useEmergingPatterns } from '../../../shared/hooks/useEmergingPatterns';
import type { EmergingPatternResponse } from '../../../api/models';
import { Card } from '../../../shared/components/layout/Card';
import { Button } from '../../../shared/components/layout/Button';

function formatDate(date: Date | undefined): string {
  if (!date) return 'Unknown';
  return new Intl.DateTimeFormat(undefined, { dateStyle: 'medium', timeStyle: 'short' }).format(date);
}

function PatternCard({ pattern }: { pattern: EmergingPatternResponse }) {
  const navigate = useNavigate();
  const isNoise = pattern.clusterLabel === -1;
  const label = isNoise ? 'Unclustered Signals' : `Pattern #${pattern.clusterLabel}`;

  return (
    <Card
      title={
        <button
          className="text-left hover:underline font-semibold"
          onClick={() => navigate(`/patterns/${pattern.clusterLabel}`)}
        >
          {label}
        </button>
      }
      content={
        <div className="flex items-center gap-6 text-sm text-muted-foreground">
          <span>
            <span className="font-semibold text-foreground">{pattern.signalCount ?? 0}</span> signals
          </span>
          <span>Last updated {formatDate(pattern.lastUpdatedAt)}</span>
        </div>
      }
    />
  );
}

export function EmergingPatternsPage() {
  const { data: patterns = [], isLoading, error, refetch } = useEmergingPatterns();

  if (error) {
    return (
      <div className="space-y-6">
        <h1 className="text-2xl font-bold text-foreground">Emerging Patterns</h1>
        <Card
          title="Error Loading Patterns"
          content={
            <p className="text-muted-foreground">There was an error loading the emerging patterns. Please try again.</p>
          }
          footer={
            <Button onClick={() => refetch()} variant="secondary">Retry</Button>
          }
        />
      </div>
    );
  }

  const clusters = patterns.filter(p => p.clusterLabel !== -1);
  const noise = patterns.find(p => p.clusterLabel === -1);

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-foreground">Emerging Patterns</h1>
      </div>

      {isLoading ? (
        <div className="space-y-4">
          {[1, 2, 3].map(i => (
            <Card
              key={i}
              content={
                <div className="animate-pulse">
                  <div className="h-4 bg-muted rounded w-1/4 mb-2"></div>
                  <div className="h-3 bg-muted rounded w-1/2"></div>
                </div>
              }
            />
          ))}
        </div>
      ) : clusters.length === 0 && !noise ? (
        <Card
          title="No patterns yet"
          content={
            <p className="text-muted-foreground">
              Emerging patterns are identified automatically as signals accumulate. Check back once more signals have been captured.
            </p>
          }
        />
      ) : (
        <div className="space-y-4">
          {clusters.map(pattern => (
            <PatternCard key={pattern.clusterLabel} pattern={pattern} />
          ))}
          {noise && <PatternCard pattern={noise} />}
        </div>
      )}
    </div>
  );
}
