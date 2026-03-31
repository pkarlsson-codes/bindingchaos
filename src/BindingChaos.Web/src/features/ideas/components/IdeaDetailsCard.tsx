import { Card, CardContent, CardHeader, CardTitle } from '../../../shared/components/ui/card';
import { Badge } from '../../../shared/components/ui/badge';
import { Button } from '../../../shared/components/ui/button';
import { useNavigate } from 'react-router-dom';
import type { IdeaListItemResponse } from '../../../api/models';

interface IdeaDetailsCardProps {
  idea: IdeaListItemResponse;
}

export function IdeaDetailsCard({ idea }: IdeaDetailsCardProps) {
  const navigate = useNavigate();

  const handleSourceSignalClick = (signalId: string) => {
    navigate(`/signals/${signalId}`);
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-xl font-semibold">{idea.title}</CardTitle>
        <div className="flex gap-2">
          {idea.tags?.map((tag) => (
            <Badge key={tag} variant="secondary">
              {tag}
            </Badge>
          ))}
        </div>
      </CardHeader>
      <CardContent>
        <div className="prose prose-sm max-w-none mb-6">
          <p className="text-muted-foreground">{idea.description}</p>
        </div>
        
        <div className="flex justify-between items-center">
          <div className="text-sm text-muted-foreground">
            {idea.createdAt && new Date(idea.createdAt).toLocaleDateString()}
          </div>
          
          {(idea as any).sourceSignalId && (
            <Button
              variant="outline"
              size="sm"
              onClick={() => handleSourceSignalClick((idea as any).sourceSignalId)}
            >
              View Source Signal
            </Button>
          )}
        </div>
      </CardContent>
    </Card>
  );
} 