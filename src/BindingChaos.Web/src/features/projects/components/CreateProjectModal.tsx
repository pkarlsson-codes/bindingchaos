import { useEffect, useState } from 'react';
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

const formSchema = z.object({
  title: z.string().min(3, 'Title must be at least 3 characters long'),
  description: z.string().min(10, 'Description must be at least 10 characters long'),
});

type FormData = z.infer<typeof formSchema>;

interface CreateProjectModalProps {
  isOpen: boolean;
  onClose: () => void;
  userGroupId: string;
}

export function CreateProjectModal({ isOpen, onClose, userGroupId }: CreateProjectModalProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { projects } = useApiClient();
  const queryClient = useQueryClient();

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<FormData>({
    resolver: zodResolver(formSchema),
    defaultValues: { title: '', description: '' },
  });

  useEffect(() => {
    if (isOpen) {
      reset({ title: '', description: '' });
    }
  }, [isOpen, reset]);

  const onSubmit = async (data: FormData) => {
    setIsSubmitting(true);

    try {
      const response = await projects.createProject({
        createProjectRequest: {
          userGroupId,
          title: data.title,
          description: data.description,
        },
      });

      if (response.data) {
        toast.success('Project created successfully!');
        await queryClient.invalidateQueries({ queryKey: ['projects', userGroupId] });
        reset();
        onClose();
      } else {
        toast.error('Failed to create project');
      }
    } catch {
      toast.error('Failed to create project');
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
          <DialogTitle>Create Project</DialogTitle>
          <DialogDescription>
            Create a new project owned by this user group.
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div>
            <Label htmlFor="title">Title</Label>
            <Input
              id="title"
              {...register('title')}
              placeholder="Enter project title..."
              className={errors.title ? 'border-red-500' : ''}
            />
            {errors.title && (
              <p className="text-sm text-red-500 mt-1">{errors.title.message}</p>
            )}
          </div>

          <div>
            <Label htmlFor="description">Description</Label>
            <Textarea
              id="description"
              {...register('description')}
              placeholder="Describe the project..."
              rows={4}
              className={errors.description ? 'border-red-500' : ''}
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
              {isSubmitting ? 'Creating...' : 'Create Project'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
