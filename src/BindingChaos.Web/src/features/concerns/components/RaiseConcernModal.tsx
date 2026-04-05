import { useState, useEffect } from 'react';
import { RotateCcw } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useQuery } from '@tanstack/react-query';
import { toast } from '../../../shared/components/ui/toast';
import { Button } from '../../../shared/components/ui/button';
import { Input } from '../../../shared/components/ui/input';
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
import { useApiClient } from '../../../shared/hooks/useApiClient';
import type { SignalDetailViewModel } from '../../../api/models';
import { ConcernOriginDto } from '../../../api/models';

function SignalItem({ signalId, onRemove }: { signalId: string; onRemove: () => void }) {
  const apiClient = useApiClient();
  const { data: signal, isLoading } = useQuery({
    queryKey: ['signals', signalId],
    queryFn: async (): Promise<SignalDetailViewModel | null> => {
      const response = await apiClient.signals.getSignalDetails({ signalId });
      return response.data ?? null;
    },
    staleTime: 5 * 60 * 1000,
  });

  if (isLoading) {
    return (
      <div className="px-3 py-2 border rounded-md animate-pulse">
        <div className="h-3 bg-muted rounded w-2/3"></div>
      </div>
    );
  }

  if (!signal) return null;

  return (
    <div className="flex items-center gap-2 px-3 py-2 border rounded-md">
      <div className="flex-1 min-w-0">
        <p className="text-sm font-medium text-foreground">{signal.title}</p>
        {signal.description && (
          <p className="text-xs text-muted-foreground mt-0.5 line-clamp-1">{signal.description}</p>
        )}
      </div>
      <button
        type="button"
        onClick={onRemove}
        className="shrink-0 text-muted-foreground hover:text-foreground transition-colors"
        aria-label="Remove signal"
      >
        ×
      </button>
    </div>
  );
}

const raiseConcernSchema = z.object({
  name: z.string().min(1, 'Title is required'),
  tags: z.array(z.string()).optional(),
  signalIds: z.array(z.string()).min(1, 'At least one signal is required'),
});

type RaiseConcernFormData = z.infer<typeof raiseConcernSchema>;

interface RaiseConcernModalProps {
  isOpen: boolean;
  onClose: () => void;
  initialSignalIds?: string[];
  initialTags?: string[];
  origin?: ConcernOriginDto;
  clusterId?: string;
}

export function RaiseConcernModal({
  isOpen,
  onClose,
  initialSignalIds = [],
  initialTags: initialTags = [],
  origin = ConcernOriginDto.Manual,
  clusterId,
}: RaiseConcernModalProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);

  const { concerns } = useApiClient();

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    setValue,
    watch,
  } = useForm<RaiseConcernFormData>({
    resolver: zodResolver(raiseConcernSchema),
    defaultValues: {
      name: '',
      tags: initialTags,
      signalIds: initialSignalIds,
    },
  });

  useEffect(() => {
    if (isOpen) {
      reset({ name: '', tags: initialTags, signalIds: [...initialSignalIds] });
    }
  }, [isOpen]);

  const selectedTags = watch('tags') || [];
  const signalIds = watch('signalIds') || [];

  const setSignalIds = (ids: string[]) => setValue('signalIds', ids, { shouldValidate: true });

  const onSubmit = async (data: RaiseConcernFormData) => {
    setIsSubmitting(true);
    try {
      const response = await concerns.raiseConcern({
        raiseConcernRequest: {
          name: data.name,
          tags: data.tags,
          signalIds: data.signalIds,
          origin,
          clusterId,
        },
      });

      if (response.data) {
        toast.success('Concern raised successfully!');
        reset({ name: '', tags: initialTags, signalIds: [...initialSignalIds] });
        onClose();
      } else {
        toast.error('Failed to raise concern');
      }
    } catch (error) {
      console.error('Error raising concern:', error);
      toast.error('Failed to raise concern');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleClose = () => {
    reset({ name: '', tags: initialTags, signalIds: [...initialSignalIds] });
    onClose();
  };

  const handleDialogOpenChange = (open: boolean) => {
    if (!open) {
      handleClose();
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={handleDialogOpenChange}>
      <DialogContent className="sm:max-w-[600px]">
        <DialogHeader>
          <DialogTitle>Raise a Concern</DialogTitle>
          <DialogDescription>
            Share a new concern with your community. Concerns help identify emerging issues and areas that need attention.
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div>
            <Label htmlFor="title">Title</Label>
            <Input
              id="title"
              {...register('name')}
              placeholder="Enter concern title..."
              className={errors.name ? 'border-red-500' : ''}
            />
            {errors.name && (
              <p className="text-sm text-red-500 mt-1">{errors.name.message}</p>
            )}
          </div>

          <div>
            <Label>Tags</Label>
            <TagSelector
              selectedTags={selectedTags}
              onTagsChange={(tags: string[]) => setValue('tags', tags)}
            />
          </div>

          <div>
            <div className="flex items-center justify-between">
              <Label>Linked Signals</Label>
              <button
                type="button"
                onClick={() => setSignalIds([...initialSignalIds])}
                className="text-muted-foreground hover:text-foreground transition-colors"
                aria-label="Reset linked signals"
              >
                <RotateCcw size={14} />
              </button>
            </div>
            {signalIds.length > 0 && (
              <div className="mt-1.5 space-y-1.5 max-h-48 overflow-y-auto">
                {signalIds.map(id => (
                  <SignalItem key={id} signalId={id} onRemove={() => setSignalIds(signalIds.filter(s => s !== id))} />
                ))}
              </div>
            )}
            {errors.signalIds && (
              <p className="text-sm text-red-500 mt-1">{errors.signalIds.message}</p>
            )}
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={handleClose}>
              Cancel
            </Button>
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting ? 'Raising...' : 'Raise Concern'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
