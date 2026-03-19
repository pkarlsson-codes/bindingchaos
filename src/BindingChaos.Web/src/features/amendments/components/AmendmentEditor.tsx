import { RichTextEditor } from '../../../shared/components/forms/RichTextEditor';

interface AmendmentEditorProps {
  content: string;
  onChange: (content: string) => void;
  placeholder?: string;
  disabled?: boolean;
  className?: string;
}

export function AmendmentEditor({
  content,
  onChange,
  placeholder = "Start writing your amendment...",
  disabled = false,
  className = ""
}: AmendmentEditorProps) {
  return (
    <div className={className}>
      <label className="block text-sm font-medium text-foreground mb-2">
        Proposed Content
      </label>
      <RichTextEditor
        content={content}
        onChange={onChange}
        placeholder={placeholder}
        disabled={disabled}
      />
    </div>
  );
}
