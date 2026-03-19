import { Card } from '../../../shared/components/layout/Card';

interface SupportStatisticsCardProps {
  totalSupporters: number;
  totalOpponents: number;
}

export function SupportStatisticsCard({ totalSupporters, totalOpponents }: SupportStatisticsCardProps) {
  const netSupport = totalSupporters - totalOpponents;
  
  return (
    <Card
      title="Support Statistics"
      content={
        <div className="space-y-3 text-sm">
          <div className="flex justify-between">
            <span>Total Supporters:</span>
            <span className="font-medium">{totalSupporters}</span>
          </div>
          <div className="flex justify-between">
            <span>Total Opponents:</span>
            <span className="font-medium">{totalOpponents}</span>
          </div>
          <div className="flex justify-between">
            <span>Net Support:</span>
            <span className={`font-medium ${netSupport >= 0 ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'}`}>
              {netSupport}
            </span>
          </div>
        </div>
      }
    />
  );
} 