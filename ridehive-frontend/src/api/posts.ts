import { ApiClient } from './base';
import type { PostCreateDto, PostResponseDto } from './types';

class PostsApi {
  private apiClient: ApiClient;

  constructor() {
    this.apiClient = new ApiClient();
  }

  // Get all posts
  async getAllPosts(): Promise<PostResponseDto[]> {
    return this.apiClient.get<PostResponseDto[]>('/api/Posts');
  }

  // Get post by ID
  async getPostById(id: number): Promise<PostResponseDto> {
    return this.apiClient.get<PostResponseDto>(`/api/Posts/${id}`);
  }

  // Get posts by owner
  async getPostsByOwner(ownerId: string): Promise<PostResponseDto[]> {
    return this.apiClient.get<PostResponseDto[]>(`/api/Posts/owner/${ownerId}`);
  }

  // Create new post
  async createPost(postData: PostCreateDto): Promise<PostResponseDto> {
    return this.apiClient.post<PostResponseDto>('/api/Posts', postData);
  }

  // Update post
  async updatePost(id: number, postData: Partial<PostCreateDto>): Promise<PostResponseDto> {
    return this.apiClient.put<PostResponseDto>(`/api/Posts/${id}`, postData);
  }

  // Delete post
  async deletePost(id: number): Promise<void> {
    return this.apiClient.delete<void>(`/api/Posts/${id}`);
  }

  // Toggle post availability
  async toggleAvailability(id: number): Promise<PostResponseDto> {
    return this.apiClient.put<PostResponseDto>(`/api/Posts/${id}/toggle-availability`, {});
  }
}

export const postsApi = new PostsApi();