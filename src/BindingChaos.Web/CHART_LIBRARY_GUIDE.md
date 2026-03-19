# Chart Library Guide - Recharts

## Overview

We use **Recharts** as our primary charting library for the following reasons:

- **React-native**: Declarative API that fits our React patterns
- **TypeScript support**: Excellent type safety
- **Small bundle size**: ~50KB gzipped
- **Highly customizable**: Easy to match our design system
- **Active community**: Well-maintained with good documentation

## Installation

```bash
npm install recharts
```

## Basic Usage

### Line Chart Example

```tsx
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

interface ChartData {
  date: string;
  value1: number;
  value2: number;
}

function MyChart({ data }: { data: ChartData[] }) {
  return (
    <div className="w-full h-64">
      <ResponsiveContainer width="100%" height="100%">
        <LineChart data={data}>
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
            iconType="line"
          />
          <Line 
            type="monotone" 
            dataKey="value1" 
            stroke="#10b981" 
            strokeWidth={2}
            dot={{ fill: '#10b981', strokeWidth: 2, r: 4 }}
            activeDot={{ r: 6, stroke: '#10b981', strokeWidth: 2 }}
            name="Value 1"
          />
          <Line 
            type="monotone" 
            dataKey="value2" 
            stroke="#ef4444" 
            strokeWidth={2}
            dot={{ fill: '#ef4444', strokeWidth: 2, r: 4 }}
            activeDot={{ r: 6, stroke: '#ef4444', strokeWidth: 2 }}
            name="Value 2"
          />
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
}
```

## Common Chart Types

### Bar Chart
```tsx
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

<BarChart data={data}>
  <CartesianGrid strokeDasharray="3 3" />
  <XAxis dataKey="name" />
  <YAxis />
  <Tooltip />
  <Legend />
  <Bar dataKey="value" fill="#10b981" />
</BarChart>
```

### Area Chart
```tsx
import { AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';

<AreaChart data={data}>
  <CartesianGrid strokeDasharray="3 3" />
  <XAxis dataKey="name" />
  <YAxis />
  <Tooltip />
  <Area type="monotone" dataKey="value" stroke="#10b981" fill="#10b981" fillOpacity={0.3} />
</AreaChart>
```

### Pie Chart
```tsx
import { PieChart, Pie, Cell, Tooltip, ResponsiveContainer } from 'recharts';

const COLORS = ['#10b981', '#ef4444', '#3b82f6', '#f59e0b'];

<PieChart>
  <Pie
    data={data}
    cx="50%"
    cy="50%"
    labelLine={false}
    label={renderCustomizedLabel}
    outerRadius={80}
    fill="#8884d8"
    dataKey="value"
  >
    {data.map((entry, index) => (
      <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
    ))}
  </Pie>
  <Tooltip />
</PieChart>
```

## Design System Integration

### Colors
Use our design system colors:
- **Primary**: `#10b981` (green)
- **Destructive**: `#ef4444` (red)
- **Secondary**: `#3b82f6` (blue)
- **Warning**: `#f59e0b` (yellow)

### Typography
- **Axis labels**: `fontSize={12}`
- **Legend**: `fontSize={12}`
- **Tooltip**: Inherits from design system

### Spacing
- **Container height**: `h-64` (256px) for standard charts
- **Responsive**: Always use `ResponsiveContainer` for mobile compatibility

## Best Practices

1. **Always use ResponsiveContainer**: Ensures charts work on all screen sizes
2. **Consistent styling**: Use the same colors and typography across all charts
3. **Accessibility**: Include proper ARIA labels and keyboard navigation
4. **Performance**: For large datasets, consider data sampling or pagination
5. **Empty states**: Always handle cases where data is empty or loading

## Common Patterns

### Loading State
```tsx
if (isLoading) {
  return (
    <Card title="Chart Title">
      <div className="animate-pulse h-64 bg-muted rounded" />
    </Card>
  );
}
```

### Empty State
```tsx
if (!data || data.length === 0) {
  return (
    <Card title="Chart Title">
      <div className="text-center py-8">
        <p className="text-muted-foreground text-sm">No data available</p>
      </div>
    </Card>
  );
}
```

### Error State
```tsx
if (error) {
  return (
    <Card title="Chart Title">
      <div className="text-center py-8">
        <p className="text-destructive text-sm">Failed to load chart data</p>
      </div>
    </Card>
  );
}
```

## Resources

- [Recharts Documentation](https://recharts.org/)
- [Recharts Examples](https://recharts.org/en-US/examples)
- [GitHub Repository](https://github.com/recharts/recharts)
