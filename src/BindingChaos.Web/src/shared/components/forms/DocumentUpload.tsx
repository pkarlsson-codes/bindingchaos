import React, { useRef, useCallback, useState } from 'react';
import { useDocumentUpload } from '../../hooks/useDocumentUpload';
import type { DocumentRecord, UploadingDocument } from '../../types/document';
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { Card } from '../ui/card';
import { X, Upload, FileText, AlertCircle, CheckCircle, Clock } from 'lucide-react';

export interface DocumentUploadProps {
  onDocumentsChange?: (documents: DocumentRecord[]) => void;
  onUploadingChange?: (isUploading: boolean) => void;
  maxFiles?: number;
  maxFileSize?: number; // in bytes
  acceptedFileTypes?: string[];
  className?: string;
}

export const DocumentUpload: React.FC<DocumentUploadProps> = ({
  onDocumentsChange,
  onUploadingChange,
  maxFiles = 10,
  maxFileSize = 10 * 1024 * 1024, // 10MB default
  acceptedFileTypes = ['image/*', 'application/pdf', 'text/*'],
  className = '',
}) => {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [isDragOver, setIsDragOver] = useState(false);
  const [dragCounter, setDragCounter] = useState(0);
  
  const {
    uploadingDocuments,
    uploadedDocuments,
    addDocument,
    removeDocument,
    clearAll,
    documentRecords,
  } = useDocumentUpload();

  // Notify parent when documents change
  React.useEffect(() => {
    onDocumentsChange?.(documentRecords);
  }, [documentRecords, onDocumentsChange]);

  React.useEffect(() => {
    onUploadingChange?.(uploadingDocuments.length > 0);
  }, [uploadingDocuments.length, onUploadingChange]);

  const handleFileSelect = useCallback((files: FileList | null) => {
    if (!files) return;

    const fileArray = Array.from(files);
    const validFiles = fileArray.filter(file => {
      // Check file size
      if (file.size > maxFileSize) {
        console.warn(`File ${file.name} is too large. Max size: ${maxFileSize / (1024 * 1024)}MB`);
        return false;
      }

      // Check file type
      const isValidType = acceptedFileTypes.some(type => {
        if (type.endsWith('/*')) {
          return file.type.startsWith(type.slice(0, -1));
        }
        return file.type === type;
      });

      if (!isValidType) {
        console.warn(`File ${file.name} has unsupported type: ${file.type}`);
        return false;
      }

      return true;
    });

    // Check max files limit
    const totalFiles = uploadedDocuments.length + uploadingDocuments.length + validFiles.length;
    if (totalFiles > maxFiles) {
      console.warn(`Too many files. Max allowed: ${maxFiles}`);
      return;
    }

    validFiles.forEach(file => addDocument(file));
  }, [addDocument, maxFileSize, acceptedFileTypes, maxFiles, uploadedDocuments.length, uploadingDocuments.length]);

  const handleDragEnter = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setDragCounter(prev => prev + 1);
    setIsDragOver(true);
  }, []);

  const handleDragLeave = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setDragCounter(prev => prev - 1);
    if (dragCounter === 0) {
      setIsDragOver(false);
    }
  }, [dragCounter]);

  const handleDragOver = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
  }, []);

  const handleDrop = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragOver(false);
    setDragCounter(0);
    
    const files = e.dataTransfer.files;
    handleFileSelect(files);
  }, [handleFileSelect]);

  const handleFileInputChange = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
    handleFileSelect(e.target.files);
    // Reset the input so the same file can be selected again
    if (fileInputRef.current) {
      fileInputRef.current.value = '';
    }
  }, [handleFileSelect]);

  const openFileDialog = useCallback(() => {
    fileInputRef.current?.click();
  }, []);

  const getStatusIcon = (status: UploadingDocument['status']) => {
    switch (status) {
      case 'uploading':
        return <Clock className="w-4 h-4 text-blue-500" />;
      case 'uploaded':
        return <CheckCircle className="w-4 h-4 text-green-500" />;
      case 'error':
        return <AlertCircle className="w-4 h-4 text-red-500" />;
      default:
        return <FileText className="w-4 h-4 text-gray-500" />;
    }
  };

  const getStatusText = (status: UploadingDocument['status']) => {
    switch (status) {
      case 'uploading':
        return 'Uploading...';
      case 'uploaded':
        return 'Uploaded';
      case 'error':
        return 'Upload failed';
      default:
        return 'Unknown';
    }
  };

  const hasDocuments = uploadedDocuments.length > 0 || uploadingDocuments.length > 0;

  return (
    <div className={`space-y-4 ${className}`}>
      {/* Hidden file input */}
      <input
        ref={fileInputRef}
        type="file"
        multiple
        accept={acceptedFileTypes.join(',')}
        onChange={handleFileInputChange}
        className="hidden"
        capture="environment" // Enable camera on mobile
      />

      {/* Upload area */}
      <div
        className={`
          border-2 border-dashed rounded-lg p-8 text-center transition-colors
          ${isDragOver 
            ? 'border-blue-500 bg-blue-50' 
            : 'border-gray-300 hover:border-gray-400'
          }
        `}
        onDragEnter={handleDragEnter}
        onDragLeave={handleDragLeave}
        onDragOver={handleDragOver}
        onDrop={handleDrop}
      >
        <Upload className="mx-auto h-12 w-12 text-gray-400 mb-4" />
        
        {!hasDocuments ? (
          <>
            <p className="text-lg font-medium text-gray-900 mb-2">
              Upload documents
            </p>
            <p className="text-gray-500 mb-4">
              Drag and drop files here, or click to browse
            </p>
          </>
        ) : (
          <p className="text-gray-500 mb-4">
            Drag and drop more files here, or click to browse
          </p>
        )}

        <Button type="button" onClick={openFileDialog} variant="outline">
          <Upload className="w-4 h-4 mr-2" />
          Choose Files
        </Button>

        <p className="text-sm text-gray-400 mt-2">
          Supports: {acceptedFileTypes.join(', ')} • Max: {maxFileSize / (1024 * 1024)}MB per file
        </p>
      </div>

      {/* Documents list */}
      {hasDocuments && (
        <div className="space-y-3">
          <div className="flex items-center justify-between">
            <h3 className="text-lg font-medium">Documents</h3>
            <Button type="button" onClick={clearAll} variant="outline" size="sm">
              Clear All
            </Button>
          </div>

          {/* Uploading documents */}
          {uploadingDocuments.map((doc) => (
            <Card key={doc.id} className="p-4">
              <div className="flex items-center justify-between">
                <div className="flex items-center space-x-3">
                  {getStatusIcon(doc.status)}
                  <div>
                    <p className="font-medium">{doc.fileName}</p>
                    <p className="text-sm text-gray-500">{getStatusText(doc.status)}</p>
                    {doc.error && (
                      <p className="text-sm text-red-500">{doc.error}</p>
                    )}
                  </div>
                </div>
                <Badge variant={doc.status === 'error' ? 'destructive' : 'secondary'}>
                  {doc.status}
                </Badge>
              </div>
            </Card>
          ))}

          {/* Uploaded documents */}
          {uploadedDocuments.map((doc) => (
            <Card key={doc.id} className="p-4">
              <div className="flex items-center justify-between">
                <div className="flex items-center space-x-3">
                  <CheckCircle className="w-4 h-4 text-green-500" />
                  <div>
                    <p className="font-medium">{doc.fileName}</p>
                    <p className="text-sm text-gray-500">Document ID: {doc.id}</p>
                  </div>
                </div>
                <Button
                  type="button"
                  onClick={() => removeDocument(doc.id)}
                  variant="ghost"
                  size="sm"
                >
                  <X className="w-4 h-4" />
                </Button>
              </div>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
};
