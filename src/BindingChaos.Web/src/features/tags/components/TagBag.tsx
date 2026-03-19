import { cn } from '../../../shared/lib/utils';
import { Badge } from '../../../shared/components/ui/badge';

interface TagBagProps {
  tags: string[];
  maxTags?: number;
  className?: string;
}

export function TagBag({
  tags,
  maxTags,
  className
}: TagBagProps) {
  const displayTags = maxTags !== undefined ? tags.slice(0, maxTags) : tags;
  const remainingCount = maxTags !== undefined ? tags.length - maxTags : 0;

  return (
    <div className={cn('flex flex-wrap gap-2', className)}>
      {displayTags.map((tag) => (
        <Badge
          key={tag}
          variant="default"
          className="bg-primary/10 text-primary border-primary/20"
        >
          {tag}
        </Badge>
      ))}
      {remainingCount > 0 && (
        <Badge variant="secondary" className="text-muted-foreground">
          +{remainingCount} more
        </Badge>
      )}
    </div>
  );
} 