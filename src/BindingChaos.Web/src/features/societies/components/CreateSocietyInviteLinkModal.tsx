import { useState } from 'react';
import { FormModal } from '../../../shared/components/modals/FormModal';
import { Label } from '../../../shared/components/ui/label';
import { Input } from '../../../shared/components/ui/input';
import { useCreateSocietyInviteLink } from '../hooks/useSocietyMutations';
import { toast } from '../../../shared/components/ui/toast';

interface CreateSocietyInviteLinkModalProps {
  societyId: string;
  isOpen: boolean;
  onClose: () => void;
}

export function CreateSocietyInviteLinkModal({
  societyId,
  isOpen,
  onClose,
}: CreateSocietyInviteLinkModalProps) {
  const [note, setNote] = useState('');
  const { mutate, isPending } = useCreateSocietyInviteLink(societyId);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    mutate(note.trim() || undefined, {
      onSuccess: () => {
        toast.success('Invite link created');
        setNote('');
        onClose();
      },
      onError: () => {
        toast.error('Failed to create invite link');
      },
    });
  };

  const handleClose = () => {
    setNote('');
    onClose();
  };

  return (
    <FormModal
      isOpen={isOpen}
      onClose={handleClose}
      title="Create invite link"
      description="Anyone with this link can join the society."
      onSubmit={handleSubmit}
      submitText="Create link"
      loading={isPending}
      size="sm"
    >
      <div className="space-y-2">
        <Label htmlFor="invite-note">Note (optional)</Label>
        <Input
          id="invite-note"
          placeholder="e.g. for Alice"
          value={note}
          onChange={(e) => setNote(e.target.value)}
          disabled={isPending}
        />
      </div>
    </FormModal>
  );
}
