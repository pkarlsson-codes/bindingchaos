import React from 'react';

interface ErrorBoundaryState {
  hasError: boolean;
  error?: Error;
}

interface ErrorBoundaryProps {
  children: React.ReactNode;
  fallback?: React.ComponentType<{ error?: Error; resetError: () => void }>;
  onError?: (error: Error, errorInfo: React.ErrorInfo) => void;
}

class ErrorBoundary extends React.Component<ErrorBoundaryProps, ErrorBoundaryState> {
  constructor(props: ErrorBoundaryProps) {
    super(props);
    this.state = { hasError: false };
  }

  static getDerivedStateFromError(error: Error): ErrorBoundaryState {
    return { hasError: true, error };
  }

  componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
    // Log to error reporting service
    console.error('Error caught by boundary:', error, errorInfo);
    
    // Call custom error handler if provided
    this.props.onError?.(error, errorInfo);
  }

  resetError = () => {
    this.setState({ hasError: false, error: undefined });
  };

  render() {
    if (this.state.hasError) {
      const FallbackComponent = this.props.fallback || DefaultFallback;
      return (
        <FallbackComponent 
          error={this.state.error} 
          resetError={this.resetError} 
        />
      );
    }

    return this.props.children;
  }
}

// Default fallback component
const DefaultFallback: React.FC<{ error?: Error; resetError: () => void }> = ({ 
  error, 
  resetError 
}) => {
  const [showDetails, setShowDetails] = React.useState(false);
  
  return (
    <div className="p-3 border border-red-200 rounded-md bg-red-50">
      <div className="flex items-start gap-2">
        <div className="flex-1">
          <h4 className="text-red-800 font-medium text-sm">Something went wrong</h4>
          <p className="text-red-600 text-xs mt-1">
            This component encountered an error. Please try again.
          </p>
          {error && (
            <details 
              className="mt-2"
              open={showDetails}
              onToggle={(e) => setShowDetails(e.currentTarget.open)}
            >
              <summary className="text-red-600 text-xs cursor-pointer hover:text-red-700">
                Error details
              </summary>
              <pre className="text-xs text-red-500 mt-1 bg-red-100 p-2 rounded overflow-auto">
                {error.message}
              </pre>
            </details>
          )}
        </div>
        <button
          onClick={resetError}
          className="px-2 py-1 bg-red-600 text-white text-xs rounded hover:bg-red-700 transition-colors"
        >
          Retry
        </button>
      </div>
    </div>
  );
};

export { ErrorBoundary }; 