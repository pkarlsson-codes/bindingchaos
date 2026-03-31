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

  return (
    <Card className="hover:shadow-md transition-shadow cursor-pointer" onClick={handleIdeaClick}>
      <CardHeader>
        <CardTitle className="text-lg font-semibold">{idea.title}</CardTitle>
      </CardHeader>
      <CardContent>
        <p className="text-muted-foreground mb-4">{idea.description}</p>
        <div className="flex justify-between items-center">
          <div className="text-sm text-muted-foreground">
            {idea.createdAt && new Date(idea.createdAt).toLocaleDateString()}
          </div>
        </div>
      </CardContent>
    </Card>
  );
} 