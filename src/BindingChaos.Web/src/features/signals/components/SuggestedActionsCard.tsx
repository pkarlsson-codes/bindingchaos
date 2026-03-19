import { formatDistanceToNow } from 'date-fns';
import { Card } from '../../../shared/components/layout/Card';
import { Button } from '../../../shared/components/layout/Button';
import { Icon, type IconName } from '../../../shared/components/layout/Icon';
import { AuthRequiredButton } from '../../auth';
import type { SuggestedActionViewModel } from '../../../api/models';

const ACTION_LABELS: Record<string, string> = {
  MakeACall: 'Make a call',
  VisitAWebpage: 'Visit a webpage',
};

const ACTION_ICONS: Record<string, IconName> = {
  MakeACall: 'phone',
  VisitAWebpage: 'link',
};

interface SuggestedActionsCardProps {
  suggestedActions: SuggestedActionViewModel[];
  onSuggestAction: () => void;
}

function ActionContent({ action }: { action: SuggestedActionViewModel }) {
  const type = action.actionType ?? '';

  if (type === 'MakeACall' && action.phoneNumber) {
    return (
      <a
        href={`tel:${action.phoneNumber}`}
        className="text-sm font-medium text-primary hover:underline"
      >
        {action.phoneNumber}
      </a>
    );
  }

  if (type === 'VisitAWebpage' && action.url) {
    return (
      <a
        href={action.url}
        target="_blank"
        rel="noopener noreferrer"
        className="text-sm font-medium text-primary hover:underline break-all"
      >
        {action.url}
      </a>
    );
  }

  return null;
}

export function SuggestedActionsCard({ suggestedActions, onSuggestAction }: SuggestedActionsCardProps) {
  const sorted = [...suggestedActions].sort(
    (a, b) => new Date(b.suggestedAt ?? 0).getTime() - new Date(a.suggestedAt ?? 0).getTime()
  );

  return (
    <Card
      title="Suggested actions"
      headerAction={
        <AuthRequiredButton action="suggest an action">
          <Button
            onClick={onSuggestAction}
            variant="outline"
            size="sm"
            icon="plus"
          >
            Suggest action
          </Button>
        </AuthRequiredButton>
      }
      content={
        sorted.length === 0 ? (
          <div className="text-center py-8 space-y-2">
            <Icon name="list-checks" size={40} className="mx-auto text-muted-foreground" />
            <p className="text-sm text-muted-foreground">No actions suggested yet</p>
            <p className="text-xs text-muted-foreground">Be the first to suggest how others can help</p>
          </div>
        ) : (
          <ul className="space-y-4">
            {sorted.map((action) => {
              const type = action.actionType ?? '';
              const icon: IconName = ACTION_ICONS[type] ?? 'check-circle';
              const label = ACTION_LABELS[type] ?? type;

              return (
                <li key={action.id} className="flex items-start gap-3">
                  <div className="flex-shrink-0 mt-0.5">
                    <Icon name={icon} size={16} className="text-primary" />
                  </div>
                  <div className="flex-1 min-w-0 space-y-1">
                    <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide">
                      {label}
                    </p>
                    <ActionContent action={action} />
                    {action.details && (
                      <p className="text-sm text-muted-foreground">{action.details}</p>
                    )}
                    <p className="text-xs text-muted-foreground">
                      {action.suggestedByPseudonym}
                      {action.suggestedAt && (
                        <> &middot; {formatDistanceToNow(new Date(action.suggestedAt), { addSuffix: true })}</>
                      )}
                    </p>
                  </div>
                </li>
              );
            })}
          </ul>
        )
      }
    />
  );
}
