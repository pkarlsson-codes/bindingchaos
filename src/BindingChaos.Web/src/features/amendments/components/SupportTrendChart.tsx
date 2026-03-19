import { useState, useMemo } from 'react';
import { Card } from '../../../shared/components/layout/Card';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, BarChart, Bar, Brush, AreaChart, Area } from 'recharts';
import { ToggleGroup, ToggleGroupItem } from '../../../shared/components/ui/ui/toggle-group';
import type { TrendPointResponse } from '../../../api/models';

interface SupportTrendChartProps {
  data: TrendPointResponse[];
  title?: string;
  amendmentCreatedAt?: string; // ISO 8601 date string for amendment creation time
}

type ChartView = 'cumulative' | 'daily' | 'ratio';

export function SupportTrendChart({ data, title = "Support Trend", amendmentCreatedAt }: SupportTrendChartProps) {
  const [chartView, setChartView] = useState<ChartView>('cumulative');

  if (!data || data.length === 0) {
    return (
      <Card
        title={title}
        content={
          <div className="text-center py-8">
            <p className="text-muted-foreground text-sm">No trend data available</p>
          </div>
        }
      />
    );
  }

  // Transform raw vote events into chart data
  const chartData = useMemo(() => {
    const sortedData = data
      .filter(item => item.date && item.eventType)
      .sort((a, b) => new Date(a.date!).getTime() - new Date(b.date!).getTime());

    if (chartView === 'ratio') {
      // Transform data for ratio chart - calculate running totals and ratio
      let runningSupporters = 0;
      let runningOpponents = 0;
      
      return sortedData.map(item => {
        const date = new Date(item.date!);
        const timestamp = date.getTime();
        
        // Update running totals based on vote type
        switch (item.eventType) {
          case 'support':
            runningSupporters += 1;
            break;
          case 'oppose':
            runningOpponents += 1;
            break;
          case 'withdraw_support':
            runningSupporters = Math.max(0, runningSupporters - 1);
            break;
          case 'withdraw_oppose':
            runningOpponents = Math.max(0, runningOpponents - 1);
            break;
        }
        
                 // Calculate net support percentage (-100% to +100%)
         const totalVotes = runningSupporters + runningOpponents;
         const netSupport = totalVotes === 0 ? 0 : ((runningSupporters - runningOpponents) / totalVotes) * 100;
         
         return {
           timestamp,
           date: date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' }),
           netSupport: Math.round(netSupport * 100) / 100, // Round to 2 decimal places
           supporters: runningSupporters,
           opponents: runningOpponents
         };
      });
         } else if (chartView === 'cumulative') {
       // Cumulative totals logic - each data point represents the running total
       let runningSupporters = 0;
       let runningOpponents = 0;
       
       return sortedData.map(item => {
         const date = new Date(item.date!);
         const timestamp = date.getTime();
         
         // Update running totals based on vote type
         switch (item.eventType) {
           case 'support':
             runningSupporters += 1;
             break;
           case 'oppose':
             runningOpponents += 1;
             break;
           case 'withdraw_support':
             runningSupporters = Math.max(0, runningSupporters - 1);
             break;
           case 'withdraw_oppose':
             runningOpponents = Math.max(0, runningOpponents - 1);
             break;
         }
         
         return {
           timestamp,
           date: date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' }),
           supporters: runningSupporters,
           opponents: runningOpponents
         };
       });
    } else {
      // Daily changes logic
      return sortedData.reduce((acc, item) => {
        const date = new Date(item.date!);
        const dayKey = date.toISOString().split('T')[0];
        
        let existingDay = acc.find(d => d.dayKey === dayKey);
        if (!existingDay) {
          existingDay = {
            dayKey,
            date: date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' }),
            supporters: 0,
            opponents: 0
          };
          acc.push(existingDay);
        }
        
        switch (item.eventType) {
          case 'support':
            existingDay.supporters += 1;
            break;
          case 'oppose':
            existingDay.opponents += 1;
            break;
          case 'withdraw_support':
            existingDay.supporters -= 1;
            break;
          case 'withdraw_oppose':
            existingDay.opponents -= 1;
            break;
        }
        
        return acc;
      }, [] as Array<{ dayKey: string; date: string; supporters: number; opponents: number }>);
    }
  }, [data, chartView]);

  // Add initial data point at amendment creation time if provided
  const finalChartData = useMemo(() => {
    if (!amendmentCreatedAt) return chartData;

    const creationDate = new Date(amendmentCreatedAt);
    const creationDayKey = creationDate.toISOString().split('T')[0];
    
         if (chartView === 'ratio') {
       // For net support chart, add a starting point at creation time
       const netSupportData = chartData as Array<{ timestamp: number; date: string; netSupport: number; supporters: number; opponents: number }>;
       
                // Check if we need to add a starting point at creation time
         const creationTimestamp = creationDate.getTime();
         const hasCreationPoint = netSupportData.some((d: any) => d.timestamp >= creationTimestamp);
         
                 if (!hasCreationPoint) {
            return [{
              timestamp: creationTimestamp,
              date: creationDate.toLocaleDateString('en-US', { month: 'short', day: 'numeric' }),
              netSupport: 0,
              supporters: 0,
              opponents: 0
            }, ...netSupportData];
        } else {
          return netSupportData;
        }
                   } else if (chartView === 'cumulative') {
        const cumulativeData = chartData as Array<{ timestamp: number; date: string; supporters: number; opponents: number }>;
        
        // Check if we need to add a starting point at creation time
        const creationTimestamp = creationDate.getTime();
        const hasCreationPoint = cumulativeData.some(d => d.timestamp >= creationTimestamp);
        
        if (!hasCreationPoint) {
          return [{
            timestamp: creationTimestamp,
            date: creationDate.toLocaleDateString('en-US', { month: 'short', day: 'numeric' }),
            supporters: 0,
            opponents: 0
          }, ...cumulativeData];
        } else {
          return cumulativeData;
        }
    } else {
      const dailyData = chartData as Array<{ dayKey: string; date: string; supporters: number; opponents: number }>;
      const existingCreationDay = dailyData.find(d => d.dayKey === creationDayKey);
      
      if (!existingCreationDay) {
        return [{
          dayKey: creationDayKey,
          date: creationDate.toLocaleDateString('en-US', { month: 'short', day: 'numeric' }),
          supporters: 0,
          opponents: 0
        }, ...dailyData];
      } else {
        return dailyData.map(day => 
          day.dayKey === creationDayKey 
            ? { ...day, supporters: Math.max(0, day.supporters), opponents: Math.max(0, day.opponents) }
            : day
        );
      }
    }
  }, [chartData, amendmentCreatedAt, chartView]);

     const renderChart = () => {
     if (chartView === 'ratio') {
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
               allowDecimals={true}
               domain={[-100, 100]}
               tickFormatter={(value) => `${value}%`}
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
             formatter={(value, name, props) => {
               const dataPoint = props.payload;
               const netSupportValue = typeof value === 'number' ? value.toFixed(1) : '0.0';
               const sentiment = typeof value === 'number' ? (value > 0 ? 'support' : value < 0 ? 'opposition' : 'neutral') : 'neutral';
               return [
                 `${netSupportValue}% ${sentiment} (${dataPoint?.supporters || 0} supporters, ${dataPoint?.opponents || 0} opponents)`,
                 'Net Support'
               ];
             }}
             labelStyle={{ color: 'hsl(var(--muted-foreground))' }}
           />
           <Legend 
             wrapperStyle={{ fontSize: '12px' }}
             iconType="line"
           />
                        <Line 
               type="monotone" 
               dataKey="netSupport" 
               stroke="#6366f1" 
               strokeWidth={2}
               dot={{ fill: '#6366f1', strokeWidth: 2, r: 4 }}
               activeDot={{ r: 6, stroke: '#6366f1', strokeWidth: 2 }}
               name="Net Support"
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
                   } else if (chartView === 'cumulative') {
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
              labelStyle={{ color: 'hsl(var(--muted-foreground))' }}
            />
            <Legend 
              wrapperStyle={{ fontSize: '12px' }}
              iconType="line"
            />
            <Line 
              type="monotone" 
              dataKey="supporters" 
              stroke="#10b981" 
              strokeWidth={2}
              dot={{ fill: '#10b981', strokeWidth: 2, r: 4 }}
              activeDot={{ r: 6, stroke: '#10b981', strokeWidth: 2 }}
              name="Supporters"
            />
            <Line 
              type="monotone" 
              dataKey="opponents" 
              stroke="#ef4444" 
              strokeWidth={2}
              dot={{ fill: '#ef4444', strokeWidth: 2, r: 4 }}
              activeDot={{ r: 6, stroke: '#ef4444', strokeWidth: 2 }}
              name="Opponents"
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
             domain={['auto', 'auto']}
           />
          <Tooltip 
            contentStyle={{
              backgroundColor: 'hsl(var(--background))',
              border: '1px solid hsl(var(--border))',
              borderRadius: '6px',
              color: 'hsl(var(--foreground))'
            }}
            labelStyle={{ color: 'hsl(var(--muted-foreground))' }}
          />
          <Legend 
            wrapperStyle={{ fontSize: '12px' }}
          />
          <Bar 
            dataKey="supporters" 
            fill="#10b981" 
            name="Supporters"
            radius={[4, 4, 0, 0]}
          />
          <Bar 
            dataKey="opponents" 
            fill="#ef4444" 
            name="Opponents"
            radius={[4, 4, 0, 0]}
          />
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
              Daily Changes
            </ToggleGroupItem>
                         <ToggleGroupItem
               value="ratio"
               className="hover:bg-green-100 hover:text-green-800 data-[state=on]:bg-green-600 data-[state=on]:text-white"
             >
               Net Support
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
