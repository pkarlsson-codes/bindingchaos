import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card } from '../../../shared/components/layout/Card';
import { Button } from '../../../shared/components/layout/Button';
import { ProcessTimeline } from './ProcessTimeline';
import { StageCard } from './StageCard';
import { Lightbulb, BookOpen, Edit3, Zap } from 'lucide-react';

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
    description: 'Initial observations or ideas from community members',
    keyPoints: [
      'Quick, informal way to share observations',
      'Captures emerging patterns and concerns',
      'Gets amplified when others resonate with it',
      'Low barrier to entry for community input',
      'Forms the foundation for more structured ideas',
    ],
  },
  {
    id: 'idea',
    number: 2,
    name: 'Idea',
    icon: <BookOpen size={24} className="text-primary" />,
    description: 'Formalized proposals that collect support',
    keyPoints: [
      'Signals that have been refined into formal proposals',
      'Attracts supporters and contributors',
      'Can be amended and improved based on feedback',
      'Tracks support trends over time',
      'Represents a shared vision for potential change',
    ],
  },
  {
    id: 'amendment',
    number: 3,
    name: 'Amendment',
    icon: <Edit3 size={24} className="text-primary" />,
    description: 'Specific changes built on ideas',
    keyPoints: [
      'Concrete proposals derived from ideas',
      'Detailed specifications and reasoning',
      'Allows supporters and opponents to weigh in',
      'Tracks voting and sentiment over time',
      'Refined through community discussion and feedback',
    ],
  },
  {
    id: 'action',
    number: 4,
    name: 'Action',
    icon: <Zap size={24} className="text-primary" />,
    description: 'Concrete steps and opportunities',
    keyPoints: [
      'Real-world implementation opportunities',
      'Specific actions individuals can take',
      'Tracks progress and outcomes',
      'Connects ideas to tangible change',
      'Enables community coordination around shared goals',
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
          A platform for communities to collaborate, deliberate, and take action together.
        </p>
      </div>

      {/* Overview */}
      <Card
        title="What is Binding Chaos?"
        content={
          <div className="space-y-4">
            <p className="text-muted-foreground">
              Binding Chaos is a democratic governance platform designed to help communities turn scattered ideas and concerns into organized action. It bridges the gap between individual observations and collective decision-making.
            </p>
            <p className="text-muted-foreground">
              The platform recognizes that meaningful change requires more than good intentions—it requires a structured way for diverse voices to contribute, propose, deliberate, and coordinate.
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
            Click on each stage to learn more about its role in the process.
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
                Bridging Ideas and Action
              </h3>
              <p className="text-muted-foreground">
                Many communities struggle to move from identifying problems to taking action. Binding Chaos provides the structure and tools to make that transition smoother.
              </p>
            </div>

            <div>
              <h3 className="font-semibold text-foreground mb-2">
                Inclusive Participation
              </h3>
              <p className="text-muted-foreground">
                Whether someone wants to share a quick observation (signal) or engage in detailed deliberation (amendments), there's a meaningful way to contribute at every level.
              </p>
            </div>

            <div>
              <h3 className="font-semibold text-foreground mb-2">
                Transparent Decision-Making
              </h3>
              <p className="text-muted-foreground">
                Every stage of the process is visible, traceable, and built on community input. Changes are documented, and the reasoning is preserved.
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
            Start by sharing your observations and ideas with your community. Whether you're just beginning or ready to dive deep into amendments, there's a place for you in this process.
          </p>
        }
        footer={
          <Button onClick={() => navigate('/signals')} variant="primary">
            Explore Signals
          </Button>
        }
      />
    </div>
  );
}
