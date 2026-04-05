import { useState, useMemo } from 'react';
import { Card } from '../../../shared/components/layout/Card';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, Brush, BarChart, Bar, Cell } from 'recharts';
import { ToggleGroup, ToggleGroupItem } from '../../../shared/components/ui/ui/toggle-group';

interface TrendPoint {
  date?: string;
  eventType?: 'amplify' | 'attenuate' | string;
}

interface AmplificationTrendChartProps {
  data: TrendPoint[];
  title?: string;
  signalCreatedAt?: string; // ISO 8601 date string for signal creation time
}

type ChartView = 'cumulative' | 'daily';

export function AmplificationTrendChart({ data, title = "Amplification Trend", signalCreatedAt }: AmplificationTrendChartProps) {
  const [chartView, setChartView] = useState<ChartView>('cumulative');

  if (!data || data.length === 0) {
    return (
      <Card
        title={title}
        content={
          <div className="text-center py-8">
            <p className="text-muted-foreground text-sm">No amplification data available</p>
          </div>
        }
      />
    );
  }

  // Transform raw amplification events into chart data
  const chartData = useMemo(() => {
    const sortedData = data
      .filter(item => item.date && item.eventType)
      .sort((a, b) => new Date(a.date!).getTime() - new Date(b.date!).getTime());

    if (chartView === 'cumulative') {
      // Cumulative totals logic - each data point represents the running total
      let runningAmplifications = 0;
      
      return sortedData.map(item => {
        const date = new Date(item.date!);
        const timestamp = date.getTime();
        
        // Update running total based on event type
        switch (item.eventType) {
          case 'amplify':
            runningAmplifications += 1;
            break;
          case 'attenuate':
            runningAmplifications = Math.max(0, runningAmplifications - 1);
            break;
        }
        
        return {
          timestamp,
          date: date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' }),
          amplifications: runningAmplifications
        };
      });
         } else {
       // Daily changes logic - aggregate by day with +1 for amplifications, -1 for attenuations
       return sortedData.reduce((acc, item) => {
         const date = new Date(item.date!);
         const dayKey = date.toISOString().split('T')[0];
         
         let dayData = acc.find((d) => d.dayKey === dayKey);
         if (!dayData) {
           dayData = {
             dayKey,
             date: date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' }),
             amplifications: 0
           };
           acc.push(dayData);
         }
         
         switch (item.eventType) {
           case 'amplify':
             dayData.amplifications += 1;
             break;
           case 'attenuate':
             dayData.amplifications -= 1;
             break;
         }
         
         return acc;
       }, [] as Array<{ dayKey: string; date: string; amplifications: number }>)
      .sort((a, b) => a.dayKey.localeCompare(b.dayKey));
     }
  }, [data, chartView]);

  // Add initial data point at signal creation time if provided
  const finalChartData = useMemo(() => {
    if (!signalCreatedAt) return chartData;

    const creationDate = new Date(signalCreatedAt);
    const creationDayKey = creationDate.toISOString().split('T')[0];
    
    if (chartView === 'cumulative') {
      const cumulativeData = chartData as Array<{ timestamp: number; date: string; amplifications: number }>;
      
      // Check if we need to add a starting point at creation time
      const creationTimestamp = creationDate.getTime();
      const hasCreationPoint = cumulativeData.some(d => d.timestamp >= creationTimestamp);
      
      if (!hasCreationPoint) {
        return [{
          timestamp: creationTimestamp,
          date: creationDate.toLocaleDateString('en-US', { month: 'short', day: 'numeric' }),
          amplifications: 0
        }, ...cumulativeData];
      } else {
        return cumulativeData;
      }
         } else {
       const dailyData = chartData as Array<{ dayKey: string; date: string; amplifications: number }>;
       const existingCreationDay = dailyData.find(d => d.dayKey === creationDayKey);
       
               let result;
        if (!existingCreationDay) {
          // Only add creation day if there are no events on that day
          result = [{
            dayKey: creationDayKey,
            date: creationDate.toLocaleDateString('en-US', { month: 'short', day: 'numeric' }),
            amplifications: 0
          }, ...dailyData];
        } else {
          // If creation day already exists in the data, don't modify it
          result = dailyData;
        }
        
        // Filter out days with zero net change after all processing
        return result.filter(day => day.amplifications !== 0);
     }
  }, [chartData, signalCreatedAt, chartView]);

  const renderChart = () => {
    if (chartView === 'cumulative') {
      return (
        <LineChart data={finalChartData}>
          <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
          <XAxis 
            dataKey="timestamp" 
            type="number"
            domain={['dataMin', 'dataMax']}
            tickFormatter={(value) => {
              const date = new Date(value);
              return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
            }}
            stroke="#6b7280"
            fontSize={12}
            tickLine={false}
            axisLine={false}
          />
          <YAxis 
            stroke="#6b7280"
            fontSize={12}
            tickLine={false}
            axisLine={false}
            allowDecimals={false}
            domain={[0, 'auto']}
          />
          <Tooltip 
            contentStyle={{
              backgroundColor: 'hsl(var(--background))',
              border: '1px solid hsl(var(--border))',
              borderRadius: '6px',
              color: 'hsl(var(--foreground))'
            }}
            labelFormatter={(value) => {
              const date = new Date(value);
              return date.toLocaleDateString('en-US', { 
                weekday: 'short',
                month: 'short', 
                day: 'numeric',
                year: 'numeric'
              });
            }}
            formatter={(value, name) => [
              `${value} amplifications`,
              'Active Amplifications'
            ]}
            labelStyle={{ color: 'hsl(var(--muted-foreground))' }}
          />
          <Legend 
            wrapperStyle={{ fontSize: '12px' }}
            iconType="line"
          />
          <Line 
            type="monotone" 
            dataKey="amplifications" 
            stroke="#6366f1" 
            strokeWidth={2}
            dot={{ fill: '#6366f1', strokeWidth: 2, r: 4 }}
            activeDot={{ r: 6, stroke: '#6366f1', strokeWidth: 2 }}
            name="Active Amplifications"
          />
          <Brush 
            dataKey="timestamp"
            height={30}
            stroke="#8884d8"
            fill="hsl(var(--muted))"
            tickFormatter={(value) => {
              const date = new Date(value);
              return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
            }}
          />
        </LineChart>
      );
    } else {
      return (
        <BarChart data={finalChartData}>
          <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
          <XAxis 
            dataKey="date" 
            stroke="#6b7280"
            fontSize={12}
            tickLine={false}
            axisLine={false}
          />
                     <YAxis 
             stroke="#6b7280"
             fontSize={12}
             tickLine={false}
             axisLine={false}
             allowDecimals={false}
             domain={[0, 'auto']}
           />
          <Tooltip 
            contentStyle={{
              backgroundColor: 'hsl(var(--background))',
              border: '1px solid hsl(var(--border))',
              borderRadius: '6px',
              color: 'hsl(var(--foreground))'
            }}
            formatter={(value: any, name: any) => [
              `${(value as number) > 0 ? '+' : ''}${value} amplifications`,
              'Daily Net Amplifications'
            ]}
            labelStyle={{ color: 'hsl(var(--muted-foreground))' }}
          />
          <Legend 
            wrapperStyle={{ fontSize: '12px' }}
            iconType="rect"
          />
          <Bar 
            dataKey="amplifications" 
            fill="#6366f1"
            name="Daily Net Amplifications"
          >
            {finalChartData.map((entry: { amplifications: number }, index: number) => (
              <Cell 
                key={`cell-${index}`}
                fill={entry.amplifications > 0 ? '#10b981' : entry.amplifications < 0 ? '#ef4444' : '#6b7280'}
              />
            ))}
          </Bar>
        </BarChart>
      );
    }
  };

  return (
    <Card
      title={
        <div className="flex items-center justify-between">
          <span>{title}</span>
          <ToggleGroup 
            type="single" 
            value={chartView} 
            onValueChange={(value) => value && setChartView(value as ChartView)}
            variant="outline" 
            size="sm"
          >
            <ToggleGroupItem
              value="cumulative"
              className="hover:bg-blue-100 hover:text-blue-800 data-[state=on]:bg-blue-600 data-[state=on]:text-white"
            >
              Cumulative
            </ToggleGroupItem>
            <ToggleGroupItem
              value="daily"
              className="hover:bg-purple-100 hover:text-purple-800 data-[state=on]:bg-purple-600 data-[state=on]:text-white"
            >
              Daily Bars
            </ToggleGroupItem>
          </ToggleGroup>
        </div>
      }
      content={
        <div className="w-full h-64">
          <ResponsiveContainer width="100%" height="100%">
            {renderChart()}
          </ResponsiveContainer>
        </div>
      }
    />
  );
}
