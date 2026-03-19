import { useState } from 'react';
import { Button } from '../../../shared/components/ui/button';
import { Input } from '../../../shared/components/ui/input';
import { Card, CardContent } from '../../../shared/components/ui/card';
import { useSocieties } from '../hooks/useSocieties';
import { SocietyCard } from './SocietyCard';
import { CreateSocietyModal } from './CreateSocietyModal';
import { AuthRequiredButton } from '../../auth';

export function SocietiesPage() {
  const [filterTag, setFilterTag] = useState('');
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);

  const { data, isLoading, error, refetch } = useSocieties({
    filterTag: filterTag.trim() || undefined,
  });

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-foreground">Societies</h1>
        <AuthRequiredButton action="create a society">
          <Button onClick={() => setIsCreateModalOpen(true)}>
            Create Society
          </Button>
        </AuthRequiredButton>
      </div>

      <div className="flex gap-4">
        <Input
          placeholder="Filter by tag..."
          value={filterTag}
          onChange={(e) => setFilterTag(e.target.value)}
          className="max-w-sm"
        />
      </div>

      {isLoading && (
        <div className="space-y-4">
          {[1, 2, 3].map((i) => (
            <Card key={i}>
              <CardContent className="p-6">
                <div className="animate-pulse space-y-2">
                  <div className="h-5 bg-muted rounded w-1/3" />
                  <div className="h-4 bg-muted rounded w-2/3" />
                  <div className="h-4 bg-muted rounded w-1/2" />
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      {error && (
        <Card>
          <CardContent className="p-6">
            <p className="text-muted-foreground mb-4">Could not load societies. Please try again.</p>
            <Button variant="outline" onClick={() => refetch()}>Retry</Button>
          </CardContent>
        </Card>
      )}

      {data && data.items.length === 0 && (
        <Card>
          <CardContent className="p-6">
            <p className="text-muted-foreground">
              {filterTag ? 'No societies match that tag.' : 'No societies yet. Be the first to create one.'}
            </p>
          </CardContent>
        </Card>
      )}

      {data && data.items.length > 0 && (
        <div className="space-y-4">
          {data.items.map((society) => (
            <SocietyCard key={society.id} society={society} />
          ))}
        </div>
      )}

      <CreateSocietyModal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
      />
    </div>
  );
}
