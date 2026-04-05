import { TagCacheService, useTagCache } from '../TagCacheService';

// Mock localStorage
const localStorageMock = {
  getItem: jest.fn(),
  setItem: jest.fn(),
  removeItem: jest.fn(),
  clear: jest.fn(),
  length: 0,
  key: jest.fn(),
};

Object.defineProperty(window, 'localStorage', {
  value: localStorageMock,
});

// Mock the API client
const mockApiClient = {
  tags: {
    getPopularTags: jest.fn(),
    searchTags: jest.fn(),
    getRecentTags: jest.fn(),
  },
};

// Mock React hooks
jest.mock('react', () => ({
  ...jest.requireActual('react'),
  useMemo: jest.fn((fn, deps) => fn()),
}));

jest.mock('../../../../shared/hooks/useApiClient', () => ({
  useApiClient: jest.fn(() => mockApiClient),
}));

describe('TagCacheService', () => {
  let tagService: TagCacheService;
  const CACHE_KEY = 'bindingchaos_tag_cache';

  beforeEach(() => {
    jest.clearAllMocks();
    localStorageMock.getItem.mockClear();
    localStorageMock.setItem.mockClear();
    localStorageMock.removeItem.mockClear();
    
    // Clear the singleton instance
    (TagCacheService as any).instance = undefined;
    
    tagService = TagCacheService.getInstance();
    tagService.setApiClient(mockApiClient);
  });

  describe('Singleton Pattern', () => {
    it('returns the same instance on multiple calls', () => {
      const instance1 = TagCacheService.getInstance();
      const instance2 = TagCacheService.getInstance();
      
      expect(instance1).toBe(instance2);
    });
  });

  describe('Cache Management', () => {
    it('initializes with empty cache when no cached data exists', () => {
      localStorageMock.getItem.mockReturnValue(null);
      
      const newService = TagCacheService.getInstance();
      const cachedData = newService.getCachedTags();
      
      expect(cachedData).toBeNull();
    });

    it('loads valid cached data from localStorage', () => {
      const mockCache = {
        recentTags: ['react', 'typescript'],
        popularTags: ['javascript', 'python'],
        workspaceTags: ['nodejs'],
        lastUpdated: Date.now(),
      };
      
      localStorageMock.getItem.mockReturnValue(JSON.stringify(mockCache));
      
      // Clear the singleton instance to force a new one
      (TagCacheService as any).instance = undefined;
      const newService = TagCacheService.getInstance();
      const cachedData = newService.getCachedTags();
      
      expect(cachedData).toEqual(mockCache);
    });

    it('ignores expired cache data', () => {
      const expiredCache = {
        recentTags: ['react'],
        popularTags: ['javascript'],
        workspaceTags: [],
        lastUpdated: Date.now() - (25 * 60 * 60 * 1000), // 25 hours ago
      };
      
      localStorageMock.getItem.mockReturnValue(JSON.stringify(expiredCache));
      
      // Clear the singleton instance to force a new one
      (TagCacheService as any).instance = undefined;
      const newService = TagCacheService.getInstance();
      const cachedData = newService.getCachedTags();
      
      expect(cachedData).toBeNull();
      expect(localStorageMock.removeItem).toHaveBeenCalledWith(CACHE_KEY);
    });

    it('handles invalid JSON in localStorage', () => {
      localStorageMock.getItem.mockReturnValue('invalid-json');
      
      const newService = TagCacheService.getInstance();
      const cachedData = newService.getCachedTags();
      
      expect(cachedData).toBeNull();
    });

    it('saves cache data to localStorage', () => {
      const mockCache = {
        recentTags: ['react'],
        popularTags: ['javascript'],
        workspaceTags: [],
        lastUpdated: Date.now(),
      };
      
      tagService['cache'] = mockCache;
      tagService['saveCache']();
      
      expect(localStorageMock.setItem).toHaveBeenCalledWith(
        CACHE_KEY,
        JSON.stringify(mockCache)
      );
    });

    it('handles localStorage errors gracefully', () => {
      localStorageMock.setItem.mockImplementation(() => {
        throw new Error('Storage quota exceeded');
      });
      
      const consoleSpy = jest.spyOn(console, 'warn').mockImplementation(() => {});
      
      tagService['cache'] = { recentTags: [], popularTags: [], workspaceTags: [], lastUpdated: Date.now() };
      tagService['saveCache']();
      
      expect(consoleSpy).toHaveBeenCalledWith('Failed to save tag cache:', expect.any(Error));
      
      consoleSpy.mockRestore();
    });
  });

  describe('getPopularTags', () => {
    it('returns cached popular tags when available', async () => {
      const cachedTags = ['react', 'typescript', 'javascript'];
      tagService['cache'] = {
        recentTags: [],
        popularTags: cachedTags,
        workspaceTags: [],
        lastUpdated: Date.now(),
      };
      
      const result = await tagService.getPopularTags('test-locality', 2);
      
      expect(result).toEqual(['react', 'typescript']);
      expect(mockApiClient.tags.getPopularTags).not.toHaveBeenCalled();
    });

    it('fetches popular tags from API when cache is empty', async () => {
      const apiTags = ['react', 'typescript', 'javascript'];
      mockApiClient.tags.getPopularTags.mockResolvedValue(apiTags);
      
      const result = await tagService.getPopularTags('test-locality', 50);
      
      expect(result).toEqual(apiTags);
      expect(mockApiClient.tags.getPopularTags).toHaveBeenCalledWith({
        localityId: 'test-locality',
        limit: 50,
      });
      expect(localStorageMock.setItem).toHaveBeenCalled();
    });

    it('handles API errors gracefully', async () => {
      mockApiClient.tags.getPopularTags.mockRejectedValue(new Error('API Error'));
      
      const consoleSpy = jest.spyOn(console, 'error').mockImplementation(() => {});
      
      const result = await tagService.getPopularTags('test-locality');
      
      expect(result).toEqual([]);
      expect(consoleSpy).toHaveBeenCalledWith('Failed to fetch popular tags:', expect.any(Error));
      
      consoleSpy.mockRestore();
    });

    it('returns empty array when API client is not set', async () => {
      const consoleSpy = jest.spyOn(console, 'warn').mockImplementation(() => {});
      
      tagService.setApiClient(null as any);
      const result = await tagService.getPopularTags('test-locality');
      
      expect(result).toEqual([]);
      expect(consoleSpy).toHaveBeenCalledWith('API client not set, returning empty array');
      
      consoleSpy.mockRestore();
    });
  });

  describe('searchTags', () => {
    it('returns empty array for empty query', async () => {
      const result = await tagService.searchTags('', 'test-locality');
      
      expect(result).toEqual([]);
      expect(mockApiClient.tags.searchTags).not.toHaveBeenCalled();
    });

    it('returns empty array for whitespace-only query', async () => {
      const result = await tagService.searchTags('   ', 'test-locality');
      
      expect(result).toEqual([]);
      expect(mockApiClient.tags.searchTags).not.toHaveBeenCalled();
    });

    it('fetches search results from API', async () => {
      const searchResults = ['react', 'react-native', 'react-router'];
      mockApiClient.tags.searchTags.mockResolvedValue(searchResults);
      
      const result = await tagService.searchTags('react', 'test-locality', 20);
      
      expect(result).toEqual(searchResults);
      expect(mockApiClient.tags.searchTags).toHaveBeenCalledWith({
        q: 'react',
        localityId: 'test-locality',
        limit: 20,
      });
    });

    it('handles API errors gracefully', async () => {
      mockApiClient.tags.searchTags.mockRejectedValue(new Error('API Error'));
      
      const consoleSpy = jest.spyOn(console, 'error').mockImplementation(() => {});
      
      const result = await tagService.searchTags('react', 'test-locality');
      
      expect(result).toEqual([]);
      expect(consoleSpy).toHaveBeenCalledWith('Failed to search tags:', expect.any(Error));
      
      consoleSpy.mockRestore();
    });

    it('returns empty array when API client is not set', async () => {
      const consoleSpy = jest.spyOn(console, 'warn').mockImplementation(() => {});
      
      tagService.setApiClient(null as any);
      const result = await tagService.searchTags('react', 'test-locality');
      
      expect(result).toEqual([]);
      expect(consoleSpy).toHaveBeenCalledWith('API client not set, returning empty array');
      
      consoleSpy.mockRestore();
    });
  });

  describe('getRecentTags', () => {
    it('returns cached recent tags when available', async () => {
      const cachedTags = ['react', 'typescript', 'javascript'];
      tagService['cache'] = {
        recentTags: cachedTags,
        popularTags: [],
        workspaceTags: [],
        lastUpdated: Date.now(),
      };
      
      const result = await tagService.getRecentTags('test-user', 2);
      
      expect(result).toEqual(['react', 'typescript']);
      expect(mockApiClient.tags.getRecentTags).not.toHaveBeenCalled();
    });

    it('fetches recent tags from API when cache is empty', async () => {
      const apiTags = ['react', 'typescript', 'javascript'];
      mockApiClient.tags.getRecentTags.mockResolvedValue(apiTags);
      
      const result = await tagService.getRecentTags('test-user', 30);
      
      expect(result).toEqual(apiTags);
      expect(mockApiClient.tags.getRecentTags).toHaveBeenCalledWith({
        userId: 'test-user',
        limit: 30,
      });
      expect(localStorageMock.setItem).toHaveBeenCalled();
    });

    it('handles API errors gracefully', async () => {
      mockApiClient.tags.getRecentTags.mockRejectedValue(new Error('API Error'));
      
      const consoleSpy = jest.spyOn(console, 'error').mockImplementation(() => {});
      
      const result = await tagService.getRecentTags('test-user');
      
      expect(result).toEqual([]);
      expect(consoleSpy).toHaveBeenCalledWith('Failed to fetch recent tags:', expect.any(Error));
      
      consoleSpy.mockRestore();
    });

    it('returns empty array when API client is not set', async () => {
      const consoleSpy = jest.spyOn(console, 'warn').mockImplementation(() => {});
      
      tagService.setApiClient(null as any);
      const result = await tagService.getRecentTags('test-user');
      
      expect(result).toEqual([]);
      expect(consoleSpy).toHaveBeenCalledWith('API client not set, returning empty array');
      
      consoleSpy.mockRestore();
    });
  });

  describe('addRecentTag', () => {
    it('adds new tag to the beginning of recent tags', () => {
      tagService['cache'] = {
        recentTags: ['react', 'typescript'],
        popularTags: [],
        workspaceTags: [],
        lastUpdated: Date.now(),
      };
      
      tagService.addRecentTag('javascript');
      
      expect(tagService['cache']!.recentTags).toEqual(['javascript', 'react', 'typescript']);
      expect(localStorageMock.setItem).toHaveBeenCalled();
    });

    it('moves existing tag to the beginning', () => {
      tagService['cache'] = {
        recentTags: ['react', 'typescript', 'javascript'],
        popularTags: [],
        workspaceTags: [],
        lastUpdated: Date.now(),
      };
      
      tagService.addRecentTag('typescript');
      
      expect(tagService['cache']!.recentTags).toEqual(['typescript', 'react', 'javascript']);
      expect(localStorageMock.setItem).toHaveBeenCalled();
    });

    it('limits recent tags to 50 items', () => {
      const manyTags = Array.from({ length: 50 }, (_, i) => `tag-${i}`);
      tagService['cache'] = {
        recentTags: manyTags,
        popularTags: [],
        workspaceTags: [],
        lastUpdated: Date.now(),
      };
      
      tagService.addRecentTag('new-tag');
      
      expect(tagService['cache']!.recentTags).toHaveLength(50);
      expect(tagService['cache']!.recentTags[0]).toBe('new-tag');
      expect(tagService['cache']!.recentTags[49]).toBe('tag-48'); // Last tag should be tag-48
    });

    it('initializes cache if it does not exist', () => {
      tagService['cache'] = null;
      
      tagService.addRecentTag('react');
      
      expect(tagService['cache']!.recentTags).toEqual(['react']);
      expect(localStorageMock.setItem).toHaveBeenCalled();
    });
  });

  describe('clearCache', () => {
    it('clears the cache and removes from localStorage', () => {
      tagService['cache'] = {
        recentTags: ['react'],
        popularTags: ['javascript'],
        workspaceTags: [],
        lastUpdated: Date.now(),
      };
      
      tagService.clearCache();
      
      expect(tagService['cache']).toBeNull();
      expect(localStorageMock.removeItem).toHaveBeenCalledWith(CACHE_KEY);
    });
  });

  describe('getCachedTags', () => {
    it('returns the current cache', () => {
      const mockCache = {
        recentTags: ['react'],
        popularTags: ['javascript'],
        workspaceTags: [],
        lastUpdated: Date.now(),
      };
      
      tagService['cache'] = mockCache;
      
      const result = tagService.getCachedTags();
      
      expect(result).toEqual(mockCache);
    });

    it('returns null when cache is empty', () => {
      tagService['cache'] = null;
      
      const result = tagService.getCachedTags();
      
      expect(result).toBeNull();
    });
  });
});

describe('useTagCache', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    (TagCacheService as any).instance = undefined;
  });

  it('returns an object with all tag cache methods', () => {
    const result = useTagCache();
    
    expect(result).toHaveProperty('getPopularTags');
    expect(result).toHaveProperty('searchTags');
    expect(result).toHaveProperty('getRecentTags');
    expect(result).toHaveProperty('addRecentTag');
    expect(result).toHaveProperty('clearCache');
    expect(result).toHaveProperty('getCachedTags');
  });

  it('sets the API client on the tag service', () => {
    const tagService = TagCacheService.getInstance();
    const setApiClientSpy = jest.spyOn(tagService, 'setApiClient');
    
    useTagCache();
    
    expect(setApiClientSpy).toHaveBeenCalledWith(mockApiClient);
  });

  it('binds methods to the tag service instance', () => {
    const tagService = TagCacheService.getInstance();
    const getPopularTagsSpy = jest.spyOn(tagService, 'getPopularTags');
    
    const result = useTagCache();
    result.getPopularTags('test-locality');
    
    expect(getPopularTagsSpy).toHaveBeenCalledWith('test-locality');
  });
}); 