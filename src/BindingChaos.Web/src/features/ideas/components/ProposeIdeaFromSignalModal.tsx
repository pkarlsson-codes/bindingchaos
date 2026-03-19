import { useState, useEffect } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import { useUser } from '../../auth';
import { FormModal } from '../../../shared/components/modals';
import { TagSelector } from '../../tags/components/TagSelector';
import { RichTextEditor } from '../../../shared/components/forms/RichTextEditor';
import { Input } from '../../../shared/components/ui/input';
import { Icon } from '../../../shared/components/layout/Icon';
import { toast } from '../../../shared/components/ui/toast';
import type { SignalDetailViewModel } from '../../../api/models';
import { useMySocietyIds } from '../../societies/hooks/useSocietyMutations';
import { useSocieties } from '../../societies/hooks/useSocieties';

interface ProposeIdeaFromSignalModalProps {
  isOpen: boolean;
  onClose: () => void;
  signal: SignalDetailViewModel;
}

export function ProposeIdeaFromSignalModal({ isOpen, onClose, signal }: ProposeIdeaFromSignalModalProps) {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [tags, setTags] = useState<string[]>([]);
  const [selectedSocietyId, setSelectedSocietyId] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});
  
  const apiClient = useApiClient();
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const { user } = useUser();
  const { data: mySocietyIds } = useMySocietyIds();
  const { data: allSocieties } = useSocieties();
  const mySocieties = (allSocieties?.items ?? []).filter(s => s.id && (mySocietyIds ?? []).includes(s.id));

  // Reset form when modal opens
  useEffect(() => {
    if (isOpen) {
      setTitle('');
      setDescription('');
      setTags(signal.tags || []);
      setErrors({});
      setIsSubmitting(false);
      setSelectedSocietyId(mySocieties.length === 1 ? (mySocieties[0].id ?? '') : '');
    }
  }, [isOpen, signal.tags, mySocieties]);

  const createIdeaMutation = useMutation({
    mutationFn: async (data: { title: string; description: string; tags: string[]; sourceSignalIds: string[]; societyId: string }) => {
      return apiClient.ideas.authorIdea({ authorIdeaRequest: data });
    },
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: ['ideas'] });
      onClose();
      resetForm();
      
      const responseData = data.data;
      const createdId = responseData;
      
      toast.success('Idea created successfully!', {
        action: createdId
          ? {
              label: 'View Idea',
              onClick: () => navigate(`/ideas/${createdId}`)
            }
          : undefined
      });
    },
    onError: (error) => {
      console.error('Failed to create idea:', error);
      setErrors({ submit: 'Failed to create idea. Please try again.' });
      setIsSubmitting(false); // Reset submitting state on error
      
      toast.error('Failed to create idea. Please try again.');
    }
  });

  const resetForm = () => {
    setTitle('');
    setDescription('');
    setTags([]);
    setSelectedSocietyId('');
    setErrors({});
    setIsSubmitting(false);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);
    setErrors({});

    // Validation
    if (!title.trim()) {
      setErrors({ title: 'Title is required' });
      setIsSubmitting(false);
      return;
    }

    if (!description.trim()) {
      setErrors({ description: 'Description is required' });
      setIsSubmitting(false);
      return;
    }

    if (!selectedSocietyId) {
      setErrors({ society: 'Please select a society' });
      setIsSubmitting(false);
      return;
    }

    // Call the mutation - error handling is done in the mutation's onError callback
    await createIdeaMutation.mutateAsync({
      title: title.trim(),
      description: description.trim(),
      tags: tags,
      sourceSignalIds: [signal.id!],
      societyId: selectedSocietyId,
    });
  };



  return (
    <FormModal
      isOpen={isOpen}
      onClose={onClose}
      title="Propose Idea from Signal"
      description="Create a new idea based on this signal to help organize and develop the concept further."
      onSubmit={handleSubmit}
      submitText="Propose Idea"
      loading={isSubmitting}
      disabled={!title.trim() || !description.trim() || !selectedSocietyId}
      size="xl"
    >
      <div className="mb-4 p-3 bg-muted border border-border rounded-md">
        <p className="text-sm text-muted-foreground">
          <Icon name="lightbulb" size={16} className="inline mr-2" />
          You're creating an idea based on the signal: <strong>"{signal.title}"</strong>
        </p>
      </div>

      <div>
        <label htmlFor="society" className="block text-sm font-medium text-foreground mb-2">
          Society *
        </label>
        <select
          id="society"
          value={selectedSocietyId}
          onChange={(e) => setSelectedSocietyId(e.target.value)}
          disabled={isSubmitting}
          className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
        >
          <option value="">Select a society...</option>
          {mySocieties.map((s) => (
            <option key={s.id} value={s.id ?? ''}>{s.name}</option>
          ))}
        </select>
        {errors.society && <p className="text-destructive text-sm mt-1">{errors.society}</p>}
        {mySocieties.length === 0 && (
          <p className="text-muted-foreground text-sm mt-1">You must join a society before proposing ideas.</p>
        )}
      </div>

      <div>
        <label htmlFor="title" className="block text-sm font-medium text-foreground mb-2">
          Idea Title *
        </label>
        <Input
          type="text"
          id="title"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          placeholder="Enter your idea title..."
          required
          disabled={isSubmitting}
        />
        {errors.title && <p className="text-destructive text-sm mt-1">{errors.title}</p>}
      </div>

      <div>
        <label htmlFor="description" className="block text-sm font-medium text-foreground mb-2">
          Idea Description *
        </label>
        <RichTextEditor
          content={description}
          onChange={setDescription}
          placeholder="Describe your idea in detail..."
          disabled={isSubmitting}
          className="min-h-[300px]"
        />
        {errors.description && <p className="text-destructive text-sm mt-1">{errors.description}</p>}
      </div>

      <div>
        <label className="block text-sm font-medium text-foreground mb-2">
          Tags
        </label>
        <TagSelector
          selectedTags={tags}
          onTagsChange={setTags}
          suggestedTags={signal.tags || []}
          placeholder="Add tags..."
          disabled={isSubmitting}
          userId={user?.id}
        />
      </div>

      {errors.submit && (
        <div className="p-3 bg-destructive/10 border border-destructive/20 rounded-md">
          <p className="text-destructive text-sm">{errors.submit}</p>
        </div>
      )}
    </FormModal>
  );
} 