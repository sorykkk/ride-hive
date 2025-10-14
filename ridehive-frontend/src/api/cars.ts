//CRUD operations on cars
import { apiClient } from './base';
import type { CarItem, CarCreateDto, CarResponseDto, CarUpdateDto } from './types';

// Map backend field names to frontend field names
export const mapBackendFieldToFrontend = (backendField: string): string => {
  const fieldMap: Record<string, string> = {
    'OwnerId': 'ownerId',
    'Brand': 'brand',
    'Model': 'model',
    'Version': 'version',
    'Color': 'color',
    'NumberDoors': 'numberDoors',
    'NumberSeats': 'numberSeats',
    'YearProduction': 'yearProduction',
    'Course': 'course',
    'Fuel': 'fuel',
    'Consumption': 'consumption',
    'Drive': 'drive',
    'Transmission': 'transmission',
    'Body': 'body',
    'Displacement': 'displacement',
    'HorsePower': 'horsePower',
    'Condition': 'condition',
    'VinNumber': 'vinNumber',
    'OwnershipDocument': 'ownershipDocument',
    'CarImages': 'carImages'
  };
  
  return fieldMap[backendField] || backendField.toLowerCase();
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
    return apiClient.get<CarResponseDto[]>('/api/Cars', { ownerId });
  }

  // Create new car with file uploads
  async createCar(carData: CarCreateDto): Promise<CarItem> {
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