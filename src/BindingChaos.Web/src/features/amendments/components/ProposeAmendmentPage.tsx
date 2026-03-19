import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { AmendmentForm } from './AmendmentForm';
import type { AmendmentFormData } from './AmendmentForm';
import { Button } from '../../../shared/components/ui/button';
import { Card } from '../../../shared/components/layout/Card';
import { useApiClient } from '../../../shared/hooks/useApiClient';
import { LoadingSpinner } from '../../../shared/components/feedback/LoadingSpinner';
import type { IdeaDetailViewModel } from '../../../api/models/IdeaDetailViewModel';

export function ProposeAmendmentPage() {
  const navigate = useNavigate();
  const { ideaId } = useParams<{ ideaId: string }>();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | undefined>();
  const apiClient = useApiClient();

  const [idea, setIdea] = useState<IdeaDetailViewModel | null>(null);
  const [isLoadingIdea, setIsLoadingIdea] = useState(true);

  useEffect(() => {
    const loadIdea = async () => {
      if (!ideaId) {
        setError('Idea ID is required');
        setIsLoadingIdea(false);
        return;
      }

      try {
        setIsLoadingIdea(true);
        setError(undefined);
        
        const response = await apiClient.ideas.getIdea({ ideaId });
        if (response.data) {
          setIdea(response.data);
        } else {
          setError('Idea not found');
        }
      } catch (err) {
        console.error('Error loading idea:', err);
        setError('Failed to load idea details');
      } finally {
        setIsLoadingIdea(false);
      }
    };

    loadIdea();
  }, [ideaId, apiClient.ideas]);

  const handleSubmit = async (formData: AmendmentFormData) => {
    if (!ideaId || !idea) return;

    try {
      setIsSubmitting(true);
      setError(undefined);

      const request = {
        targetIdeaVersion: 1, // TODO: Get actual version from API when available
        proposedTitle: formData.proposedTitle,
        proposedBody: formData.proposedBody,
        amendmentTitle: formData.amendmentTitle,
        amendmentDescription: formData.amendmentDescription
      };

      const response = await apiClient.amendments.proposeAmendment({ 
        ideaId: ideaId!, 
        proposeAmendmentRequest: request 
      });
      
      if (response.data) {
        // Navigate back to the idea details page
        navigate(`/ideas/${ideaId}`, { 
          state: { 
            message: 'Amendment proposed successfully!',
            amendmentId: response.data.amendmentId 
          }
        });
      } else {
        setError('Failed to propose amendment');
      }
    } catch (err: any) {
      console.error('Error proposing amendment:', err);
      setError(err.response?.data?.message || 'Failed to propose amendment');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleCancel = () => {
    navigate(`/ideas/${ideaId}`);
  };

  if (isLoadingIdea) {
    return (
      <div className="flex justify-center items-center min-h-[400px]">
        <LoadingSpinner />
      </div>
    );
  }

  if (error && !idea) {
    return (
      <div className="max-w-4xl mx-auto p-6">
        <Card
          title="Error"
          content={
            <div className="text-center">
              <p className="text-destructive mb-4">{error}</p>
              <Button onClick={() => navigate(`/ideas`)}>
                Back to Ideas
              </Button>
            </div>
          }
        />
      </div>
    );
  }

  if (!idea) {
    return null;
  }

  return (
    <div className="max-w-4xl mx-auto p-6 space-y-6">
      {/* Header */}
      <div className="flex items-center gap-4">
        <Button
          variant="outline"
          size="sm"
          onClick={handleCancel}
          className="flex items-center gap-2"
        >
          ← Back to Idea
        </Button>
        <div>
          <h1 className="text-2xl font-bold text-foreground">
            Propose Amendment
          </h1>
          <p className="text-muted-foreground">
            Suggest changes to "{idea.idea?.title || 'Unknown Idea'}"
          </p>
        </div>
      </div>

      {/* Amendment Form */}
      <AmendmentForm
        originalContent={idea.idea?.body || ''}
        originalTitle={idea.idea?.title || ''}
        onSubmit={handleSubmit}
        onCancel={handleCancel}
        isLoading={isSubmitting}
        error={error}
      />
    </div>
  );
}
