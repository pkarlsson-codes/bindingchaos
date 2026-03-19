import { Card } from '../../../shared/components/layout/Card';
import { Button } from '../../../shared/components/layout/Button';
import { ChevronDown, ChevronUp } from 'lucide-react';

interface StageCardProps {
  stageName: string;
  stageNumber: number;
  description: string;
  keyPoints: string[];
  isExpanded: boolean;
  onClick: () => void;
  icon: React.ReactNode;
}

export function StageCard({
  stageName,
  stageNumber,
  description,
  keyPoints,
  isExpanded,
  onClick,
  icon,
}: StageCardProps) {
  return (
    <div
      className="border border-border rounded-lg overflow-hidden transition-all cursor-pointer hover:shadow-md"
      onClick={onClick}
    >
      <div className="bg-muted p-4 sm:p-6">
        <div className="flex items-start justify-between">
          <div className="flex items-start gap-4">
            <div className="flex-shrink-0 text-primary mt-1">{icon}</div>
            <div>
              <div className="text-sm font-semibold text-muted-foreground">
                Stage {stageNumber}
              </div>
              <h3 className="text-xl font-bold text-foreground mt-1">
                {stageName}
              </h3>
              <p className="text-sm text-muted-foreground mt-2">{description}</p>
            </div>
          </div>
          <div className="text-muted-foreground flex-shrink-0">
            {isExpanded ? <ChevronUp size={24} /> : <ChevronDown size={24} />}
          </div>
        </div>
      </div>

      {isExpanded && (
        <div className="border-t border-border p-4 sm:p-6 bg-background">
          <div className="space-y-4">
            <div>
              <h4 className="font-semibold text-foreground mb-3">
                Key Points:
              </h4>
              <ul className="space-y-2">
                {keyPoints.map((point, index) => (
                  <li key={index} className="flex gap-3">
                    <span className="text-primary font-bold flex-shrink-0">
                      •
                    </span>
                    <span className="text-muted-foreground">{point}</span>
                  </li>
                ))}
              </ul>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
