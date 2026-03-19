import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { AmendmentForm } from './AmendmentForm';

// Mock the AmendmentEditor component
jest.mock('./AmendmentEditor', () => ({
  AmendmentEditor: ({ content, onChange }: any) => (
    <div data-testid="amendment-editor">
      <textarea
        value={content}
        onChange={(e) => onChange(e.target.value)}
        placeholder="Edit content..."
      />
    </div>
  ),
}));

describe('AmendmentForm', () => {
  const mockOnSubmit = jest.fn();
  const mockOnCancel = jest.fn();
  const originalContent = '<p>Original content</p>';

  const defaultProps = {
    originalContent,
    originalTitle: 'Original Title',
    onSubmit: mockOnSubmit,
    onCancel: mockOnCancel,
  };

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it('renders all form fields', () => {
    render(<AmendmentForm {...defaultProps} />);
    
    expect(screen.getByLabelText('Amendment Title *')).toBeInTheDocument();
    expect(screen.getByLabelText('Amendment Description *')).toBeInTheDocument();
    expect(screen.getByLabelText('Proposed Title *')).toBeInTheDocument();
    expect(screen.getByTestId('amendment-editor')).toBeInTheDocument();
  });

  it('shows validation errors for empty required fields', async () => {
    render(<AmendmentForm {...defaultProps} />);
    
    // Clear the pre-filled proposed title to trigger validation
    fireEvent.change(screen.getByLabelText('Proposed Title *'), {
      target: { value: '' }
    });
    
    const submitButton = screen.getByText('Propose Amendment');
    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText('Amendment title is required')).toBeInTheDocument();
      expect(screen.getByText('Amendment description is required')).toBeInTheDocument();
      expect(screen.getByText('Proposed title is required')).toBeInTheDocument();
      expect(screen.getByText('Content must be different from the original')).toBeInTheDocument();
    });
  });

  it('submits form with valid data', async () => {
    render(<AmendmentForm {...defaultProps} />);
    
    // Fill in required fields
    fireEvent.change(screen.getByLabelText('Amendment Title *'), {
      target: { value: 'Test Amendment Title' }
    });
    
    fireEvent.change(screen.getByLabelText('Amendment Description *'), {
      target: { value: 'Test amendment description' }
    });
    
    // Proposed title should be pre-filled with original title
    expect(screen.getByLabelText('Proposed Title *')).toHaveValue('Original Title');

    // Change content in the editor
    const editor = screen.getByTestId('amendment-editor').querySelector('textarea');
    fireEvent.change(editor!, {
      target: { value: '<p>Modified content</p>' }
    });

    // Submit form
    const submitButton = screen.getByText('Propose Amendment');
    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(mockOnSubmit).toHaveBeenCalledWith({
        amendmentTitle: 'Test Amendment Title',
        amendmentDescription: 'Test amendment description',
        proposedTitle: 'Original Title',
        proposedBody: '<p>Modified content</p>'
      });
    });
  });

  it('calls onCancel when cancel button is clicked', () => {
    render(<AmendmentForm {...defaultProps} />);
    
    const cancelButton = screen.getByText('Cancel');
    fireEvent.click(cancelButton);
    
    expect(mockOnCancel).toHaveBeenCalled();
  });

  it('shows loading state when isLoading is true', () => {
    render(<AmendmentForm {...defaultProps} isLoading={true} />);
    
    expect(screen.getByText('Proposing Amendment...')).toBeInTheDocument();
    expect(screen.getByText('Cancel')).toBeDisabled();
  });

  it('shows error message when error prop is provided', () => {
    const errorMessage = 'Something went wrong';
    render(<AmendmentForm {...defaultProps} error={errorMessage} />);
    
    expect(screen.getByText(errorMessage)).toBeInTheDocument();
  });
});
