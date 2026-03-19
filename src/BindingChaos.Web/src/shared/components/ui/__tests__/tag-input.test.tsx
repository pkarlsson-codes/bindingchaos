import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { TagInput } from '../tag-input';

describe('TagInput', () => {
  const mockSetTags = jest.fn();
  const defaultProps = {
    tags: [],
    setTags: mockSetTags,
    allTags: [
      { label: 'react', value: 'react' },
      { label: 'typescript', value: 'typescript' },
      { label: 'javascript', value: 'javascript' }
    ],
    placeholder: 'Add tags...'
  };

  beforeEach(() => {
    mockSetTags.mockClear();
  });

  it('should create a new tag when Enter is pressed with text input', async () => {
    const user = userEvent.setup();
    render(<TagInput {...defaultProps} />);

    const input = screen.getByPlaceholderText('Add tags...');
    
    // Type a new tag name
    await user.type(input, 'newtag');
    
    // Press Enter
    await user.keyboard('{Enter}');

    // Verify the new tag was added
    expect(mockSetTags).toHaveBeenCalledWith([
      { label: 'newtag', value: 'newtag' }
    ]);
  });

  it('should not create a tag when Enter is pressed with empty input', async () => {
    const user = userEvent.setup();
    render(<TagInput {...defaultProps} />);

    const input = screen.getByPlaceholderText('Add tags...');
    
    // Press Enter without typing anything
    await user.keyboard('{Enter}');

    // Verify no tag was added
    expect(mockSetTags).not.toHaveBeenCalled();
  });

  it('should not create duplicate tags (case-insensitive)', async () => {
    const user = userEvent.setup();
    const existingTags = [{ label: 'React', value: 'React' }];
    
    render(<TagInput 
      {...defaultProps} 
      tags={existingTags}
    />);

    const input = screen.getByPlaceholderText('Add tags...');
    
    // Try to add the same tag with different case
    await user.type(input, 'react');
    await user.keyboard('{Enter}');

    // Verify no duplicate was added
    expect(mockSetTags).not.toHaveBeenCalled();
  });

  it('should trim whitespace from new tags', async () => {
    const user = userEvent.setup();
    render(<TagInput {...defaultProps} />);

    const input = screen.getByPlaceholderText('Add tags...');
    
    // Type a tag with whitespace
    await user.type(input, '  newtag  ');
    await user.keyboard('{Enter}');

    // Verify the trimmed tag was added
    expect(mockSetTags).toHaveBeenCalledWith([
      { label: 'newtag', value: 'newtag' }
    ]);
  });

  it('should clear input after creating a new tag', async () => {
    const user = userEvent.setup();
    render(<TagInput {...defaultProps} />);

    const input = screen.getByPlaceholderText('Add tags...') as HTMLInputElement;
    
    // Type and create a tag
    await user.type(input, 'newtag');
    await user.keyboard('{Enter}');

    // Verify input is cleared
    expect(input.value).toBe('');
  });

  it('should still allow selecting from existing tags', async () => {
    const user = userEvent.setup();
    render(<TagInput {...defaultProps} />);

    const input = screen.getByPlaceholderText('Add tags...');
    
    // Type to show suggestions
    await user.type(input, 'react');
    
    // Click on the suggestion
    const suggestion = screen.getByText('react');
    await user.click(suggestion);

    // Verify the existing tag was selected
    expect(mockSetTags).toHaveBeenCalledWith([
      { label: 'react', value: 'react' }
    ]);
  });

  it('should handle Backspace to remove last tag when input is empty', async () => {
    const user = userEvent.setup();
    const existingTags = [
      { label: 'react', value: 'react' },
      { label: 'typescript', value: 'typescript' }
    ];
    
    render(<TagInput 
      {...defaultProps} 
      tags={existingTags}
    />);

    const input = screen.getByPlaceholderText('Add tags...');
    
    // Focus the input first to ensure it's ready
    await user.click(input);
    
    // Press Backspace with empty input
    await user.keyboard('{Backspace}');

    // Verify the last tag was removed
    expect(mockSetTags).toHaveBeenCalledWith([
      { label: 'react', value: 'react' }
    ]);
  });
}); 