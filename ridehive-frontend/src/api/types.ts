// Type definitions for API models - matching backend JSON response (camelCase)
export interface CarItem {
  carId: number;
  ownerId: string;
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
  ownershipDocumentPath?: string;
  ownershipDocumentContentType?: string;
}

// Car type used exclusively to deal with form car data at creation
export interface CarCreateDto {
  ownerId: string;
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
}

export interface CarResponseDto {
  carId: number;
  ownerId: string;
  brand: string;
  model: string;
  version?: string;
  color: string;
  numberDoors: number;
  numberSeats: number;
  yearProduction: number;
  course: number;
  consumption?: number;
  displacement: number;
  horsePower: number;
  vinNumber: string;
  // Display values (user-friendly descriptions from backend)
  fuelDisplay: string;
  driveDisplay: string;
  transmissionDisplay: string;
  bodyDisplay: string;
  conditionDisplay: string;
  carImages: CarImageData[];
}

export interface CarUpdateDto {
  brand?: string;
  model?: string;
  version?: string;
  color?: string;
  numberDoors?: number;
  numberSeats?: number;
  yearProduction?: number;
  course?: number;
  fuel?: string;
  consumption?: number;
  drive?: string;
  transmission?: string;
  body?: string;
  displacement?: number;
  horsePower?: number;
  condition?: string;
}

export interface CarImageData {
  carImageId: number;
  carId: number;
  imagePath: string; // Changed from imageData to imagePath
  imageContentType: string;
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

// Post-related interfaces
export interface PostItem {
  postId: number;
  ownerId: string;
  carId: number;
  title: string;
  description?: string;
  price: number;
  specialRequirements?: string;
  location: string;
  availableTimeSlots: string[];
  postedAt: string;
  available: boolean;
  // Optional car data for display (when populated)
  car?: CarItem;
}

export interface PostCreateDto {
  ownerId: string;
  carId: number;
  title: string;
  description?: string;
  price: number;
  specialRequirements?: string;
  location: string;
  availableTimeSlots: string[];
}

export interface PostResponseDto {
  postId: number;
  ownerId: string;
  carId: number;
  title: string;
  description?: string;
  price: number;
  specialRequirements?: string;
  location: string;
  availableTimeSlots: string[];
  postedAt: string;
  available: boolean;
}

// Owner mock interface
export interface Owner {
  ownerId: string;
  name: string;
  posts?: PostItem[];
}

// helper functions
// Dynamic case conversion utilities
export const toCamelCase = (str: string): string => {
  return str.charAt(0).toLowerCase() + str.slice(1);
};

export const toPascalCase = (str: string): string => {
  return str.charAt(0).toUpperCase() + str.slice(1);
};