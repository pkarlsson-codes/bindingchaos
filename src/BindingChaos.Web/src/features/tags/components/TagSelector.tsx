import React, { useState, useEffect, useRef, useMemo, useCallback } from 'react';
import { TagInput } from '../../../shared/components/ui/tag-input';
import type { Tag } from '../../../shared/components/ui/tag-input';
import { useTagCache } from '../services/TagCacheService';

interface TagSelectorProps {
  selectedTags: string[];
  onTagsChange: (tags: string[]) => void;
  availableTags?: string[];
  suggestedTags?: string[];
  placeholder?: string;
  disabled?: boolean;
  localityId?: string;
  userId?: string;
}

export function TagSelector({ 
  selectedTags, 
  onTagsChange, 
  availableTags = [], 
  suggestedTags = [],
  placeholder = "Type tags, press Space or Enter...",
  disabled = false,
  localityId,
  userId
}: TagSelectorProps) {
  const [cachedAvailableTags, setCachedAvailableTags] = useState<string[]>([]);
  const [isLoadingTags, setIsLoadingTags] = useState(false);
  
  const tagCache = useTagCache();

  // Load popular tags on component mount
  useEffect(() => {
    const loadPopularTags = async () => {
      if (availableTags.length === 0 && !isLoadingTags) {
        setIsLoadingTags(true);
        try {
          const popularTags = await tagCache.getPopularTags(localityId, 50);
          setCachedAvailableTags(popularTags);
        } catch (error) {
          console.error('Failed to load popular tags:', error);
        } finally {
          setIsLoadingTags(false);
        }
      }
    };

    loadPopularTags();
  }, [availableTags.length, localityId, tagCache]);

  // Combine available tags (from props or cache)
  const allAvailableTags = availableTags.length > 0 ? availableTags : cachedAvailableTags;

  // Memoize selected tag objects to prevent recreation on every render
  const selectedTagObjects: Tag<string>[] = useMemo(() => 
    selectedTags.map(tag => ({
      label: tag,
      value: tag
    })), [selectedTags]
  );

  // Memoize all tag objects to prevent recreation on every render
  const allTagObjects: Tag<string>[] = useMemo(() => [
    // Suggested tags first
    ...suggestedTags
      .filter(tag => !selectedTags.includes(tag))
      .map(tag => ({ label: `#${tag} (suggested)`, value: tag })),
    // Then available tags
    ...allAvailableTags
      .filter(tag => !selectedTags.includes(tag) && !suggestedTags.includes(tag))
      .map(tag => ({ label: `#${tag}`, value: tag }))
  ], [suggestedTags, selectedTags, allAvailableTags]);

  // Memoize the handleTagsChange function to prevent recreation on every render
  const handleTagsChange = useCallback((newTagObjects: Tag<string>[]) => {
    const newTags = newTagObjects.map(tag => tag.value);

    // Find the newly added tag
    const addedTag = newTags.find(tag => !selectedTags.includes(tag));
    if (addedTag && userId) {
      // Add to recent tags cache
      tagCache.addRecentTag(addedTag);
    }

    onTagsChange(newTags);
  }, [selectedTags, userId, tagCache, onTagsChange]);

  return (
    <div className="space-y-3">
      <TagInput
        tags={selectedTagObjects}
        setTags={handleTagsChange}
        allTags={allTagObjects}
        placeholder={isLoadingTags ? "Loading tags..." : placeholder}
        className={disabled ? "opacity-50 pointer-events-none" : ""}
        AllTagsLabel={({ value }) => (
          <span className="flex items-center gap-2">
            {value}
          </span>
        )}
      />
    </div>
  );
} 