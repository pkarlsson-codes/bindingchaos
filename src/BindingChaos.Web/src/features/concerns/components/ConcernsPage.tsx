import { useNavigate } from 'react-router-dom';
import { useConcerns } from '../../../shared/hooks/useConcerns';
import { useAffectednessState } from '../../../shared/hooks/useAffectednessState';
import type { ConcernListItemResponse } from '../../../api/models';
import { Card } from '../../../shared/components/layout/Card';
import { Button } from '../../../shared/components/layout/Button';
import { Badge } from '../../../shared/components/ui/badge';
import { AuthRequiredButton } from '../../auth';

function AffectednessButton({ concern }: { concern: ConcernListItemResponse }) {
  const { affectedCount, isAffected, isPending, toggle } = useAffectednessState({
    concernId: concern.id!,
    initialAffectedCount: concern.affectedCount ?? 0,
    initialIsAffected: concern.isAffectedByCurrentUser ?? false,
  });

  return (
    <AuthRequiredButton action="declare affectedness">
      <Button
        onClick={toggle}
        size="sm"
        variant={isAffected ? 'outline' : 'secondary'}
        disabled={isPending}
        loading={isPending}
        className="flex items-center gap-2"
        aria-label={isAffected ? 'Withdraw affectedness' : 'This concern affects me'}
      >
        <span>{isAffected ? 'Affects me' : 'This affects me'}</span>
        {affectedCount > 0 && (
          <span className="ml-1 px-1.5 py-0.5 rounded-full text-xs font-medium bg-current/20">
            {affectedCount}
          </span>
        )}
      </Button>
    </AuthRequiredButton>
  );
}

function ConcernCard({ concern, onClick }: { concern: ConcernListItemResponse; onClick: () => void }) {
  return (
    <button type="button" onClick={onClick} className="w-full text-left">
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
        footer={<AffectednessButton concern={concern} />}
      />
    </button>
  );
}

export function ConcernsPage() {
  const navigate = useNavigate();
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
            <ConcernCard key={concern.id} concern={concern} onClick={() => navigate(`/concerns/${concern.id}`)} />
          ))}
        </div>
      )}
    </div>
  );
}
