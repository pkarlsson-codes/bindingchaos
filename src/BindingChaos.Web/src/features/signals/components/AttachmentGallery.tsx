import { useState } from 'react';
import { SignalThumbnail } from './SignalThumbnail';
import { Badge } from '../../../shared/components/ui/badge';
import { Icon } from '../../../shared/components/layout/Icon';

interface AttachmentItem {
  documentId: string;
  caption?: string;
  thumbnailUrl?: string;
  displayUrl?: string;
}

interface AttachmentGalleryProps {
  attachments?: Array<AttachmentItem>;
  baseUrl?: string;
  className?: string;
}

/**
 * Component for displaying multiple signal attachments in a gallery format.
 * Supports pre-built URLs (thumbnailUrl/displayUrl) or constructing them from baseUrl + documentId.
 */
export function AttachmentGallery({ attachments = [], baseUrl, className = "" }: AttachmentGalleryProps) {
  const [selectedIndex, setSelectedIndex] = useState(0);
  
  if (!attachments.length) {
    return null;
  }

  const getDisplayUrl = (attachment: AttachmentItem) => {
    if (attachment.displayUrl) return attachment.displayUrl;
    if (baseUrl) return `${baseUrl}/api/v1/documents/${attachment.documentId}/display`;
    return '';
  };

  const getThumbnailUrl = (attachment: AttachmentItem) => {
    if (attachment.thumbnailUrl) return attachment.thumbnailUrl;
    if (baseUrl) return `${baseUrl}/api/v1/documents/${attachment.documentId}/thumbnail`;
    return '';
  };

  const selectedAttachment = attachments[selectedIndex];

  return (
    <div className={`space-y-4 ${className}`}>
      {/* Attachment count badge */}
      <div className="flex items-center gap-2">
        <Badge variant="outline" className="gap-1">
          <Icon name="paperclip" size={12} />
          {attachments.length} {attachments.length === 1 ? 'attachment' : 'attachments'}
        </Badge>
      </div>

      {/* Main display */}
      <div className="space-y-2">
        <SignalThumbnail
          src={getDisplayUrl(selectedAttachment)}
          className="w-full h-64 max-w-md"
          alt={selectedAttachment.caption || `Attachment ${selectedIndex + 1}`}
        />
        
        {selectedAttachment.caption && (
          <p className="text-sm text-muted-foreground italic">
            {selectedAttachment.caption}
          </p>
        )}
      </div>

      {/* Thumbnail grid for multiple attachments */}
      {attachments.length > 1 && (
        <div className="flex gap-2 overflow-x-auto">
          {attachments.map((attachment, index) => (
            <button
              key={attachment.documentId}
              className={`flex-shrink-0 rounded border-2 transition-colors ${
                index === selectedIndex 
                  ? 'border-primary' 
                  : 'border-border hover:border-primary/50'
              }`}
              onClick={() => setSelectedIndex(index)}
            >
              <SignalThumbnail
                src={getThumbnailUrl(attachment)}
                className="w-16 h-16"
                alt={attachment.caption || `Thumbnail ${index + 1}`}
              />
            </button>
          ))}
        </div>
      )}
    </div>
  );
}