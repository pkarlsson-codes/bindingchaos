import { useState, useEffect } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import { FormModal } from '../../../shared/components/modals';
import { RichTextEditor } from '../../../shared/components/forms/RichTextEditor';
import { Input } from '../../../shared/components/ui/input';
import { Icon } from '../../../shared/components/layout/Icon';
import { toast } from '../../../shared/components/ui/toast';
import type { IdeaDetailViewModel } from '../../../api/models';

interface CreateActionOpportunityModalProps {
  isOpen: boolean;
  onClose: () => void;
  idea: IdeaDetailViewModel;
}

export function CreateActionOpportunityModal({ isOpen, onClose, idea }: CreateActionOpportunityModalProps) {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});
  
  const apiClient = useApiClient();
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  // Reset form when modal opens
  useEffect(() => {
    if (isOpen) {
      setTitle('');
      setDescription('');
      setErrors({});
      setIsSubmitting(false);
    }
  }, [isOpen]);

  const createActionOpportunityMutation = useMutation({
    mutationFn: async (data: { title: string; description: string; parentIdeaId: string }) => {
      return apiClient.actionOpportunities.createActionOpportunity({ createActionOpportunityRequest: data });
    },
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: ['actionOpportunities'] });
      onClose();
      resetForm();
      
      // Navigate to the newly created action opportunity
      if (data?.data?.id) {
        navigate(`/actions/${data.data.id}`);
      }
      
      toast.success('Action opportunity created successfully!');
    },
    onError: (error) => {
      console.error('Failed to create action opportunity:', error);
      setErrors({ submit: 'Failed to create action opportunity. Please try again.' });
      
      toast.error('Failed to create action opportunity. Please try again.');
    }
  });

  const resetForm = () => {
    setTitle('');
    setDescription('');
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

    try {
      await createActionOpportunityMutation.mutateAsync({
        title: title.trim(),
        description: description.trim(),
        parentIdeaId: idea.idea?.id || ''
      });
    } catch (error) {
      // Error is handled by the mutation
      setIsSubmitting(false);
    }
  };

  const handleClose = () => {
    if (!isSubmitting) {
      onClose();
      resetForm();
    }
  };

  return (
        <FormModal
      isOpen={isOpen}
      onClose={handleClose}
      title="Create Action Opportunity"
      description={`Create a new action opportunity from "${idea.idea?.title || ''}"`}
      onSubmit={handleSubmit}
      loading={isSubmitting}
      submitText="Create Action Opportunity"
    >
      <div className="space-y-6">
        {/* Title Input */}
        <div className="space-y-2">
          <label htmlFor="title" className="text-sm font-medium text-foreground">
            Action Opportunity Title *
          </label>
          <Input
            id="title"
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Enter a clear, actionable title..."
            className={errors.title ? 'border-destructive' : ''}
            disabled={isSubmitting}
          />
          {errors.title && (
            <p className="text-sm text-destructive">{errors.title}</p>
          )}
        </div>

        {/* Description Input */}
        <div className="space-y-2">
          <label htmlFor="description" className="text-sm font-medium text-foreground">
            Description *
          </label>
          <RichTextEditor
            content={description}
            onChange={setDescription}
            placeholder="Describe what needs to be done, who can help, and what resources are needed..."
            disabled={isSubmitting}
            className={errors.description ? 'border-destructive' : ''}
          />
          {errors.description && (
            <p className="text-sm text-destructive">{errors.description}</p>
          )}
        </div>

        {/* Parent Idea Info */}
        <div className="p-4 bg-muted/50 rounded-lg border">
          <div className="flex items-start gap-3">
            <Icon name="lightbulb" className="w-5 h-5 text-primary mt-0.5" />
            <div className="space-y-1">
              <h4 className="text-sm font-medium text-foreground">Parent Idea</h4>
              <p className="text-sm text-muted-foreground">{idea.idea?.title}</p>
            </div>
          </div>
        </div>

        {/* Submit Error */}
        {errors.submit && (
          <div className="p-3 bg-destructive/10 border border-destructive/20 rounded-lg">
            <p className="text-sm text-destructive">{errors.submit}</p>
          </div>
        )}
      </div>
    </FormModal>
  );
} 