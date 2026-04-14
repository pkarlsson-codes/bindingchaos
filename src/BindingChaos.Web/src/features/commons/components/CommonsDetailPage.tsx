import { useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useCommons } from '../../../shared/hooks/useCommons';
import { useUserGroups } from '../../../shared/hooks/useUserGroups';
import { useCommonsLinkedConcerns } from '../../../shared/hooks/useCommonsLinkedConcerns';
import { useProjectsForUserGroup } from '../../../shared/hooks/useProjectsForUserGroup';
import type { UserGroupListItemResponse, ConcernListItemResponse } from '../../../api/models';
import { Card } from '../../../shared/components/layout/Card';
import { Button } from '../../../shared/components/layout/Button';
import { Badge } from '../../../shared/components/ui/badge';
import { AuthRequiredButton } from '../../auth';
import { FormUserGroupModal } from './FormUserGroupModal';
import { CreateProjectModal } from '../../projects/components/CreateProjectModal';

function ConcernCard({ concern }: { concern: ConcernListItemResponse }) {
  return (
    <Card
      title={<span className="font-semibold">{concern.name}</span>}
      content={
        <div className="flex items-center gap-6 text-sm text-muted-foreground">
          {concern.raisedByPseudonym && (
            <span>
              Raised by <span className="font-semibold text-foreground">{concern.raisedByPseudonym}</span>
            </span>
          )}
          {concern.affectedCount !== undefined && (
            <span>{concern.affectedCount} affected</span>
          )}
        </div>
      }
    />
  );
}

function UserGroupCard({ group }: { group: UserGroupListItemResponse }) {
  const [isCreateProjectModalOpen, setIsCreateProjectModalOpen] = useState(false);
  const userGroupId = group.id ?? '';
  const { data: projects = [], isLoading: isProjectsLoading } = useProjectsForUserGroup(userGroupId);
  const previewProjects = projects.slice(0, 3);

  return (
    <>
      <Card
        title={
          <div className="flex items-center gap-2">
            <Link
              to={`/user-groups/${group.id}`}
              className="font-semibold text-foreground hover:underline"
              onClick={(e) => e.stopPropagation()}
            >
              {group.name ?? 'Unnamed Group'}
            </Link>
            {group.joinPolicy && <Badge variant="secondary">{group.joinPolicy}</Badge>}
          </div>
        }
        content={
          <div className="space-y-3">
            {group.philosophy && (
              <p className="text-sm text-muted-foreground">{group.philosophy}</p>
            )}
            <div className="flex items-center gap-6 text-sm text-muted-foreground">
              {group.foundedByPseudonym && (
                <span>
                  Founded by <span className="font-semibold text-foreground">{group.foundedByPseudonym}</span>
                </span>
              )}
              {group.formedAt && (
                <span>{new Date(group.formedAt).toLocaleDateString()}</span>
              )}
              {group.memberCount !== undefined && (
                <span>{group.memberCount} {group.memberCount === 1 ? 'member' : 'members'}</span>
              )}
            </div>

            <div className="space-y-2">
              <div className="flex items-center justify-between">
                <h4 className="text-sm font-semibold text-foreground">Projects</h4>
                <div className="flex items-center gap-2">
                  {userGroupId && (
                    <Link to={`/user-groups/${userGroupId}/projects`} className="text-xs text-muted-foreground hover:text-foreground">
                      View all ({projects.length})
                    </Link>
                  )}
                  {userGroupId && (
                    <AuthRequiredButton action="create a project">
                      <Button size="sm" variant="outline" onClick={() => setIsCreateProjectModalOpen(true)}>
                        Create Project
                      </Button>
                    </AuthRequiredButton>
                  )}
                </div>
              </div>

              {isProjectsLoading ? (
                <p className="text-xs text-muted-foreground">Loading projects...</p>
              ) : previewProjects.length === 0 ? (
                <p className="text-xs text-muted-foreground">No projects yet.</p>
              ) : (
                <div className="space-y-1">
                  {previewProjects.map(project => (
                    <Link
                      key={project.id}
                      to={`/projects/${project.id}`}
                      className="block text-sm text-foreground hover:underline"
                    >
                      {project.title ?? 'Untitled Project'}
                      <span className="ml-2 text-xs text-muted-foreground">
                        ({project.activeAmendmentCount ?? 0} active, {project.contestedAmendmentCount ?? 0} contested)
                      </span>
                    </Link>
                  ))}
                </div>
              )}
            </div>
          </div>
        }
      />

      {userGroupId && (
        <CreateProjectModal
          isOpen={isCreateProjectModalOpen}
          onClose={() => setIsCreateProjectModalOpen(false)}
          userGroupId={userGroupId}
        />
      )}
    </>
  );
}

export function CommonsDetailPage() {
  const { commonsId } = useParams<{ commonsId: string }>();
  const { data: allCommons = [] } = useCommons();
  const { data: userGroups = [], isLoading, error, refetch } = useUserGroups(commonsId ?? '');
  const { data: linkedConcerns = [] } = useCommonsLinkedConcerns(commonsId ?? '');
  const [isModalOpen, setIsModalOpen] = useState(false);

  const commons = allCommons.find(c => c.id === commonsId);

  return (
    <div className="space-y-6">
      <div>
        <Link to="/commons" className="text-sm text-muted-foreground hover:text-foreground">
          ← Commons
        </Link>
      </div>

      <div className="space-y-1">
        <h1 className="text-2xl font-bold text-foreground">
          {commons?.name ?? 'Commons'}
        </h1>
        {commons?.description && (
          <p className="text-muted-foreground">{commons.description}</p>
        )}
      </div>

      <div className="flex justify-between items-center">
        <h2 className="text-lg font-semibold text-foreground">User Groups</h2>
        <AuthRequiredButton action="form a user group">
          <Button onClick={() => setIsModalOpen(true)}>Form User Group</Button>
        </AuthRequiredButton>
      </div>

      {error ? (
        <Card
          title="Error Loading User Groups"
          content={
            <p className="text-muted-foreground">There was an error loading user groups. Please try again.</p>
          }
          footer={
            <Button onClick={() => refetch()} variant="secondary">Retry</Button>
          }
        />
      ) : isLoading ? (
        <div className="space-y-4">
          {[1, 2].map(i => (
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
      ) : userGroups.length === 0 ? (
        <Card
          title="No user groups yet"
          content={
            <p className="text-muted-foreground">
              No user groups have been formed to govern this commons yet.
            </p>
          }
        />
      ) : (
        <div className="space-y-4">
          {userGroups.map(g => (
            <UserGroupCard key={g.id} group={g} />
          ))}
        </div>
      )}

      <div className="flex justify-between items-center">
        <h2 className="text-lg font-semibold text-foreground">Linked Concerns</h2>
      </div>

      {linkedConcerns.length === 0 ? (
        <Card
          title="No concerns linked yet"
          content={
            <p className="text-muted-foreground">No concerns have been linked to this commons yet.</p>
          }
        />
      ) : (
        <div className="space-y-4">
          {linkedConcerns.map(c => (
            <ConcernCard key={c.id} concern={c} />
          ))}
        </div>
      )}

      {commonsId && (
        <FormUserGroupModal
          isOpen={isModalOpen}
          onClose={() => setIsModalOpen(false)}
          commonsId={commonsId}
        />
      )}

    </div>
  );
}
