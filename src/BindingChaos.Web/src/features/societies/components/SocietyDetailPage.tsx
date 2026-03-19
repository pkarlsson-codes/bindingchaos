import { useNavigate, useParams } from 'react-router-dom';
import { Button } from '../../../shared/components/ui/button';
import { Badge } from '../../../shared/components/ui/badge';
import { Card, CardContent, CardHeader, CardTitle } from '../../../shared/components/ui/card';
import { useSociety } from '../hooks/useSociety';
import { SocietyMembersCard } from './SocietyMembersCard';
import { useJoinSociety, useLeaveSociety, useMySocietyIds } from '../hooks/useSocietyMutations';
import { useAuth } from '../../auth/contexts/AuthContext';
import { toast } from '../../../shared/components/ui/toast';

export function SocietyDetailPage() {
  const { societyId } = useParams<{ societyId: string }>();
  const navigate = useNavigate();
  const { user } = useAuth();
  const { data: society, isLoading, error } = useSociety(societyId);
  const { mutate: joinSociety, isPending: isJoining } = useJoinSociety(societyId ?? '');
  const { mutate: leaveSociety, isPending: isLeaving } = useLeaveSociety(societyId ?? '');
  const { data: mySocietyIds } = useMySocietyIds();
  const isMember = societyId ? (mySocietyIds ?? []).includes(societyId) : false;

  if (isLoading) {
    return (
      <div className="space-y-6">
        <Button variant="ghost" size="sm" onClick={() => navigate('/societies')}>
          ← Back to Societies
        </Button>
        <Card>
          <CardContent className="p-6">
            <div className="animate-pulse space-y-4">
              <div className="h-8 bg-muted rounded w-1/3" />
              <div className="h-4 bg-muted rounded w-2/3" />
              <div className="h-4 bg-muted rounded w-1/2" />
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }

  if (error || !society) {
    return (
      <div className="space-y-6">
        <Button variant="ghost" size="sm" onClick={() => navigate('/societies')}>
          ← Back to Societies
        </Button>
        <Card>
          <CardContent className="p-6">
            <p className="text-muted-foreground">Society not found or could not be loaded.</p>
          </CardContent>
        </Card>
      </div>
    );
  }

  const handleJoin = () => {
    const contractId = society.currentSocialContractId;
    if (!contractId) {
      toast({ title: 'Cannot join', description: 'Social contract not yet available.' });
      return;
    }
    joinSociety(contractId, {
      onSuccess: () => toast({ title: 'Joined society' }),
      onError: () => toast({ title: 'Failed to join society' }),
    });
  };

  const handleLeave = () => {
    leaveSociety(undefined, {
      onSuccess: () => toast({ title: 'Left society' }),
      onError: () => toast({ title: 'Failed to leave society' }),
    });
  };

  return (
    <div className="space-y-6">
      <Button variant="ghost" size="sm" onClick={() => navigate('/societies')}>
        ← Back to Societies
      </Button>

      {/* Society header */}
      <Card>
        <CardHeader>
          <div className="flex justify-between items-start gap-4">
            <div className="space-y-2">
              <CardTitle className="text-2xl">{society.name}</CardTitle>
              {society.tags && society.tags.length > 0 && (
                <div className="flex flex-wrap gap-1">
                  {society.tags.map((tag) => (
                    <Badge key={tag} variant="secondary">{tag}</Badge>
                  ))}
                </div>
              )}
            </div>
            <div className="flex flex-col items-end gap-2 shrink-0">
              <span className="text-sm text-muted-foreground">
                {society.activeMemberCount ?? 0} members
              </span>
              {user && (
                <div className="flex gap-2">
                  {isMember ? (
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={handleLeave}
                      disabled={isLeaving}
                    >
                      {isLeaving ? 'Leaving...' : 'Leave'}
                    </Button>
                  ) : (
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={handleJoin}
                      disabled={isJoining}
                    >
                      {isJoining ? 'Joining...' : 'Join'}
                    </Button>
                  )}
                </div>
              )}
            </div>
          </div>
        </CardHeader>
        <CardContent>
          <p className="text-muted-foreground">{society.description}</p>
          {society.createdAt && (
            <p className="text-sm text-muted-foreground mt-4">
              Created {new Date(society.createdAt).toLocaleDateString()}
            </p>
          )}
        </CardContent>
      </Card>

      {/* Members */}
      {societyId && <SocietyMembersCard societyId={societyId} />}
    </div>
  );
}
