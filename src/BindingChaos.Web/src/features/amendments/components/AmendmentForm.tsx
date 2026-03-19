import { useState, useEffect } from 'react';
import { AmendmentEditor } from './AmendmentEditor';
import { Button } from '../../../shared/components/ui/button';
import { Input } from '../../../shared/components/ui/input';
import { Label } from '../../../shared/components/ui/label';
import { Textarea } from '../../../shared/components/ui/textarea';
import { Card, CardContent } from '../../../shared/components/ui/card';

interface AmendmentFormProps {
  originalContent: string;
  originalTitle: string;
  onSubmit: (data: AmendmentFormData) => void;
  onCancel: () => void;
  isLoading?: boolean;
  error?: string;
}

export interface AmendmentFormData {
  amendmentTitle: string;
  amendmentDescription: string;
  proposedTitle: string;
  proposedBody: string;
}

export function AmendmentForm({
  originalContent,
  originalTitle,
  onSubmit,
  onCancel,
  isLoading = false,
  error
}: AmendmentFormProps) {
  const [formData, setFormData] = useState<AmendmentFormData>({
    amendmentTitle: '',
    amendmentDescription: '',
    proposedTitle: '',
    proposedBody: ''
  });

  useEffect(() => {
    setFormData(prev => ({
      ...prev,
      proposedTitle: originalTitle,
      proposedBody: originalContent
    }));
  }, [originalTitle, originalContent]);

  const [errors, setErrors] = useState<Partial<AmendmentFormData>>({});

  const validateForm = (): boolean => {
    const newErrors: Partial<AmendmentFormData> = {};

    if (!formData.amendmentTitle.trim()) {
      newErrors.amendmentTitle = 'Amendment title is required';
    }

    if (!formData.amendmentDescription.trim()) {
      newErrors.amendmentDescription = 'Amendment description is required';
    }

    if (!formData.proposedTitle.trim()) {
      newErrors.proposedTitle = 'Proposed title is required';
    }

    if (!formData.proposedBody.trim() || formData.proposedBody === originalContent) {
      newErrors.proposedBody = 'Content must be different from the original';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (validateForm()) {
      onSubmit(formData);
    }
  };

  const handleInputChange = (field: keyof AmendmentFormData, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <Card>
        <CardContent className="p-6 space-y-4">
          <div>
            <Label htmlFor="amendmentTitle">Amendment Title *</Label>
            <Input
              id="amendmentTitle"
              value={formData.amendmentTitle}
              onChange={(e) => handleInputChange('amendmentTitle', e.target.value)}
              placeholder="Brief title describing your amendment"
              className={errors.amendmentTitle ? 'border-destructive' : ''}
            />
            {errors.amendmentTitle && (
              <p className="text-sm text-destructive mt-1">{errors.amendmentTitle}</p>
            )}
          </div>

          <div>
            <Label htmlFor="amendmentDescription">Amendment Description *</Label>
            <Textarea
              id="amendmentDescription"
              value={formData.amendmentDescription}
              onChange={(e) => handleInputChange('amendmentDescription', e.target.value)}
              placeholder="Explain the intent and reasoning behind your amendment"
              rows={3}
              className={errors.amendmentDescription ? 'border-destructive' : ''}
            />
            {errors.amendmentDescription && (
              <p className="text-sm text-destructive mt-1">{errors.amendmentDescription}</p>
            )}
          </div>

          <div>
            <Label htmlFor="proposedTitle">Proposed Title *</Label>
            <Input
              id="proposedTitle"
              value={formData.proposedTitle}
              onChange={(e) => handleInputChange('proposedTitle', e.target.value)}
              placeholder="The new title for the idea"
              className={errors.proposedTitle ? 'border-destructive' : ''}
            />
            {errors.proposedTitle && (
              <p className="text-sm text-destructive mt-1">{errors.proposedTitle}</p>
            )}
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardContent className="p-6">
          <AmendmentEditor
            content={formData.proposedBody}
            onChange={(content) => handleInputChange('proposedBody', content)}
            placeholder="Edit the content to propose your changes..."
            className={errors.proposedBody ? 'border-destructive' : ''}
          />
          {errors.proposedBody && (
            <p className="text-sm text-destructive mt-2">{errors.proposedBody}</p>
          )}
        </CardContent>
      </Card>

      {error && (
        <div className="p-4 bg-destructive/10 border border-destructive/20 rounded-lg">
          <p className="text-sm text-destructive">{error}</p>
        </div>
      )}

      <div className="flex justify-end gap-3">
        <Button
          type="button"
          variant="outline"
          onClick={onCancel}
          disabled={isLoading}
        >
          Cancel
        </Button>
        <Button
          type="submit"
          disabled={isLoading}
        >
          {isLoading ? 'Proposing Amendment...' : 'Propose Amendment'}
        </Button>
      </div>
    </form>
  );
}
