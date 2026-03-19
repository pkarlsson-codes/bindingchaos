import React, { useMemo } from 'react';
import { useApiClient } from '../../../shared/hooks/useApiClient';

interface TagCache {
  recentTags: string[];
  popularTags: string[];
  workspaceTags: string[];
  lastUpdated: number;
}

const CACHE_KEY = 'bindingchaos_tag_cache';
const CACHE_DURATION = 24 * 60 * 60 * 1000; // 24 hours in milliseconds

export class TagCacheService {
  private static instance: TagCacheService;
  private cache: TagCache | null = null;
  private apiClient: any;

  private constructor() {
    this.loadCache();
  }

  public static getInstance(): TagCacheService {
    if (!TagCacheService.instance) {
      TagCacheService.instance = new TagCacheService();
    }
    return TagCacheService.instance;
  }

  public setApiClient(apiClient: any) {
    this.apiClient = apiClient;
  }

  private loadCache(): void {
    try {
      const cached = localStorage.getItem(CACHE_KEY);
      if (cached) {
        this.cache = JSON.parse(cached);
        
        // Check if cache is expired
        if (Date.now() - this.cache!.lastUpdated > CACHE_DURATION) {
          this.cache = null;
          localStorage.removeItem(CACHE_KEY);
        }
      }
    } catch (error) {
      console.warn('Failed to load tag cache:', error);
      this.cache = null;
    }
  }

  private saveCache(): void {
    try {
      if (this.cache) {
        this.cache.lastUpdated = Date.now();
        localStorage.setItem(CACHE_KEY, JSON.stringify(this.cache));
      }
    } catch (error) {
      console.warn('Failed to save tag cache:', error);
    }
  }

  private initializeCache(): TagCache {
    if (!this.cache) {
      this.cache = {
        recentTags: [],
        popularTags: [],
        workspaceTags: [],
        lastUpdated: Date.now()
      };
    }
    return this.cache;
  }

  public async getPopularTags(localityId?: string, limit: number = 50): Promise<string[]> {
    const cache = this.initializeCache();
    
    // Return cached popular tags if available
    if (cache.popularTags.length > 0) {
      return cache.popularTags.slice(0, limit);
    }

    // Fetch from API if no cache
    try {
      if (!this.apiClient) {
        console.warn('API client not set, returning empty array');
        return [];
      }

      const response = await this.apiClient.tags.getPopularTags({ 
        localityId, 
        limit 
      });
      
      if (Array.isArray(response)) {
        cache.popularTags = response;
        this.saveCache();
        return response;
      }
    } catch (error) {
      console.error('Failed to fetch popular tags:', error);
    }

    return [];
  }

  public async searchTags(query: string, localityId?: string, limit: number = 20): Promise<string[]> {
    if (!query.trim()) {
      return [];
    }

    try {
      if (!this.apiClient) {
        console.warn('API client not set, returning empty array');
        return [];
      }

      const response = await this.apiClient.tags.searchTags({ 
        q: query, 
        localityId, 
        limit 
      });
      
      if (Array.isArray(response)) {
        return response;
      }
    } catch (error) {
      console.error('Failed to search tags:', error);
    }

    return [];
  }

  public async getRecentTags(userId: string, limit: number = 30): Promise<string[]> {
    const cache = this.initializeCache();
    
    // Return cached recent tags if available
    if (cache.recentTags.length > 0) {
      return cache.recentTags.slice(0, limit);
    }

    // Fetch from API if no cache
    try {
      if (!this.apiClient) {
        console.warn('API client not set, returning empty array');
        return [];
      }

      const response = await this.apiClient.tags.getRecentTags({ 
        userId, 
        limit 
      });
      
      if (Array.isArray(response)) {
        cache.recentTags = response;
        this.saveCache();
        return response;
      }
    } catch (error) {
      console.error('Failed to fetch recent tags:', error);
    }

    return [];
  }

  public addRecentTag(tag: string): void {
    const cache = this.initializeCache();
    
    // Remove tag if it already exists
    cache.recentTags = cache.recentTags.filter(t => t !== tag);
    
    // Add to beginning of array
    cache.recentTags.unshift(tag);
    
    // Keep only the most recent 50 tags
    cache.recentTags = cache.recentTags.slice(0, 50);
    
    this.saveCache();
  }

  public clearCache(): void {
    this.cache = null;
    localStorage.removeItem(CACHE_KEY);
  }

  public getCachedTags(): TagCache | null {
    return this.cache;
  }
}

// React hook for using the tag cache service
export function useTagCache() {
  const apiClient = useApiClient();
  const tagService = TagCacheService.getInstance();
  
  // Set the API client
  tagService.setApiClient(apiClient);

  return useMemo(() => ({
    getPopularTags: tagService.getPopularTags.bind(tagService),
    searchTags: tagService.searchTags.bind(tagService),
    getRecentTags: tagService.getRecentTags.bind(tagService),
    addRecentTag: tagService.addRecentTag.bind(tagService),
    clearCache: tagService.clearCache.bind(tagService),
    getCachedTags: tagService.getCachedTags.bind(tagService)
  }), [tagService]);
} 