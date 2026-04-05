import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import { AmplifyButton } from '../AmplifyButton';
import { useAmplifyState } from '../useAmplifyState';

jest.mock('../useAmplifyState', () => ({
  useAmplifyState: jest.fn(),
}));

jest.mock('../../../../auth', () => ({
  AuthRequiredButton: ({ children }: { children: React.ReactNode }) => <>{children}</>,
}));

const mockUseAmplifyState = useAmplifyState as jest.MockedFunction<typeof useAmplifyState>;

describe('AmplifyButton', () => {
  const mockAmplify = jest.fn();
  const mockDeamplify = jest.fn();

  beforeEach(() => {
    jest.clearAllMocks();
    mockUseAmplifyState.mockReturnValue({
      amplifyCount: 3,
      isAmplified: false,
      isPending: false,
      amplify: mockAmplify,
      deamplify: mockDeamplify,
    });
  });

  it('renders idle amplify state', () => {
    render(
      <AmplifyButton
        signalId="signal-1"
        amplifyCount={3}
        isAmplifiedByCurrentUser={false}
      />,
    );

    expect(screen.getByRole('button', { name: 'Amplify signal' })).toBeInTheDocument();
    expect(screen.getByText('Amplify')).toBeInTheDocument();
    expect(screen.getByText('3')).toBeInTheDocument();
  });

  it('requires confirmation before amplify', () => {
    render(
      <AmplifyButton
        signalId="signal-1"
        amplifyCount={3}
        isAmplifiedByCurrentUser={false}
      />,
    );

    fireEvent.click(screen.getByRole('button', { name: 'Amplify signal' }));

    expect(screen.getByText('Confirm?')).toBeInTheDocument();
    expect(mockAmplify).not.toHaveBeenCalled();

    fireEvent.click(screen.getByRole('button', { name: 'Confirm? signal' }));

    expect(mockAmplify).toHaveBeenCalledTimes(1);
  });

  it('renders amplified state and hover text', () => {
    mockUseAmplifyState.mockReturnValue({
      amplifyCount: 5,
      isAmplified: true,
      isPending: false,
      amplify: mockAmplify,
      deamplify: mockDeamplify,
    });

    render(
      <AmplifyButton
        signalId="signal-1"
        amplifyCount={5}
        isAmplifiedByCurrentUser
      />,
    );

    const button = screen.getByRole('button', { name: 'Amplified signal' });
    expect(screen.getByText('Amplified')).toBeInTheDocument();

    fireEvent.mouseEnter(button);
    expect(screen.getByText('Withdraw?')).toBeInTheDocument();

    fireEvent.mouseLeave(button);
    expect(screen.getByText('Amplified')).toBeInTheDocument();
  });

  it('requires confirmation before deamplify and supports outside-click cancel', () => {
    mockUseAmplifyState.mockReturnValue({
      amplifyCount: 5,
      isAmplified: true,
      isPending: false,
      amplify: mockAmplify,
      deamplify: mockDeamplify,
    });

    render(
      <AmplifyButton
        signalId="signal-1"
        amplifyCount={5}
        isAmplifiedByCurrentUser
      />,
    );

    fireEvent.click(screen.getByRole('button', { name: 'Amplified signal' }));
    expect(screen.getByText('Confirm?')).toBeInTheDocument();

    fireEvent.mouseDown(document.body);
    expect(screen.queryByText('Confirm?')).not.toBeInTheDocument();
    expect(screen.getByText('Amplified')).toBeInTheDocument();

    fireEvent.click(screen.getByRole('button', { name: 'Amplified signal' }));
    fireEvent.click(screen.getByRole('button', { name: 'Confirm? signal' }));
    expect(mockDeamplify).toHaveBeenCalledTimes(1);
  });

  it('disables button while pending', () => {
    mockUseAmplifyState.mockReturnValue({
      amplifyCount: 3,
      isAmplified: false,
      isPending: true,
      amplify: mockAmplify,
      deamplify: mockDeamplify,
    });

    render(
      <AmplifyButton
        signalId="signal-1"
        amplifyCount={3}
        isAmplifiedByCurrentUser={false}
      />,
    );

    expect(screen.getByRole('button', { name: 'Amplify signal' })).toBeDisabled();
  });
});
