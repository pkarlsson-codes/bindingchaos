import React from 'react';
import { ErrorBoundary } from '../../../../shared/components/feedback/ErrorBoundary';
import { AmplifyButton } from './';
import type { AmplifyButtonProps } from './';

// Custom fallback component for AmplifyButton errors
const AmplifyButtonErrorFallback: React.FC<{ error?: Error; resetError: () => void }> = ({ 
  error, 
  resetError 
}) => (
  <div className="p-2 border border-red-200 rounded bg-red-50">
    <div className="flex items-center gap-2">
      <div className="flex-1">
        <p className="text-red-600 text-xs">
          Amplify button unavailable
        </p>
        {error && (
          <details className="mt-1">
            <summary className="text-red-500 text-xs cursor-pointer">
              Details
            </summary>
            <pre className="text-xs text-red-400 mt-1 bg-red-100 p-1 rounded">
              {error.message}
            </pre>
          </details>
        )}
      </div>
      <button
        onClick={resetError}
        className="px-2 py-1 bg-red-600 text-white text-xs rounded hover:bg-red-700 transition-colors"
        aria-label="Retry amplify button"
      >
        Retry
      </button>
    </div>
  </div>
);

interface AmplifyButtonWithErrorBoundaryProps extends AmplifyButtonProps {
  onBoundaryError?: (error: Error, errorInfo: React.ErrorInfo) => void;
}

export function AmplifyButtonWithErrorBoundary(props: AmplifyButtonWithErrorBoundaryProps) {
  const { onBoundaryError, ...amplifyButtonProps } = props;

  return (
    <ErrorBoundary 
      fallback={AmplifyButtonErrorFallback}
      onError={onBoundaryError}
    >
      <AmplifyButton {...amplifyButtonProps} />
    </ErrorBoundary>
  );
} 