import { useNavigate } from 'react-router-dom';
import { Card, CardContent, CardHeader, CardTitle } from '../../../shared/components/ui/card';
import { Badge } from '../../../shared/components/ui/badge';
import { Button } from '../../../shared/components/ui/button';
import type { SocietyListItemResponse } from '../../../api/models';

interface SocietyCardProps {
  society: SocietyListItemResponse;
  onJoin?: (societyId: string) => void;
  isJoining?: boolean;
}

export function SocietyCard({ society, onJoin, isJoining }: SocietyCardProps) {
  const navigate = useNavigate();

  const handleClick = () => {
    if (society.id) {
      navigate(`/societies/${society.id}`);
    }
  };

  const handleJoin = (e: React.MouseEvent) => {
    e.stopPropagation();
    if (society.id && onJoin) {
      onJoin(society.id);
    }
  };

  return (
    <Card className="hover:shadow-md transition-shadow cursor-pointer" onClick={handleClick}>
      <CardHeader>
        <div className="flex justify-between items-start gap-4">
          <CardTitle className="text-lg font-semibold">{society.name}</CardTitle>
          <span className="text-sm text-muted-foreground shrink-0">
            {society.activeMemberCount ?? 0} members
          </span>
        </div>
        {society.tags && society.tags.length > 0 && (
          <div className="flex flex-wrap gap-1">
            {society.tags.map((tag) => (
              <Badge key={tag} variant="secondary">{tag}</Badge>
            ))}
          </div>
        )}
      </CardHeader>
      <CardContent>
        <p className="text-muted-foreground mb-4 line-clamp-3">{society.description}</p>
        <div className="flex justify-between items-center">
          <span className="text-sm text-muted-foreground">
            {society.createdAt && new Date(society.createdAt).toLocaleDateString()}
          </span>
          {onJoin && (
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
      </CardContent>
    </Card>
  );
}
