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
