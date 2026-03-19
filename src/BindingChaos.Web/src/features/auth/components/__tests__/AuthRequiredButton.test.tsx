import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import { AuthRequiredButton } from '../AuthRequiredButton';

// No LoginModal in redirect flow

// Mock the useUser hook
const mockUseUser = jest.fn();

jest.mock('../../contexts/AuthContext', () => ({
  useUser: () => mockUseUser()
}));

describe('AuthRequiredButton', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe('when user is not authenticated', () => {
    beforeEach(() => {
      mockUseUser.mockReturnValue({ user: null });
    });

    it('prevents original button click handler from firing', () => {
      const handleClick = jest.fn();
      
      render(
        <AuthRequiredButton action="test action">
          <button onClick={handleClick} data-testid="button">Test Button</button>
        </AuthRequiredButton>
      );

      fireEvent.click(screen.getByTestId('button'));
      expect(handleClick).not.toHaveBeenCalled();
    });

    it('navigates to OIDC login when wrapper is clicked', () => {
      // Spy on window.location.href setter
      const originalLocation = window.location;
      const setHref = jest.fn();
      // @ts-ignore
      delete (window as any).location;
      Object.defineProperty(window, 'location', {
        value: {
          ...originalLocation,
          get href() { return 'http://localhost/'; },
          set href(val: string) { setHref(val); },
        },
        configurable: true,
      });

      render(
        <AuthRequiredButton action="test action">
          <button data-testid="button">Test Button</button>
        </AuthRequiredButton>
      );

      const wrapper = screen.getByTestId('button').parentElement;
      fireEvent.click(wrapper!);

      expect(setHref).toHaveBeenCalled();

      // Restore original location
      Object.defineProperty(window, 'location', { value: originalLocation });
    });

    it('shows "Login" text when hovering', () => {
      render(
        <AuthRequiredButton action="test action">
          <button data-testid="button">Amplify</button>
        </AuthRequiredButton>
      );

      // Initially shows original text
      expect(screen.getByText('Amplify')).toBeInTheDocument();

      // Hover over the wrapper
      const wrapper = screen.getByTestId('button').parentElement;
      fireEvent.mouseEnter(wrapper!);
      
      // Should now show "Login"
      expect(screen.getByText('Login')).toBeInTheDocument();
      expect(screen.queryByText('Amplify')).not.toBeInTheDocument();

      // Stop hovering
      fireEvent.mouseLeave(wrapper!);
      
      // Should show original text again
      expect(screen.getByText('Amplify')).toBeInTheDocument();
      expect(screen.queryByText('Login')).not.toBeInTheDocument();
    });
  });

  describe('when user is authenticated', () => {
    beforeEach(() => {
      mockUseUser.mockReturnValue({ user: { id: '1', name: 'Test User' } });
    });

    it('allows button click to work normally', () => {
      const handleClick = jest.fn();
      
      render(
        <AuthRequiredButton action="test action">
          <button onClick={handleClick} data-testid="button">Test Button</button>
        </AuthRequiredButton>
      );

      fireEvent.click(screen.getByTestId('button'));
      expect(handleClick).toHaveBeenCalledTimes(1);
    });

    it('does not change text on hover', () => {
      render(
        <AuthRequiredButton action="test action">
          <button data-testid="button">Amplify</button>
        </AuthRequiredButton>
      );

      // Hover over the button
      fireEvent.mouseEnter(screen.getByTestId('button'));
      
      // Should still show original text
      expect(screen.getByText('Amplify')).toBeInTheDocument();
      expect(screen.queryByText('Login')).not.toBeInTheDocument();
    });
  });
}); 