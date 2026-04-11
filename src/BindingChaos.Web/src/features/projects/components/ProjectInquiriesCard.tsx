import { useState } from 'react';
import { Card } from '../../../shared/components/layout/Card';
import { Button } from '../../../shared/components/layout/Button';
import { Badge } from '../../../shared/components/ui/badge';
import { FormModal } from '../../../shared/components/modals/FormModal';
import { AuthRequiredButton } from '../../auth';
import { toast } from '../../../shared/components/ui/toast';
import { useProjectInquiries, useProjectInquiryMutations } from '../hooks/useProjectInquiries';
import type { ProjectInquiryResponse } from '../../../api/models';

interface ProjectInquiriesCardProps {
  projectId: string;
}

type ModalState =
  | { type: 'none' }
  | { type: 'raise' }
  | { type: 'respond'; inquiryId: string }
  | { type: 'update'; inquiryId: string; currentBody: string }
  | { type: 'reopen'; inquiryId: string };

function statusVariant(status: string): 'default' | 'secondary' | 'outline' | 'destructive' {
  switch (status.toLowerCase()) {
    case 'open': return 'default';
    case 'responded': return 'secondary';
    case 'resolved': return 'outline';
    case 'lapsed': return 'destructive';
    default: return 'secondary';
  }
}

export function ProjectInquiriesCard({ projectId }: ProjectInquiriesCardProps) {
  const { data: inquiries = [], isLoading } = useProjectInquiries(projectId);
  const { raiseInquiry, respond, resolve, update, reopen } = useProjectInquiryMutations(projectId);

  const [modal, setModal] = useState<ModalState>({ type: 'none' });
  const [textInput, setTextInput] = useState('');
  const [resolvingId, setResolvingId] = useState<string | null>(null);

  const closeModal = () => {
    setModal({ type: 'none' });
    setTextInput('');
  };

  const handleRaise = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await raiseInquiry.mutateAsync(textInput);
      toast.success('Inquiry raised.');
      closeModal();
    } catch {
      toast.error('Failed to raise inquiry.');
    }
  };

  const handleRespond = async (e: React.FormEvent) => {
    e.preventDefault();
    if (modal.type !== 'respond') return;
    try {
      await respond.mutateAsync({ inquiryId: modal.inquiryId, response: textInput });
      toast.success('Response submitted.');
      closeModal();
    } catch {
      toast.error('Failed to submit response.');
    }
  };

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (modal.type !== 'update') return;
    try {
      await update.mutateAsync({ inquiryId: modal.inquiryId, newBody: textInput });
      toast.success('Inquiry updated.');
      closeModal();
    } catch {
      toast.error('Failed to update inquiry.');
    }
  };

  const handleReopen = async (e: React.FormEvent) => {
    e.preventDefault();
    if (modal.type !== 'reopen') return;
    try {
      await reopen.mutateAsync({ inquiryId: modal.inquiryId, newBody: textInput || undefined });
      toast.success('Inquiry reopened.');
      closeModal();
    } catch {
      toast.error('Failed to reopen inquiry.');
    }
  };

  const handleResolve = async (inquiry: ProjectInquiryResponse) => {
    if (!inquiry.id) return;
    setResolvingId(inquiry.id);
    try {
      await resolve.mutateAsync(inquiry.id);
      toast.success('Inquiry resolved.');
    } catch {
      toast.error('Failed to resolve inquiry.');
    } finally {
      setResolvingId(null);
    }
  };

  return (
    <>
      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <h2 className="text-lg font-semibold text-foreground">Inquiries</h2>
          <AuthRequiredButton action="raise an inquiry">
            <Button size="sm" onClick={() => setModal({ type: 'raise' })}>
              Raise Inquiry
            </Button>
          </AuthRequiredButton>
        </div>

        {isLoading ? (
          <Card content={<div className="animate-pulse h-10 bg-muted rounded" />} />
        ) : inquiries.length === 0 ? (
          <Card
            title="No inquiries"
            content={<p className="text-muted-foreground">No inquiries have been raised for this project.</p>}
          />
        ) : (
          inquiries.map(inquiry => (
            <InquiryCard
              key={inquiry.id}
              inquiry={inquiry}
              resolvingId={resolvingId}
              onRespond={() => setModal({ type: 'respond', inquiryId: inquiry.id! })}
              onResolve={() => handleResolve(inquiry)}
              onUpdate={() => setModal({ type: 'update', inquiryId: inquiry.id!, currentBody: inquiry.body ?? '' })}
              onReopen={() => setModal({ type: 'reopen', inquiryId: inquiry.id! })}
            />
          ))
        )}
      </div>

      {/* Raise inquiry modal */}
      <FormModal
        isOpen={modal.type === 'raise'}
        onClose={closeModal}
        title="Raise Inquiry"
        description="Describe your concern about this project."
        onSubmit={handleRaise}
        submitText="Raise"
        loading={raiseInquiry.isPending}
        disabled={!textInput.trim()}
      >
        <textarea
          className="w-full min-h-[120px] rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
          placeholder="Describe your concern..."
          value={textInput}
          onChange={e => setTextInput(e.target.value)}
          required
        />
      </FormModal>

      {/* Respond modal */}
      <FormModal
        isOpen={modal.type === 'respond'}
        onClose={closeModal}
        title="Respond to Inquiry"
        description="Provide a response on behalf of the user group."
        onSubmit={handleRespond}
        submitText="Submit Response"
        loading={respond.isPending}
        disabled={!textInput.trim()}
      >
        <textarea
          className="w-full min-h-[120px] rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
          placeholder="Your response..."
          value={textInput}
          onChange={e => setTextInput(e.target.value)}
          required
        />
      </FormModal>

      {/* Update modal */}
      <FormModal
        isOpen={modal.type === 'update'}
        onClose={closeModal}
        title="Update Inquiry"
        description="Update the inquiry body. This will reset the status to open."
        onSubmit={handleUpdate}
        submitText="Update"
        loading={update.isPending}
        disabled={!textInput.trim()}
      >
        <textarea
          className="w-full min-h-[120px] rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
          placeholder="Updated concern..."
          value={textInput}
          onChange={e => setTextInput(e.target.value)}
          required
        />
      </FormModal>

      {/* Reopen modal */}
      <FormModal
        isOpen={modal.type === 'reopen'}
        onClose={closeModal}
        title="Reopen Inquiry"
        description="Reopen this lapsed inquiry, optionally with an updated body."
        onSubmit={handleReopen}
        submitText="Reopen"
        loading={reopen.isPending}
      >
        <textarea
          className="w-full min-h-[120px] rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
          placeholder="Updated body (optional)..."
          value={textInput}
          onChange={e => setTextInput(e.target.value)}
        />
      </FormModal>
    </>
  );
}

interface InquiryCardProps {
  inquiry: ProjectInquiryResponse;
  resolvingId: string | null;
  onRespond: () => void;
  onResolve: () => void;
  onUpdate: () => void;
  onReopen: () => void;
}

function InquiryCard({ inquiry, resolvingId, onRespond, onResolve, onUpdate, onReopen }: InquiryCardProps) {
  const status = inquiry.status ?? 'unknown';
  const isOpen = status.toLowerCase() === 'open';
  const isResponded = status.toLowerCase() === 'responded';
  const isLapsed = status.toLowerCase() === 'lapsed';

  const isResolving = resolvingId === inquiry.id;

  return (
    <Card
      title={
        <div className="flex items-center justify-between gap-4 flex-wrap">
          <div className="flex items-center gap-2">
            <span className="text-sm font-medium text-muted-foreground">Inquiry</span>
            <Badge variant={statusVariant(status)}>{status}</Badge>
          </div>
          <div className="flex items-center gap-2 flex-wrap">
            {/* User group member: can respond to open inquiries */}
            {isOpen && inquiry.isCurrentUserInProjectUserGroup && (
              <AuthRequiredButton action="respond to this inquiry">
                <Button size="sm" variant="outline" onClick={onRespond}>
                  Respond
                </Button>
              </AuthRequiredButton>
            )}
            {/* Raiser: can resolve when responded, update when open, reopen when lapsed */}
            {isResponded && inquiry.isRaisedByCurrentUser && (
              <>
                <AuthRequiredButton action="resolve this inquiry">
                  <Button
                    size="sm"
                    variant="outline"
                    onClick={onResolve}
                    disabled={isResolving}
                  >
                    {isResolving ? 'Resolving...' : 'Resolve'}
                  </Button>
                </AuthRequiredButton>
                <AuthRequiredButton action="update this inquiry">
                  <Button size="sm" variant="outline" onClick={onUpdate}>
                    Update
                  </Button>
                </AuthRequiredButton>
              </>
            )}
            {isOpen && inquiry.isRaisedByCurrentUser && (
              <AuthRequiredButton action="update this inquiry">
                <Button size="sm" variant="outline" onClick={onUpdate}>
                  Update
                </Button>
              </AuthRequiredButton>
            )}
            {isLapsed && inquiry.isRaisedByCurrentUser && (
              <AuthRequiredButton action="reopen this inquiry">
                <Button size="sm" variant="outline" onClick={onReopen}>
                  Reopen
                </Button>
              </AuthRequiredButton>
            )}
          </div>
        </div>
      }
      content={
        <div className="space-y-3 text-sm">
          <p className="text-foreground">{inquiry.body}</p>
          {inquiry.response && (
            <div className="border-l-2 border-muted-foreground/30 pl-3">
              <p className="text-xs text-muted-foreground mb-1">User group response</p>
              <p className="text-foreground">{inquiry.response}</p>
            </div>
          )}
          <div className="text-xs text-muted-foreground flex gap-4 flex-wrap">
            {inquiry.raisedAt && <span>Raised {new Date(inquiry.raisedAt).toLocaleDateString()}</span>}
            {inquiry.lastUpdatedAt && inquiry.lastUpdatedAt !== inquiry.raisedAt && (
              <span>Updated {new Date(inquiry.lastUpdatedAt).toLocaleDateString()}</span>
            )}
            {inquiry.discourseThreadId && (
              <span>Thread: {inquiry.discourseThreadId}</span>
            )}
          </div>
        </div>
      }
    />
  );
}
