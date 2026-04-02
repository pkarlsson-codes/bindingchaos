import { Card, CardContent, CardHeader, CardTitle } from '../../../shared/components/ui/card';
import { useMySocietyInviteLinks } from '../hooks/useSocietyMutations';

interface SocietyInviteLinksCardProps {
  societyId: string;
}

export function SocietyInviteLinksCard({ societyId }: SocietyInviteLinksCardProps) {
  const { data: links, isLoading } = useMySocietyInviteLinks(societyId);

  if (isLoading) return null;
  if (!links || links.length === 0) return null;

  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-base">Your invite links</CardTitle>
      </CardHeader>
      <CardContent>
        <ul className="space-y-3">
          {links.map((link) => {
            const url = `${window.location.origin}/societies/${societyId}/invitations/${link.token}`;
            return (
              <li key={link.id} className="space-y-1">
                <div className="flex items-center gap-2">
                  <input
                    readOnly
                    value={url}
                    className="flex-1 text-sm bg-muted rounded px-2 py-1 font-mono truncate"
                    onFocus={(e) => e.target.select()}
                  />
                  <button
                    type="button"
                    className="text-sm text-muted-foreground hover:text-foreground shrink-0"
                    onClick={() => navigator.clipboard.writeText(url)}
                  >
                    Copy
                  </button>
                </div>
                {link.note && (
                  <p className="text-xs text-muted-foreground">{link.note}</p>
                )}
                <p className="text-xs text-muted-foreground">
                  Created {link.createdAt ? new Date(link.createdAt).toLocaleDateString() : '—'}
                  {link.isRevoked && <span className="ml-2 text-destructive">Revoked</span>}
                </p>
              </li>
            );
          })}
        </ul>
      </CardContent>
    </Card>
  );
}
