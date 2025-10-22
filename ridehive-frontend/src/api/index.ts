// Export all API modules and types
export { ApiClient, ApiError, apiClient } from './base';
export { CarsApi, carsApi } from './cars';
export { EnumsApi, enumsApi } from './enums';
export { postsApi } from './posts';
export { usersApi } from './users';

// Import for internal use
import { carsApi } from './cars';
import { enumsApi } from './enums';
import { postsApi } from './posts';
import { usersApi } from './users';

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
  UserAuthResponseDto,
  UserLoginDto,
  UserRegisterDto,
  UpdateProfileDto,
  BasicUserInfo
} from './types';

// Main API class that combines all APIs
export class Api {
  public cars = carsApi;
  public enums = enumsApi;
  public posts = postsApi;
  public users = usersApi;
}

// Create main API instance
export const api = new Api();

// For backward compatibility, you can also import individual APIs
export default api;