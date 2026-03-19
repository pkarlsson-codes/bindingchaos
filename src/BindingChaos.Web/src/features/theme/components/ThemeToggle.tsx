import React from 'react';
import { useTheme } from '../providers/ThemeProvider';
import { Button } from '../../../shared/components/ui/button';
import { Moon, Sun, Monitor } from 'lucide-react';

type Theme = 'light' | 'dark' | 'system';

export function ThemeToggle() {
  const { theme, setTheme } = useTheme();

  const toggleTheme = () => {
    const themes: Theme[] = ['light', 'dark', 'system'];
    const currentIndex = themes.indexOf(theme);
    const nextIndex = (currentIndex + 1) % themes.length;
    setTheme(themes[nextIndex]);
  };

  const getIcon = () => {
    switch (theme) {
      case 'light':
        return <Sun className="h-4 w-4 text-foreground" />;
      case 'dark':
        return <Moon className="h-4 w-4 text-foreground" />;
      case 'system':
        return <Monitor className="h-4 w-4 text-foreground" />;
      default:
        return <Sun className="h-4 w-4 text-foreground" />;
    }
  };

  const getLabel = () => {
    switch (theme) {
      case 'light':
        return 'Light';
      case 'dark':
        return 'Dark';
      case 'system':
        return 'System';
      default:
        return 'Light';
    }
  };

  return (
    <Button
      variant="ghost"
      size="sm"
      onClick={toggleTheme}
      className="h-9 w-9 p-0 relative"
      aria-label={`Switch to ${theme === 'light' ? 'dark' : theme === 'dark' ? 'system' : 'light'} theme`}
      title={`Current: ${getLabel()} - Click to change`}
    >
      {getIcon()}
      <span className="sr-only">Switch to {getLabel()} theme</span>
    </Button>
  );
} 