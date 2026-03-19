#!/usr/bin/env node

import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const SRC_DIR = path.join(__dirname, '..', 'src');
const IGNORED_DIRS = ['node_modules', '.git', 'dist', 'build', '__tests__', 'test', 'tests'];
const IGNORED_FILES = ['main.tsx', 'App.tsx', 'vite-env.d.ts'];
const IGNORED_PATTERNS = [
  /\.d\.ts$/,
  /\.test\.(ts|tsx)$/,
  /\.spec\.(ts|tsx)$/,
  /\.stories\.(ts|tsx)$/,
  /\.config\.(js|ts)$/,
  /\.setup\.(js|ts)$/,
  /api\/models\/.*\.ts$/, // Auto-generated API models
  /api\/apis\/.*\.ts$/,   // Auto-generated API classes
  /api\/runtime\.ts$/,    // Auto-generated runtime
  /api\/index\.ts$/,      // Auto-generated index
  /api\/apis\/index\.ts$/, // Auto-generated API index
  /shared\/components\/ui\/.*\.tsx$/, // UI components (may be used dynamically)
  /shared\/components\/ui\/ui\/.*\.tsx$/, // Nested UI components
];

// Track all exports and imports
const exports = new Map(); // file -> Map of exportName -> exportType
const imports = new Map(); // file -> Set of imported files
const namedImports = new Map(); // file -> Map of importedFile -> Set of imported names
const files = new Set();

function shouldIgnoreFile(filePath) {
  const relativePath = path.relative(SRC_DIR, filePath);
  const fileName = path.basename(filePath);
  
  // Check ignored directories
  for (const ignoredDir of IGNORED_DIRS) {
    if (relativePath.includes(ignoredDir)) {
      return true;
    }
  }
  
  // Check ignored files
  if (IGNORED_FILES.includes(fileName)) {
    return true;
  }
  
  // Check ignored patterns
  for (const pattern of IGNORED_PATTERNS) {
    if (pattern.test(filePath)) {
      return true;
    }
  }
  
  return false;
}

function extractExports(content, filePath) {
  const exportMap = new Map();
  
  // Match different types of exports
  const patterns = [
    // Named exports: export const/function/class/interface/type/enum name
    {
      regex: /export\s+(?:const|function|class|interface|type|enum)\s+(\w+)/g,
      type: 'named'
    },
    // Named exports from export { ... }
    {
      regex: /export\s*{\s*([^}]+)\s*}/g,
      type: 'named'
    },
    // Default exports: export default function/class name
    {
      regex: /export\s+default\s+(?:function\s+)?(\w+)/g,
      type: 'default'
    },
    // Re-exports: export * from '...'
    {
      regex: /export\s*\*\s+from\s+['"]([^'"]+)['"]/g,
      type: 're-export'
    }
  ];
  
  for (const pattern of patterns) {
    let match;
    while ((match = pattern.regex.exec(content)) !== null) {
      if (match[1]) {
        if (pattern.type === 'named') {
          // Handle named exports from export { ... }
          const names = match[1].split(',').map(name => name.trim().split(' as ')[0]);
          names.forEach(name => {
            if (name && !name.startsWith('//')) {
              exportMap.set(name, pattern.type);
            }
          });
        } else {
          exportMap.set(match[1], pattern.type);
        }
      }
    }
  }
  
  return exportMap;
}

function extractImports(content, filePath) {
  const importSet = new Set();
  const namedImportMap = new Map();
  
  // Match import statements
  const importPatterns = [
    // Named imports: import { name1, name2 } from 'path'
    {
      regex: /import\s*{\s*([^}]+)\s*}\s+from\s+['"]([^'"]+)['"]/g,
      type: 'named'
    },
    // Default imports: import name from 'path'
    {
      regex: /import\s+(\w+)\s+from\s+['"]([^'"]+)['"]/g,
      type: 'default'
    },
    // Namespace imports: import * as name from 'path'
    {
      regex: /import\s+\*\s+as\s+(\w+)\s+from\s+['"]([^'"]+)['"]/g,
      type: 'namespace'
    }
  ];
  
  for (const pattern of importPatterns) {
    let match;
    while ((match = pattern.regex.exec(content)) !== null) {
      const importPath = match[2] || match[1];
      
      // Resolve relative imports
      if (importPath.startsWith('.')) {
        const resolvedPath = resolveImportPath(importPath, filePath);
        if (resolvedPath) {
          importSet.add(resolvedPath);
          
          if (pattern.type === 'named') {
            const names = match[1].split(',').map(name => name.trim().split(' as ')[0]);
            if (!namedImportMap.has(resolvedPath)) {
              namedImportMap.set(resolvedPath, new Set());
            }
            names.forEach(name => {
              if (name && !name.startsWith('//')) {
                namedImportMap.get(resolvedPath).add(name);
              }
            });
          }
        }
      }
    }
  }
  
  return { importSet, namedImportMap };
}

function resolveImportPath(importPath, currentFile) {
  const resolvedPath = path.resolve(path.dirname(currentFile), importPath);
  const possibleExtensions = ['.ts', '.tsx', '.js', '.jsx'];
  
  for (const ext of possibleExtensions) {
    const fullPath = resolvedPath + ext;
    if (fs.existsSync(fullPath)) {
      return fullPath;
    }
    // Also check for index files
    const indexPath = path.join(resolvedPath, 'index' + ext);
    if (fs.existsSync(indexPath)) {
      return indexPath;
    }
  }
  
  return null;
}

function scanDirectory(dir) {
  const entries = fs.readdirSync(dir, { withFileTypes: true });
  
  for (const entry of entries) {
    const fullPath = path.join(dir, entry.name);
    
    if (entry.isDirectory()) {
      if (!shouldIgnoreFile(fullPath)) {
        scanDirectory(fullPath);
      }
    } else if (entry.isFile() && (entry.name.endsWith('.ts') || entry.name.endsWith('.tsx'))) {
      if (!shouldIgnoreFile(fullPath)) {
        files.add(fullPath);
        
        try {
          const content = fs.readFileSync(fullPath, 'utf8');
          exports.set(fullPath, extractExports(content, fullPath));
          const { importSet, namedImportMap } = extractImports(content, fullPath);
          imports.set(fullPath, importSet);
          namedImports.set(fullPath, namedImportMap);
        } catch (error) {
          console.warn(`Warning: Could not read ${fullPath}: ${error.message}`);
        }
      }
    }
  }
}

function findUnusedFiles() {
  const unusedFiles = [];
  
  for (const file of files) {
    const fileImports = imports.get(file) || new Set();
    let isUsed = false;
    
    // Check if this file is imported by any other file
    for (const [importingFile, importingImports] of imports) {
      if (importingFile !== file && importingImports.has(file)) {
        isUsed = true;
        break;
      }
    }
    
    // Check if this is a main entry point
    const fileName = path.basename(file);
    if (fileName === 'main.tsx' || fileName === 'App.tsx' || fileName === 'index.ts') {
      isUsed = true;
    }
    
    if (!isUsed) {
      unusedFiles.push(file);
    }
  }
  
  return unusedFiles;
}

function findUnusedExports() {
  const unusedExports = new Map();
  
  for (const [file, exportMap] of exports) {
    const unusedInFile = [];
    
    for (const [exportName, exportType] of exportMap) {
      let isUsed = false;
      
      // Check if this export is imported by any other file
      for (const [importingFile, namedImportMap] of namedImports) {
        if (importingFile !== file) {
          const importedNames = namedImportMap.get(file);
          if (importedNames && importedNames.has(exportName)) {
            isUsed = true;
            break;
          }
        }
      }
      
      // Check if this is a default export (harder to track, assume used if file is imported)
      if (exportType === 'default') {
        for (const [importingFile, importingImports] of imports) {
          if (importingFile !== file && importingImports.has(file)) {
            isUsed = true;
            break;
          }
        }
      }
      
      if (!isUsed) {
        unusedInFile.push({ name: exportName, type: exportType });
      }
    }
    
    if (unusedInFile.length > 0) {
      unusedExports.set(file, unusedInFile);
    }
  }
  
  return unusedExports;
}

function findUnusedImports() {
  const unusedImports = new Map();
  
  for (const [file, namedImportMap] of namedImports) {
    const unusedInFile = [];
    
    for (const [importedFile, importedNames] of namedImportMap) {
      const fileExports = exports.get(importedFile);
      if (fileExports) {
        for (const importedName of importedNames) {
          if (!fileExports.has(importedName)) {
            unusedInFile.push({ file: importedFile, name: importedName });
          }
        }
      }
    }
    
    if (unusedInFile.length > 0) {
      unusedImports.set(file, unusedInFile);
    }
  }
  
  return unusedImports;
}

function printResults(unusedFiles, unusedExports, unusedImports) {
  console.log('\n🔍 Advanced Dead Code Detection Results\n');
  
  if (unusedFiles.length === 0 && unusedExports.size === 0 && unusedImports.size === 0) {
    console.log('✅ No dead code found!');
    return;
  }
  
  if (unusedFiles.length > 0) {
    console.log('📁 Unused Files:');
    for (const file of unusedFiles) {
      const relativePath = path.relative(SRC_DIR, file);
      console.log(`  - ${relativePath}`);
    }
    console.log();
  }
  
  if (unusedExports.size > 0) {
    console.log('📦 Unused Exports:');
    for (const [file, exportList] of unusedExports) {
      const relativePath = path.relative(SRC_DIR, file);
      console.log(`  - ${relativePath}:`);
      for (const { name, type } of exportList) {
        console.log(`    * ${name} (${type})`);
      }
    }
    console.log();
  }
  
  if (unusedImports.size > 0) {
    console.log('📥 Unused Imports:');
    for (const [file, importList] of unusedImports) {
      const relativePath = path.relative(SRC_DIR, file);
      console.log(`  - ${relativePath}:`);
      for (const { file: importedFile, name } of importList) {
        const importedRelativePath = path.relative(SRC_DIR, importedFile);
        console.log(`    * ${name} from ${importedRelativePath}`);
      }
    }
    console.log();
  }
}

function removeUnusedFiles(unusedFiles) {
  console.log('\n🗑️  Removing unused files...\n');
  
  for (const file of unusedFiles) {
    const relativePath = path.relative(SRC_DIR, file);
    try {
      fs.unlinkSync(file);
      console.log(`✅ Removed: ${relativePath}`);
    } catch (error) {
      console.error(`❌ Failed to remove ${relativePath}: ${error.message}`);
    }
  }
}

function main() {
  const args = process.argv.slice(2);
  const shouldRemove = args.includes('--remove');
  const shouldList = args.includes('--list') || args.length === 0;
  
  console.log('🔍 Scanning for dead code...');
  
  // Scan the source directory
  scanDirectory(SRC_DIR);
  
  // Find unused files, exports, and imports
  const unusedFiles = findUnusedFiles();
  const unusedExports = findUnusedExports();
  const unusedImports = findUnusedImports();
  
  if (shouldList) {
    printResults(unusedFiles, unusedExports, unusedImports);
  }
  
  if (shouldRemove && unusedFiles.length > 0) {
    removeUnusedFiles(unusedFiles);
  }
  
  // Summary
  console.log(`\n📊 Summary:`);
  console.log(`  - Total files scanned: ${files.size}`);
  console.log(`  - Unused files: ${unusedFiles.length}`);
  console.log(`  - Files with unused exports: ${unusedExports.size}`);
  console.log(`  - Files with unused imports: ${unusedImports.size}`);
  
  if (unusedFiles.length > 0 || unusedExports.size > 0 || unusedImports.size > 0) {
    console.log(`\n💡 Run with --remove to automatically remove unused files`);
    process.exit(1);
  }
}

main();
