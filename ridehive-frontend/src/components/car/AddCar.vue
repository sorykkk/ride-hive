<script setup lang="ts">
import { ref, onBeforeMount, computed, nextTick } from 'vue';
import { useRouter } from 'vue-router';
import { useEnumStore } from '@/stores/enums.store';
import { 
  NCard, 
  NForm, 
  NFormItem, 
  NInput, 
  NInputNumber, 
  NSelect, 
  NButton, 
  NUpload, 
  NSpace,
  NGrid,
  NGridItem,
  useMessage,
  type FormInst,
  type FormRules,
  type UploadFileInfo
} from 'naive-ui';
import { api } from '@/api';
import { toCamelCase } from '@/api/cars';
import type { CarCreateDto } from '@/api/types';

const router = useRouter();
const message = useMessage();

// Form reference
const formRef = ref<FormInst | null>(null);

// Loading state
const loading = ref(false);

// Mock user ID - in real app this would come from auth store
const currentUserId = 1;

// Initialize enum store
const enumStore = useEnumStore();

// Form-specific type for UI state management - extends CarCreateDto with nullable fields for validation
interface CarFormData extends Omit<CarCreateDto, 'course' | 'displacement' | 'horsePower' | 'ownershipDocument' | 'carImages'> {
  course: number | null;
  displacement: number | null;
  horsePower: number | null;
  ownershipDocument: File | null;
  carImages: File[];
}

// Form data - using CarFormData type for better type safety
const formData = ref<CarFormData>({
  ownerId: currentUserId,
  brand: '',
  model: '',
  version: '',
  color: '',
  numberDoors: 4,
  numberSeats: 5,
  yearProduction: new Date().getFullYear(),
  course: null, // null to properly trigger validation
  fuel: '',
  consumption: 0.1, // default value
  drive: '',
  transmission: '',
  body: '',
  displacement: null,
  horsePower: null, 
  condition: 'BrandNew', // Default to BrandNew enum value
  vinNumber: '',
  ownershipDocument: null,
  carImages: []
});

// File upload handling
const ownershipDocumentList = ref<UploadFileInfo[]>([]);
const carImagesList = ref<UploadFileInfo[]>([]);

// Enhanced validation rules that include backend errors
const createDynamicRules = (): FormRules => {
  const baseRules: FormRules = {
    // Only essential UX validations that prevent unnecessary API calls
    brand: [
      { required: true, message: 'Brand is required', trigger: 'blur' }
    ],
    model: [
      { required: true, message: 'Model is required', trigger: 'blur' }
    ],
    color: [
      { required: true, message: 'Color is required', trigger: 'blur' }
    ],
    fuel: [
      { required: true, message: 'Fuel type is required', trigger: 'change' }
    ],
    drive: [
      { required: true, message: 'Drive type is required', trigger: 'change' }
    ],
    transmission: [
      { required: true, message: 'Transmission is required', trigger: 'change' }
    ],
    body: [
      { required: true, message: 'Body type is required', trigger: 'change' }
    ],
    vinNumber: [
      { required: true, message: 'VIN number is required', trigger: 'blur' }
    ],
    course: [
      { 
        required: true, 
        type: 'number',
        message: "Current mileage is required", 
        trigger: 'blur'
      }
    ],
    displacement: [
      {
        required: true,
        type: 'number',
        message: 'Displacement is required',
        trigger: 'blur'
      }
    ],
    horsePower: [
      {
        required: true,
        type: 'number',
        message: 'Horse Power is required',
        trigger: 'blur'
      }
    ],
    ownershipDocument: [
      {
        required: true,
        type: 'object',
        message: 'Document that proves ownership is required',
        trigger: ['blur', 'change']
      }
    ],
    carImages: [
      {
        required: true,
        validator: () => {
          if (!formData.value.carImages || formData.value.carImages.length === 0) {
            return Promise.reject(new Error('At least one car image is required'));
          }
          return Promise.resolve();
        },
        trigger: ['blur', 'change']
      }
    ]
  };

  // Add backend validation errors to rules
  Object.entries(backendValidationErrors.value).forEach(([field, messages]) => {
    const errorMessage = messages.join(', ');
    console.log(`Adding backend validation for field '${field}': ${errorMessage}`);
    
    if (baseRules[field]) {
      // Add backend errors to existing field rules
      const existingRules = baseRules[field];
      if (Array.isArray(existingRules)) {
        existingRules.push({
          validator: () => {
            return Promise.reject(new Error(errorMessage));
          },
          trigger: ['blur', 'change']
        });
      }
    } else {
      // Create new rule for backend-only validations
      baseRules[field] = [{
        validator: () => {
          return Promise.reject(new Error(errorMessage));
        },
        trigger: ['blur', 'change']
      }];
    }
  });

  console.log('Generated rules:', Object.keys(baseRules));
  return baseRules;
};

// Reactive rules that update when backend errors change
const rules = computed(() => createDynamicRules());

// Handle ownership document upload
const handleOwnershipDocumentChange = (options: { fileList: UploadFileInfo[] }) => {
  ownershipDocumentList.value = options.fileList;
  if (options.fileList.length > 0 && options.fileList[0]?.file) {
    formData.value.ownershipDocument = options.fileList[0].file as File;
  } else {
    formData.value.ownershipDocument = null;
  }
};

// Handle car images upload
const handleCarImagesChange = (options: { fileList: UploadFileInfo[] }) => {
  carImagesList.value = options.fileList;
  formData.value.carImages = options.fileList.map(item => item.file as File).filter(Boolean);
  
  // Clear backend validation error when user uploads images
  clearBackendError('carImages');
};

// Store backend validation errors
const backendValidationErrors = ref<Record<string, string[]>>({});

// Clear backend validation error for a specific field
const clearBackendError = (fieldName: string) => {
  if (backendValidationErrors.value[fieldName]) {
    delete backendValidationErrors.value[fieldName];
    console.log(`Cleared backend error for field: ${fieldName}`);
  }
};

// Submit form
const handleSubmit = async () => {
  if (!formRef.value) return;
  
  // Clear any previous backend validation errors at the start of each submission
  backendValidationErrors.value = {};
  
  // First, validate the form - if validation fails, don't proceed
  try {
    await formRef.value.validate();
  } catch (validationError) {
    // Form validation failed - Naive UI will show field-level errors
    // No need to show additional error messages, just return
    console.log('Form validation failed:', validationError);
    return;
  }

  // If we reach here, validation passed - proceed with API call
  loading.value = true;
  
  try {
    // Create CarCreateDto from formData with required field validation
    const carData: CarCreateDto = {
      ...formData.value,
      version: formData.value.version?.trim() || undefined,
      course: formData.value.course ?? 0,
      consumption: formData.value.consumption || undefined,
      displacement: formData.value.displacement ?? 0,
      horsePower: formData.value.horsePower ?? 0,
      ownershipDocument: formData.value.ownershipDocument || undefined,
      carImages: formData.value.carImages // Required field, should always have at least one image
    };

    const result = await api.cars.createCar(carData);
    console.log('Car created successfully:', result);
    
    message.success('Car added successfully!');
    router.push('/owned-properties');
  } catch (error: any) {
    console.error('Failed to create car:', error);
    console.log('Error name:', error.name);
    console.log('Error status:', error.status);
    console.log('Error data:', error.data);
    console.log('Is validation error:', error.isValidationError);
    
    // Handle our custom ApiError
    if (error.name === 'ApiError') {
      console.log('Processing ApiError...');
      
      if (error.isValidationError) {
        console.log('Processing validation errors...');
        
        let validationErrors: any = null;
        
        // Try to get validation errors from error.data.errors first
        if (error.data?.errors) {
          validationErrors = error.data.errors;
          console.log('Found errors in error.data.errors:', validationErrors);
        }
        // If not found, try to parse from error.data.message (JSON string)
        else if (error.data?.message && typeof error.data.message === 'string') {
          try {
            const parsedData = JSON.parse(error.data.message);
            if (parsedData.errors) {
              validationErrors = parsedData.errors;
              console.log('Found errors in parsed message:', validationErrors);
            }
          } catch (parseError) {
            console.log('Failed to parse error message as JSON:', parseError);
          }
        }
        
        if (validationErrors) {
          console.log('Backend validation errors received:', validationErrors);
          
          // Clear previous backend errors
          backendValidationErrors.value = {};
          
          // Process backend validation errors
          for (const [field, messages] of Object.entries(validationErrors)) {
            if (Array.isArray(messages)) {
              // Convert backend field names to frontend field names dynamically
              const frontendFieldName = toCamelCase(field);
              backendValidationErrors.value[frontendFieldName] = messages;
              console.log(`Mapped ${field} -> ${frontendFieldName}:`, messages);
            }
          }
          
          console.log('Final backend validation errors:', backendValidationErrors.value);
          
          // Force form to revalidate and show the backend errors
          if (formRef.value) {
            // Use nextTick to ensure the reactive rules have been updated
            nextTick(() => {
              formRef.value?.validate().catch(() => {
                // Expected to fail, backend validation errors will be shown
                console.log('Form revalidation triggered with backend errors');
              });
            });
          }
          return;
        }
      }
      
      // Handle other API errors
      if (error.data?.message) {
        message.error(error.data.message);
        return;
      }
      
      // Fallback for API errors
      message.error(`Server error: ${error.message}`);
      return;
    }
    
    // Handle network/other errors
    message.error('Failed to add car. Please check your connection and try again.');
  } finally {
    loading.value = false;
  }
};

// Cancel and go back
const handleCancel = () => {
  router.push('/profile');
};

// Initialize enums when component mounts
onBeforeMount(async () => {
  await enumStore.initializeEnums();
});
</script>

<template>
  <div class="add-car-container">
    <NCard class="add-car-card" title="Add New Car">
      <NForm
        ref="formRef"
        :model="formData"
        :rules="rules"
        label-placement="top"
        require-mark-placement="left"
        class="car-form"
      >
        <NGrid :cols="24" :x-gap="16">
          <!-- Basic Information -->
          <NGridItem :span="24">
            <h3 class="section-title">Basic Information</h3>
          </NGridItem>
          
          <NGridItem :span="12">
            <NFormItem label="Brand" path="brand">
              <NInput
                v-model:value="formData.brand"
                placeholder="Enter car brand"
                clearable
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="12">
            <NFormItem label="Model" path="model">
              <NInput
                v-model:value="formData.model"
                placeholder="Enter car model"
                clearable
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="12">
            <NFormItem label="Version">
              <NInput
                v-model:value="formData.version"
                placeholder="Enter car version (optional)"
                clearable
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="12">
            <NFormItem label="Color" path="color">
              <NInput
                v-model:value="formData.color"
                placeholder="Enter car color"
                clearable
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="8">
            <NFormItem label="Doors" path="numberDoors">
              <NInputNumber
                v-model:value="formData.numberDoors"
                :min="2"
                :max="6"
                placeholder="Doors"
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="8">
            <NFormItem label="Seats" path="numberSeats">
              <NInputNumber
                v-model:value="formData.numberSeats"
                :min="2"
                :max="9"
                placeholder="Seats"
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="8">
            <NFormItem label="Year" path="yearProduction">
              <NInputNumber
                v-model:value="formData.yearProduction"
                :min="1900"
                :max="new Date().getFullYear()"
                placeholder="Year"
              />
            </NFormItem>
          </NGridItem>

          <!-- Technical Specifications -->
          <NGridItem :span="24">
            <h3 class="section-title">Technical Specifications</h3>
          </NGridItem>
          
          <NGridItem :span="12">
            <NFormItem label="Fuel Type" path="fuel">
              <NSelect
                v-model:value="formData.fuel"
                :options="enumStore.fuelOptions"
                placeholder="Select fuel type"
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="12">
            <NFormItem label="Drive Type" path="drive">
              <NSelect
                v-model:value="formData.drive"
                :options="enumStore.driveOptions"
                placeholder="Select drive type"
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="12">
            <NFormItem label="Transmission" path="transmission">
              <NSelect
                v-model:value="formData.transmission"
                :options="enumStore.transmissionOptions"
                placeholder="Select transmission"
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="12">
            <NFormItem label="Body Type" path="body">
              <NSelect
                v-model:value="formData.body"
                :options="enumStore.bodyOptions"
                placeholder="Select body type"
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="8">
            <NFormItem label="Displacement (cm³)" path="displacement">
              <NInputNumber
                v-model:value="formData.displacement"
                :min="50"
                :max="10_000"
                placeholder="Engine size"
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="8">
            <NFormItem label="Horse Power" path="horsePower">
              <NInputNumber
                v-model:value="formData.horsePower"
                :min="10"
                :max="2_000"
                placeholder="HP"
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="8">
            <NFormItem label="Consumption (L/100km)">
              <NInputNumber
                v-model:value="formData.consumption"
                :min="0.1"
                :max="50"
                :precision="1"
                placeholder="Fuel consumption"
              />
            </NFormItem>
          </NGridItem>

          <!-- Additional Information -->
          <NGridItem :span="24">
            <h3 class="section-title">Additional Information</h3>
          </NGridItem>
          
          <NGridItem :span="12">
            <NFormItem label="Mileage (km)" path="course">
              <NInputNumber
                v-model:value="formData.course"
                :min="0"
                :max="1_000_000_000"
                placeholder="Current mileage"
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="12">
            <NFormItem label="Condition">
              <NSelect
                v-model:value="formData.condition"
                :options="enumStore.conditionOptions"
                placeholder="Select condition"
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="24">
            <NFormItem label="VIN Number" path="vinNumber">
              <NInput
                v-model:value="formData.vinNumber"
                placeholder="Enter 17-character VIN number"
                clearable
                minlength="17"
                maxlength="17"
                @input="() => clearBackendError('vinNumber')"
              />
            </NFormItem>
          </NGridItem>

          <!-- File Uploads -->
          <NGridItem :span="24">
            <h3 class="section-title">Documents & Images</h3>
          </NGridItem>
          
          <NGridItem :span="12">
            <NFormItem label="Ownership Document" path="ownershipDocument">
              <NUpload
                v-model:file-list="ownershipDocumentList"
                accept=".pdf,.doc,.docx,.jpg,.png"
                :max="1"
                @change="handleOwnershipDocumentChange"
                class="upload-area"
              >
                <NButton>Upload Ownership Document</NButton>
              </NUpload>
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="12">
            <NFormItem label="Car Images" path="carImages">
              <NUpload
                v-model:file-list="carImagesList"
                accept="image/*"
                :max="10"
                multiple
                @change="handleCarImagesChange"
                class="upload-area"
              >
                <NButton>Upload Car Images</NButton>
              </NUpload>
            </NFormItem>
          </NGridItem>

          <!-- Actions -->
          <NGridItem :span="24">
            <NSpace justify="end" class="form-actions">
              <NButton @click="handleCancel">
                Cancel
              </NButton>
              <NButton
                type="primary"
                :loading="loading"
                @click="handleSubmit"
              >
                Add Car
              </NButton>
            </NSpace>
          </NGridItem>
        </NGrid>
      </NForm>
    </NCard>
  </div>
</template>

<style scoped>
.add-car-container {
  max-width: 1000px;
  margin: 0 auto;
  padding: 2rem;
}

.add-car-card {
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
  border-radius: 12px;
}

.car-form {
  margin-top: 1rem;
}

.section-title {
  color: #2c3e50;
  font-size: 1.2rem;
  font-weight: 600;
  margin: 1.5rem 0 1rem 0;
  padding-bottom: 0.5rem;
  border-bottom: 2px solid #e0e0e6;
}

.section-title:first-child {
  margin-top: 0;
}

.upload-area {
  width: 100%;
}

.form-actions {
  margin-top: 2rem;
  padding-top: 1.5rem;
  border-top: 1px solid #e0e0e6;
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .add-car-container {
    padding: 1rem;
  }
}
</style>