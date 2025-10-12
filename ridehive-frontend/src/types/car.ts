// Re-export types from the auto-generated API client
export type { 
  CarItem, 
  CarResponseDto, 
  UpdateCarDto,
  BodyType,
  ConditionType,
  DriveTrainLayoutType,
  FuelType,
  TransmissionType
} from '@/api/client';

import { 
  FuelType, 
  BodyType, 
  ConditionType, 
  DriveTrainLayoutType, 
  TransmissionType 
} from '@/api/client';

// Helper function to convert enum to dropdown options
function enumToOptions<T extends Record<string, string>>(enumObject: T) {
  return Object.values(enumObject).map(value => ({
    value,
    label: value
  }));
}

// Dropdown options generated from auto-generated enums
export const FuelOptions = enumToOptions(FuelType);
export const DriveOptions = enumToOptions(DriveTrainLayoutType);
export const TransmissionOptions = enumToOptions(TransmissionType);
export const BodyOptions = enumToOptions(BodyType);
export const ConditionOptions = enumToOptions(ConditionType);