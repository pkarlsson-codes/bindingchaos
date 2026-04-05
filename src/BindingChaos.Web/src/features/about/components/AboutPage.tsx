import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card } from '../../../shared/components/layout/Card';
import { Button } from '../../../shared/components/layout/Button';
import { ProcessTimeline } from './ProcessTimeline';
import { StageCard } from './StageCard';
import { Lightbulb, Radar, TriangleAlert, LandPlot, Users, FolderKanban } from 'lucide-react';

interface Stage {
  id: string;
  number: number;
  name: string;
  icon: React.ReactNode;
  description: string;
  keyPoints: string[];
}

const stages: Stage[] = [
  {
    id: 'signal',
    number: 1,
    name: 'Signal',
    icon: <Lightbulb size={24} className="text-primary" />,
    description: 'Raw observations from participants about what they notice',
    keyPoints: [
      'Fast, low-friction way to share what is happening',
      'Supports tags, attachments, comments, and amplification',
      'Builds the public trace of what the community is noticing',
      'Can be explored in feed and detail views',
      'Provides the raw material for clustering and concern formation',
    ],
  },
  {
    id: 'pattern',
    number: 2,
    name: 'Emerging Pattern',
    icon: <Radar size={24} className="text-primary" />,
    description: 'Signal clusters identified automatically as activity grows',
    keyPoints: [
      'Groups related signals by similarity',
      'Shows count, keywords, and recency for each cluster',
      'Helps communities distinguish noise from recurring topics',
      'Makes collective attention visible before formal governance starts',
      'Creates clearer context for raising and claiming concerns',
    ],
  },
  {
    id: 'concern',
    number: 3,
    name: 'Concern',
    icon: <TriangleAlert size={24} className="text-primary" />,
    description: 'Named recurring issues linked to one or more signals',
    keyPoints: [
      'Represents a recurring problem the community recognizes',
      'Tracks affectedness and participating voices',
      'Can be claimed for a commons',
      'Acts as the bridge between observation and governance',
      'Keeps issue context grounded in real traces',
    ],
  },
  {
    id: 'commons',
    number: 4,
    name: 'Commons',
    icon: <LandPlot size={24} className="text-primary" />,
    description: 'Shared domain where concerns are governed together',
    keyPoints: [
      'Proposed and maintained as a shared space for coordination',
      'Links to relevant concerns',
      'Holds one or more user groups with distinct approaches',
      'Moves from issue recognition to concrete organizing',
      'Provides structure without requiring top-down hierarchy',
    ],
  },
  {
    id: 'user-group',
    number: 5,
    name: 'User Group',
    icon: <Users size={24} className="text-primary" />,
    description: 'People who organize around a commons and run work together',
    keyPoints: [
      'Formed within a commons to govern and coordinate execution',
      'Can create and track projects',
      'Membership and participation remain visible',
      'Supports different governance styles around the same commons',
      'Turns shared intent into sustained collaboration',
    ],
  },
  {
    id: 'project',
    number: 6,
    name: 'Project',
    icon: <FolderKanban size={24} className="text-primary" />,
    description: 'Concrete work items that evolve through amendments',
    keyPoints: [
      'Created by user groups as bounded efforts',
      'Amendments can be proposed and contested',
      'Status and change history remain transparent',
      'Keeps adaptation continuous instead of one-time voting',
      'Connects governance to execution and outcomes',
    ],
  },
];

export function AboutPage() {
  const navigate = useNavigate();
  const [expandedStage, setExpandedStage] = useState<string | null>(null);

  const toggleStage = (stageId: string) => {
    setExpandedStage(expandedStage === stageId ? null : stageId);
  };

  return (
    <div className="space-y-6">
      {/* Hero Section */}
      <div className="space-y-3">
        <h1 className="text-3xl sm:text-4xl font-bold text-foreground">
          About Binding Chaos
        </h1>
        <p className="text-lg text-muted-foreground max-w-2xl">
          A platform for making collective attention visible and turning it into shared governance.
        </p>
      </div>

      {/* Overview */}
      <Card
        title="What is Binding Chaos?"
        content={
          <div className="space-y-4">
            <p className="text-muted-foreground">
              Binding Chaos helps communities move from scattered signals to coordinated work without relying on opaque, top-down control. Instead of forcing everything through one linear voting process, it keeps each step visible and participatory.
            </p>
            <p className="text-muted-foreground">
              People can surface what they observe, discover recurring patterns, define concerns, claim concerns into commons, form user groups, and run projects that evolve through amendments and contestation.
            </p>
          </div>
        }
      />

      {/* Process Timeline */}
      <ProcessTimeline />

      {/* Detailed Stage Information */}
      <div className="space-y-4">
        <div>
          <h2 className="text-2xl font-bold text-foreground mb-4">
            Understanding Each Stage
          </h2>
          <p className="text-muted-foreground mb-4">
            Click each stage to see how discovery, governance, and execution connect in the current platform.
          </p>
        </div>

        {stages.map((stage) => (
          <StageCard
            key={stage.id}
            stageName={stage.name}
            stageNumber={stage.number}
            description={stage.description}
            keyPoints={stage.keyPoints}
            isExpanded={expandedStage === stage.id}
            onClick={() => toggleStage(stage.id)}
            icon={stage.icon}
          />
        ))}
      </div>

      {/* Why It Matters */}
      <Card
        title="Why This Matters"
        content={
          <div className="space-y-4">
            <div>
              <h3 className="font-semibold text-foreground mb-2">
                Discovery That Stays Grounded
              </h3>
              <p className="text-muted-foreground">
                Signals, clustering, and concerns keep the early phase tied to real observations instead of abstract agendas.
              </p>
            </div>

            <div>
              <h3 className="font-semibold text-foreground mb-2">
                Governance by User Group
              </h3>
              <p className="text-muted-foreground">
                Commons and user groups let people organize around what affects them, while still allowing multiple approaches to coexist.
              </p>
            </div>

            <div>
              <h3 className="font-semibold text-foreground mb-2">
                Transparent Adaptation
              </h3>
              <p className="text-muted-foreground">
                Projects evolve through visible amendments and contestation, so change is continuous, inspectable, and open to challenge.
              </p>
            </div>
          </div>
        }
      />

      {/* Getting Started */}
      <Card
        title="Ready to Participate?"
        content={
          <p className="text-muted-foreground mb-4">
            Start with a signal, explore emerging patterns, or join the governance side by working with concerns, commons, and projects.
          </p>
        }
        footer={
          <div className="flex gap-3 flex-wrap">
            <Button onClick={() => navigate('/signals')} variant="primary">
              Explore Signals
            </Button>
            <Button onClick={() => navigate('/commons')} variant="secondary">
              Browse Commons
            </Button>
          </div>
        }
      />
    </div>
  );
}
