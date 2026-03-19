import { useState } from 'react';
import { Icon } from '../../../shared/components/layout/Icon';

interface SignalThumbnailProps {
  src: string;
  alt?: string;
  className?: string;
}

export function SignalThumbnail({ src, alt = "Signal attachment", className = "" }: SignalThumbnailProps) {
  const [imageError, setImageError] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  const handleImageLoad = () => {
    setIsLoading(false);
  };

  const handleImageError = () => {
    setIsLoading(false);
    setImageError(true);
  };

  if (imageError) {
    return (
      <div className={`flex items-center justify-center bg-muted rounded-lg border border-border ${className}`}>
        <Icon name="image" size={16} className="text-muted-foreground" />
      </div>
    );
  }

  return (
    <div className={`relative rounded-lg border border-border overflow-hidden ${className}`}>
      {isLoading && (
        <div className="absolute inset-0 flex items-center justify-center bg-muted animate-pulse">
          <Icon name="image" size={16} className="text-muted-foreground" />
        </div>
      )}
      <img
        src={src}
        alt={alt}
        className="w-full h-full object-cover"
        onLoad={handleImageLoad}
        onError={handleImageError}
        style={{ display: isLoading ? 'none' : 'block' }}
      />
    </div>
  );
}