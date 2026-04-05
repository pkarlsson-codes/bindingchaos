import { useState, useEffect } from 'react';
import { useQueryClient } from '@tanstack/react-query';
import { toast } from '../../../shared/components/ui/toast';
import { Button } from '../../../shared/components/ui/button';
import { Label } from '../../../shared/components/ui/label';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '../../../shared/components/ui/dialog';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import { useMyUserGroups } from '../../../shared/hooks/useMyUserGroups';

interface ClaimConcernModalProps {
  isOpen: boolean;
  onClose: () => void;
  concernId: string;
}

export function ClaimConcernModal({ isOpen, onClose, concernId }: ClaimConcernModalProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [selectedGroupId, setSelectedGroupId] = useState('');
  const { commons } = useApiClient();
  const queryClient = useQueryClient();
  const { data: groups = [] } = useMyUserGroups();

  useEffect(() => {
    if (isOpen) {
      setSelectedGroupId('');
    }
  }, [isOpen]);

  const selectedGroup = groups.find(g => g.id === selectedGroupId);

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedGroup?.commonsId) return;

    setIsSubmitting(true);
    try {
      await commons.linkConcernToCommons({ commonsId: selectedGroup.commonsId, concernId });

      toast.success('Concern claimed for commons successfully!');
      await queryClient.invalidateQueries({ queryKey: ['commonsLinkedConcerns', selectedGroup.commonsId] });
      await queryClient.invalidateQueries({ queryKey: ['myUserGroups'] });
      setSelectedGroupId('');
      onClose();
    } catch {
      toast.error('Failed to claim concern for commons');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDialogOpenChange = (open: boolean) => {
    if (!open) {
      setSelectedGroupId('');
      onClose();
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={handleDialogOpenChange}>
      <DialogContent className="sm:max-w-[480px]">
        <DialogHeader>
          <DialogTitle>Claim for a Commons</DialogTitle>
          <DialogDescription>
            Claim this concern for a commons governed by one of your user groups.
          </DialogDescription>
        </DialogHeader>

        {groups.length === 0 ? (
          <p className="text-sm text-muted-foreground py-2">
            You are not a member of any user groups. Join or form a user group to claim concerns for a commons.
          </p>
        ) : (
          <form onSubmit={onSubmit} className="space-y-4">
            <div>
              <Label htmlFor="group">User Group</Label>
              <select
                id="group"
                value={selectedGroupId}
                onChange={e => setSelectedGroupId(e.target.value)}
                className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-xs transition-colors focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring"
              >
                <option value="">Select a user group...</option>
                {groups.map(g => (
                  <option key={g.id} value={g.id}>
                    {g.name}
                  </option>
                ))}
              </select>
            </div>

            <DialogFooter>
              <Button type="button" variant="outline" onClick={() => { setSelectedGroupId(''); onClose(); }}>
                Cancel
              </Button>
              <Button type="submit" disabled={isSubmitting || !selectedGroupId}>
                {isSubmitting ? 'Claiming...' : 'Claim for Commons'}
              </Button>
            </DialogFooter>
          </form>
        )}

        {groups.length === 0 && (
          <DialogFooter>
            <Button type="button" variant="outline" onClick={onClose}>
              Close
            </Button>
          </DialogFooter>
        )}
      </DialogContent>
    </Dialog>
  );
}
