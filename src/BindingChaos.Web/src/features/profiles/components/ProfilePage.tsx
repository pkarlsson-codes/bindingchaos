import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import { useAuth } from '../../auth/contexts/AuthContext';
import { Card } from '../../../shared/components/layout/Card';
import { Button } from '../../../shared/components/layout/Button';
import { Badge } from '../../../shared/components/ui/badge';
import type {
  ParticipantProfileResponse,
  UserGroupListItemResponse,
  SignalViewModel,
  IdeaListItemResponse,
  ConcernListItemResponse,
} from '@/api/models';

type Tab = 'userGroups' | 'amplifiedSignals' | 'capturedSignals' | 'ideas' | 'concerns';

function useProfile(pseudonym: string | undefined) {
  const apiClient = useApiClient();
  return useQuery({
    queryKey: ['profiles', pseudonym],
    queryFn: async (): Promise<ParticipantProfileResponse | null> => {
      if (!pseudonym) return null;
      const response = await apiClient.profiles.getParticipantProfile({ pseudonym });
      return response.data ?? null;
    },
    enabled: !!pseudonym,
    staleTime: 5 * 60 * 1000,
  });
}

function useUserGroups(participantId: string | undefined) {
  const apiClient = useApiClient();
  return useQuery({
    queryKey: ['userGroups', 'forParticipant', participantId],
    queryFn: async (): Promise<UserGroupListItemResponse[]> => {
      if (!participantId) return [];
      const response = await apiClient.userGroups.getUserGroupsForParticipant({ participantId });
      return response.data ?? [];
    },
    enabled: !!participantId,
    staleTime: 5 * 60 * 1000,
  });
}

function useAmplifiedSignals(participantId: string | undefined) {
  const apiClient = useApiClient();
  return useQuery({
    queryKey: ['signals', 'amplifiedBy', participantId],
    queryFn: async (): Promise<SignalViewModel[]> => {
      if (!participantId) return [];
      const response = await apiClient.signals.getSignals({
        filterAmplifiedByParticipantId: participantId,
        pageNumber: 1,
        pageSize: 50,
      });
      return response.data?.signals?.items ?? [];
    },
    enabled: !!participantId,
    staleTime: 5 * 60 * 1000,
  });
}

function useCapturedSignals(participantId: string | undefined) {
  const apiClient = useApiClient();
  return useQuery({
    queryKey: ['signals', 'capturedBy', participantId],
    queryFn: async (): Promise<SignalViewModel[]> => {
      if (!participantId) return [];
      const response = await apiClient.signals.getSignals({
        filterCapturedByParticipantId: participantId,
        pageNumber: 1,
        pageSize: 50,
      });
      return response.data?.signals?.items ?? [];
    },
    enabled: !!participantId,
    staleTime: 5 * 60 * 1000,
  });
}

function useIdeas(authorId: string | undefined) {
  const apiClient = useApiClient();
  return useQuery({
    queryKey: ['ideas', 'byAuthor', authorId],
    queryFn: async (): Promise<IdeaListItemResponse[]> => {
      if (!authorId) return [];
      const response = await apiClient.ideas.getIdeas({
        filterAuthorId: authorId,
        pageNumber: 1,
        pageSize: 50,
      });
      return response.data?.ideas?.items ?? [];
    },
    enabled: !!authorId,
    staleTime: 5 * 60 * 1000,
  });
}

function useConcerns(raisedByParticipantId: string | undefined) {
  const apiClient = useApiClient();
  return useQuery({
    queryKey: ['concerns', 'raisedBy', raisedByParticipantId],
    queryFn: async (): Promise<ConcernListItemResponse[]> => {
      if (!raisedByParticipantId) return [];
      const response = await apiClient.concerns.getConcerns({
        filterRaisedByParticipantId: raisedByParticipantId,
        pageNumber: 1,
        pageSize: 50,
      });
      return response.data?.items ?? [];
    },
    enabled: !!raisedByParticipantId,
    staleTime: 5 * 60 * 1000,
  });
}

function TabButton({ label, active, onClick }: { label: string; active: boolean; onClick: () => void }) {
  return (
    <button
      onClick={onClick}
      className={`px-4 py-2 text-sm font-medium border-b-2 transition-colors ${
        active
          ? 'border-primary text-primary'
          : 'border-transparent text-muted-foreground hover:text-foreground hover:border-border'
      }`}
    >
      {label}
    </button>
  );
}

function SkeletonList() {
  return (
    <div className="space-y-3">
      {[1, 2, 3].map(i => (
        <div key={i} className="p-4 border rounded-lg animate-pulse">
          <div className="h-4 bg-muted rounded w-1/3 mb-2" />
          <div className="h-3 bg-muted rounded w-2/3" />
        </div>
      ))}
    </div>
  );
}

function UserGroupsTab({ participantId }: { participantId: string }) {
  const navigate = useNavigate();
  const { data: groups = [], isLoading } = useUserGroups(participantId);

  if (isLoading) return <SkeletonList />;
  if (groups.length === 0) return <p className="text-muted-foreground text-sm">No group memberships.</p>;

  return (
    <div className="space-y-3">
      {groups.map(g => (
        <div
          key={g.id}
          className="p-4 border rounded-lg hover:bg-muted/50 cursor-pointer transition-colors"
          onClick={() => navigate(`/user-groups/${g.id}`)}
        >
          <p className="font-medium text-foreground">{g.name}</p>
          {g.philosophy && (
            <p className="text-sm text-muted-foreground mt-1 line-clamp-2">{g.philosophy}</p>
          )}
        </div>
      ))}
    </div>
  );
}

function SignalsTab({ participantId, mode }: { participantId: string; mode: 'amplified' | 'captured' }) {
  const navigate = useNavigate();
  const amplifiedQuery = useAmplifiedSignals(mode === 'amplified' ? participantId : undefined);
  const capturedQuery = useCapturedSignals(mode === 'captured' ? participantId : undefined);
  const { data: signals = [], isLoading } = mode === 'amplified' ? amplifiedQuery : capturedQuery;

  if (isLoading) return <SkeletonList />;
  if (signals.length === 0) return <p className="text-muted-foreground text-sm">No signals.</p>;

  return (
    <div className="space-y-3">
      {signals.map(s => (
        <div
          key={s.id}
          className="p-4 border rounded-lg hover:bg-muted/50 cursor-pointer transition-colors"
          onClick={() => navigate(`/signals/${s.id}`)}
        >
          <p className="font-medium text-foreground">{s.title}</p>
          {s.tags && s.tags.length > 0 && (
            <div className="flex gap-1 flex-wrap mt-2">
              {s.tags.map((tag: string) => (
                <Badge key={tag} variant="secondary">{tag}</Badge>
              ))}
            </div>
          )}
        </div>
      ))}
    </div>
  );
}

function IdeasTab({ participantId }: { participantId: string }) {
  const navigate = useNavigate();
  const { data: ideas = [], isLoading } = useIdeas(participantId);

  if (isLoading) return <SkeletonList />;
  if (ideas.length === 0) return <p className="text-muted-foreground text-sm">No ideas.</p>;

  return (
    <div className="space-y-3">
      {ideas.map(idea => (
        <div
          key={idea.id}
          className="p-4 border rounded-lg hover:bg-muted/50 cursor-pointer transition-colors"
          onClick={() => navigate(`/ideas/${idea.id}`)}
        >
          <p className="font-medium text-foreground">{idea.title}</p>
          {idea.status && (
            <Badge variant="outline" className="mt-1">{idea.status}</Badge>
          )}
        </div>
      ))}
    </div>
  );
}

function ConcernsTab({ participantId }: { participantId: string }) {
  const navigate = useNavigate();
  const { data: concerns = [], isLoading } = useConcerns(participantId);

  if (isLoading) return <SkeletonList />;
  if (concerns.length === 0) return <p className="text-muted-foreground text-sm">No concerns raised.</p>;

  return (
    <div className="space-y-3">
      {concerns.map(c => (
        <div
          key={c.id}
          className="p-4 border rounded-lg hover:bg-muted/50 cursor-pointer transition-colors"
          onClick={() => navigate(`/concerns/${c.id}`)}
        >
          <p className="font-medium text-foreground">{c.name}</p>
          {c.tags && c.tags.length > 0 && (
            <div className="flex gap-1 flex-wrap mt-2">
              {c.tags.map((tag: string) => (
                <Badge key={tag} variant="secondary">{tag}</Badge>
              ))}
            </div>
          )}
        </div>
      ))}
    </div>
  );
}

export function ProfilePage() {
  const { pseudonym: pseudonymParam } = useParams<{ pseudonym?: string }>();
  const { user } = useAuth();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState<Tab>('userGroups');

  const pseudonym = pseudonymParam ?? user?.pseudonym;
  const { data: profile, isLoading, error } = useProfile(pseudonym);

  if (!pseudonym) {
    return (
      <div className="space-y-6">
        <Card
          title="Not signed in"
          content={<p className="text-muted-foreground">Sign in to view your profile.</p>}
          footer={<Button onClick={() => navigate('/login')} variant="primary">Sign in</Button>}
        />
      </div>
    );
  }

  if (error) {
    return (
      <div className="space-y-6">
        <h1 className="text-2xl font-bold text-foreground">{pseudonym}</h1>
        <Card
          title="Error loading profile"
          content={<p className="text-muted-foreground">There was an error loading this profile. Please try again.</p>}
        />
      </div>
    );
  }

  if (!isLoading && !profile) {
    return (
      <div className="space-y-6">
        <h1 className="text-2xl font-bold text-foreground">Profile not found</h1>
        <Card
          title="Not found"
          content={<p className="text-muted-foreground">No participant with that pseudonym exists.</p>}
        />
      </div>
    );
  }

  const participantId = profile?.userId;
  const isOwnProfile = !!user && !!participantId && user.id === participantId;

  const tabs: { key: Tab; label: string }[] = [
    { key: 'userGroups', label: 'User Groups' },
    { key: 'amplifiedSignals', label: 'Amplified Signals' },
    { key: 'capturedSignals', label: 'Captured Signals' },
    { key: 'ideas', label: 'Ideas' },
    { key: 'concerns', label: 'Concerns' },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="space-y-1">
        {isLoading ? (
          <div className="h-8 bg-muted rounded w-48 animate-pulse" />
        ) : (
          <h1 className="text-2xl font-bold text-foreground">{profile?.pseudonym}</h1>
        )}
        {profile?.joinedAt && (
          <p className="text-sm text-muted-foreground">
            Member since {new Date(profile.joinedAt).toLocaleDateString()}
          </p>
        )}
        {isOwnProfile && (
          <Badge variant="secondary">You</Badge>
        )}
      </div>

      {/* Tabs */}
      <div className="border-b border-border">
        <div className="flex gap-1 overflow-x-auto">
          {tabs.map(tab => (
            <TabButton
              key={tab.key}
              label={tab.label}
              active={activeTab === tab.key}
              onClick={() => setActiveTab(tab.key)}
            />
          ))}
        </div>
      </div>

      {/* Tab content */}
      <div>
        {participantId ? (
          <>
            {activeTab === 'userGroups' && <UserGroupsTab participantId={participantId} />}
            {activeTab === 'amplifiedSignals' && <SignalsTab participantId={participantId} mode="amplified" />}
            {activeTab === 'capturedSignals' && <SignalsTab participantId={participantId} mode="captured" />}
            {activeTab === 'ideas' && <IdeasTab participantId={participantId} />}
            {activeTab === 'concerns' && <ConcernsTab participantId={participantId} />}
          </>
        ) : (
          <SkeletonList />
        )}
      </div>
    </div>
  );
}
