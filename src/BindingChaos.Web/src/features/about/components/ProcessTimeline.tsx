import { Card } from '../../../shared/components/layout/Card';
import { Lightbulb, Radar, TriangleAlert, LandPlot, Users, FolderKanban } from 'lucide-react';

interface Stage {
  name: string;
  icon: React.ReactNode;
  description: string;
}

const stages: Stage[] = [
  {
    name: 'Signal',
    icon: <Lightbulb size={32} />,
    description: 'Raw observations, media, and tags shared by participants'
  },
  {
    name: 'Pattern',
    icon: <Radar size={32} />,
    description: 'Automatic clustering that reveals recurring topics'
  },
  {
    name: 'Concern',
    icon: <TriangleAlert size={32} />,
    description: 'Named issue linked to related signals'
  },
  {
    name: 'Commons',
    icon: <LandPlot size={32} />,
    description: 'Shared domain where concerns are governed'
  },
  {
    name: 'User Group',
    icon: <Users size={32} />,
    description: 'Participants organizing work in the commons'
  },
  {
    name: 'Project',
    icon: <FolderKanban size={32} />,
    description: 'Execution unit that evolves through amendments'
  }
];

export function ProcessTimeline() {
  return (
    <Card
      title="How Binding Chaos Works"
      description="From signals to governance and project execution"
      content={
        <div className="w-full">
          {/* Mobile view (stacked vertically) */}
          <div className="md:hidden space-y-4">
            {stages.map((stage, index) => (
              <div key={index}>
                <div className="flex gap-4">
                  <div className="flex flex-col items-center gap-2">
                    <div className="bg-primary text-primary-foreground p-3 rounded-full text-muted-foreground">
                      {stage.icon}
                    </div>
                    {index < stages.length - 1 && (
                      <div className="w-1 h-8 bg-border" />
                    )}
                  </div>
                  <div className="pt-2">
                    <h3 className="font-bold text-foreground text-lg">
                      {stage.name}
                    </h3>
                    <p className="text-sm text-muted-foreground mt-1">
                      {stage.description}
                    </p>
                  </div>
                </div>
              </div>
            ))}
          </div>

          {/* Desktop view (horizontal) */}
          <div className="hidden md:flex gap-2 items-stretch">
            {stages.map((stage, index) => (
              <div key={index} className="flex-1 flex flex-col items-center">
                <div className="flex flex-col items-center mb-4 w-full">
                  <div className="bg-primary text-primary-foreground p-4 rounded-full mb-3 flex items-center justify-center">
                    {stage.icon}
                  </div>
                  <h3 className="font-bold text-foreground text-center">
                    {stage.name}
                  </h3>
                  <p className="text-xs text-muted-foreground text-center mt-2 px-2">
                    {stage.description}
                  </p>
                </div>

                {index < stages.length - 1 && (
                  <div className="flex-1 flex items-center w-full mb-4">
                    <div className="flex-1 border-t-2 border-muted"></div>
                    <div className="text-muted-foreground mx-2">→</div>
                  </div>
                )}
              </div>
            ))}
          </div>
        </div>
      }
    />
  );
}
