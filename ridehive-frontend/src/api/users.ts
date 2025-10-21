import { ApiClient } from './base';
import type { BasicUserInfo } from './types';

class UsersApi {
  private apiClient: ApiClient;

  constructor() {
    this.apiClient = new ApiClient();
  }

  // Get basic user information by ID
  async getBasicUserInfo(userId: string): Promise<BasicUserInfo> {
    return this.apiClient.get<BasicUserInfo>(`/api/user/basic-info/${userId}`);
  }
}

export const usersApi = new UsersApi();