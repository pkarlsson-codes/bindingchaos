import { Card } from '../../../shared/components/layout/Card';
import { Badge } from '../../../shared/components/ui/badge';
import { Icon } from '../../../shared/components/layout/Icon';
import type { AmendmentOpponentResponse } from '../../../api/models';

interface OpponentsListProps {
  opponents: AmendmentOpponentResponse[];
}

export function OpponentsList({ opponents }: OpponentsListProps) {
  if (opponents.length === 0) {
    return (
      <Card
        title="Opponents"
        content={
          <div className="text-center py-8">
            <Icon name="users" className="mx-auto h-12 w-12 text-muted-foreground mb-4" />
            <p className="text-muted-foreground">No opponents yet</p>
            <p className="text-sm text-muted-foreground mt-1">
              This amendment has unanimous support so far!
            </p>
          </div>
        }
      />
    );
  }

  return (
    <Card
      title={`Opponents (${opponents.length})`}
      content={
        <div className="space-y-4">
          {opponents.map((opponent) => (
            <div
              key={opponent.id}
              className="border rounded-lg p-4 hover:bg-muted/50 transition-colors"
            >
              <div className="flex items-start justify-between mb-3">
                <div className="flex items-center gap-2">
                  <div className="w-8 h-8 bg-red-100 rounded-full flex items-center justify-center">
                    <Icon name="thumbs-down" className="w-4 h-4 text-red-600" />
                  </div>
                  <div>
                    <h4 className="font-medium text-foreground">{opponent.pseudonym}</h4>
                    <p className="text-sm text-muted-foreground">
                      Opposed {opponent.opposedAt ? new Date(opponent.opposedAt).toLocaleDateString() : 'Unknown'} at{' '}
                      {opponent.opposedAt ? new Date(opponent.opposedAt).toLocaleTimeString() : 'Unknown'}
                    </p>
                  </div>
                </div>
                <Badge variant="destructive" className="bg-red-100 text-red-800">
                  Opponent
                </Badge>
              </div>
              
              <div className="bg-red-50 border-l-4 border-red-200 p-3 rounded-r">
                <p className="text-sm text-foreground">{opponent.reason}</p>
              </div>
            </div>
          ))}
        </div>
      }
    />
  );
}
