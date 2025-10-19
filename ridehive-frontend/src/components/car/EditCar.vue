<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useEnumStore } from '@/stores/enums.store';
import { 
  NCard, 
  NForm, 
  NFormItem, 
  NInput, 
  NInputNumber, 
  NSelect, 
  NButton, 
  NSpace,
  NGrid,
  NGridItem,
  NPageHeader,
  NSpin,
  useMessage,
  type FormInst,
  type FormRules
} from 'naive-ui';
import { carsApi, ApiError } from '@/api';
import type { CarUpdateDto, CarItem } from '@/api/types';

const router = useRouter();
const route = useRoute();
const message = useMessage();

// Form reference
const formRef = ref<FormInst | null>(null);

// Loading states
const loading = ref(false);
const saving = ref(false);

// Car ID from route parameter
const carId = computed(() => {
  const id = route.params.id;
  return typeof id === 'string' ? parseInt(id) : 0;
});

// Initialize enum store
const enumStore = useEnumStore();

// Form data - using CarUpdateDto type with proper initialization
const formData = ref<CarUpdateDto>({
  brand: '',
  model: '',
  version: '',
  color: '',
  numberDoors: 4,
  numberSeats: 5,
  yearProduction: new Date().getFullYear(),
  course: 0,
  fuel: '',
  consumption: 0.1,
  drive: '',
  transmission: '',
  body: '',
  displacement: 0,
  horsePower: 0,
  condition: 'BrandNew'
});

// Backend validation errors (merged with frontend rules)
const backendValidationErrors = ref<Record<string, string[]>>({});

// Enhanced validation rules that include backend errors
const createDynamicRules = (): FormRules => {
  const baseRules: FormRules = {
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
    condition: [
      { required: true, message: 'Condition is required', trigger: 'change' }
    ],
    course: [
      { type: 'number', required: true, message: 'Course/Mileage is required', trigger: 'blur' },
      { type: 'number', min: 0, message: 'Course must be a positive number', trigger: 'blur' }
    ],
    displacement: [
      { type: 'number', required: true, message: 'Engine displacement is required', trigger: 'blur' },
      { type: 'number', min: 0.1, message: 'Engine displacement must be greater than 0', trigger: 'blur' }
    ],
    horsePower: [
      { type: 'number', required: true, message: 'Horse power is required', trigger: 'blur' },
      { type: 'number', min: 1, message: 'Horse power must be at least 1', trigger: 'blur' }
    ]
  };

  // Merge backend validation errors into rules
  Object.entries(backendValidationErrors.value).forEach(([field, messages]) => {
    if (messages.length > 0) {
      const rule = {
        validator: () => false, // Always fail validation to show backend error
        message: messages[0], // Show first backend error
        trigger: 'blur'
      };

      if (baseRules[field]) {
        baseRules[field] = [...(baseRules[field] as any[]), rule];
      } else {
        baseRules[field] = [rule];
      }
    }
  });

  return baseRules;
};

// Reactive validation rules
const validationRules = computed(() => createDynamicRules());

// Load car data
const loadCarData = async () => {
  try {
    loading.value = true;
    const car: CarItem = await carsApi.getCarById(carId.value);
    
    // Map CarItem to CarUpdateDto (only updatable fields)
    formData.value = {
      brand: car.brand,
      model: car.model,
      version: car.version || '',
      color: car.color,
      numberDoors: car.numberDoors,
      numberSeats: car.numberSeats,
      yearProduction: car.yearProduction,
      course: car.course,
      fuel: car.fuel || '',
      consumption: car.consumption,
      drive: car.drive || '',
      transmission: car.transmission || '',
      body: car.body || '',
      displacement: car.displacement,
      horsePower: car.horsePower,
      condition: car.condition || 'BrandNew'
    };
    
    console.log('Loaded car data:', car);
    console.log('Form data after mapping:', formData.value);
    console.log('Available enum options:', {
      fuelOptions: enumStore.fuelOptions,
      driveOptions: enumStore.driveOptions,
      transmissionOptions: enumStore.transmissionOptions,
      bodyOptions: enumStore.bodyOptions,
      conditionOptions: enumStore.conditionOptions
    });
    
    // Debug: Check if car values match available options
    console.log('Value matching check:');
    console.log('Car fuel:', car.fuel, 'Available fuel options:', enumStore.fuelOptions.map(o => o.value));
    console.log('Car drive:', car.drive, 'Available drive options:', enumStore.driveOptions.map(o => o.value));
    console.log('Car transmission:', car.transmission, 'Available transmission options:', enumStore.transmissionOptions.map(o => o.value));
    console.log('Car body:', car.body, 'Available body options:', enumStore.bodyOptions.map(o => o.value));
    console.log('Car condition:', car.condition, 'Available condition options:', enumStore.conditionOptions.map(o => o.value));
  } catch (error) {
    console.error('Failed to load car:', error);
    if (error instanceof ApiError) {
      message.error(`Failed to load car: ${error.message}`);
    } else {
      message.error('Failed to load car. Please try again.');
    }
    router.push('/owned-properties');
  } finally {
    loading.value = false;
  }
};

// Submit handler
const handleSubmit = async () => {
  if (!formRef.value) return;

  try {
    // Clear previous backend errors
    backendValidationErrors.value = {};
    
    // Validate frontend first
    await formRef.value.validate();
    
    saving.value = true;
    
    // Update the car
    await carsApi.updateCar(carId.value, formData.value);
    
    message.success('Car updated successfully!');
    router.push('/owned-properties');
    
  } catch (error) {
    console.error('Update car error:', error);
    
    if (error instanceof ApiError) {
      message.error(`Failed to update car: ${error.message}`);
    } else {
      message.error('An unexpected error occurred. Please try again.');
    }
  } finally {
    saving.value = false;
  }
};

// Cancel handler
const handleCancel = () => {
  router.push('/owned-properties');
};

// Load enums and car data on component mount
onMounted(async () => {
  try {
    // Load enums first to ensure options are available
    await enumStore.fetchEnums();
    console.log('Enums loaded:', {
      fuelOptions: enumStore.fuelOptions,
      driveOptions: enumStore.driveOptions,
      transmissionOptions: enumStore.transmissionOptions,
      bodyOptions: enumStore.bodyOptions,
      conditionOptions: enumStore.conditionOptions
    });
    
    // Then load car data
    console.log('Enums loaded, now loading car data...');
    await loadCarData();
  } catch (error) {
    console.error('Error during component initialization:', error);
    message.error('Failed to initialize component. Please refresh the page.');
  }
});
</script>

<template>
  <div class="edit-car-container">
    <NPageHeader @back="handleCancel">
      <template #title>Edit Car</template>
      <template #subtitle>Update your vehicle information</template>
    </NPageHeader>

    <NSpin :show="loading" description="Loading car data...">
      <div v-if="!loading" class="form-container">
        <NCard title="Vehicle Information">
          <NForm
            ref="formRef"
            :model="formData"
            :rules="validationRules"
            label-placement="top"
            size="large"
            class="car-form"
          >
            <NGrid :cols="1" :x-gap="16" :y-gap="16">
              <!-- Basic Information -->
              <NGridItem>
                <div class="form-section">
                  <h3 class="section-title">Basic Information</h3>
                  <NGrid :cols="2" :x-gap="16" :y-gap="16" responsive="screen">
                    <NGridItem span="2 s:1">
                      <NFormItem label="Brand" path="brand">
                        <NInput v-model:value="formData.brand" placeholder="e.g., BMW, Mercedes, Audi" />
                      </NFormItem>
                    </NGridItem>
                    
                    <NGridItem span="2 s:1">
                      <NFormItem label="Model" path="model">
                        <NInput v-model:value="formData.model" placeholder="e.g., X5, C-Class, A4" />
                      </NFormItem>
                    </NGridItem>
                    
                    <NGridItem span="2 s:1">
                      <NFormItem label="Version (Optional)" path="version">
                        <NInput v-model:value="formData.version" placeholder="e.g., Sport, Premium, Base" />
                      </NFormItem>
                    </NGridItem>
                    
                    <NGridItem span="2 s:1">
                      <NFormItem label="Color" path="color">
                        <NInput v-model:value="formData.color" placeholder="e.g., Black, White, Red" />
                      </NFormItem>
                    </NGridItem>
                  </NGrid>
                </div>
              </NGridItem>

              <!-- Physical Specifications -->
              <NGridItem>
                <div class="form-section">
                  <h3 class="section-title">Physical Specifications</h3>
                  <NGrid :cols="2" :x-gap="16" :y-gap="16" responsive="screen">
                    <NGridItem span="2 s:1">
                      <NFormItem label="Number of Doors" path="numberDoors">
                        <NInputNumber 
                          v-model:value="formData.numberDoors" 
                          :min="2"
                          :max="6"
                          :step="1"
                          class="full-width"
                          placeholder="Enter number of doors"
                        />
                      </NFormItem>
                    </NGridItem>
                    
                    <NGridItem span="2 s:1">
                      <NFormItem label="Number of Seats" path="numberSeats">
                        <NInputNumber 
                          v-model:value="formData.numberSeats" 
                          :min="2"
                          :max="9"
                          :step="1"
                          class="full-width"
                          placeholder="Enter number of seats"
                        />
                      </NFormItem>
                    </NGridItem>
                    
                    <NGridItem span="2 s:1">
                      <NFormItem label="Year of Production" path="yearProduction">
                        <NInputNumber 
                          v-model:value="formData.yearProduction" 
                          :min="1990"
                          :max="new Date().getFullYear()"
                          :step="1"
                          class="full-width"
                          placeholder="Enter production year"
                        />
                      </NFormItem>
                    </NGridItem>
                    
                    <NGridItem span="2 s:1">
                      <NFormItem label="Mileage (km)" path="course">
                        <NInputNumber 
                          v-model:value="formData.course" 
                          :min="0"
                          :step="1000"
                          class="full-width"
                          placeholder="Enter current mileage"
                        />
                      </NFormItem>
                    </NGridItem>
                  </NGrid>
                </div>
              </NGridItem>

              <!-- Technical Specifications -->
              <NGridItem>
                <div class="form-section">
                  <h3 class="section-title">Technical Specifications</h3>
                  <NGrid :cols="2" :x-gap="16" :y-gap="16" responsive="screen">
                    <NGridItem span="2 s:1">
                      <NFormItem label="Fuel Type" path="fuel">
                        <NSelect 
                          v-model:value="formData.fuel" 
                          :options="enumStore.fuelOptions" 
                          value-field="value"
                          label-field="label"
                          placeholder="Select fuel type"
                        />
                      </NFormItem>
                    </NGridItem>
                    
                    <NGridItem span="2 s:1">
                      <NFormItem label="Fuel Consumption (L/100km)" path="consumption">
                        <NInputNumber 
                          v-model:value="formData.consumption" 
                          :min="0.1"
                          :step="0.1"
                          :precision="1"
                          class="full-width"
                          placeholder="e.g., 7.5"
                        />
                      </NFormItem>
                    </NGridItem>
                    
                    <NGridItem span="2 s:1">
                      <NFormItem label="Drive Type" path="drive">
                        <NSelect 
                          v-model:value="formData.drive" 
                          :options="enumStore.driveOptions"
                          value-field="value"
                          label-field="label"
                          placeholder="Select drive type"
                        />
                      </NFormItem>
                    </NGridItem>
                    
                    <NGridItem span="2 s:1">
                      <NFormItem label="Transmission" path="transmission">
                        <NSelect 
                          v-model:value="formData.transmission" 
                          :options="enumStore.transmissionOptions"
                          value-field="value"
                          label-field="label"
                          placeholder="Select transmission"
                        />
                      </NFormItem>
                    </NGridItem>
                  </NGrid>
                </div>
              </NGridItem>

              <!-- Engine & Condition -->
              <NGridItem>
                <div class="form-section">
                  <h3 class="section-title">Engine & Condition</h3>
                  <NGrid :cols="2" :x-gap="16" :y-gap="16" responsive="screen">
                    <NGridItem span="2 s:1">
                      <NFormItem label="Body Type" path="body">
                        <NSelect 
                          v-model:value="formData.body" 
                          :options="enumStore.bodyOptions"
                          value-field="value"
                          label-field="label"
                          placeholder="Select body type"
                        />
                      </NFormItem>
                    </NGridItem>
                    
                    <NGridItem span="2 s:1">
                      <NFormItem label="Engine Displacement (cm³)" path="displacement">
                        <NInputNumber 
                          v-model:value="formData.displacement" 
                          :min="0.1"
                          :step="0.1"
                          :precision="1"
                          class="full-width"
                          placeholder="e.g., 2.0"
                        />
                      </NFormItem>
                    </NGridItem>
                    
                    <NGridItem span="2 s:1">
                      <NFormItem label="Horse Power" path="horsePower">
                        <NInputNumber 
                          v-model:value="formData.horsePower" 
                          :min="1"
                          :step="10"
                          class="full-width"
                          placeholder="e.g., 150"
                        />
                      </NFormItem>
                    </NGridItem>
                    
                    <NGridItem span="2 s:1">
                      <NFormItem label="Condition" path="condition">
                        <NSelect 
                          v-model:value="formData.condition" 
                          :options="enumStore.conditionOptions"
                          value-field="value"
                          label-field="label"
                          placeholder="Select condition"
                        />
                      </NFormItem>
                    </NGridItem>
                  </NGrid>
                </div>
              </NGridItem>
            </NGrid>
          </NForm>

          <template #action>
            <NSpace justify="end">
              <NButton @click="handleCancel">Cancel</NButton>
              <NButton 
                type="primary" 
                @click="handleSubmit"
                :loading="saving"
              >
                Update Car
              </NButton>
            </NSpace>
          </template>
        </NCard>
      </div>
    </NSpin>
  </div>
</template>

<style scoped>
.edit-car-container {
  max-width: 900px;
  margin: 0 auto;
  padding: 24px;
}

.form-container {
  margin-top: 24px;
}

.car-form {
  padding: 8px 0;
}

.form-section {
  padding: 20px 0;
  border-bottom: 1px solid #f0f0f0;
}

.form-section:last-child {
  border-bottom: none;
}

.section-title {
  margin: 0 0 20px 0;
  font-size: 1.1rem;
  font-weight: 600;
  color: #2c3e50;
  border-left: 4px solid #667eea;
  padding-left: 12px;
}

.full-width {
  width: 100%;
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .edit-car-container {
    padding: 16px;
  }
  
  .section-title {
    font-size: 1rem;
  }
}
</style>