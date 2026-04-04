import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useCommons } from '../../../shared/hooks/useCommons';
import type { CommonsListItemResponse } from '../../../api/models';
import { Card } from '../../../shared/components/layout/Card';
import { Button } from '../../../shared/components/layout/Button';
import { Badge } from '../../../shared/components/ui/badge';
import { AuthRequiredButton } from '../../auth';
import { ProposeCommonsModal } from './ProposeCommonsModal';

function CommonsBadge({ status }: { status?: string }) {
  if (!status) return null;
  const variant = status === 'Active' ? 'default' : 'secondary';
  return <Badge variant={variant}>{status}</Badge>;
}

function CommonsCard({ commons, onClick }: { commons: CommonsListItemResponse; onClick: () => void }) {
  return (
    <button type="button" onClick={onClick} className="w-full text-left">
    <Card
      title={
        <div className="flex items-center gap-2">
          <span className="font-semibold">{commons.name ?? 'Unnamed Commons'}</span>
          <CommonsBadge status={commons.status} />
        </div>
      }
      content={
        <div className="space-y-2">
          {commons.description && (
            <p className="text-sm text-muted-foreground">{commons.description}</p>
          )}
          <div className="flex items-center gap-6 text-sm text-muted-foreground">
            {commons.proposedByPseudonym && (
              <span>
                Proposed by <span className="font-semibold text-foreground">{commons.proposedByPseudonym}</span>
              </span>
            )}
            {commons.proposedAt && (
              <span>{new Date(commons.proposedAt).toLocaleDateString()}</span>
            )}
          </div>
        </div>
      }
    />
    </button>
  );
}

export function CommonsPage() {
  const navigate = useNavigate();
  const { data: commons = [], isLoading, error, refetch } = useCommons();
  const [isModalOpen, setIsModalOpen] = useState(false);

  if (error) {
    return (
      <div className="space-y-6">
        <h1 className="text-2xl font-bold text-foreground">Commons</h1>
        <Card
          title="Error Loading Commons"
          content={
            <p className="text-muted-foreground">There was an error loading commons. Please try again.</p>
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
        <h1 className="text-2xl font-bold text-foreground">Commons</h1>
        <AuthRequiredButton action="propose a commons">
          <Button onClick={() => setIsModalOpen(true)}>Propose Commons</Button>
        </AuthRequiredButton>
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
      ) : commons.length === 0 ? (
        <Card
          title="No commons yet"
          content={
            <p className="text-muted-foreground">
              No commons have been proposed yet. Commons are shared resources that communities govern together.
            </p>
          }
        />
      ) : (
        <div className="space-y-4">
          {commons.map(c => (
            <CommonsCard key={c.id} commons={c} onClick={() => navigate(`/commons/${c.id}`)} />
          ))}
        </div>
      )}

      <ProposeCommonsModal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} />
    </div>
  );
}
