import React from 'react';
import {
  Select as ShadcnSelect,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '../ui/select';

interface SelectItem {
  label: string;
  value: string;
  icon?: React.ReactNode;
  disabled?: boolean;
}

interface SelectProps {
  items: SelectItem[];
  value?: string;
  onChange: (value: string) => void;
  placeholder?: string;
  disabled?: boolean;
  className?: string;
}

export function Select({ 
  items, 
  value, 
  onChange, 
  placeholder = 'Select an option',
  disabled = false,
  className = ''
}: SelectProps) {
  return (
    <ShadcnSelect value={value} onValueChange={onChange} disabled={disabled}>
      <SelectTrigger className={className}>
        <SelectValue placeholder={placeholder} />
      </SelectTrigger>
      <SelectContent>
        {items.map((item) => (
          <SelectItem 
            key={item.value} 
            value={item.value}
            disabled={item.disabled}
          >
            <div className="flex items-center">
              {item.icon && (
                <span className="mr-3 h-5 w-5">{item.icon}</span>
              )}
              {item.label}
            </div>
          </SelectItem>
        ))}
      </SelectContent>
    </ShadcnSelect>
  );
} 