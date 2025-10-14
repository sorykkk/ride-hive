<script setup lang="ts">
import { ref, onBeforeMount } from 'vue';
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

// Form data
const formData = ref({
  ownerId: currentUserId,
  brand: '',
  model: '',
  version: '' as string | null,
  color: '',
  numberDoors: 4,
  numberSeats: 5,
  yearProduction: new Date().getFullYear(),
  course: null as number | null, // null to properly trigger validation
  fuel: '',
  consumption: 0.1, // default value
  drive: '',
  transmission: '',
  body: '',
  displacement: null as number | null,
  horsePower: null as number | null, 
  condition: 'Brand new',
  vinNumber: '',
  ownershipDocument: null as File | null,
  carImages: [] as File[]
});

// File upload handling
const ownershipDocumentList = ref<UploadFileInfo[]>([]);
const carImagesList = ref<UploadFileInfo[]>([]);

// Minimal frontend validation - let backend handle detailed validation
const rules: FormRules = {
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
      // trigger: ['blur', 'change'] 
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
  ]
};

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
};

// Submit form
const handleSubmit = async () => {
  if (!formRef.value) return;
  
  try {
    await formRef.value.validate();
    loading.value = true;

    const carData = {
      ownerId: formData.value.ownerId,
      brand: formData.value.brand,
      model: formData.value.model,
      version: formData.value.version?.trim() ?? "",
      color: formData.value.color,
      numberDoors: formData.value.numberDoors,
      numberSeats: formData.value.numberSeats,
      yearProduction: formData.value.yearProduction,
      course: formData.value.course || 0,
      fuel: formData.value.fuel,
      consumption: formData.value.consumption || 0, // Send 0 for optional numeric field
      drive: formData.value.drive,
      transmission: formData.value.transmission,
      body: formData.value.body,
      displacement: formData.value.displacement || 0,
      horsePower: formData.value.horsePower || 0,
      condition: formData.value.condition,
      vinNumber: formData.value.vinNumber,
      ownershipDocument: formData.value.ownershipDocument || undefined,
      carImages: formData.value.carImages.length > 0 ? formData.value.carImages : undefined
    };

    const result = await api.cars.createCar(carData);
    console.log('Car created successfully:', result);
    
    message.success('Car added successfully!');
    router.push('/profile');
  } catch (error: any) {
    console.error('Failed to create car:', error);
    
    // Handle our custom ApiError
    if (error.name === 'ApiError') {
      if (error.isValidationError && error.data?.errors) {
        // Handle ASP.NET Core validation errors
        const validationErrors = error.data.errors;
        for (const [field, messages] of Object.entries(validationErrors)) {
          if (Array.isArray(messages)) {
            messages.forEach((msg: string) => message.error(`${field}: ${msg}`));
          }
        }
        return;
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
            <NFormItem label="Car Images">
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