import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useQueryClient } from '@tanstack/react-query';
import { toast } from '../../../shared/components/ui/toast';
import { Button } from '../../../shared/components/ui/button';
import { Input } from '../../../shared/components/ui/input';
import { Textarea } from '../../../shared/components/ui/textarea';
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
import { UserGroupJoinPolicyDto } from '../../../api/models';

const formSchema = z.object({
  name: z.string().min(1, 'Name is required'),
  philosophy: z.string().min(1, 'Philosophy is required'),
});

type FormData = z.infer<typeof formSchema>;

interface FormUserGroupModalProps {
  isOpen: boolean;
  onClose: () => void;
  commonsId: string;
}

export function FormUserGroupModal({ isOpen, onClose, commonsId }: FormUserGroupModalProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { userGroups } = useApiClient();
  const queryClient = useQueryClient();

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<FormData>({
    resolver: zodResolver(formSchema),
    defaultValues: { name: '', philosophy: '' },
  });

  useEffect(() => {
    if (isOpen) {
      reset({ name: '', philosophy: '' });
    }
  }, [isOpen]);

  const onSubmit = async (data: FormData) => {
    setIsSubmitting(true);
    try {
      await userGroups.formUserGroup({
        formUserGroupRequest: {
          commonsId,
          name: data.name,
          philosophy: data.philosophy,
          charter: {
            contestationRules: {
              rejectionThreshold: 0.5,
              resolutionWindow: '3.00:00:00',
            },
            membershipRules: {
              joinPolicy: UserGroupJoinPolicyDto.Open,
              memberListPublic: true,
              approvalSettings: null,
              maxMembers: null,
              entryRequirements: null,
            },
            shunningRules: {
              approvalThreshold: 0.5,
            },
          },
        },
      });

      toast.success('User group formed successfully!');
      await queryClient.invalidateQueries({ queryKey: ['userGroups', commonsId] });
      reset();
      onClose();
    } catch {
      toast.error('Failed to form user group');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDialogOpenChange = (open: boolean) => {
    if (!open) {
      reset();
      onClose();
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={handleDialogOpenChange}>
      <DialogContent className="sm:max-w-[480px]">
        <DialogHeader>
          <DialogTitle>Form a User Group</DialogTitle>
          <DialogDescription>
            Form a new user group to govern this commons with a shared approach and philosophy.
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div>
            <Label htmlFor="name">Name</Label>
            <Input
              id="name"
              {...register('name')}
              placeholder="Enter group name..."
              className={errors.name ? 'border-red-500' : ''}
            />
            {errors.name && (
              <p className="text-sm text-red-500 mt-1">{errors.name.message}</p>
            )}
          </div>

          <div>
            <Label htmlFor="philosophy">Philosophy</Label>
            <Textarea
              id="philosophy"
              {...register('philosophy')}
              placeholder="Describe your group's governing philosophy..."
              className={errors.philosophy ? 'border-red-500' : ''}
              rows={3}
            />
            {errors.philosophy && (
              <p className="text-sm text-red-500 mt-1">{errors.philosophy.message}</p>
            )}
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => { reset(); onClose(); }}>
              Cancel
            </Button>
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting ? 'Forming...' : 'Form Group'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
