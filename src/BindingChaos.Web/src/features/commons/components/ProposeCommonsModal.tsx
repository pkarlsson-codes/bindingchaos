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

const proposeCommonsSchema = z.object({
  name: z.string().min(1, 'Name is required'),
  description: z.string().min(1, 'Description is required'),
});

type ProposeCommonsFormData = z.infer<typeof proposeCommonsSchema>;

interface ProposeCommonsModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export function ProposeCommonsModal({ isOpen, onClose }: ProposeCommonsModalProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { commons } = useApiClient();
  const queryClient = useQueryClient();

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<ProposeCommonsFormData>({
    resolver: zodResolver(proposeCommonsSchema),
    defaultValues: { name: '', description: '' },
  });

  useEffect(() => {
    if (isOpen) {
      reset({ name: '', description: '' });
    }
  }, [isOpen]);

  const onSubmit = async (data: ProposeCommonsFormData) => {
    setIsSubmitting(true);
    try {
      const response = await commons.proposeCommons({
        proposeCommonsRequest: { name: data.name, description: data.description },
      });

      if (response.data) {
        toast.success('Commons proposed successfully!');
        await queryClient.invalidateQueries({ queryKey: ['commons'] });
        reset();
        onClose();
      } else {
        toast.error('Failed to propose commons');
      }
    } catch {
      toast.error('Failed to propose commons');
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
          <DialogTitle>Propose a Commons</DialogTitle>
          <DialogDescription>
            Propose a new commons as a shared resource or space for your community to govern together.
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div>
            <Label htmlFor="name">Name</Label>
            <Input
              id="name"
              {...register('name')}
              placeholder="Enter commons name..."
              className={errors.name ? 'border-red-500' : ''}
            />
            {errors.name && (
              <p className="text-sm text-red-500 mt-1">{errors.name.message}</p>
            )}
          </div>

          <div>
            <Label htmlFor="description">Description</Label>
            <Textarea
              id="description"
              {...register('description')}
              placeholder="Describe this commons..."
              className={errors.description ? 'border-red-500' : ''}
              rows={3}
            />
            {errors.description && (
              <p className="text-sm text-red-500 mt-1">{errors.description.message}</p>
            )}
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => { reset(); onClose(); }}>
              Cancel
            </Button>
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting ? 'Proposing...' : 'Propose Commons'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
