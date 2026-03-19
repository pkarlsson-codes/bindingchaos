import { useEffect, useRef, useState } from 'react';

interface UseIntersectionObserverOptions {
  threshold?: number;
  root?: Element | null;
  rootMargin?: string;
  enabled?: boolean;
}

/**
 * Hook to detect when an element is visible in the viewport using Intersection Observer API.
 * Useful for implementing infinite scroll, lazy loading, and scroll-triggered animations.
 */
export function useIntersectionObserver({
  threshold = 0,
  root = null,
  rootMargin = '0px',
  enabled = true,
}: UseIntersectionObserverOptions = {}) {
  const [isIntersecting, setIsIntersecting] = useState(false);
  const targetRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!enabled) {
      return;
    }

    const target = targetRef.current;
    if (!target) {
      return;
    }

    const observer = new IntersectionObserver(
      ([entry]) => {
        setIsIntersecting(entry.isIntersecting);
      },
      {
        threshold,
        root,
        rootMargin,
      }
    );

    observer.observe(target);

    return () => {
      observer.disconnect();
    };
  }, [threshold, root, rootMargin, enabled]);

  return { ref: targetRef, isIntersecting };
}
