import { useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { useQueryClient } from '@tanstack/react-query';
import { AuthRequiredButton } from '../../auth';
import { Button } from '../../../shared/components/layout/Button';
import { Card } from '../../../shared/components/layout/Card';
import { Badge } from '../../../shared/components/ui/badge';
import { toast } from '../../../shared/components/ui/toast';
import { useProject } from '../../../shared/hooks/useProject';
import { useApiClient } from '../../../shared/hooks/useApiClient';

export function ProjectDetailPage() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { projects } = useApiClient();
  const { projectId } = useParams<{ projectId: string }>();

  const [isProposing, setIsProposing] = useState(false);
  const [contestingAmendmentId, setContestingAmendmentId] = useState<string | null>(null);

  const { data: project, isLoading, error } = useProject(projectId ?? '');

  if (!projectId) {
    return (
      <Card
        title="Missing project"
        content={<p className="text-muted-foreground">No project identifier was provided.</p>}
      />
    );
  }

  const refreshProject = async () => {
    await queryClient.invalidateQueries({ queryKey: ['project', projectId] });
    if (project?.userGroupId) {
      await queryClient.invalidateQueries({ queryKey: ['projects', project.userGroupId] });
    }
  };

  const handleProposeAmendment = async () => {
    setIsProposing(true);
    try {
      const response = await projects.proposeProjectAmendment({ projectId });
      if (response.data) {
        toast.success('Amendment proposed.');
      } else {
        toast.error('Failed to propose amendment.');
      }
      await refreshProject();
    } catch {
      toast.error('Failed to propose amendment.');
    } finally {
      setIsProposing(false);
    }
  };

  const handleContestAmendment = async (amendmentId: string) => {
    setContestingAmendmentId(amendmentId);
    try {
      await projects.contestProjectAmendment({ projectId, amendmentId });
      toast.success('Amendment contested.');
      await refreshProject();
    } catch {
      toast.error('Failed to contest amendment.');
    } finally {
      setContestingAmendmentId(null);
    }
  };

  if (isLoading) {
    return (
      <div className="space-y-6">
        <Button onClick={() => navigate(-1)} variant="ghost" size="sm">← Back</Button>
        <Card
          content={
            <div className="animate-pulse">
              <div className="h-8 bg-muted rounded w-1/3 mb-4"></div>
              <div className="h-4 bg-muted rounded w-1/2 mb-2"></div>
              <div className="h-4 bg-muted rounded w-3/4"></div>
            </div>
          }
        />
      </div>
    );
  }

  if (error || !project) {
    return (
      <div className="space-y-6">
        <Button onClick={() => navigate(-1)} variant="ghost" size="sm">← Back</Button>
        <Card
          title="Error Loading Project"
          content={
            <p className="text-muted-foreground">
              {error instanceof Error ? error.message : 'Unable to load project details.'}
            </p>
          }
        />
      </div>
    );
  }

  const sortedAmendments = [...(project.amendments || [])].sort((a, b) => {
    const aTime = a.proposedAt ? new Date(a.proposedAt).getTime() : 0;
    const bTime = b.proposedAt ? new Date(b.proposedAt).getTime() : 0;
    return bTime - aTime;
  });

  return (
    <div className="space-y-6">
      <div className="space-y-2">
        <Link
          to={project.userGroupId ? `/user-groups/${project.userGroupId}/projects` : '/commons'}
          className="text-sm text-muted-foreground hover:text-foreground"
        >
          ← Back to Projects
        </Link>
      </div>

      <Card
        title={
          <div className="flex items-start justify-between gap-4">
            <div>
              <h1 className="text-2xl font-bold text-foreground">{project.title ?? 'Untitled Project'}</h1>
              <p className="text-sm text-muted-foreground mt-1">User group: {project.userGroupId}</p>
            </div>
            <AuthRequiredButton action="propose an amendment">
              <Button onClick={handleProposeAmendment} disabled={isProposing}>
                {isProposing ? 'Proposing...' : 'Propose Amendment'}
              </Button>
            </AuthRequiredButton>
          </div>
        }
        content={
          <div className="space-y-3">
            <p className="text-muted-foreground">{project.description ?? 'No description.'}</p>
            <div className="text-xs text-muted-foreground flex items-center gap-4">
              {project.createdAt && <span>Created {new Date(project.createdAt).toLocaleDateString()}</span>}
              {project.lastUpdatedAt && <span>Updated {new Date(project.lastUpdatedAt).toLocaleDateString()}</span>}
            </div>
          </div>
        }
      />

      <div className="space-y-4">
        <h2 className="text-lg font-semibold text-foreground">Amendments</h2>

        {sortedAmendments.length === 0 ? (
          <Card
            title="No amendments yet"
            content={<p className="text-muted-foreground">No amendments have been proposed for this project yet.</p>}
          />
        ) : (
          sortedAmendments.map(amendment => {
            const isActive = amendment.status === 'Active';
            const isContesting = contestingAmendmentId === amendment.id;

            return (
              <Card
                key={amendment.id}
                title={
                  <div className="flex items-center justify-between gap-4">
                    <div className="flex items-center gap-2">
                      <span className="font-semibold">Amendment {amendment.id}</span>
                      <Badge variant={isActive ? 'default' : 'secondary'}>{amendment.status ?? 'Unknown'}</Badge>
                    </div>
                    {isActive && amendment.id && (
                      <AuthRequiredButton action="contest this amendment">
                        <Button
                          variant="outline"
                          onClick={() => handleContestAmendment(amendment.id!)}
                          disabled={isContesting}
                        >
                          {isContesting ? 'Contesting...' : 'Contest'}
                        </Button>
                      </AuthRequiredButton>
                    )}
                  </div>
                }
                content={
                  <div className="text-sm text-muted-foreground space-y-1">
                    <p>Proposed by: {amendment.proposedById ?? 'Unknown'}</p>
                    {amendment.proposedAt && <p>Proposed at: {new Date(amendment.proposedAt).toLocaleString()}</p>}
                    {amendment.contestedById && <p>Contested by: {amendment.contestedById}</p>}
                    {amendment.contestedAt && <p>Contested at: {new Date(amendment.contestedAt).toLocaleString()}</p>}
                  </div>
                }
              />
            );
          })
        )}
      </div>
    </div>
  );
}
