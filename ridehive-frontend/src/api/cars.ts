//CRUD operations on cars
import { apiClient } from './base';
import type { CarItem, CarResponseDto, UpdateCarDto } from './types';

export interface CarCreateData {
  ownerId: number;
  brand: string;
  model: string;
  version?: string; //todo: shouldn't it be version?: string | null?
  color: string;
  numberDoors: number;
  numberSeats: number;
  yearProduction: number;
  course: number;
  fuel: string;
  consumption?: number;
  drive: string;
  transmission: string;
  body: string;
  displacement: number;
  horsePower: number;
  condition: string;
  vinNumber: string;
  ownershipDocument?: File;
  carImages?: File[];
}

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
    return apiClient.get<CarResponseDto[]>('/api/Cars', { ownerId });
  }

  // Create new car with file uploads
  async createCar(carData: CarCreateData): Promise<CarItem> {
    const formData = new FormData();

    // Add all car properties to FormData
    formData.append('OwnerId', carData.ownerId.toString());
    formData.append('Brand', carData.brand);
    formData.append('Model', carData.model);
    
    if (carData.version) {
      formData.append('Version', carData.version);
    }
    
    formData.append('Color', carData.color);
    formData.append('NumberDoors', carData.numberDoors.toString());
    formData.append('NumberSeats', carData.numberSeats.toString());
    formData.append('YearProduction', carData.yearProduction.toString());
    formData.append('Course', carData.course.toString());
    formData.append('Fuel', carData.fuel);
    
    if (carData.consumption !== undefined && carData.consumption !== null) {
      formData.append('Consumption', carData.consumption.toString());
    }
    
    formData.append('Drive', carData.drive);
    formData.append('Transmission', carData.transmission);
    formData.append('Body', carData.body);
    formData.append('Displacement', carData.displacement.toString());
    formData.append('HorsePower', carData.horsePower.toString());
    formData.append('Condition', carData.condition);
    formData.append('VinNumber', carData.vinNumber);

    // Add ownership document if provided
    if (carData.ownershipDocument) {
      formData.append('OwnershipDocument', carData.ownershipDocument);
    }

    // Add car images if provided
    if (carData.carImages && carData.carImages.length > 0) {
      carData.carImages.forEach((image) => {
        formData.append(`CarImages`, image);
      });
    }

    return apiClient.postFormData<CarItem>('/api/Cars', formData);
  }

  // Update existing car
  async updateCar(id: number, carData: UpdateCarDto): Promise<void> {
    return apiClient.put<void>(`/api/Cars/${id}`, carData);
  }

  // Delete car
  async deleteCar(id: number): Promise<void> {
    return apiClient.delete<void>(`/api/Cars/${id}`);
  }
}

// Create singleton instance
export const carsApi = new CarsApi();