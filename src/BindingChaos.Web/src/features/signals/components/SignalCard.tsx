import { Card, CardContent, CardHeader, CardTitle, CardFooter } from '../../../shared/components/ui/card';
import { Badge } from '../../../shared/components/ui/badge';
import { Icon } from '../../../shared/components/layout/Icon';
import { useNavigate } from 'react-router-dom';
import type { SignalViewModel } from '../../../api/models';
import { AmplifyButtonWithErrorBoundary } from './amplify-button';
import { SignalThumbnail } from './SignalThumbnail';

interface SignalCardProps {
  signal: SignalViewModel;
}

export function SignalCard({ signal }: SignalCardProps) {
  const navigate = useNavigate();

  const handleSignalClick = () => {
    if (signal.id) {
      navigate(`/signals/${signal.id}`);
    }
  };

  return (
    <Card className="hover:shadow-md transition-shadow cursor-pointer" onClick={handleSignalClick}>
      <CardHeader>
        <div className="flex justify-between items-start">
          <CardTitle className="text-lg font-semibold">{signal.title}</CardTitle>
          <AmplifyButtonWithErrorBoundary
            signalId={signal.id || ''}
            amplifyCount={signal.amplificationCount || 0}
            isAmplifiedByCurrentUser={signal.isAmplifiedByCurrentUser || false}
            isOriginator={signal.isOriginator || false}
            size="sm"
          />
        </div>
      </CardHeader>
      <CardContent>
        <div className="flex gap-4">
          {/* Main content area */}
          <div className="flex-1">
            <p className="text-muted-foreground mb-4">{signal.description}</p>
            <div className="flex gap-2">
              {signal.tags?.map((tag) => (
                <Badge key={tag} variant="secondary">
                  {tag}
                </Badge>
              ))}
            </div>
          </div>
          
          {/* Thumbnail area */}
          {signal.firstAttachmentThumbnail && (
            <div className="flex-shrink-0">
              <SignalThumbnail
                src={signal.firstAttachmentThumbnail}
                className="w-16 h-16"
                alt={`Thumbnail for ${signal.title}`}
              />
            </div>
          )}
        </div>
      </CardContent>
      <CardFooter>
        <div className="flex justify-between items-center w-full">
          <div className="text-sm text-muted-foreground">
            Created: {signal.createdAt && new Date(signal.createdAt).toLocaleDateString()}
          </div>
          
          {/* Attachment count badge */}
          {(signal.attachmentCount ?? 0) > 0 && (
            <Badge variant="outline" className="gap-1">
              <Icon name="paperclip" size={12} />
              {signal.attachmentCount}
              {signal.attachmentCount === 1 ? ' attachment' : ' attachments'}
            </Badge>
          )}
        </div>
      </CardFooter>
    </Card>
  );
}