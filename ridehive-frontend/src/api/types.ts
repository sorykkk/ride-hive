// Type definitions for API models
export interface CarItem {
  carId: number;
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
  carImages: CarImageData[];
  ownershipDocumentData?: Uint8Array;
  ownershipDocumentContentType?: string;
}

export interface CarResponseDto {
  carId: number;
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
  carImages: CarImageData[];
}

export interface UpdateCarDto {
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
}

export interface CarImageData {
  carImageId: number;
  carId: number;
  imageData: Uint8Array;
  contentType: string;
  fileName: string;
  uploadedAt: string;
}

export interface EnumOption {
  value: string;
  label: string;
  numericValue: number;
}

export interface EnumCollections {
  fuelTypes: EnumOption[];
  driveTypes: EnumOption[];
  transmissionTypes: EnumOption[];
  bodyTypes: EnumOption[];
  conditionTypes: EnumOption[];
}