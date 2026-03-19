import { Card } from '../../../shared/components/layout/Card';
import { Badge } from '../../../shared/components/ui/badge';
import { Icon } from '../../../shared/components/layout/Icon';
import type { AmendmentSupporterResponse } from '../../../api/models';

interface SupportersListProps {
  supporters: AmendmentSupporterResponse[];
}

export function SupportersList({ supporters }: SupportersListProps) {
  if (supporters.length === 0) {
    return (
      <Card
        title="Supporters"
        content={
          <div className="text-center py-8">
            <Icon name="users" className="mx-auto h-12 w-12 text-muted-foreground mb-4" />
            <p className="text-muted-foreground">No supporters yet</p>
            <p className="text-sm text-muted-foreground mt-1">
              Be the first to support this amendment!
            </p>
          </div>
        }
      />
    );
  }

  return (
    <Card
      title={`Supporters (${supporters.length})`}
      content={
        <div className="space-y-4">
          {supporters.map((supporter) => (
            <div
              key={supporter.id}
              className="border rounded-lg p-4 hover:bg-muted/50 transition-colors"
            >
              <div className="flex items-start justify-between mb-3">
                <div className="flex items-center gap-2">
                  <div className="w-8 h-8 bg-green-100 rounded-full flex items-center justify-center">
                    <Icon name="thumbs-up" className="w-4 h-4 text-green-600" />
                  </div>
                  <div>
                    <h4 className="font-medium text-foreground">{supporter.pseudonym}</h4>
                    <p className="text-sm text-muted-foreground">
                      Supported {supporter.supportedAt ? new Date(supporter.supportedAt).toLocaleDateString() : 'Unknown'} at{' '}
                      {supporter.supportedAt ? new Date(supporter.supportedAt).toLocaleTimeString() : 'Unknown'}
                    </p>
                  </div>
                </div>
                <Badge variant="default" className="bg-green-100 text-green-800">
                  Supporter
                </Badge>
              </div>
              
              <div className="bg-green-50 border-l-4 border-green-200 p-3 rounded-r">
                <p className="text-sm text-foreground">{supporter.reason}</p>
              </div>
            </div>
          ))}
        </div>
      }
    />
  );
}
