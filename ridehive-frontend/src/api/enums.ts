// API for enum fetching
import { apiClient } from './base';
import type { EnumCollections } from './types';

export class EnumsApi {
  /**
   * Get all enum collections in a single request for better performance
   */
  async getAllEnums(): Promise<EnumCollections> {
    return apiClient.get<EnumCollections>('/api/Enums/all');
  }
}

// Create singleton instance
export const enumsApi = new EnumsApi();