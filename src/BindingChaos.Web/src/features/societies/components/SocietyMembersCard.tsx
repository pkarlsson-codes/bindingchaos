import { Card, CardContent, CardHeader, CardTitle } from '../../../shared/components/ui/card';
import { useSocietyMembers } from '../hooks/useSocietyMembers';

interface SocietyMembersCardProps {
  societyId: string;
}

export function SocietyMembersCard({ societyId }: SocietyMembersCardProps) {
  const { data, isLoading, error } = useSocietyMembers(societyId);

  return (
    <Card>
      <CardHeader>
        <CardTitle>Members</CardTitle>
      </CardHeader>
      <CardContent>
        {isLoading && (
          <div className="space-y-2">
            {[1, 2, 3].map((i) => (
              <div key={i} className="animate-pulse h-4 bg-muted rounded w-1/2" />
            ))}
          </div>
        )}
        {error && (
          <p className="text-sm text-destructive">Could not load members.</p>
        )}
        {data && data.items.length === 0 && (
          <p className="text-sm text-muted-foreground">No members yet.</p>
        )}
        {data && data.items.length > 0 && (
          <ul className="space-y-2">
            {data.items.map((member) => (
              <li key={member.membershipId} className="text-sm">
                <span className="font-medium">{member.pseudonym}</span>
                {member.joinedAt && (
                  <span className="ml-2 text-muted-foreground text-xs">
                    joined {new Date(member.joinedAt).toLocaleDateString()}
                  </span>
                )}
              </li>
            ))}
          </ul>
        )}
        {data && data.totalCount > data.items.length && (
          <p className="text-sm text-muted-foreground mt-3">
            Showing {data.items.length} of {data.totalCount} members.
          </p>
        )}
      </CardContent>
    </Card>
  );
}
