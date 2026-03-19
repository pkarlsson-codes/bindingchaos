import type { DocumentUploadResponse } from '../types/document';

const GATEWAY_BASE_URL = import.meta.env.VITE_GATEWAY_URL || 'http://localhost:4000';

export class DocumentService {
  private static csrfToken: string | null = null;

  private static async ensureCsrfToken(): Promise<string> {
    if (this.csrfToken) {
      return this.csrfToken;
    }

    const response = await fetch(`${GATEWAY_BASE_URL}/api/v1/auth/csrf`, {
      method: 'GET',
      credentials: 'include',
    });

    if (!response.ok) {
      throw new Error(`Failed to get CSRF token: ${response.statusText}`);
    }

    const cookies = document.cookie.split(';');
    const csrfCookie = cookies.find(cookie => cookie.trim().startsWith('bc_csrf='));
    
    if (!csrfCookie) {
      throw new Error('CSRF token cookie not found');
    }

    this.csrfToken = csrfCookie.split('=')[1];
    return this.csrfToken;
  }

  static async uploadDocument(file: File): Promise<DocumentUploadResponse> {
    const csrfToken = await this.ensureCsrfToken();
    
    const formData = new FormData();
    formData.append('file', file);

    const response = await fetch(`${GATEWAY_BASE_URL}/api/v1/documents`, {
      method: 'POST',
      body: formData,
      credentials: 'include',
      headers: {
        'X-CSRF-Token': csrfToken,
      },
    });

    if (!response.ok) {
      throw new Error(`Upload failed: ${response.statusText}`);
    }

    return await response.json();
  }
}
