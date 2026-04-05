import { useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { AuthRequiredButton } from '../../auth';
import { Button } from '../../../shared/components/layout/Button';
import { Card } from '../../../shared/components/layout/Card';
import { Badge } from '../../../shared/components/ui/badge';
import { useProjectsForUserGroup } from '../../../shared/hooks/useProjectsForUserGroup';
import { CreateProjectModal } from './CreateProjectModal';

export function ProjectsByUserGroupPage() {
  const navigate = useNavigate();
  const { userGroupId } = useParams<{ userGroupId: string }>();
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);

  const {
    data: projects = [],
    isLoading,
    error,
    refetch,
  } = useProjectsForUserGroup(userGroupId ?? '');

  if (!userGroupId) {
    return (
      <Card
        title="Missing user group"
        content={<p className="text-muted-foreground">No user group identifier was provided.</p>}
      />
    );
  }

  return (
    <div className="space-y-6">
      <div className="space-y-2">
        <Link to="/commons" className="text-sm text-muted-foreground hover:text-foreground">
          ← Commons
        </Link>
      </div>

      <div className="flex justify-between items-center gap-4">
        <div>
          <h1 className="text-2xl font-bold text-foreground">Projects</h1>
          <p className="text-muted-foreground text-sm">User group: {userGroupId}</p>
        </div>
        <AuthRequiredButton action="create a project">
          <Button onClick={() => setIsCreateModalOpen(true)}>Create Project</Button>
        </AuthRequiredButton>
      </div>

      {error ? (
        <Card
          title="Error Loading Projects"
          content={<p className="text-muted-foreground">There was an error loading projects. Please try again.</p>}
          footer={<Button onClick={() => refetch()} variant="secondary">Retry</Button>}
        />
      ) : isLoading ? (
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
      ) : projects.length === 0 ? (
        <Card
          title="No projects yet"
          content={<p className="text-muted-foreground">This user group has not started any projects yet.</p>}
          footer={
            <AuthRequiredButton action="create a project">
              <Button onClick={() => setIsCreateModalOpen(true)}>Create First Project</Button>
            </AuthRequiredButton>
          }
        />
      ) : (
        <div className="space-y-4">
          {projects.map(project => (
            <button
              key={project.id}
              type="button"
              className="w-full text-left"
              onClick={() => navigate(`/projects/${project.id}`)}
            >
              <Card
                title={
                  <div className="flex items-center gap-2">
                    <span className="font-semibold">{project.title ?? 'Untitled Project'}</span>
                    <Badge variant="secondary">{project.activeAmendmentCount ?? 0} active</Badge>
                    <Badge variant="outline">{project.contestedAmendmentCount ?? 0} contested</Badge>
                  </div>
                }
                content={
                  <div className="space-y-2">
                    <p className="text-sm text-muted-foreground">{project.description ?? 'No description.'}</p>
                    <div className="text-xs text-muted-foreground flex items-center gap-4">
                      {project.createdAt && <span>Created {new Date(project.createdAt).toLocaleDateString()}</span>}
                      {project.lastUpdatedAt && <span>Updated {new Date(project.lastUpdatedAt).toLocaleDateString()}</span>}
                      <span>{project.rejectedAmendmentCount ?? 0} rejected</span>
                    </div>
                  </div>
                }
              />
            </button>
          ))}
        </div>
      )}

      <CreateProjectModal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        userGroupId={userGroupId}
      />
    </div>
  );
}
