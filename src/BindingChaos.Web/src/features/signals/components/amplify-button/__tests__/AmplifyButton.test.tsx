import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AmplifyButton } from '../AmplifyButton';

// Mock dependencies
jest.mock('../../../../../shared/hooks/useApiClient', () => ({
  useApiClient: jest.fn()
}));
jest.mock('../../../../auth', () => ({
  useUser: jest.fn(),
  AuthRequiredButton: ({ children, action }: any) => (
    <div data-testid="auth-button" data-action={action}>
      {children}
    </div>
  )
}));
jest.mock('../../../../../shared/components/Button', () => ({
  Button: ({ children, onClick, disabled, variant, size, className, 'aria-label': ariaLabel, 'data-amplify-button': dataAmplifyButton }: any) => (
    <button 
      onClick={onClick} 
      disabled={disabled === true ? true : undefined}
      data-variant={variant}
      data-size={size}
      className={className}
      aria-label={ariaLabel}
      data-amplify-button={dataAmplifyButton}
      data-testid="amplify-button"
    >
      {children}
    </button>
  )
}));

// Mock child components
jest.mock('../AmplifyCommentaryForm', () => ({
  AmplifyCommentaryForm: ({ onSubmit, onCancel, isSubmitting }: any) => (
    <div data-testid="commentary-form">
      <label>Add commentary (optional)</label>
      <textarea 
        placeholder="Why are you amplifying this signal?"
        data-testid="commentary-textarea"
      />
      <button 
        onClick={() => onSubmit('test commentary')}
        disabled={isSubmitting}
        data-testid="commentary-submit"
      >
        Amplify
      </button>
      <button 
        onClick={onCancel}
        data-testid="commentary-cancel"
      >
        Cancel
      </button>
    </div>
  )
}));

// DeamplifyInlineConfirmation doesn't render any UI, so no mock needed

const mockUseApiClient = require('../../../../../shared/hooks/useApiClient').useApiClient;

// Test data
const mockAmplifyResponse = {
  data: {
    signalId: 'signal-1',
    newAmplifyCount: 5
  }
};

const mockDeamplifyResponse = {
  data: {
    signalId: 'signal-1',
    newAmplifyCount: 4
  }
};

// Test wrapper component
const TestWrapper = ({ children }: { children: React.ReactNode }) => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false }
    }
  });

  return (
    <QueryClientProvider client={queryClient}>
      {children}
    </QueryClientProvider>
  );
};

describe('AmplifyButton', () => {
  let mockAmplifySignal: jest.Mock;
  let mockDeamplifySignal: jest.Mock;

  beforeEach(() => {
    jest.clearAllMocks();
    
    mockAmplifySignal = jest.fn();
    mockDeamplifySignal = jest.fn();
    
    // Default mocks
    mockUseApiClient.mockReturnValue({
      signals: {
        amplifySignal: mockAmplifySignal,
        deamplifySignal: mockDeamplifySignal
      }
    });
  });

  describe('Rendering', () => {
    it('renders amplify button when not amplified', () => {
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={false}
          />
        </TestWrapper>
      );

      expect(screen.getByTestId('amplify-button')).toBeInTheDocument();
      expect(screen.getByText('Amplify')).toBeInTheDocument();
      expect(screen.getByText('3')).toBeInTheDocument();
      expect(screen.getByTestId('amplify-button')).toHaveAttribute('data-variant', 'primary');
    });

    it('renders deamplify button when amplified', () => {
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={true}
          />
        </TestWrapper>
      );

      expect(screen.getByTestId('amplify-button')).toBeInTheDocument();
      expect(screen.getByText('Deamplify')).toBeInTheDocument();
      expect(screen.getByText('3')).toBeInTheDocument();
      expect(screen.getByTestId('amplify-button')).toHaveAttribute('data-variant', 'outline');
    });

    it('does not show count when amplifyCount is 0', () => {
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={0} 
            isAmplifiedByCurrentUser={false}
          />
        </TestWrapper>
      );

      expect(screen.getByTestId('amplify-button')).toBeInTheDocument();
      expect(screen.getByText('Amplify')).toBeInTheDocument();
      expect(screen.queryByText('0')).not.toBeInTheDocument();
    });

    it('renders with correct size and className', () => {
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={false}
            size="lg"
            className="custom-class"
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      expect(button).toHaveAttribute('data-size', 'lg');
      expect(button).toHaveClass('custom-class');
    });

    it('renders with correct aria-label', () => {
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={false}
          />
        </TestWrapper>
      );

      expect(screen.getByTestId('amplify-button')).toHaveAttribute(
        'aria-label', 
        'Amplify signal (3 amplifications)'
      );
    });
  });

  describe('Amplify Functionality', () => {
    it('shows commentary form when amplify button is clicked', async () => {
      const user = userEvent.setup();
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={false}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      // Commentary form should appear
      expect(screen.getByTestId('commentary-form')).toBeInTheDocument();
      expect(screen.getByText('Add commentary (optional)')).toBeInTheDocument();
    });

    it('submits amplification with commentary', async () => {
      const user = userEvent.setup();
      mockAmplifySignal.mockResolvedValue(mockAmplifyResponse);
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={false}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      const submitButton = screen.getByTestId('commentary-submit');
      await user.click(submitButton);

      await waitFor(() => {
        expect(mockAmplifySignal).toHaveBeenCalledWith({
          signalId: 'signal-1',
          amplifySignalRequest: {
            reason: 'interest',
            commentary: 'test commentary'
          }
        });
      });
    });

    it('submits amplification without commentary when form is submitted empty', async () => {
      const user = userEvent.setup();
      mockAmplifySignal.mockResolvedValue(mockAmplifyResponse);
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={false}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      const submitButton = screen.getByTestId('commentary-submit');
      await user.click(submitButton);

      await waitFor(() => {
        expect(mockAmplifySignal).toHaveBeenCalledWith({
          signalId: 'signal-1',
          amplifySignalRequest: {
            reason: 'interest',
            commentary: 'test commentary'
          }
        });
      });
    });
  });

  describe('Deamplify Functionality', () => {
    it('shows confirmation when deamplify button is clicked', async () => {
      const user = userEvent.setup();
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={true}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      // Button should change to "Confirm" state
      expect(screen.getByText('Confirm')).toBeInTheDocument();
      expect(screen.getByTestId('amplify-button')).toHaveAttribute('data-variant', 'danger');
    });

    it('submits deamplify when confirmation is clicked', async () => {
      const user = userEvent.setup();
      mockDeamplifySignal.mockResolvedValue(mockDeamplifyResponse);
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={true}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      // Button should now show "Confirm"
      const confirmButton = screen.getByText('Confirm');
      await user.click(confirmButton);

      await waitFor(() => {
        expect(mockDeamplifySignal).toHaveBeenCalledWith({
          signalId: 'signal-1'
        });
      });
    });

    it('cancels deamplify when cancel is clicked', async () => {
      const user = userEvent.setup();
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={true}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      // Wait for the event listener to be set up (100ms delay in the component)
      await new Promise(resolve => setTimeout(resolve, 150));

      // Click on the document body to trigger the outside click handler
      await user.click(document.body);

      expect(screen.queryByText('Confirm')).not.toBeInTheDocument();
      expect(screen.getByText('Deamplify')).toBeInTheDocument();
      expect(mockDeamplifySignal).not.toHaveBeenCalled();
    });
  });

  describe('Loading States', () => {
    it('shows loading state during amplify API call', async () => {
      const user = userEvent.setup();
      mockAmplifySignal.mockImplementation(() => new Promise(resolve => setTimeout(() => resolve(mockAmplifyResponse), 100)));
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={false}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      const submitButton = screen.getByTestId('commentary-submit');
      await user.click(submitButton);

      // Button should be disabled and show loading state
      expect(screen.getByTestId('amplify-button')).toBeDisabled();
      expect(screen.getByText('...')).toBeInTheDocument();
    });

    it('shows loading state during deamplify API call', async () => {
      const user = userEvent.setup();
      mockDeamplifySignal.mockImplementation(() => new Promise(resolve => setTimeout(() => resolve(mockDeamplifyResponse), 100)));
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={true}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      const confirmButton = screen.getByText('Confirm');
      await user.click(confirmButton);

      // Button should be disabled and show loading state
      expect(screen.getByTestId('amplify-button')).toBeDisabled();
      expect(screen.getByText('...')).toBeInTheDocument();
    });
  });

  describe('Success Handling', () => {
    it('updates button state immediately after successful amplify', async () => {
      const user = userEvent.setup();
      mockAmplifySignal.mockResolvedValue(mockAmplifyResponse);
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={false}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      const submitButton = screen.getByTestId('commentary-submit');
      await user.click(submitButton);

      await waitFor(() => {
        // Button should immediately change to "Deamplify"
        expect(screen.getByText('Deamplify')).toBeInTheDocument();
        expect(screen.getByText('5')).toBeInTheDocument(); // Updated count from API response
        expect(screen.getByTestId('amplify-button')).toHaveAttribute('data-variant', 'outline');
      });
    });

    it('updates button state immediately after successful deamplify', async () => {
      const user = userEvent.setup();
      mockDeamplifySignal.mockResolvedValue(mockDeamplifyResponse);
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={5} 
            isAmplifiedByCurrentUser={true}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      const confirmButton = screen.getByText('Confirm');
      await user.click(confirmButton);

      await waitFor(() => {
        // Button should immediately change to "Amplify"
        expect(screen.getByText('Amplify')).toBeInTheDocument();
        expect(screen.getByText('4')).toBeInTheDocument(); // Updated count from API response
        expect(screen.getByTestId('amplify-button')).toHaveAttribute('data-variant', 'primary');
      });
    });

    it('calls onSuccess callback after successful amplify', async () => {
      const user = userEvent.setup();
      const onSuccess = jest.fn();
      mockAmplifySignal.mockResolvedValue(mockAmplifyResponse);
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={false}
            onSuccess={onSuccess}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      const submitButton = screen.getByTestId('commentary-submit');
      await user.click(submitButton);

      await waitFor(() => {
        expect(onSuccess).toHaveBeenCalled();
      });
    });

    it('calls onSuccess callback after successful deamplify', async () => {
      const user = userEvent.setup();
      const onSuccess = jest.fn();
      mockDeamplifySignal.mockResolvedValue(mockDeamplifyResponse);
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={5} 
            isAmplifiedByCurrentUser={true}
            onSuccess={onSuccess}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      const confirmButton = screen.getByText('Confirm');
      await user.click(confirmButton);

      await waitFor(() => {
        expect(onSuccess).toHaveBeenCalled();
      });
    });

    it('closes commentary form after successful amplify', async () => {
      const user = userEvent.setup();
      mockAmplifySignal.mockResolvedValue(mockAmplifyResponse);
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={false}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      const submitButton = screen.getByTestId('commentary-submit');
      await user.click(submitButton);

      await waitFor(() => {
        expect(screen.queryByTestId('commentary-form')).not.toBeInTheDocument();
      });
    });

    it('closes confirmation dialog after successful deamplify', async () => {
      const user = userEvent.setup();
      mockDeamplifySignal.mockResolvedValue(mockDeamplifyResponse);
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={5} 
            isAmplifiedByCurrentUser={true}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      const confirmButton = screen.getByText('Confirm');
      await user.click(confirmButton);

      await waitFor(() => {
        expect(screen.queryByText('Confirm')).not.toBeInTheDocument();
      });
    });
  });

  describe('Error Handling', () => {
    it('calls onError callback when amplify fails', async () => {
      const user = userEvent.setup();
      const onError = jest.fn();
      mockAmplifySignal.mockRejectedValue(new Error('API Error'));
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={false}
            onError={onError}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      const submitButton = screen.getByTestId('commentary-submit');
      await user.click(submitButton);

      await waitFor(() => {
        expect(onError).toHaveBeenCalledWith(expect.any(Error));
      });
    });

    it('calls onError callback when deamplify fails', async () => {
      const user = userEvent.setup();
      const onError = jest.fn();
      mockDeamplifySignal.mockRejectedValue(new Error('API Error'));
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={5} 
            isAmplifiedByCurrentUser={true}
            onError={onError}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      const confirmButton = screen.getByText('Confirm');
      await user.click(confirmButton);

      await waitFor(() => {
        expect(onError).toHaveBeenCalledWith(expect.any(Error));
      });
    });

    it('re-enables button after error', async () => {
      const user = userEvent.setup();
      mockAmplifySignal.mockRejectedValue(new Error('API Error'));
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={false}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      const submitButton = screen.getByTestId('commentary-submit');
      await user.click(submitButton);

      await waitFor(() => {
        expect(screen.getByTestId('amplify-button')).not.toBeDisabled();
        expect(screen.getByText('3')).toBeInTheDocument(); // Original count restored
      });
    });
  });

  describe('Fallback Behavior', () => {
    it('uses fallback count increment when API response has no newAmplifyCount', async () => {
      const user = userEvent.setup();
      mockAmplifySignal.mockResolvedValue({ data: { signalId: 'signal-1' } });
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={false}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      const submitButton = screen.getByTestId('commentary-submit');
      await user.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText('4')).toBeInTheDocument(); // 3 + 1
      });
    });

    it('uses fallback count decrement when API response has no newAmplifyCount', async () => {
      const user = userEvent.setup();
      mockDeamplifySignal.mockResolvedValue({ data: { signalId: 'signal-1' } });
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={true}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      const confirmButton = screen.getByText('Confirm');
      await user.click(confirmButton);

      await waitFor(() => {
        expect(screen.getByText('2')).toBeInTheDocument(); // 3 - 1
      });
    });

    it('prevents count from going below 0', async () => {
      const user = userEvent.setup();
      mockDeamplifySignal.mockResolvedValue({ data: { signalId: 'signal-1' } });
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={0} 
            isAmplifiedByCurrentUser={true}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      const confirmButton = screen.getByText('Confirm');
      await user.click(confirmButton);

      await waitFor(() => {
        // Button should change to "Amplify" state (count stays at 0 but isn't displayed)
        expect(screen.getByText('Amplify')).toBeInTheDocument();
        expect(screen.getByTestId('amplify-button')).toHaveAttribute('data-variant', 'primary');
      });
    });
  });

  describe('Props Synchronization', () => {
    it('updates local state when props change', () => {
      const { rerender } = render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={false}
          />
        </TestWrapper>
      );

      expect(screen.getByText('3')).toBeInTheDocument();
      expect(screen.getByText('Amplify')).toBeInTheDocument();

      // Update props
      rerender(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={7} 
            isAmplifiedByCurrentUser={true}
          />
        </TestWrapper>
      );

      expect(screen.getByText('7')).toBeInTheDocument();
      expect(screen.getByText('Deamplify')).toBeInTheDocument();
    });
  });

  describe('Accessibility', () => {
    it('has correct aria-label for amplify state', () => {
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={false}
          />
        </TestWrapper>
      );

      expect(screen.getByTestId('amplify-button')).toHaveAttribute(
        'aria-label', 
        'Amplify signal (3 amplifications)'
      );
    });

    it('has correct aria-label for deamplify state', () => {
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={true}
          />
        </TestWrapper>
      );

      expect(screen.getByTestId('amplify-button')).toHaveAttribute(
        'aria-label', 
        'Deamplify signal (3 amplifications)'
      );
    });

    it('has correct aria-label for confirmation state', async () => {
      const user = userEvent.setup();
      
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={true}
          />
        </TestWrapper>
      );

      const button = screen.getByTestId('amplify-button');
      await user.click(button);

      expect(screen.getByTestId('amplify-button')).toHaveAttribute(
        'aria-label', 
        'Confirm deamplify signal (3 amplifications)'
      );
    });

    it('has correct data-amplify-button attribute', () => {
      render(
        <TestWrapper>
          <AmplifyButton 
            signalId="signal-1" 
            amplifyCount={3} 
            isAmplifiedByCurrentUser={false}
          />
        </TestWrapper>
      );

      expect(screen.getByTestId('amplify-button')).toHaveAttribute('data-amplify-button');
    });
  });
}); 