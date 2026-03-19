import React from 'react';
import {
  ThumbsUp,
  ThumbsDown,
  TrendingUp,
  TrendingDown,
  Lightbulb,
  Sparkles,
  Clock,
  Users,
  Edit,
  Radio,
  Check,
  CheckCircle,
  ListChecks,
  Plus,
  X,
  Paperclip,
  Image as ImageIcon,
  Phone,
  Link,
  ArrowLeft,
  type LucideIcon,
} from 'lucide-react';
import { cn } from '@/shared/lib/utils';

// Define only the icon names we actually use
export type IconName =
  | 'thumbs-up'
  | 'thumbs-down'
  | 'trending-up'
  | 'trending-down'
  | 'lightbulb'
  | 'sparkles'
  | 'clock'
  | 'users'
  | 'edit'
  | 'signal'
  | 'check'
  | 'check-circle'
  | 'list-checks'
  | 'plus'
  | 'x'
  | 'paperclip'
  | 'image'
  | 'phone'
  | 'link'
  | 'arrow-left';

interface IconProps {
  name: IconName;
  size?: number;
  className?: string;
  color?: string;
}

export function Icon({ name, size = 16, className, color }: IconProps) {
  // Map kebab-case names to PascalCase component names
  const iconMap: Record<IconName, LucideIcon> = {
    'thumbs-up': ThumbsUp,
    'thumbs-down': ThumbsDown,
    'trending-up': TrendingUp,
    'trending-down': TrendingDown,
    'lightbulb': Lightbulb,
    'sparkles': Sparkles,
    'clock': Clock,
    'users': Users,
    'edit': Edit,
    'signal': Radio,
    'check': Check,
    'check-circle': CheckCircle,
    'list-checks': ListChecks,
    'plus': Plus,
    'x': X,
    'paperclip': Paperclip,
    'image': ImageIcon,
    'phone': Phone,
    'link': Link,
    'arrow-left': ArrowLeft,
  };

  const IconComponent = iconMap[name];

  if (!IconComponent) {
    console.warn(`Icon "${name}" not found in icon map`);
    return null;
  }

  return (
    <IconComponent
      size={size}
      className={cn('inline-block', className)}
      color={color}
    />
  );
}