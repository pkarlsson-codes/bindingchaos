import { useParams } from 'react-router-dom';
import { useAuth } from '@/features/auth/contexts/AuthContext';
import { useResolveInviteLink } from '../hooks/useResolveInviteLink';
import { Button } from '@/shared/components/layout/Button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/shared/components/ui/card';
import { LoadingSpinner } from '@/shared/components/feedback/LoadingSpinner';
import { API_CONFIG } from '@/config/api';

function InvalidLinkPage() {
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

function UnauthenticatedInviteLanding({ token }: { token: string }) {
  const returnUrl = `/invite/${token}`;
  const loginUrl = `${API_CONFIG.baseUrl}/auth/login?returnUrl=${encodeURIComponent(returnUrl)}`;
  const registerUrl = `${API_CONFIG.baseUrl}/auth/register?returnUrl=${encodeURIComponent(returnUrl)}`;

  return (
    <div className="min-h-[60vh] flex items-center justify-center">
      <Card className="w-full max-w-md">
        <CardHeader className="text-center space-y-1">
          <CardTitle className="text-2xl">You've been invited to BindingChaos</CardTitle>
          <CardDescription>
            Sign in or create an account to connect with the person who invited you.
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

function InviterProfile({ inviterUserId }: { inviterUserId: string }) {
  return (
    <div className="max-w-lg mx-auto">
      <Card>
        <CardHeader>
          <CardTitle>Invitation</CardTitle>
          <CardDescription>
            You were invited by a BindingChaos participant.
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <p className="text-sm text-muted-foreground">
            Participant ID: <span className="font-mono text-foreground">{inviterUserId}</span>
          </p>
          <Button className="w-full">Extend Trust</Button>
        </CardContent>
      </Card>
    </div>
  );
}

export function InvitePage() {
  const { token } = useParams<{ token: string }>();
  const { user, isLoading: isAuthLoading } = useAuth();
  const { data: resolved, isLoading: isResolving, isError } = useResolveInviteLink(token ?? '');

  if (!token) return <InvalidLinkPage />;
  if (isAuthLoading || isResolving) {
    return (
      <div className="min-h-[60vh] flex items-center justify-center">
        <LoadingSpinner />
      </div>
    );
  }
  if (isError || !resolved) return <InvalidLinkPage />;

  if (!user) {
    return <UnauthenticatedInviteLanding token={token} />;
  }

  return <InviterProfile inviterUserId={resolved.inviterUserId} />;
}
