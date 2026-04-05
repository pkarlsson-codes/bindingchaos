import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import { AuthRequiredButton } from '../AuthRequiredButton';
 
const mockUseOptionalAuth = jest.fn();
const mockNavigate = jest.fn();

jest.mock('../../contexts/AuthContext', () => ({
  useOptionalAuth: () => mockUseOptionalAuth()
}));

jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  useNavigate: () => mockNavigate
}));

describe('AuthRequiredButton', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe('when user is not authenticated', () => {
    beforeEach(() => {
      mockUseOptionalAuth.mockReturnValue({ user: null });
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

    it('navigates to login when wrapper is clicked', () => {
      render(
        <AuthRequiredButton action="test action">
          <button data-testid="button">Test Button</button>
        </AuthRequiredButton>
      );

      fireEvent.click(screen.getByTestId('button'));

      expect(mockNavigate).toHaveBeenCalledWith('/login');
    });

    it('shows "Login to continue" text when hovering', () => {
      render(
        <AuthRequiredButton action="test action">
          <button data-testid="button">Amplify</button>
        </AuthRequiredButton>
      );

      // Initially shows original text
      expect(screen.getByText('Amplify')).toBeInTheDocument();

      // Hover over the button
      fireEvent.mouseEnter(screen.getByTestId('button'));
      
      // Should now show login prompt text
      expect(screen.getByText('Login to continue')).toBeInTheDocument();
      expect(screen.queryByText('Amplify')).not.toBeInTheDocument();

      // Stop hovering
      fireEvent.mouseLeave(screen.getByTestId('button'));
      
      // Should show original text again
      expect(screen.getByText('Amplify')).toBeInTheDocument();
      expect(screen.queryByText('Login to continue')).not.toBeInTheDocument();
    });
  });

  describe('when user is authenticated', () => {
    beforeEach(() => {
      mockUseOptionalAuth.mockReturnValue({ user: { id: '1', username: 'test-user' } });
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
      expect(screen.queryByText('Login to continue')).not.toBeInTheDocument();
    });
  });
}); 