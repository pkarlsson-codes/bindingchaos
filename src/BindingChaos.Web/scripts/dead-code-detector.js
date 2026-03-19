#!/usr/bin/env node

import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const SRC_DIR = path.join(__dirname, '..', 'src');
const IGNORED_DIRS = ['node_modules', '.git', 'dist', 'build', '__tests__', 'test', 'tests'];
const IGNORED_FILES = ['index.ts', 'index.tsx', 'main.tsx', 'App.tsx', 'vite-env.d.ts'];
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
const exports = new Map(); // file -> Set of exported names
const imports = new Map(); // file -> Set of imported names
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
  const exportSet = new Set();
  
  // Match export statements
  const exportPatterns = [
    /export\s+(?:const|function|class|interface|type|enum)\s+(\w+)/g,
    /export\s*{\s*([^}]+)\s*}/g,
    /export\s+default\s+(?:function\s+)?(\w+)/g,
  ];
  
  for (const pattern of exportPatterns) {
    let match;
    while ((match = pattern.exec(content)) !== null) {
      if (match[1]) {
        // Handle named exports from export { ... }
        const names = match[1].split(',').map(name => name.trim().split(' as ')[0]);
        names.forEach(name => {
          if (name && !name.startsWith('//')) {
            exportSet.add(name);
          }
        });
      }
    }
  }
  
  return exportSet;
}

function extractImports(content, filePath) {
  const importSet = new Set();
  
  // Match import statements
  const importPattern = /import\s+(?:{[^}]*}|\*\s+as\s+\w+|\w+)\s+from\s+['"]([^'"]+)['"]/g;
  
  let match;
  while ((match = importPattern.exec(content)) !== null) {
    const importPath = match[1];
    
    // Resolve relative imports
    if (importPath.startsWith('.')) {
      const resolvedPath = path.resolve(path.dirname(filePath), importPath);
      const possibleExtensions = ['.ts', '.tsx', '.js', '.jsx'];
      
      for (const ext of possibleExtensions) {
        const fullPath = resolvedPath + ext;
        if (fs.existsSync(fullPath)) {
          importSet.add(fullPath);
          break;
        }
        // Also check for index files
        const indexPath = path.join(resolvedPath, 'index' + ext);
        if (fs.existsSync(indexPath)) {
          importSet.add(indexPath);
          break;
        }
      }
    }
  }
  
  return importSet;
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
          imports.set(fullPath, extractImports(content, fullPath));
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
  
  for (const [file, exportSet] of exports) {
    const unusedInFile = [];
    
    for (const exportName of exportSet) {
      let isUsed = false;
      
      // Check if this export is imported by any other file
      for (const [importingFile, importingImports] of imports) {
        if (importingFile !== file) {
          // This is a simplified check - in a real implementation you'd need to parse the actual import names
          // For now, we'll assume if a file imports from this file, the exports are used
          if (importingImports.has(file)) {
            isUsed = true;
            break;
          }
        }
      }
      
      if (!isUsed) {
        unusedInFile.push(exportName);
      }
    }
    
    if (unusedInFile.length > 0) {
      unusedExports.set(file, unusedInFile);
    }
  }
  
  return unusedExports;
}

function printResults(unusedFiles, unusedExports) {
  console.log('\n🔍 Dead Code Detection Results\n');
  
  if (unusedFiles.length === 0 && unusedExports.size === 0) {
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
    for (const [file, exportNames] of unusedExports) {
      const relativePath = path.relative(SRC_DIR, file);
      console.log(`  - ${relativePath}:`);
      for (const exportName of exportNames) {
        console.log(`    * ${exportName}`);
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
  
  // Find unused files and exports
  const unusedFiles = findUnusedFiles();
  const unusedExports = findUnusedExports();
  
  if (shouldList) {
    printResults(unusedFiles, unusedExports);
  }
  
  if (shouldRemove && unusedFiles.length > 0) {
    removeUnusedFiles(unusedFiles);
  }
  
  // Summary
  console.log(`\n📊 Summary:`);
  console.log(`  - Total files scanned: ${files.size}`);
  console.log(`  - Unused files: ${unusedFiles.length}`);
  console.log(`  - Files with unused exports: ${unusedExports.size}`);
  
  if (unusedFiles.length > 0 || unusedExports.size > 0) {
    console.log(`\n💡 Run with --remove to automatically remove unused files`);
    process.exit(1);
  }
}

main();
