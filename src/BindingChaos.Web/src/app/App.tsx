import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Toaster } from '../shared/components/ui/toast';
import { ThemeProvider } from '../features/theme/providers/ThemeProvider';
import { ErrorBoundary } from '../shared/components/feedback/ErrorBoundary';
import { Header } from '../shared/components/Header';
import { SignalFeed } from '../features/signals/components/SignalFeed';
import { SignalDetailsPage } from '../features/signals/components/SignalDetailsPage';
import { IdeasPage } from '../features/ideas/components/IdeasPage';
import { IdeaDetailsPage } from '../features/ideas/components/IdeaDetailsPage';
import { SocietiesPage } from '../features/societies/components/SocietiesPage';
import { SocietyDetailPage } from '../features/societies/components/SocietyDetailPage';
import { SocietyInvitePage } from '../features/societies/components/SocietyInvitePage';
import { InvitePage } from '../features/trust/components/InvitePage';
import { EmergingPatternsPage } from '../features/emerging-patterns/components/EmergingPatternsPage';
import { EmergingPatternDetailsPage } from '../features/emerging-patterns/components/EmergingPatternDetailsPage';
import { ConcernsPage } from '../features/concerns/components/ConcernsPage';
import { AboutPage } from '../features/about/components/AboutPage';

import {
  AuthProvider,
  LoginPage,
  AuthErrorBoundary,
} from '../features/auth';
import '../App.css';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000, // 5 minutes
      retry: 1,
    },
  },
});

function App() {
  return (
    <ErrorBoundary>
      <ThemeProvider defaultTheme="system" storageKey="bindingchaos-ui-theme">
        <QueryClientProvider client={queryClient}>
          <AuthErrorBoundary>
            <AuthProvider>
              <Router>
                <div className="min-h-screen bg-background">
                  <Header />
                  <main className="container mx-auto px-4 py-8 max-w-6xl">
                    <Routes>
                      {/* Auth */}
                      <Route path="/login" element={<LoginPage />} />

                      {/* Signals */}
                      <Route path="/signals" element={<SignalFeed />} />
                      <Route path="/signals/:signalId" element={<SignalDetailsPage />} />

                      {/* Ideas */}
                      <Route path="/ideas" element={<IdeasPage />} />
                      <Route path="/ideas/:ideaId" element={<IdeaDetailsPage />} />

                      {/* Societies */}
                      <Route path="/societies" element={<SocietiesPage />} />
                      <Route path="/societies/:societyId/invitations/:token" element={<SocietyInvitePage />} />
                      <Route path="/societies/:societyId" element={<SocietyDetailPage />} />

                      {/* Concerns */}
                      <Route path="/concerns" element={<ConcernsPage />} />

                      {/* Emerging Patterns */}
                      <Route path="/patterns" element={<EmergingPatternsPage />} />
                      <Route path="/patterns/:clusterLabel" element={<EmergingPatternDetailsPage />} />

                      {/* About */}
                      <Route path="/about" element={<AboutPage />} />

                      {/* Invites */}
                      <Route path="/invite/:token" element={<InvitePage />} />

                      {/* Default */}
                      <Route path="/" element={<Navigate to="/signals" replace />} />
                      <Route path="*" element={<Navigate to="/signals" replace />} />
                    </Routes>
                  </main>
                </div>
              </Router>
              <Toaster />
            </AuthProvider>
          </AuthErrorBoundary>
        </QueryClientProvider>
      </ThemeProvider>
    </ErrorBoundary>
  );
}

export default App;
