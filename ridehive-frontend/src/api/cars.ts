//CRUD operations on cars
import { toPascalCase } from './types';
import { apiClient } from './base';
import type { CarItem, CarCreateDto, CarResponseDto, CarUpdateDto, CarImageData } from './types';

export class CarsApi {
  // Get all cars
  async getAllCars(): Promise<CarResponseDto[]> {
    return apiClient.get<CarResponseDto[]>('/api/Cars');
  }

  // Get car by ID
  async getCarById(id: number): Promise<CarResponseDto> {
    return apiClient.get<CarResponseDto>(`/api/Cars/${id}`);
  }

  // Get cars by owner ID
  async getCarsByOwner(ownerId: string): Promise<CarResponseDto[]> {
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

  // Helper function to get image URLs from car images
  getCarImageUrls(carImages: CarImageData[]): string[] {
    console.log('Processing car images:', carImages);
    
    if (!carImages || carImages.length === 0) {
      console.log('No car images to process');
      return [];
    }
    
    return carImages.map((image, index) => {
      const imageUrl = `${apiClient.getBaseUrl()}/${image.imagePath}`;
      console.log(`Image ${index + 1}:`, {
        carImageId: image.carImageId,
        imagePath: image.imagePath,
        contentType: image.imageContentType,
        fullUrl: imageUrl
      });
      return imageUrl;
    });
  }
}

// Create singleton instance
export const carsApi = new CarsApi();