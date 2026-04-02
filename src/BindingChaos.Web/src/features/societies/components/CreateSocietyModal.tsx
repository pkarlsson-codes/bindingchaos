import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
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
import { TagSelector } from '../../tags/components/TagSelector';
import { useCreateSociety } from '../hooks/useSocietyMutations';
import { useNavigate } from 'react-router-dom';

const createSocietySchema = z.object({
  name: z.string().min(1, 'Name is required'),
  description: z.string().min(1, 'Description is required'),
  tags: z.array(z.string()).optional(),
  ratificationThreshold: z.number().min(0).max(1).optional(),
  reviewWindowHours: z.number().int().min(1).optional(),
  allowVeto: z.boolean().optional(),
});

type CreateSocietyFormData = z.infer<typeof createSocietySchema>;

interface CreateSocietyModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export function CreateSocietyModal({ isOpen, onClose }: CreateSocietyModalProps) {
  const navigate = useNavigate();
  const { mutateAsync: createSociety } = useCreateSociety();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    setValue,
    watch,
  } = useForm<CreateSocietyFormData>({
    resolver: zodResolver(createSocietySchema),
    defaultValues: {
      name: '',
      description: '',
      tags: [],
      ratificationThreshold: 0.5,
      reviewWindowHours: 72,
      allowVeto: false,
    },
  });

  const tags = watch('tags') ?? [];

  const onSubmit = async (data: CreateSocietyFormData) => {
    setIsSubmitting(true);
    try {
      const response = await createSociety({
        name: data.name,
        description: data.description,
        tags: data.tags,
        ratificationThreshold: data.ratificationThreshold,
        reviewWindowHours: data.reviewWindowHours,
        allowVeto: data.allowVeto,
      });
      const newId = response.data;
      reset();
      onClose();
      if (newId) {
        navigate(`/societies/${newId}`);
      }
    } catch {
      toast.error("Failed to create society. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleClose = () => {
    reset();
    onClose();
  };

  return (
    <Dialog open={isOpen} onOpenChange={(open) => !open && handleClose()}>
      <DialogContent className="max-w-lg">
        <DialogHeader>
          <DialogTitle>Create Society</DialogTitle>
          <DialogDescription>
            Start a new society with a shared social contract.
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="name">Name</Label>
            <Input
              id="name"
              placeholder="Society name"
              {...register('name')}
            />
            {errors.name && (
              <p className="text-sm text-destructive">{errors.name.message}</p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="description">Description</Label>
            <Textarea
              id="description"
              placeholder="What is this society about?"
              rows={3}
              {...register('description')}
            />
            {errors.description && (
              <p className="text-sm text-destructive">{errors.description.message}</p>
            )}
          </div>

          <div className="space-y-2">
            <Label>Tags</Label>
            <TagSelector
              selectedTags={tags}
              onTagsChange={(newTags) => setValue('tags', newTags)}
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="ratificationThreshold">Ratification threshold</Label>
              <Input
                id="ratificationThreshold"
                type="number"
                step="0.01"
                min="0"
                max="1"
                placeholder="0.5"
                {...register('ratificationThreshold', { valueAsNumber: true })}
              />
              <p className="text-xs text-muted-foreground">0–1, e.g. 0.5 = 50%</p>
            </div>
            <div className="space-y-2">
              <Label htmlFor="reviewWindowHours">Review window (hours)</Label>
              <Input
                id="reviewWindowHours"
                type="number"
                min="1"
                placeholder="72"
                {...register('reviewWindowHours', { valueAsNumber: true })}
              />
            </div>
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={handleClose} disabled={isSubmitting}>
              Cancel
            </Button>
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting ? 'Creating...' : 'Create Society'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
