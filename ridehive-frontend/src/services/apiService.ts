import { Client, CarResponseDto, CarItem, UpdateCarDto } from '@/api/client';

export class ApiService {
  private client: Client;

  constructor(baseUrl: string = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7000') {
    this.client = new Client(baseUrl);
  }

  // Car-related methods
  async getAllCars(): Promise<CarResponseDto[]> {
    try {
      return await this.client.carsAll();
    } catch (error) {
      console.error('Error fetching all cars:', error);
      throw error;
    }
  }

  async getCarById(id: number): Promise<CarItem> {
    try {
      return await this.client.carsGET(id);
    } catch (error) {
      console.error(`Error fetching car with ID ${id}:`, error);
      throw error;
    }
  }

  async createCar(carData: {
    ownerId: number;
    brand: string;
    model: string;
    version?: string;
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
  }): Promise<CarItem> {
    try {
      // Convert File objects to FileParameter
      const ownershipDocParam = carData.ownershipDocument 
        ? { data: carData.ownershipDocument, fileName: carData.ownershipDocument.name }
        : undefined;

      const carImagesParams = carData.carImages?.map(file => ({
        data: file,
        fileName: file.name
      }));

      return await this.client.carsPOST(
        carData.ownerId,
        carData.brand,
        carData.model,
        carData.version,
        carData.color,
        carData.numberDoors,
        carData.numberSeats,
        carData.yearProduction,
        carData.course,
        carData.fuel,
        carData.consumption,
        carData.drive,
        carData.transmission,
        carData.body,
        carData.displacement,
        carData.horsePower,
        carData.condition,
        carData.vinNumber,
        ownershipDocParam,
        carImagesParams
      );
    } catch (error) {
      console.error('Error creating car:', error);
      throw error;
    }
  }

  async updateCar(id: number, carData: UpdateCarDto): Promise<void> {
    try {
      return await this.client.carsPUT(id, carData);
    } catch (error) {
      console.error(`Error updating car with ID ${id}:`, error);
      throw error;
    }
  }

  async deleteCar(id: number): Promise<void> {
    try {
      return await this.client.carsDELETE(id);
    } catch (error) {
      console.error(`Error deleting car with ID ${id}:`, error);
      throw error;
    }
  }

  // Utility method to get cars by owner (filter from all cars)
  async getCarsByOwner(ownerId: number): Promise<CarResponseDto[]> {
    try {
      const allCars = await this.getAllCars();
      return allCars.filter(car => car.ownerId === ownerId);
    } catch (error) {
      console.error(`Error fetching cars for owner ${ownerId}:`, error);
      throw error;
    }
  }
}

// Create a singleton instance
export const apiService = new ApiService();