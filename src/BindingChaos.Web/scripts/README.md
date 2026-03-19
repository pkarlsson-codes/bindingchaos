# Dead Code Detection System

This directory contains scripts to detect and remove dead code from the frontend codebase.

## Scripts

### Basic Dead Code Detector (`dead-code-detector.js`)

A simple dead code detector that finds unused files and exports.

**Usage:**
```bash
# List dead code (default)
npm run dead-code:list

# Remove unused files
npm run dead-code:remove
```

### Advanced Dead Code Detector (`advanced-dead-code-detector.js`)

A more comprehensive dead code detector that finds:
- Unused files
- Unused exports within files
- Unused imports

**Usage:**
```bash
# List all dead code (including unused imports)
npm run dead-code:advanced
```

## NPM Scripts

The following npm scripts are available in `package.json`:

- `dead-code:list` - List all dead code found
- `dead-code:remove` - Automatically remove unused files
- `dead-code:advanced` - Run advanced analysis (includes unused imports)
- `dead-code:clean` - Remove dead code and run linting

## What Gets Ignored

The scripts automatically ignore:
- Test files (`.test.ts`, `.test.tsx`, `.spec.ts`, `.spec.tsx`)
- Story files (`.stories.ts`, `.stories.tsx`)
- Configuration files (`.config.js`, `.config.ts`)
- Auto-generated API files (`api/models/`, `api/apis/`, `api/runtime.ts`, `api/index.ts`)
- Type definition files (`.d.ts`)
- Main entry points (`main.tsx`, `App.tsx`, `index.ts`)

## How It Works

1. **File Scanning**: Recursively scans the `src` directory for TypeScript/React files
2. **Export Analysis**: Extracts all exported functions, classes, interfaces, etc.
3. **Import Analysis**: Tracks which files import from other files
4. **Usage Detection**: Determines which exports are actually used
5. **Reporting**: Provides detailed reports of unused code

## Safety Features

- **Dry Run**: The `--list` flag shows what would be removed without actually deleting files
- **Confirmation**: The `--remove` flag must be explicitly used to delete files
- **Error Handling**: Gracefully handles file read errors and continues scanning
- **Backup**: Always review the output before running with `--remove`

## Best Practices

1. **Regular Checks**: Run `npm run dead-code:list` regularly to catch dead code early
2. **Review Before Removal**: Always review the list before running `npm run dead-code:remove`
3. **Test After Removal**: Run tests after removing dead code to ensure nothing breaks
4. **Use Advanced Mode**: Use `npm run dead-code:advanced` for more thorough analysis

## Limitations

- The basic detector may have false positives for complex import/export patterns
- The advanced detector is more accurate but slower
- Auto-generated files (like API models) are ignored to prevent breaking the build
- Some dynamic imports may not be detected

## Troubleshooting

If you encounter issues:

1. **False Positives**: Some files may appear unused but are actually needed (e.g., CSS modules, dynamic imports)
2. **Build Errors**: If removing files causes build errors, restore them and investigate the dependency
3. **Performance**: The advanced detector may be slow on large codebases

## Contributing

To improve the dead code detection:

1. Add new patterns to the ignore lists if needed
2. Improve the regex patterns for better export/import detection
3. Add support for new file types or import patterns
