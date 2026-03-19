import { useState, useCallback } from 'react';
import type { UploadingDocument, DocumentRecord } from '../types/document';
import { DocumentService } from '../services/documentService';

export const useDocumentUpload = () => {
  const [uploadingDocuments, setUploadingDocuments] = useState<UploadingDocument[]>([]);
  const [uploadedDocuments, setUploadedDocuments] = useState<DocumentRecord[]>([]);

  const addDocument = useCallback((file: File) => {
    const tempId = `temp-${Date.now()}-${Math.random()}`;
    const newDocument: UploadingDocument = {
      id: tempId,
      fileName: file.name,
      file,
      status: 'uploading',
    };

    setUploadingDocuments(prev => [...prev, newDocument]);

    uploadDocument(newDocument);
  }, []);

  const uploadDocument = useCallback(async (document: UploadingDocument) => {
    try {
      const response = await DocumentService.uploadDocument(document.file);

      setUploadingDocuments(prev => prev.filter(doc => doc.id !== document.id));

      setUploadedDocuments(prev => [
        ...prev,
        {
          id: response.data,
          fileName: document.fileName,
        }
      ]);

    } catch (error) {
      setUploadingDocuments(prev =>
        prev.map(doc => 
          doc.id === document.id 
            ? { ...doc, status: 'error', error: error instanceof Error ? error.message : 'Upload failed' }
            : doc
        )
      );
    }
  }, []);

  const removeDocument = useCallback((documentId: string) => {
    setUploadedDocuments(prev => prev.filter(doc => doc.id !== documentId));
  }, []);

  const clearAll = useCallback(() => {
    setUploadingDocuments([]);
    setUploadedDocuments([]);
  }, []);

  return {
    uploadingDocuments,
    uploadedDocuments,
    addDocument,
    removeDocument,
    clearAll,
    documentRecords: uploadedDocuments,
  };
};
