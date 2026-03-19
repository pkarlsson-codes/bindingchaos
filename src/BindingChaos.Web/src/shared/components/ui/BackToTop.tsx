import { useEffect, useState } from 'react';
import { ArrowUpIcon } from '@heroicons/react/24/outline';

interface BackToTopProps {
  threshold?: number;
}

/**
 * Floating "Back to top" button that appears after scrolling past a threshold.
 */
export function BackToTop({ threshold = 400 }: BackToTopProps) {
  const [isVisible, setIsVisible] = useState(false);

  useEffect(() => {
    const handleScroll = () => {
      setIsVisible(window.scrollY > threshold);
    };

    window.addEventListener('scroll', handleScroll);
    
    // Check initial scroll position
    handleScroll();

    return () => {
      window.removeEventListener('scroll', handleScroll);
    };
  }, [threshold]);

  const scrollToTop = () => {
    window.scrollTo({
      top: 0,
      behavior: 'smooth',
    });
  };

  if (!isVisible) {
    return null;
  }

  return (
    <button
      onClick={scrollToTop}
      className="fixed bottom-6 right-6 z-50 p-3 bg-primary text-primary-foreground rounded-full shadow-lg hover:bg-primary/90 transition-all duration-200 hover:scale-110 focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2"
      aria-label="Back to top"
    >
      <ArrowUpIcon className="h-6 w-6" />
    </button>
  );
}
