import { useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useConcerns } from '../../../shared/hooks/useConcerns';
import { useAffectednessState } from '../../../shared/hooks/useAffectednessState';
import type { ConcernListItemResponse } from '../../../api/models';
import { Card } from '../../../shared/components/layout/Card';
import { Button } from '../../../shared/components/layout/Button';
import { Badge } from '../../../shared/components/ui/badge';
import { AuthRequiredButton } from '../../auth';
import { ClaimConcernModal } from './ClaimConcernModal';

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

export function ConcernDetailPage() {
  const { concernId } = useParams<{ concernId: string }>();
  const { data: concerns = [], isLoading, error } = useConcerns();
  const [isClaimModalOpen, setIsClaimModalOpen] = useState(false);

  const concern = concerns.find(c => c.id === concernId);

  if (error) {
    return (
      <div className="space-y-6">
        <div>
          <Link to="/concerns" className="text-sm text-muted-foreground hover:text-foreground">
            ← Concerns
          </Link>
        </div>
        <Card
          title="Error Loading Concern"
          content={
            <p className="text-muted-foreground">There was an error loading this concern. Please try again.</p>
          }
        />
      </div>
    );
  }

  if (isLoading) {
    return (
      <div className="space-y-6">
        <div>
          <Link to="/concerns" className="text-sm text-muted-foreground hover:text-foreground">
            ← Concerns
          </Link>
        </div>
        <Card
          content={
            <div className="animate-pulse space-y-3">
              <div className="h-6 bg-muted rounded w-1/3"></div>
              <div className="h-4 bg-muted rounded w-2/3"></div>
              <div className="h-4 bg-muted rounded w-1/2"></div>
            </div>
          }
        />
      </div>
    );
  }

  if (!concern) {
    return (
      <div className="space-y-6">
        <div>
          <Link to="/concerns" className="text-sm text-muted-foreground hover:text-foreground">
            ← Concerns
          </Link>
        </div>
        <Card
          title="Concern not found"
          content={
            <p className="text-muted-foreground">This concern could not be found.</p>
          }
        />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div>
        <Link to="/concerns" className="text-sm text-muted-foreground hover:text-foreground">
          ← Concerns
        </Link>
      </div>

      <div className="space-y-1">
        <h1 className="text-2xl font-bold text-foreground">
          {concern.name ?? 'Unnamed Concern'}
        </h1>
        <div className="flex items-center gap-6 text-sm text-muted-foreground">
          {concern.raisedByPseudonym && (
            <span>
              Raised by <span className="font-semibold text-foreground">{concern.raisedByPseudonym}</span>
            </span>
          )}
          {concern.signals && concern.signals.length > 0 && (
            <span>
              <span className="font-semibold text-foreground">{concern.signals.length}</span>{' '}
              signal{concern.signals.length !== 1 ? 's' : ''}
            </span>
          )}
        </div>
      </div>

      {concern.tags && concern.tags.length > 0 && (
        <div className="flex gap-2 flex-wrap">
          {concern.tags.map(tag => (
            <Badge key={tag} variant="secondary">{tag}</Badge>
          ))}
        </div>
      )}

      <div className="flex items-center gap-4">
        <AffectednessButton concern={concern} />
        <AuthRequiredButton action="claim this concern for a commons">
          <Button onClick={() => setIsClaimModalOpen(true)}>Claim for a Commons</Button>
        </AuthRequiredButton>
      </div>

      {concernId && (
        <ClaimConcernModal
          isOpen={isClaimModalOpen}
          onClose={() => setIsClaimModalOpen(false)}
          concernId={concernId}
        />
      )}
    </div>
  );
}
