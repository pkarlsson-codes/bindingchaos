import { useQuery } from '@tanstack/react-query';
import { useParams, useNavigate } from 'react-router-dom';
import { Card, CardContent, CardHeader, CardTitle } from '../../../shared/components/ui/card';
import { Badge } from '../../../shared/components/ui/badge';
import { Button } from '../../../shared/components/ui/button';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import type { AmendmentsListItemResponse } from '../../../api/models';

export function AmendmentsCard() {
  const { ideaId } = useParams<{ ideaId: string }>();
  const { amendments } = useApiClient();
  const navigate = useNavigate();

  const { data: amendmentsResponse, isLoading, error } = useQuery({
    queryKey: ['amendments', ideaId],
    queryFn: () => amendments.getAmendments({
      ideaId: ideaId!,
      filterStatusFilter: 'all'
    }),
    enabled: !!ideaId,
  });

  const amendmentList = amendmentsResponse?.data?.items || [];

  if (isLoading) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Amendments</CardTitle>
        </CardHeader>
        <CardContent>
          <p>Loading amendments...</p>
        </CardContent>
      </Card>
    );
  }

  if (error) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Amendments</CardTitle>
        </CardHeader>
        <CardContent>
          <p className="text-red-500">Error loading amendments</p>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>Amendments ({amendmentList.length})</CardTitle>
      </CardHeader>
      <CardContent>
        {amendmentList.length === 0 ? (
          <p className="text-muted-foreground">No amendments yet.</p>
        ) : (
          <div className="space-y-4">
            {amendmentList.map((amendment: AmendmentsListItemResponse) => (
              <div key={amendment.id} className="border rounded-lg p-4">
                <div className="flex justify-between items-start mb-2">
                  <h4 className="font-medium">{amendment.amendmentTitle}</h4>
                  <Badge variant="outline">{amendment.status}</Badge>
                </div>
                <p className="text-sm text-muted-foreground mb-3">
                  {amendment.amendmentDescription}
                </p>
                <div className="flex justify-between items-center">
                  <span className="text-xs text-muted-foreground">
                    by {amendment.authorPseudonym}
                  </span>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => navigate(`/ideas/${ideaId}/amendments/${amendment.id}`)}
                  >
                    View Details
                  </Button>
                </div>
              </div>
            ))}
          </div>
        )}
      </CardContent>
    </Card>
  );
} 