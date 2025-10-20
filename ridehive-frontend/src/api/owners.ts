// CRUD operations on owners
import { apiClient } from './base';
import type { Owner } from './types';

export class OwnersApi {
  // Get all owners
  async getAllOwners(): Promise<Owner[]> {
    return apiClient.get<Owner[]>('/api/Owners');
  }

  // Get owner by ID
  async getOwnerById(id: string): Promise<Owner> {
    return apiClient.get<Owner>(`/api/Owners/${id}`);
  }

  // Create new owner
  async createOwner(ownerData: { ownerId: string; name: string }): Promise<Owner> {
    return apiClient.post<Owner>('/api/Owners', ownerData);
  }
}

// Create singleton instance
export const ownersApi = new OwnersApi();