<script setup lang="ts">
import { ref, reactive } from 'vue';
import { useRouter } from 'vue-router';
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
import { apiService } from '@/services/apiService';
import { 
  FuelOptions, 
  DriveOptions, 
  TransmissionOptions, 
  BodyOptions, 
  ConditionOptions 
} from '@/types/car';

const router = useRouter();
const message = useMessage();

// Form reference
const formRef = ref<FormInst | null>(null);

// Loading state
const loading = ref(false);

// Mock user ID - in real app this would come from auth store
const currentUserId = 1;

// Form data
const formData = reactive({
  ownerId: currentUserId,
  brand: '',
  model: '',
  version: '',
  color: '',
  numberDoors: 4,
  numberSeats: 5,
  yearProduction: new Date().getFullYear(),
  course: 0,
  fuel: '',
  consumption: 0,
  drive: '',
  transmission: '',
  body: '',
  displacement: 0,
  horsePower: 0,
  condition: '',
  vinNumber: '',
  ownershipDocument: null as File | null,
  carImages: [] as File[]
});

// File upload handling
const ownershipDocumentList = ref<UploadFileInfo[]>([]);
const carImagesList = ref<UploadFileInfo[]>([]);

// Form validation rules
const rules: FormRules = {
  brand: [
    { required: true, message: 'Please enter car brand', trigger: 'blur' }
  ],
  model: [
    { required: true, message: 'Please enter car model', trigger: 'blur' }
  ],
  color: [
    { required: true, message: 'Please enter car color', trigger: 'blur' }
  ],
  yearProduction: [
    { required: true, message: 'Please enter production year', trigger: 'blur' },
    { type: 'number', min: 1900, max: new Date().getFullYear() + 1, message: 'Please enter a valid year', trigger: 'blur' }
  ],
  fuel: [
    { required: true, message: 'Please select fuel type', trigger: 'change' }
  ],
  drive: [
    { required: true, message: 'Please select drive type', trigger: 'change' }
  ],
  transmission: [
    { required: true, message: 'Please select transmission type', trigger: 'change' }
  ],
  body: [
    { required: true, message: 'Please select body type', trigger: 'change' }
  ],
  condition: [
    { required: true, message: 'Please select condition', trigger: 'change' }
  ],
  vinNumber: [
    { required: true, message: 'Please enter VIN number', trigger: 'blur' },
    { min: 17, max: 17, message: 'VIN number must be exactly 17 characters', trigger: 'blur' }
  ]
};

// Handle ownership document upload
const handleOwnershipDocumentChange = (options: { fileList: UploadFileInfo[] }) => {
  ownershipDocumentList.value = options.fileList;
  if (options.fileList.length > 0 && options.fileList[0]?.file) {
    formData.ownershipDocument = options.fileList[0].file as File;
  } else {
    formData.ownershipDocument = null;
  }
};

// Handle car images upload
const handleCarImagesChange = (options: { fileList: UploadFileInfo[] }) => {
  carImagesList.value = options.fileList;
  formData.carImages = options.fileList.map(item => item.file as File).filter(Boolean);
};

// Submit form
const handleSubmit = async () => {
  if (!formRef.value) return;
  
  try {
    await formRef.value.validate();
    loading.value = true;

    const carData = {
      ownerId: formData.ownerId,
      brand: formData.brand,
      model: formData.model,
      version: formData.version || undefined,
      color: formData.color,
      numberDoors: formData.numberDoors,
      numberSeats: formData.numberSeats,
      yearProduction: formData.yearProduction,
      course: formData.course,
      fuel: formData.fuel,
      consumption: formData.consumption || undefined,
      drive: formData.drive,
      transmission: formData.transmission,
      body: formData.body,
      displacement: formData.displacement,
      horsePower: formData.horsePower,
      condition: formData.condition,
      vinNumber: formData.vinNumber,
      ownershipDocument: formData.ownershipDocument || undefined,
      carImages: formData.carImages.length > 0 ? formData.carImages : undefined
    };

    const result = await apiService.createCar(carData);
    console.log('Car created successfully:', result);
    
    message.success('Car added successfully!');
    router.push('/profile');
  } catch (error) {
    console.error('Failed to create car:', error);
    message.error('Failed to add car. Please try again.');
  } finally {
    loading.value = false;
  }
};

// Cancel and go back
const handleCancel = () => {
  router.push('/profile');
};
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
                :max="new Date().getFullYear() + 1"
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
                :options="FuelOptions"
                placeholder="Select fuel type"
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="12">
            <NFormItem label="Drive Type" path="drive">
              <NSelect
                v-model:value="formData.drive"
                :options="DriveOptions"
                placeholder="Select drive type"
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="12">
            <NFormItem label="Transmission" path="transmission">
              <NSelect
                v-model:value="formData.transmission"
                :options="TransmissionOptions"
                placeholder="Select transmission"
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="12">
            <NFormItem label="Body Type" path="body">
              <NSelect
                v-model:value="formData.body"
                :options="BodyOptions"
                placeholder="Select body type"
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="8">
            <NFormItem label="Displacement (L)">
              <NInputNumber
                v-model:value="formData.displacement"
                :min="0.1"
                :max="10"
                :precision="1"
                placeholder="Engine size"
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="8">
            <NFormItem label="Horse Power">
              <NInputNumber
                v-model:value="formData.horsePower"
                :min="10"
                :max="2000"
                placeholder="HP"
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="8">
            <NFormItem label="Consumption (L/100km)">
              <NInputNumber
                v-model:value="formData.consumption"
                :min="0"
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
            <NFormItem label="Mileage (km)">
              <NInputNumber
                v-model:value="formData.course"
                :min="0"
                :max="1000000"
                placeholder="Current mileage"
              />
            </NFormItem>
          </NGridItem>
          
          <NGridItem :span="12">
            <NFormItem label="Condition" path="condition">
              <NSelect
                v-model:value="formData.condition"
                :options="ConditionOptions"
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
                maxlength="17"
              />
            </NFormItem>
          </NGridItem>

          <!-- File Uploads -->
          <NGridItem :span="24">
            <h3 class="section-title">Documents & Images</h3>
          </NGridItem>
          
          <NGridItem :span="12">
            <NFormItem label="Ownership Document">
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