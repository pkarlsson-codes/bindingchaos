import { useQuery } from '@tanstack/react-query';
import { API_CONFIG } from '@/config/api';

export interface ResolvedInviteLink {
  inviterUserId: string;
}

async function resolveInviteLink(token: string): Promise<ResolvedInviteLink | null> {
  const response = await fetch(
    `${API_CONFIG.baseUrl}/api/v1/invite-links/resolve?token=${encodeURIComponent(token)}`,
    { credentials: 'include' }
  );
  if (response.status === 404) return null;
  if (!response.ok) throw new Error(`Unexpected response: ${response.status}`);
  const json = await response.json() as { data?: ResolvedInviteLink | null };
  return json.data ?? null;
}

export function useResolveInviteLink(token: string) {
  return useQuery({
    queryKey: ['invite-link', token],
    queryFn: () => resolveInviteLink(token),
    retry: false,
    staleTime: 60 * 1000,
  });
}
