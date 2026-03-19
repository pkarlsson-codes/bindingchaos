import { useMemo } from 'react';
import { formatDistanceToNow } from 'date-fns';
import { Card } from '../../../shared/components/layout/Card';
import { Icon } from '../../../shared/components/layout/Icon';
import type { TrendPointResponse, AmplificationViewModel } from '../../../api/models';

interface AmplificationsTimelineProps {
  dataPoints: TrendPointResponse[];
  amplifications?: AmplificationViewModel[];
  title?: string;
}

interface TimelineEvent {
  id: string;
  type: 'amplify' | 'attenuate';
  pseudonym: string;
  timestamp: Date;
  relativeTime: string;
}

export function AmplificationsTimeline({ dataPoints, amplifications, title = "Active Amplifications" }: AmplificationsTimelineProps) {
  const timelineEvents = useMemo(() => {
    if (!amplifications || amplifications.length === 0) {
      return [];
    }

    // Only show active amplifications (which are the ones in the amplifications array)
    return amplifications
      .filter(amp => amp.amplifiedAt)
      .sort((a, b) => new Date(b.amplifiedAt!).getTime() - new Date(a.amplifiedAt!).getTime())
      .map((amp, index) => ({
        id: `amplify-${index}`,
        type: 'amplify' as const,
        pseudonym: amp.amplifierPseudonym || 'Anonymous',
        timestamp: new Date(amp.amplifiedAt!),
        relativeTime: formatDistanceToNow(new Date(amp.amplifiedAt!), { addSuffix: true })
      }));
  }, [amplifications]);

  if (timelineEvents.length === 0) {
    return (
      <Card
        title={title}
        content={
          <div className="text-center py-8">
            <div className="text-4xl mb-2">
              <Icon name="trending-up" size={48} className="text-muted-foreground" />
            </div>
            <p>No amplification activity yet</p>
            <p className="text-sm">Be the first to amplify this signal</p>
          </div>
        }
      />
    );
  }

  return (
    <Card
      title={title}
      content={
        <div className="space-y-4">
          {timelineEvents.map((event) => (
            <div key={event.id} className="flex items-start gap-3">
              {/* Event Icon */}
              <div 
                className="flex-shrink-0 mt-1"
                title={`${event.type === 'amplify' ? 'Amplified' : 'Attenuated'} this signal`}
              >
                {event.type === 'amplify' ? (
                  <Icon 
                    name="trending-up" 
                    size={16} 
                    className="text-green-600" 
                  />
                ) : (
                  <Icon 
                    name="trending-down" 
                    size={16} 
                    className="text-red-600" 
                  />
                )}
              </div>

              {/* Event Content */}
              <div className="flex-1 min-w-0">
                <div className="flex items-center justify-between">
                  <span className="font-medium text-sm">
                    {event.pseudonym}
                  </span>
                  <span className="text-xs text-muted-foreground">
                    {event.relativeTime}
                  </span>
                </div>
              </div>
            </div>
          ))}
        </div>
      }
    />
  );
}
