// Export all API modules and types
export { ApiClient, ApiError, apiClient } from './base';
export { CarsApi, carsApi } from './cars';
export { EnumsApi, enumsApi } from './enums';
export { postsApi } from './posts';
export { ownersApi } from './owners';

// Import for internal use
import { carsApi } from './cars';
import { enumsApi } from './enums';
import { postsApi } from './posts';
import { ownersApi } from './owners';

// Export all types
export type {
  CarItem,
  CarResponseDto,
  CarUpdateDto,
  CarImageData,
  EnumOption,
  EnumCollections,
  PostItem,
  PostCreateDto,
  PostResponseDto,
  Owner
} from './types';

// Main API class that combines all APIs
export class Api {
  public cars = carsApi;
  public enums = enumsApi;
  public posts = postsApi;
  public owners = ownersApi;
}

// Create main API instance
export const api = new Api();

// For backward compatibility, you can also import individual APIs
export default api;