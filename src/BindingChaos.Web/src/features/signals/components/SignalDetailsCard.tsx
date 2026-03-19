import type { SignalDetailViewModel } from '../../../api/models';
import { Button } from '../../../shared/components/layout/Button';
import { Card } from '../../../shared/components/layout/Card';
import { TagBag } from '../../tags/components/TagBag';
import { Icon } from '../../../shared/components/layout/Icon';
import { SafeHtmlRenderer } from '../../../shared/components/feedback/SafeHtmlRenderer';
import { AmplifyButtonWithErrorBoundary } from './amplify-button';
import { AuthRequiredButton } from '../../auth';
import { AttachmentGallery } from './AttachmentGallery';

interface SignalDetailsCardProps {
  signalDetail: SignalDetailViewModel;
  onProposeIdea?: () => void;
}

export function SignalDetailsCard({ signalDetail, onProposeIdea }: SignalDetailsCardProps) {
  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  return (
    <Card
      title={signalDetail.title}
      description={`by ${signalDetail.authorPseudonym || 'Anonymous'} • ${formatDate(signalDetail.createdAt || '')}`}
      headerAction={
        <AmplifyButtonWithErrorBoundary
          signalId={signalDetail.id}
          amplifyCount={signalDetail.amplifyCount || 0}
          isAmplifiedByCurrentUser={signalDetail.isAmplifiedByCurrentUser || false}
          isOriginator={signalDetail.isOriginator || false}
          size="sm"
          className="shadow-sm"
        />
      }
      content={
        <>
          <SafeHtmlRenderer 
            html={signalDetail.description || ''} 
            className="mb-4"
          />

          <TagBag 
            tags={signalDetail.tags || []} 
          />

          {signalDetail.attachments && signalDetail.attachments.length > 0 && (
            <AttachmentGallery
              attachments={signalDetail.attachments.map(a => ({
                documentId: a.documentId || '',
                caption: a.caption ?? undefined,
                thumbnailUrl: a.thumbnailUrl,
                displayUrl: a.displayUrl,
              }))}
              className="mt-4"
            />
          )}
        </>
      }
      footer={
        <div className="flex items-center justify-between gap-4 w-full">
          <div className="flex items-center gap-6 text-sm flex-wrap">
            <div className="flex items-center gap-1">
              <Icon name="trending-up" size={16} className="text-primary" />
              <span>{signalDetail.amplifyCount || 0} amplifications</span>
            </div>
            {signalDetail.lastAmplifiedAt && (
              <div className="flex items-center gap-1">
                <Icon name="clock" size={16} className="text-muted-foreground" />
                <span>Last amplified {formatDate(signalDetail.lastAmplifiedAt)}</span>
              </div>
            )}
          </div>

          {/* Propose Idea Button */}
          {onProposeIdea && (
            <div className="ml-auto">
              <AuthRequiredButton action="propose an idea">
                <Button
                  onClick={onProposeIdea}
                  variant="primary"
                  size="sm"
                  icon="lightbulb"
                >
                  I have an idea
                </Button>
              </AuthRequiredButton>
            </div>
          )}
        </div>
      }
    />
  );
} 