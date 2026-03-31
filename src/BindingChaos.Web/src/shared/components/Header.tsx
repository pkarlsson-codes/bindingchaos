import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useState } from 'react';
import { ThemeToggle } from '../../features/theme/components/ThemeToggle';
import { AuthStatus } from '../../features/auth';
import {
  NavigationMenu,
  NavigationMenuItem,
  NavigationMenuLink,
  NavigationMenuList,
  navigationMenuTriggerStyle,
} from './ui/navigation-menu';
import { cn } from '@/shared/lib/utils';

export function Header() {
  const location = useLocation();
  const navigate = useNavigate();
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  const isActive = (basePath: string) => {
    if (basePath === '/signals') {
      return location.pathname === '/signals' || location.pathname.startsWith('/signals/');
    }
    return location.pathname === basePath || location.pathname.startsWith(basePath + '/');
  };

  const navItems = [
    { path: '/signals', label: 'Signals', onClick: () => navigate('/signals') },
    { path: '/patterns', label: 'Patterns', onClick: () => navigate('/patterns') },
    { path: '/ideas', label: 'Ideas', onClick: () => navigate('/ideas') },
    { path: '/societies', label: 'Societies', onClick: () => navigate('/societies') },
  ];

  return (
    <header className="bg-background shadow-sm border-b border-border">
      <div className="container mx-auto px-4 max-w-6xl">
        {/* Row 1: Brand + Controls */}
        <div className="flex items-center gap-4 py-2">
          {/* Brand */}
          <Link to="/signals" className="text-xl font-bold text-primary shrink-0">
            BindingChaos
          </Link>

          <div className="flex-1" />

          {/* Right side controls */}
          <div className="flex items-center space-x-2 shrink-0">
            <ThemeToggle />
            <AuthStatus />

            {/* Mobile menu button */}
            <button
              className="md:hidden p-2"
              onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
            >
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
              </svg>
            </button>
          </div>
        </div>

        {/* Row 2: Desktop Navigation */}
        <nav className="hidden md:flex justify-center border-t border-border/40 py-1">
          <NavigationMenu>
            <NavigationMenuList>
              {navItems.map((item) => (
                <NavigationMenuItem key={item.path}>
                  <NavigationMenuLink
                    className={cn(
                      navigationMenuTriggerStyle(),
                      isActive(item.path) && 'bg-primary text-primary-foreground hover:bg-primary/90 hover:text-primary-foreground'
                    )}
                    onClick={item.onClick}
                  >
                    {item.label}
                  </NavigationMenuLink>
                </NavigationMenuItem>
              ))}
            </NavigationMenuList>
          </NavigationMenu>
        </nav>

        {/* Mobile Navigation */}
        {isMobileMenuOpen && (
          <nav className="md:hidden mt-2 pb-4 border-t border-border">
            <div className="flex flex-col space-y-2 pt-4">
              {navItems.map((item) => (
                <button
                  key={item.path}
                  className={cn(
                    'text-left px-4 py-2 rounded-md transition-colors',
                    isActive(item.path)
                      ? 'bg-primary text-primary-foreground'
                      : 'hover:bg-muted'
                  )}
                  onClick={() => {
                    item.onClick();
                    setIsMobileMenuOpen(false);
                  }}
                >
                  {item.label}
                </button>
              ))}
            </div>
          </nav>
        )}
      </div>
    </header>
  );
}
