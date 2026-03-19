# Amendment Components

This directory contains components for managing amendments to ideas.

## Components

### AmendmentDetailsPage
- **Purpose**: Displays detailed information about a specific amendment
- **Features**:
  - Shows amendment details (current vs proposed changes)
  - Displays support statistics (supporters, opponents, net support)
  - Tabbed interface for details, supporters, and opponents
  - Voting controls for supporting/opposing the amendment
  - Lists all supporters and opponents with their reasons
- **Route**: `/l/:localitySlug/ideas/:ideaId/amendments/:amendmentId`
- **Status**: Uses fake data - backend integration needed

### AmendmentVotingControls
- **Purpose**: Provides buttons for supporting/opposing amendments
- **Features**:
  - Support/Oppose buttons with reason forms
  - Withdraw support/opposition functionality
  - Real-time count updates
  - Loading states and error handling

### SupportersList
- **Purpose**: Displays all supporters of an amendment
- **Features**:
  - Shows supporter pseudonyms and timestamps
  - Displays support reasons
  - Empty state when no supporters

### OpponentsList
- **Purpose**: Displays all opponents of an amendment
- **Features**:
  - Shows opponent pseudonyms and timestamps
  - Displays opposition reasons
  - Empty state when no opponents

### ProposeAmendmentPage
- **Purpose**: Form for creating new amendments
- **Features**:
  - Amendment title and description
  - Proposed changes to idea title and content
  - Form validation and submission

### AmendmentForm
- **Purpose**: Reusable form component for amendment creation/editing

### AmendmentEditor
- **Purpose**: Rich text editor for amendment content

## State Management Hooks

### useAmendmentSupportState
- Manages support state and API calls for supporting amendments

### useAmendmentOpposeState
- Manages opposition state and API calls for opposing amendments

### useAmendmentWithdrawSupportState
- Manages withdrawal of support

### useAmendmentWithdrawOppositionState
- Manages withdrawal of opposition

## Backend Integration

The amendment details page currently uses fake data. The following backend endpoints are needed:

1. `GET /api/amendments/{amendmentId}` - Get amendment details
2. `GET /api/amendments/{amendmentId}/supporters` - Get supporters list
3. `GET /api/amendments/{amendmentId}/opponents` - Get opponents list
4. `POST /api/amendments/{amendmentId}/support` - Support amendment
5. `POST /api/amendments/{amendmentId}/oppose` - Oppose amendment
6. `DELETE /api/amendments/{amendmentId}/support` - Withdraw support
7. `DELETE /api/amendments/{amendmentId}/opposition` - Withdraw opposition

## Data Models

### AmendmentViewModel
```typescript
interface AmendmentViewModel {
  id?: string;
  title?: string;
  shortDescription?: string;
  proposedTitle?: string;
  proposedBody?: string;
  proposerId?: string;
  proposerPseudonym?: string;
  status?: string;
  targetVersionNumber?: number;
  supporters?: Array<string>;
  opponents?: Array<string>;
  createdAt?: string;
  acceptedOn?: string | null;
  rejectedOn?: string | null;
  supporterCount?: number;
  opponentCount?: number;
  netSupport?: number;
  isOpen?: boolean;
  isResolved?: boolean;
}
```

### Supporter/Opponent Data
```typescript
interface Supporter {
  id: string;
  pseudonym: string;
  reason: string;
  supportedAt: string;
}

interface Opponent {
  id: string;
  pseudonym: string;
  reason: string;
  opposedAt: string;
}
```

