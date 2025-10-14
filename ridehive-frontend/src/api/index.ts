// Export all API modules and types
export { ApiClient, ApiError, apiClient } from './base';
export { CarsApi, carsApi, type CarCreateData } from './cars';
export { EnumsApi, enumsApi } from './enums';

// Import for internal use
import { carsApi } from './cars';
import { enumsApi } from './enums';

// Export all types
export type {
  CarItem,
  CarResponseDto,
  UpdateCarDto,
  CarImageData,
  EnumOption,
  EnumCollections
} from './types';

// Main API class that combines all APIs
export class Api {
  public cars = carsApi;
  public enums = enumsApi;
}

// Create main API instance
export const api = new Api();

// For backward compatibility, you can also import individual APIs
export default api;