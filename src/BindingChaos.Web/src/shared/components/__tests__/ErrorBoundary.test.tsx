import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import { ErrorBoundary } from '../feedback/ErrorBoundary';

// Component that throws an error for testing
const ThrowError = ({ shouldThrow }: { shouldThrow: boolean }) => {
  if (shouldThrow) {
    throw new Error('Test error message');
  }
  return <div>Normal component</div>;
};

// Component that can be controlled to throw errors
const ControlledErrorComponent = ({ shouldThrow, onRender }: { shouldThrow: boolean; onRender: () => void }) => {
  onRender();
  if (shouldThrow) {
    throw new Error('Test error message');
  }
  return <div>Normal component</div>;
};

// Custom fallback component for testing
const CustomFallback = ({ error, resetError }: { error?: Error; resetError: () => void }) => (
  <div>
    <p>Custom error message</p>
    {error && <p>Error: {error.message}</p>}
    <button onClick={resetError}>Custom Retry</button>
  </div>
);

describe('ErrorBoundary', () => {
  // Suppress console.error for these tests since we're intentionally throwing errors
  const originalError = console.error;
  beforeAll(() => {
    console.error = jest.fn();
  });

  afterAll(() => {
    console.error = originalError;
  });

  it('renders children when there is no error', () => {
    render(
      <ErrorBoundary>
        <div>Test content</div>
      </ErrorBoundary>
    );

    expect(screen.getByText('Test content')).toBeInTheDocument();
  });

  it('renders default fallback when child throws error', () => {
    render(
      <ErrorBoundary>
        <ThrowError shouldThrow={true} />
      </ErrorBoundary>
    );

    expect(screen.getByText('Something went wrong')).toBeInTheDocument();
    expect(screen.getByText('This component encountered an error. Please try again.')).toBeInTheDocument();
    expect(screen.getByText('Retry')).toBeInTheDocument();
  });

  it('renders custom fallback when provided', () => {
    render(
      <ErrorBoundary fallback={CustomFallback}>
        <ThrowError shouldThrow={true} />
      </ErrorBoundary>
    );

    expect(screen.getByText('Custom error message')).toBeInTheDocument();
    expect(screen.getByText('Error: Test error message')).toBeInTheDocument();
    expect(screen.getByText('Custom Retry')).toBeInTheDocument();
  });

  it('calls onError callback when error occurs', () => {
    const onError = jest.fn();
    
    render(
      <ErrorBoundary onError={onError}>
        <ThrowError shouldThrow={true} />
      </ErrorBoundary>
    );

    expect(onError).toHaveBeenCalledWith(
      expect.any(Error),
      expect.objectContaining({
        componentStack: expect.any(String)
      })
    );
  });

  it('has retry button that can be clicked', () => {
    render(
      <ErrorBoundary>
        <ThrowError shouldThrow={true} />
      </ErrorBoundary>
    );

    // Error should be shown
    expect(screen.getByText('Something went wrong')).toBeInTheDocument();

    // Retry button should be present and clickable
    const retryButton = screen.getByText('Retry');
    expect(retryButton).toBeInTheDocument();
    expect(retryButton).not.toBeDisabled();
  });

  it('shows error details when expanded', () => {
    render(
      <ErrorBoundary>
        <ThrowError shouldThrow={true} />
      </ErrorBoundary>
    );

    // Error details should be available (may be visible or hidden depending on browser)
    expect(screen.getByText('Error details')).toBeInTheDocument();
    
    // The error message should be in the DOM somewhere
    expect(screen.getByText('Test error message')).toBeInTheDocument();
  });
}); 