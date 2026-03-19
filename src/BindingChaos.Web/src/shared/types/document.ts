export interface DocumentRecord {
  id: string;
  fileName: string;
}

export interface UploadingDocument {
  id: string; // temporary ID for tracking
  fileName: string;
  file: File;
  status: 'uploading' | 'uploaded' | 'error';
  documentId?: string; // actual document ID from API
  error?: string;
}

export interface DocumentUploadResponse {
  data: string; // document ID
}
