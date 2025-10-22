// helper functions
// Dynamic case conversion utilities
export const toCamelCase = (str: string): string => {
  return str.charAt(0).toLowerCase() + str.slice(1);
};

export const toPascalCase = (str: string): string => {
  return str.charAt(0).toUpperCase() + str.slice(1);
};


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
  // Enum values (for form editing)
  fuel: string;
  drive: string;
  transmission: string;
  body: string;
  condition: string;
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

export interface UserAuthResponseDto {
  userId: string
  email: string
  name: string
  surname: string
  role: 'Client' | 'Owner'
  registeredAt: string
  phone?: number
  age?: number
  bio?: string
  location?: string
  hasProfileImage?: boolean
}

export interface UserLoginDto {
  email: string
  password: string
}

export interface UserRegisterDto {
  name: string
  surname: string
  email: string
  password: string
  role: 'Client' | 'Owner'
  age?: number
  phone?: number
  drivingLicenseImage?: File
}

export interface UpdateProfileDto {
  name: string
  surname: string
  phone?: number
  age?: number
  bio?: string
  location?: string
}

// Basic user info for displaying owner names, etc.
export interface BasicUserInfo {
  userId: string
  name: string
  surname: string
  fullName: string
  role: 'Client' | 'Owner'
  hasProfileImage: boolean
}