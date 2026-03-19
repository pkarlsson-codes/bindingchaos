import { Card, CardContent, CardHeader, CardTitle } from '../../../shared/components/ui/card';
import { Badge } from '../../../shared/components/ui/badge';
import { Button } from '../../../shared/components/ui/button';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../auth/contexts/AuthContext';
import { useState } from 'react';
import type { IdeaListItemResponse } from '../../../api/models';

interface IdeaCardProps {
  idea: IdeaListItemResponse;
}

export function IdeaCard({ idea }: IdeaCardProps) {
  const navigate = useNavigate();
  const { user } = useAuth();
  const [isProposeModalOpen, setIsProposeModalOpen] = useState(false);

  const handleIdeaClick = () => {
    if (idea.id) {
      navigate(`/ideas/${idea.id}`);
    }
  };

  const handleSourceSignalClick = (e: React.MouseEvent) => {
    e.stopPropagation();
    if ((idea as any).sourceSignalId) {
      navigate(`/signals/${(idea as any).sourceSignalId}`);
    }
  };

  return (
    <Card className="hover:shadow-md transition-shadow cursor-pointer" onClick={handleIdeaClick}>
      <CardHeader>
        <CardTitle className="text-lg font-semibold">{idea.title}</CardTitle>
        <div className="flex gap-2">
          {idea.tags?.map((tag) => (
            <Badge key={tag} variant="secondary">
              {tag}
            </Badge>
          ))}
        </div>
      </CardHeader>
      <CardContent>
        <p className="text-muted-foreground mb-4">{idea.body}</p>
        <div className="flex justify-between items-center">
          <div className="text-sm text-muted-foreground">
            {idea.createdAt && new Date(idea.createdAt).toLocaleDateString()}
          </div>
          <div className="flex gap-2">
            {(idea as any).sourceSignalId && (
              <Button
                variant="outline"
                size="sm"
                onClick={handleSourceSignalClick}
              >
                View Source Signal
              </Button>
            )}
            {user && (
              <Button
                variant="outline"
                size="sm"
                onClick={(e) => {
                  e.stopPropagation();
                  setIsProposeModalOpen(true);
                }}
              >
                Propose Amendment
              </Button>
            )}
          </div>
        </div>
      </CardContent>
      
      {/* Note: ProposeIdeaFromSignalModal requires a signal prop, not ideaId */}
      {/* This would need to be implemented differently for ideas */}
    </Card>
  );
} 