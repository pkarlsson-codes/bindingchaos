import { Card } from '../../../shared/components/layout/Card';
import { Icon } from '../../../shared/components/layout/Icon';
import type { AmplificationViewModel } from '../../../api/models';

interface AmplificationsCardProps {
  amplifications: AmplificationViewModel[];
}

export function AmplificationsCard({ amplifications }: AmplificationsCardProps) {
  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  return (
    <Card
      title="Amplifications"

      content={
        amplifications.length === 0 ? (
          <div className="text-center py-8">
            <div className="text-4xl mb-2">
              <Icon name="trending-up" size={48} className="text-muted-foreground" />
            </div>
            <p>No amplifications yet</p>
            <p className="text-sm">Be the first to amplify this signal</p>
          </div>
        ) : (
          <div className="space-y-4">
            {amplifications.map((amplification, index) => (
              <div key={amplification.id || `amplification-${index}`} className="border-b pb-4 last:border-b-0">
                <div className="flex items-start justify-between mb-2">
                  <div className="flex items-center gap-2">
                    <span className="font-medium">
                      {amplification.amplifierPseudonym || 'Anonymous'}
                    </span>
                    <Icon name="trending-up" size={16} className="text-primary" />
                  </div>
                  <span className="text-sm">
                    {formatDate(amplification.amplifiedAt || '')}
                  </span>
                </div>
                {amplification.comment && (
                  <p className="text-sm mt-2">{amplification.comment}</p>
                )}
              </div>
            ))}
          </div>
        )
      }
    />
  );
} 