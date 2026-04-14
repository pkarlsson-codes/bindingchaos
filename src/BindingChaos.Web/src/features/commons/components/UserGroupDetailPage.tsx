import { useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { useUserGroup } from '../../../shared/hooks/useUserGroup';
import { useUserGroupMembers } from '../../../shared/hooks/useUserGroupMembers';
import { useActiveProjectsForUserGroup } from '../../../shared/hooks/useActiveProjectsForUserGroup';
import { Card } from '../../../shared/components/layout/Card';
import { Button } from '../../../shared/components/layout/Button';
import { Badge } from '../../../shared/components/ui/badge';
import { AuthRequiredButton } from '../../auth';
import { Collapsible, CollapsibleTrigger, CollapsibleContent } from '../../../shared/components/ui/ui/collapsible';
import type {
  UserGroupDetailResponse,
  UserGroupCharterResponse,
  UserGroupMembershipRulesResponse,
  UserGroupContestationRulesResponse,
  UserGroupShunningRulesResponse,
  ProjectListItemResponse,
  UserGroupMemberResponse,
} from '../../../api/models';
import { useOptionalAuth } from '../../auth';

function JoinButton({ joinPolicy, isMember }: { joinPolicy?: string; isMember?: boolean }) {
  const auth = useOptionalAuth();
  const isAuthenticated = !!auth?.user;

  if (isMember) {
    return <Badge variant="secondary">Member ✓</Badge>;
  }

  if (joinPolicy === 'InviteOnly') {
    return null;
  }

  const label = joinPolicy === 'Approval' ? 'Request to Join' : 'Join';

  if (!isAuthenticated) {
    return (
      <AuthRequiredButton action="join this group">
        <Button variant="outline" size="sm">{label}</Button>
      </AuthRequiredButton>
    );
  }

  return <Button variant="outline" size="sm">{label}</Button>;
}

function CharterRow({ label, value }: { label: string; value: React.ReactNode }) {
  return (
    <div className="flex justify-between items-start gap-4 py-1">
      <span className="text-sm text-muted-foreground">{label}</span>
      <span className="text-sm text-foreground text-right">{value}</span>
    </div>
  );
}

function MembershipRulesSection({ rules }: { rules: UserGroupMembershipRulesResponse }) {
  return (
    <div className="space-y-1">
      <h4 className="text-sm font-semibold text-foreground mb-2">Membership Rules</h4>
      {rules.joinPolicy && <CharterRow label="Join policy" value={rules.joinPolicy} />}
      {rules.maxMembers != null && <CharterRow label="Max members" value={rules.maxMembers} />}
      {rules.entryRequirements && <CharterRow label="Entry requirements" value={rules.entryRequirements} />}
      <CharterRow label="Member list" value={rules.memberListPublic ? 'Public' : 'Private'} />
      {rules.approvalSettings && (
        <>
          {rules.approvalSettings.approvalThreshold != null && (
            <CharterRow label="Approval threshold" value={`${Math.round(rules.approvalSettings.approvalThreshold * 100)}%`} />
          )}
          {rules.approvalSettings.vetoEnabled != null && (
            <CharterRow label="Veto enabled" value={rules.approvalSettings.vetoEnabled ? 'Yes' : 'No'} />
          )}
        </>
      )}
    </div>
  );
}

function ContestationRulesSection({ rules }: { rules: UserGroupContestationRulesResponse }) {
  return (
    <div className="space-y-1">
      <h4 className="text-sm font-semibold text-foreground mb-2">Contestation Rules</h4>
      {rules.resolutionWindow && <CharterRow label="Resolution window" value={rules.resolutionWindow} />}
      {rules.rejectionThreshold != null && (
        <CharterRow label="Rejection threshold" value={`${Math.round(rules.rejectionThreshold * 100)}%`} />
      )}
    </div>
  );
}

function ShunningRulesSection({ rules }: { rules: UserGroupShunningRulesResponse }) {
  return (
    <div className="space-y-1">
      <h4 className="text-sm font-semibold text-foreground mb-2">Shunning Rules</h4>
      {rules.approvalThreshold != null && (
        <CharterRow label="Approval threshold" value={`${Math.round(rules.approvalThreshold * 100)}%`} />
      )}
    </div>
  );
}

function ActiveProjectsSection({ userGroupId }: { userGroupId: string }) {
  const {
    data,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
    isLoading,
    isError: isLoadMoreError,
    refetch,
  } = useActiveProjectsForUserGroup(userGroupId);

  const allProjects: ProjectListItemResponse[] = data?.pages.flatMap(
    (page) => page?.data?.items ?? []
  ) ?? [];

  const isInitialLoading = isLoading && !data;
  const isInitialError = isLoadMoreError && !data;

  return (
    <div className="space-y-3">
      <div className="flex items-center justify-between">
        <h2 className="text-lg font-semibold text-foreground">Active Projects</h2>
        <Link
          to={`/user-groups/${userGroupId}/projects`}
          className="text-sm text-muted-foreground hover:text-foreground"
        >
          View all projects →
        </Link>
      </div>

      {isInitialLoading ? (
        <Card content={
          <div className="animate-pulse space-y-3">
            {[1, 2, 3].map(i => (
              <div key={i} className="space-y-1">
                <div className="h-4 bg-muted rounded w-1/3" />
                <div className="h-3 bg-muted rounded w-2/3" />
              </div>
            ))}
          </div>
        } />
      ) : isInitialError ? (
        <Card
          title="Failed to load projects"
          content={<p className="text-muted-foreground">Could not load active projects.</p>}
          footer={<Button variant="secondary" size="sm" onClick={() => refetch()}>Retry</Button>}
        />
      ) : allProjects.length === 0 ? (
        <Card content={<p className="text-muted-foreground text-sm">No active projects.</p>} />
      ) : (
        <Card content={
          <div className="space-y-4">
            {allProjects.map(project => (
              <Link
                key={project.id}
                to={`/projects/${project.id}`}
                className="block hover:bg-muted/50 rounded p-1 -m-1 transition-colors"
              >
                <div className="flex items-center justify-between gap-2">
                  <span className="font-medium text-foreground text-sm">{project.title ?? 'Untitled'}</span>
                  <div className="flex items-center gap-2 flex-shrink-0">
                    <span className="text-xs text-muted-foreground">
                      {project.activeAmendmentCount ?? 0} active amendments
                    </span>
                    {(project.contestedAmendmentCount ?? 0) > 0 && (
                      <Badge variant="destructive" className="text-xs">Contested</Badge>
                    )}
                  </div>
                </div>
                {project.description && (
                  <p className="text-xs text-muted-foreground mt-1 line-clamp-2">{project.description}</p>
                )}
              </Link>
            ))}

            {hasNextPage && (
              <Button
                variant="ghost"
                size="sm"
                onClick={() => fetchNextPage()}
                disabled={isFetchingNextPage}
                className="mt-1"
              >
                {isFetchingNextPage ? 'Loading…' : 'Load more'}
              </Button>
            )}

            {isLoadMoreError && !isInitialLoading && (
              <div className="flex items-center gap-2 text-sm text-destructive">
                <span>Failed to load more projects.</span>
                <Button variant="ghost" size="sm" onClick={() => refetch()}>Retry</Button>
              </div>
            )}
          </div>
        } />
      )}
    </div>
  );
}

function MembersSection({ userGroupId, memberCount }: { userGroupId: string; memberCount: number }) {
  const {
    data,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
    isLoading,
    isError: isLoadMoreError,
    refetch,
  } = useUserGroupMembers(userGroupId);

  const allMembers: UserGroupMemberResponse[] = data?.pages.flatMap(
    (page) => page?.data?.items ?? []
  ) ?? [];

  const isInitialError = isLoadMoreError && !data;

  return (
    <div className="space-y-3">
      <h2 className="text-lg font-semibold text-foreground">Members ({memberCount})</h2>

      {isLoading ? (
        <Card content={
          <div className="space-y-2">
            {(['w-1/3', 'w-1/2', 'w-2/3'] as const).map((width, i) => (
              <div key={i} className={`animate-pulse h-4 bg-muted rounded ${width}`} />
            ))}
          </div>
        } />
      ) : isInitialError ? (
        <Card
          title="Failed to load members"
          content={<p className="text-muted-foreground">Could not load member list.</p>}
          footer={<Button variant="secondary" size="sm" onClick={() => refetch()}>Retry</Button>}
        />
      ) : (
        <Card content={
          <div className="space-y-2">
            {allMembers.map((member) => (
              <Link
                key={member.pseudonym}
                to={`/profiles/${member.pseudonym}`}
                className="block text-sm text-foreground hover:underline"
              >
                {member.pseudonym}
              </Link>
            ))}

            {hasNextPage && (
              <Button
                variant="ghost"
                size="sm"
                onClick={() => fetchNextPage()}
                disabled={isFetchingNextPage}
                className="mt-2"
              >
                {isFetchingNextPage ? 'Loading…' : 'Load more'}
              </Button>
            )}

            {isLoadMoreError && !isInitialError && (
              <div className="flex items-center gap-2 text-sm text-destructive mt-2">
                <span>Failed to load more members.</span>
                <Button variant="ghost" size="sm" onClick={() => refetch()}>Retry</Button>
              </div>
            )}
          </div>
        } />
      )}
    </div>
  );
}

function GovernanceCard({ charter }: { charter: UserGroupCharterResponse }) {
  const [open, setOpen] = useState(false);

  return (
    <Collapsible open={open} onOpenChange={setOpen}>
      <Card
        title={
          <CollapsibleTrigger asChild>
            <button className="flex items-center justify-between w-full text-left">
              <span className="font-semibold">Governance Charter</span>
              <span className="text-sm text-muted-foreground">{open ? '▲ Collapse' : '▼ Expand'}</span>
            </button>
          </CollapsibleTrigger>
        }
        content={
          <CollapsibleContent>
            <div className="space-y-6 pt-2">
              {charter.membershipRules && (
                <MembershipRulesSection rules={charter.membershipRules} />
              )}
              {charter.contestationRules && (
                <ContestationRulesSection rules={charter.contestationRules} />
              )}
              {charter.shunningRules && (
                <ShunningRulesSection rules={charter.shunningRules} />
              )}
            </div>
          </CollapsibleContent>
        }
      />
    </Collapsible>
  );
}

export function UserGroupDetailPage() {
  const { userGroupId } = useParams<{ userGroupId: string }>();
  const { data: group, isLoading, error } = useUserGroup(userGroupId ?? '');

  if (!userGroupId) {
    return (
      <Card
        title="Not found"
        content={<p className="text-muted-foreground">No user group identifier provided.</p>}
        footer={<Link to="/commons" className="text-sm text-muted-foreground hover:text-foreground">← Commons</Link>}
      />
    );
  }

  if (isLoading) {
    return (
      <div className="space-y-6">
        <div className="h-4 bg-muted rounded w-24 animate-pulse" />
        <Card content={
          <div className="animate-pulse space-y-3">
            <div className="h-8 bg-muted rounded w-1/3" />
            <div className="h-4 bg-muted rounded w-1/2" />
            <div className="h-4 bg-muted rounded w-2/3" />
          </div>
        } />
        <Card content={<div className="animate-pulse h-12 bg-muted rounded" />} />
        <Card content={<div className="animate-pulse h-32 bg-muted rounded" />} />
      </div>
    );
  }

  if (error || !group) {
    return (
      <Card
        title="User group not found"
        content={<p className="text-muted-foreground">This user group could not be found or may no longer exist.</p>}
        footer={<Link to="/commons" className="text-sm text-muted-foreground hover:text-foreground">← Commons</Link>}
      />
    );
  }

  return (
    <div className="space-y-6">
      <div>
        <Link
          to={`/commons/${group.commonsId}`}
          className="text-sm text-muted-foreground hover:text-foreground"
        >
          ← {group.commonsName ?? 'Commons'}
        </Link>
      </div>

      <div className="flex items-start justify-between gap-4">
        <div className="space-y-1">
          <h1 className="text-2xl font-bold text-foreground">{group.name}</h1>
          {group.philosophy && (
            <p className="text-muted-foreground">{group.philosophy}</p>
          )}
          <div className="flex flex-wrap items-center gap-4 text-sm text-muted-foreground pt-1">
            {group.foundedByPseudonym && (
              <span>Founded by <span className="font-semibold text-foreground">{group.foundedByPseudonym}</span></span>
            )}
            {group.formedAt && (
              <span>{new Date(group.formedAt).toLocaleDateString()}</span>
            )}
            {group.memberCount !== undefined && (
              <span>{group.memberCount} {group.memberCount === 1 ? 'member' : 'members'}</span>
            )}
            {group.joinPolicy && (
              <Badge variant="secondary">{group.joinPolicy}</Badge>
            )}
          </div>
        </div>
        <div className="flex-shrink-0">
          <JoinButton joinPolicy={group.joinPolicy} isMember={group.isMember} />
        </div>
      </div>

      {group.charter && <GovernanceCard charter={group.charter} />}

      {group.charter?.membershipRules?.memberListPublic && (
        <MembersSection userGroupId={userGroupId} memberCount={group.memberCount ?? 0} />
      )}

      <ActiveProjectsSection userGroupId={userGroupId} />
    </div>
  );
}
