import { useConcerns } from '../../../shared/hooks/useConcerns';
import type { ConcernListItemResponse } from '../../../api/models';
import { Card } from '../../../shared/components/layout/Card';
import { Button } from '../../../shared/components/layout/Button';
import { Badge } from '../../../shared/components/ui/badge';

function ConcernCard({ concern }: { concern: ConcernListItemResponse }) {
  return (
    <Card
      title={<span className="font-semibold">{concern.name ?? 'Unnamed Concern'}</span>}
      content={
        <div className="space-y-2">
          <div className="flex items-center gap-6 text-sm text-muted-foreground">
            {concern.raisedByPseudonym && (
              <span>Raised by <span className="font-semibold text-foreground">{concern.raisedByPseudonym}</span></span>
            )}
            {concern.signals && concern.signals.length > 0 && (
              <span>
                <span className="font-semibold text-foreground">{concern.signals.length}</span> signal{concern.signals.length !== 1 ? 's' : ''}
              </span>
            )}
          </div>
          {concern.tags && concern.tags.length > 0 && (
            <div className="flex gap-2 flex-wrap">
              {concern.tags.map(tag => (
                <Badge key={tag} variant="secondary">{tag}</Badge>
              ))}
            </div>
          )}
        </div>
      }
    />
  );
}

export function ConcernsPage() {
  const { data: concerns = [], isLoading, error, refetch } = useConcerns();

  if (error) {
    return (
      <div className="space-y-6">
        <h1 className="text-2xl font-bold text-foreground">Concerns</h1>
        <Card
          title="Error Loading Concerns"
          content={
            <p className="text-muted-foreground">There was an error loading concerns. Please try again.</p>
          }
          footer={
            <Button onClick={() => refetch()} variant="secondary">Retry</Button>
          }
        />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-foreground">Concerns</h1>
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
      ) : concerns.length === 0 ? (
        <Card
          title="No concerns yet"
          content={
            <p className="text-muted-foreground">
              No concerns have been raised yet. Concerns can be raised from individual signals.
            </p>
          }
        />
      ) : (
        <div className="space-y-4">
          {concerns.map(concern => (
            <ConcernCard key={concern.id} concern={concern} />
          ))}
        </div>
      )}
    </div>
  );
}
