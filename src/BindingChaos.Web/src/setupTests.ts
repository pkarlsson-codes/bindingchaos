/// <reference types="node" />
import '@testing-library/jest-dom';

// Mock the API config module
jest.mock('./config/api', () => ({
  API_CONFIG: {
    baseUrl: 'http://localhost:4000',
    version: 'v1',
    timeout: 30000,
    retryAttempts: 3,
    retryDelay: 1000,
  },
  getApiBasePath: () => 'http://localhost:4000/api/v1',
  getApiUrl: (endpoint: string) => `http://localhost:4000/api/v1/${endpoint.replace(/^\/+/, '')}`,
}));

// Mock window.matchMedia
Object.defineProperty(window, 'matchMedia', {
  writable: true,
  value: jest.fn().mockImplementation(query => ({
    matches: false,
    media: query,
    onchange: null,
    addListener: jest.fn(), // deprecated
    removeListener: jest.fn(), // deprecated
    addEventListener: jest.fn(),
    removeEventListener: jest.fn(),
    dispatchEvent: jest.fn(),
  })),
});

// Mock IntersectionObserver
(global as any).IntersectionObserver = class IntersectionObserver {
  constructor() {}
  disconnect() {}
  observe() {}
  unobserve() {}
  takeRecords() { return []; }
  root: Element | null = null;
  rootMargin: string = '';
  thresholds: ReadonlyArray<number> = [];
};

// Mock ResizeObserver
(global as any).ResizeObserver = class ResizeObserver {
  constructor() {}
  disconnect() {}
  observe() {}
  unobserve() {}
}; 