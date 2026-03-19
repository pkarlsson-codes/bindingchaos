import React from 'react';
import { Icon } from './layout/Icon';

interface SupportRatioIndicatorProps {
  supporters?: number;
  opponents?: number;
}

export function SupportRatioIndicator({ supporters = 0, opponents = 0 }: SupportRatioIndicatorProps) {
  const total = supporters + opponents;
  const supportRatio = total > 0 ? supporters / total : 0;
  
  // Determine color based on ratio
  const getColorClass = (ratio: number) => {
    if (ratio >= 0.7) return 'bg-green-500 dark:bg-green-600'; // Strong support
    if (ratio >= 0.6) return 'bg-green-400 dark:bg-green-500'; // Good support
    if (ratio >= 0.5) return 'bg-yellow-500 dark:bg-yellow-600'; // Neutral
    if (ratio >= 0.4) return 'bg-orange-400 dark:bg-orange-500'; // Some opposition
    return 'bg-red-500 dark:bg-red-600'; // Strong opposition
  };

  const getTextColor = (ratio: number) => {
    if (ratio >= 0.7) return 'text-green-700 dark:text-green-300';
    if (ratio >= 0.6) return 'text-green-600 dark:text-green-400';
    if (ratio >= 0.5) return 'text-yellow-700 dark:text-yellow-300';
    if (ratio >= 0.4) return 'text-orange-600 dark:text-orange-400';
    return 'text-red-700 dark:text-red-300';
  };

  if (total === 0) {
    return (
      <div className="flex items-center gap-2 text-sm text-muted-foreground">
        <div className="w-16 h-2 bg-muted rounded-full"></div>
        <span>No votes yet</span>
      </div>
    );
  }

  return (
    <div className="flex items-center gap-2 text-sm">
      <div className="flex items-center gap-1">
        <Icon name="thumbs-up" className="text-green-600 dark:text-green-400" />
        <span className="font-medium">{supporters}</span>
      </div>
      <div className="flex items-center gap-1">
        <Icon name="thumbs-down" className="text-red-600 dark:text-red-400" />
        <span className="font-medium">{opponents}</span>
      </div>
      <div className="flex items-center gap-1">
        <div className="w-16 h-2 bg-muted rounded-full overflow-hidden">
          <div 
            className={`h-full ${getColorClass(supportRatio)} transition-all duration-300`}
            style={{ width: `${supportRatio * 100}%` }}
          ></div>
        </div>
        <span className={`text-xs font-medium ${getTextColor(supportRatio)}`}>
          {Math.round(supportRatio * 100)}%
        </span>
      </div>
    </div>
  );
} 