import { useNavigate, useParams } from 'react-router-dom';
import { useAuth } from '../../auth/contexts/AuthContext';
import { useSociety } from '../hooks/useSociety';
import { useJoinSociety } from '../hooks/useSocietyMutations';
import { Button } from '../../../shared/components/ui/button';
import { Badge } from '../../../shared/components/ui/badge';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../../../shared/components/ui/card';
import { LoadingSpinner } from '../../../shared/components/feedback/LoadingSpinner';
import { toast } from '../../../shared/components/ui/toast';
import { API_CONFIG } from '../../../config/api';

function InvalidInvitePage() {
  return (
    <div className="min-h-[60vh] flex items-center justify-center">
      <Card className="w-full max-w-md text-center">
        <CardHeader>
          <CardTitle>Invalid Invite Link</CardTitle>
          <CardDescription>
            This invite link is invalid or has been revoked.
          </CardDescription>
        </CardHeader>
      </Card>
    </div>
  );
}

function UnauthenticatedInviteLanding({ societyId, token, societyName }: { societyId: string; token: string; societyName?: string }) {
  const returnUrl = `/societies/${societyId}/invitations/${token}`;
  const loginUrl = `${API_CONFIG.baseUrl}/auth/login?returnUrl=${encodeURIComponent(returnUrl)}`;
  const registerUrl = `${API_CONFIG.baseUrl}/auth/register?returnUrl=${encodeURIComponent(returnUrl)}`;

  return (
    <div className="min-h-[60vh] flex items-center justify-center">
      <Card className="w-full max-w-md">
        <CardHeader className="text-center space-y-1">
          <CardTitle className="text-2xl">
            {societyName ? `You've been invited to join ${societyName}` : "You've been invited to join a society"}
          </CardTitle>
          <CardDescription>
            Sign in or create an account to join.
          </CardDescription>
        </CardHeader>
        <CardContent className="flex flex-col gap-3">
          <a href={loginUrl}>
            <Button className="w-full">Sign In</Button>
          </a>
          <a href={registerUrl}>
            <Button variant="outline" className="w-full">Register</Button>
          </a>
        </CardContent>
      </Card>
    </div>
  );
}

export function SocietyInvitePage() {
  const { societyId, token } = useParams<{ societyId: string; token: string }>();
  const navigate = useNavigate();
  const { user, isLoading: isAuthLoading } = useAuth();
  const { data: society, isLoading: isSocietyLoading, isError: isSocietyError } = useSociety(societyId);
  const { mutate: joinSociety, isPending: isJoining } = useJoinSociety(societyId ?? '');

  if (!societyId || !token) return <InvalidInvitePage />;

  if (isAuthLoading || isSocietyLoading) {
    return (
      <div className="min-h-[60vh] flex items-center justify-center">
        <LoadingSpinner />
      </div>
    );
  }

  if (isSocietyError || !society) return <InvalidInvitePage />;

  if (!user) {
    return (
      <UnauthenticatedInviteLanding
        societyId={societyId}
        token={token}
        societyName={society.name}
      />
    );
  }

  const handleJoin = () => {
    const contractId = society.currentSocialContractId;
    if (!contractId) {
      toast.error("Cannot join society. Social contract not yet available.");
      return;
    }
    joinSociety({ socialContractId: contractId, inviteToken: token }, {
      onSuccess: () => {
        toast.success(`Joined ${society.name}`);
        navigate(`/societies/${societyId}`);
      },
      onError: () => toast.error("Failed to join society"),
    });
  };

  return (
    <div className="min-h-[60vh] flex items-center justify-center">
      <Card className="w-full max-w-lg">
        <CardHeader>
          <CardTitle className="text-2xl">{society.name}</CardTitle>
          {society.tags && society.tags.length > 0 && (
            <div className="flex flex-wrap gap-1 pt-1">
              {society.tags.map((tag) => (
                <Badge key={tag} variant="secondary">{tag}</Badge>
              ))}
            </div>
          )}
          <CardDescription className="pt-2">{society.description}</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <p className="text-sm text-muted-foreground">
            {society.activeMemberCount ?? 0} members
          </p>
          <Button className="w-full" onClick={handleJoin} disabled={isJoining}>
            {isJoining ? 'Joining...' : `Join ${society.name}`}
          </Button>
        </CardContent>
      </Card>
    </div>
  );
}
