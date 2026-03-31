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
import { DocumentUpload } from '../../../shared/components/forms/DocumentUpload';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import type { DocumentRecord } from '../../../shared/types/document';

const createSignalSchema = z.object({
  title: z.string().min(1, 'Title is required'),
  description: z.string().min(1, 'Description is required'),
  tags: z.array(z.string()).optional(),
});

type CreateSignalFormData = z.infer<typeof createSignalSchema>;

interface CreateSignalModalProps {
  isOpen: boolean;
  onClose: () => void;
  signalId?: string;
}

export function CreateSignalModal({ isOpen, onClose }: CreateSignalModalProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isUploadingDocuments, setIsUploadingDocuments] = useState(false);
  const [attachedDocuments, setAttachedDocuments] = useState<DocumentRecord[]>([]);
  const { signals } = useApiClient();

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    setValue,
    watch,
  } = useForm<CreateSignalFormData>({
    resolver: zodResolver(createSignalSchema),
    defaultValues: {
      title: '',
      description: '',
      tags: [],
    },
  });

  const selectedTags = watch('tags') || [];

  const onSubmit = async (data: CreateSignalFormData) => {
    if (isUploadingDocuments) {
      return;
    }

    setIsSubmitting(true);
    try {
      const response = await signals.captureSignal({
        captureSignalRequest: {
          title: data.title,
          description: data.description,
          tags: data.tags || [],
          attachmentIds: attachedDocuments.map(doc => doc.id)
        },
      });

      if (response.data) {
        toast.success('Signal created successfully!');
        reset();
        setAttachedDocuments([]);
        onClose();
      } else {
        toast.error('Failed to create signal');
      }
    } catch (error) {
      console.error('Error creating signal:', error);
      toast.error('Failed to create signal');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleClose = () => {
    reset();
    setAttachedDocuments([]);
    onClose();
  };

  const handleDocumentsChange = (documents: DocumentRecord[]) => {
    setAttachedDocuments(documents);
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
          <DialogTitle>Create New Signal</DialogTitle>
          <DialogDescription>
            Share a new signal with your community. Signals help identify emerging trends and opportunities.
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div>
            <Label htmlFor="title">Title</Label>
            <Input
              id="title"
              {...register('title')}
              placeholder="Enter signal title..."
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
              placeholder="Describe the signal..."
              rows={4}
              className={errors.description ? 'border-red-500' : ''}
            />
            {errors.description && (
              <p className="text-sm text-red-500 mt-1">{errors.description.message}</p>
            )}
          </div>

          <div>
            <Label>Tags</Label>
            <TagSelector
              selectedTags={selectedTags}
              onTagsChange={(tagIds: string[]) => setValue('tags', tagIds)}
            />
          </div>

          <div>
            <Label>Attachments</Label>
            <DocumentUpload
              onDocumentsChange={handleDocumentsChange}
              onUploadingChange={setIsUploadingDocuments}
              maxFiles={5}
              maxFileSize={5 * 1024 * 1024} // 5MB per file
              acceptedFileTypes={['image/*', 'application/pdf']}
            />
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={handleClose}>
              Cancel
            </Button>
            <Button type="submit" disabled={isSubmitting || isUploadingDocuments}>
              {isSubmitting ? 'Creating...' : isUploadingDocuments ? 'Uploading documents...' : 'Create Signal'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
} 