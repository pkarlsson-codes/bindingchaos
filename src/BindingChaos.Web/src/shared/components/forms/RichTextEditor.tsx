import { useEditor, EditorContent } from '@tiptap/react';
import { useEffect } from 'react';
import StarterKit from '@tiptap/starter-kit';
import { Table } from '@tiptap/extension-table';
import { TableRow } from '@tiptap/extension-table-row';
import { TableCell } from '@tiptap/extension-table-cell';
import { TableHeader } from '@tiptap/extension-table-header';
import { Image } from '@tiptap/extension-image';
import { Link } from '@tiptap/extension-link';
import { 
  BoldIcon, 
  ItalicIcon, 
  ListBulletIcon, 
  ChatBubbleLeftRightIcon, 
  CodeBracketIcon,
  TableCellsIcon,
  LinkIcon,
  PhotoIcon,
  DocumentTextIcon,
  DocumentIcon,
  DocumentDuplicateIcon,
  StrikethroughIcon,
  UnderlineIcon
} from '@heroicons/react/24/outline';

interface RichTextEditorProps {
  content: string;
  onChange: (content: string) => void;
  placeholder?: string;
  disabled?: boolean;
  className?: string;
}

const MenuBar = ({ editor }: { editor: any }) => {
  if (!editor) {
    return null;
  }

  const addImage = () => {
    const url = window.prompt('Enter image URL:');
    if (url) {
      editor.chain().focus().setImage({ src: url }).run();
    }
  };

  const setLink = () => {
    const url = window.prompt('Enter URL:');
    if (url) {
      editor.chain().focus().setLink({ href: url }).run();
    }
  };

  const addTable = () => {
    editor.chain().focus().insertTable({ rows: 3, cols: 3, withHeaderRow: true }).run();
  };

  return (
    <div className="border-b border-border p-2 bg-muted rounded-t-lg">
      <div className="flex flex-wrap gap-1">
        {/* Headers */}
        <button
          onClick={() => editor.chain().focus().toggleHeading({ level: 1 }).run()}
          className={`p-2 rounded hover:bg-accent ${editor.isActive('heading', { level: 1 }) ? 'bg-primary/10 text-primary' : 'text-muted-foreground'}`}
          title="Heading 1"
        >
          <DocumentTextIcon className="h-4 w-4" />
        </button>
        <button
          onClick={() => editor.chain().focus().toggleHeading({ level: 2 }).run()}
          className={`p-2 rounded hover:bg-accent ${editor.isActive('heading', { level: 2 }) ? 'bg-primary/10 text-primary' : 'text-muted-foreground'}`}
          title="Heading 2"
        >
          <DocumentIcon className="h-4 w-4" />
        </button>
        <button
          onClick={() => editor.chain().focus().toggleHeading({ level: 3 }).run()}
          className={`p-2 rounded hover:bg-accent ${editor.isActive('heading', { level: 3 }) ? 'bg-primary/10 text-primary' : 'text-muted-foreground'}`}
          title="Heading 3"
        >
          <DocumentDuplicateIcon className="h-4 w-4" />
        </button>

        <div className="w-px h-6 bg-border mx-1" />

        {/* Text formatting */}
        <button
          onClick={() => editor.chain().focus().toggleBold().run()}
          className={`p-2 rounded hover:bg-accent ${editor.isActive('bold') ? 'bg-primary/10 text-primary' : 'text-muted-foreground'}`}
          title="Bold"
        >
          <BoldIcon className="h-4 w-4" />
        </button>
        <button
          onClick={() => editor.chain().focus().toggleItalic().run()}
          className={`p-2 rounded hover:bg-accent ${editor.isActive('italic') ? 'bg-primary/10 text-primary' : 'text-muted-foreground'}`}
          title="Italic"
        >
          <ItalicIcon className="h-4 w-4" />
        </button>
        <button
          onClick={() => editor.chain().focus().toggleUnderline().run()}
          className={`p-2 rounded hover:bg-accent ${editor.isActive('underline') ? 'bg-primary/10 text-primary' : 'text-muted-foreground'}`}
          title="Underline"
        >
          <UnderlineIcon className="h-4 w-4" />
        </button>
        <button
          onClick={() => editor.chain().focus().toggleStrike().run()}
          className={`p-2 rounded hover:bg-accent ${editor.isActive('strike') ? 'bg-primary/10 text-primary' : 'text-muted-foreground'}`}
          title="Strikethrough"
        >
          <StrikethroughIcon className="h-4 w-4" />
        </button>

        <div className="w-px h-6 bg-border mx-1" />

        {/* Lists */}
        <button
          onClick={() => editor.chain().focus().toggleBulletList().run()}
          className={`p-2 rounded hover:bg-accent ${editor.isActive('bulletList') ? 'bg-primary/10 text-primary' : 'text-muted-foreground'}`}
          title="Bullet List"
        >
          <ListBulletIcon className="h-4 w-4" />
        </button>
        <button
          onClick={() => editor.chain().focus().toggleOrderedList().run()}
          className={`p-2 rounded hover:bg-accent ${editor.isActive('orderedList') ? 'bg-primary/10 text-primary' : 'text-muted-foreground'}`}
          title="Numbered List"
        >
          <ListBulletIcon className="h-4 w-4" />
        </button>

        <div className="w-px h-6 bg-border mx-1" />

        {/* Block elements */}
        <button
          onClick={() => editor.chain().focus().toggleBlockquote().run()}
          className={`p-2 rounded hover:bg-accent ${editor.isActive('blockquote') ? 'bg-primary/10 text-primary' : 'text-muted-foreground'}`}
          title="Quote"
        >
          <ChatBubbleLeftRightIcon className="h-4 w-4" />
        </button>
        <button
          onClick={() => editor.chain().focus().toggleCodeBlock().run()}
          className={`p-2 rounded hover:bg-accent ${editor.isActive('codeBlock') ? 'bg-primary/10 text-primary' : 'text-muted-foreground'}`}
          title="Code Block"
        >
          <CodeBracketIcon className="h-4 w-4" />
        </button>

        <div className="w-px h-6 bg-border mx-1" />

        {/* Tables and media */}
        <button
          onClick={addTable}
          className="p-2 rounded hover:bg-accent text-muted-foreground"
          title="Insert Table"
        >
          <TableCellsIcon className="h-4 w-4" />
        </button>
        <button
          onClick={setLink}
          className={`p-2 rounded hover:bg-accent ${editor.isActive('link') ? 'bg-primary/10 text-primary' : 'text-muted-foreground'}`}
          title="Add Link"
        >
          <LinkIcon className="h-4 w-4" />
        </button>
        <button
          onClick={addImage}
          className="p-2 rounded hover:bg-accent text-muted-foreground"
          title="Insert Image"
        >
          <PhotoIcon className="h-4 w-4" />
        </button>
      </div>
    </div>
  );
};

export function RichTextEditor({ 
  content, 
  onChange, 
  placeholder = "Start writing your idea...",
  disabled = false,
  className = ""
}: RichTextEditorProps) {
  const editor = useEditor({
    extensions: [
      StarterKit.configure({
        link: false, // Disable the link extension from StarterKit to avoid duplicates
      }),
      Table.configure({
        resizable: true,
      }),
      TableRow,
      TableHeader,
      TableCell,
      Image,
      Link.configure({
        openOnClick: false,
        HTMLAttributes: {
          class: 'text-primary underline cursor-pointer',
        },
      }),
    ],
    content,
    editable: !disabled,
    onUpdate: ({ editor }) => {
      onChange(editor.getHTML());
    },
    editorProps: {
      attributes: {
        class: 'focus:outline-none min-h-[200px] px-4 py-3 text-foreground leading-relaxed',
      },
    },
  });

  useEffect(() => {
    if (editor && content !== editor.getHTML()) {
      editor.commands.setContent(content);
    }
  }, [content, editor]);

  return (
    <div className={`border border-border rounded-lg overflow-hidden relative ${className}`}>
      <MenuBar editor={editor} />
      <div className="relative">
        <EditorContent 
          editor={editor} 
          className={`${disabled ? 'bg-muted' : 'bg-background'}`}
        />
        {!editor?.getText().trim() && !disabled && (
          <div className="absolute top-3 left-4 text-muted-foreground pointer-events-none">
            {placeholder}
          </div>
        )}
      </div>
    </div>
  );
} 