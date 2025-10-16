//CRUD operations on cars
import { apiClient } from './base';
import type { CarItem, CarCreateDto, CarResponseDto, CarUpdateDto } from './types';

// Dynamic case conversion utilities - no more manual field mapping!
export const toCamelCase = (str: string): string => {
  return str.charAt(0).toLowerCase() + str.slice(1);
};

export const toPascalCase = (str: string): string => {
  return str.charAt(0).toUpperCase() + str.slice(1);
};

export class CarsApi {
  // Get all cars
  async getAllCars(): Promise<CarResponseDto[]> {
    return apiClient.get<CarResponseDto[]>('/api/Cars');
  }

  // Get car by ID
  async getCarById(id: number): Promise<CarItem> {
    return apiClient.get<CarItem>(`/api/Cars/${id}`);
  }

  // Get cars by owner ID
  async getCarsByOwner(ownerId: number): Promise<CarResponseDto[]> {
    return apiClient.get<CarResponseDto[]>(`/api/Cars/owner/${ownerId}`);
  }

  // Create new car with file uploads
  async createCar(carData: CarCreateDto): Promise<CarItem> {
    const formData = new FormData();

    // Dynamically add all properties - fully automated!
    Object.entries(carData).forEach(([key, value]) => {
      if (value !== undefined && value !== null) {
        // Convert camelCase to PascalCase for backend
        const backendKey = toPascalCase(key);
        
        if (key === 'carImages' && Array.isArray(value)) {
          // Handle multiple files
          value.forEach(file => formData.append('CarImages', file));
        } else if (key === 'ownershipDocument' && value instanceof File) {
          // Handle single file
          formData.append('OwnershipDocument', value);
        } else if (!(value instanceof File) && !Array.isArray(value)) {
          // Handle primitive values
          formData.append(backendKey, value.toString());
        }
      }
    });

    return apiClient.postFormData<CarItem>('/api/Cars', formData);
  }

  // Update existing car
  async updateCar(id: number, carData: CarUpdateDto): Promise<void> {
    return apiClient.put<void>(`/api/Cars/${id}`, carData);
  }

  // Delete car
  async deleteCar(id: number): Promise<void> {
    return apiClient.delete<void>(`/api/Cars/${id}`);
  }
}

// Create singleton instance
export const carsApi = new CarsApi();