<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
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
  NDatePicker,
  NTag,
  NEmpty,
  useMessage,
  type FormInst,
  type FormRules,
  type SelectOption
} from 'naive-ui';
import { carsApi, postsApi, ApiError } from '@/api';
import type { PostCreateDto, CarResponseDto } from '@/api/types';
import { formatDateForAPI } from '@/utils/dateUtils';

const router = useRouter();
const message = useMessage();

// Form reference
const formRef = ref<FormInst | null>(null);

// Loading states
const loading = ref(false);
const saving = ref(false);
const loadingCars = ref(false);

// TODO: Replace with actual authenticated user ID from auth service
const currentUserId = "1";

// User's cars for selection
const userCars = ref<CarResponseDto[]>([]);

// Selected time slots
const selectedTimeSlots = ref<number[]>([]);

// Form data
const formData = ref<PostCreateDto>({
  ownerId: currentUserId,
  carId: 0,
  title: '',
  description: '',
  price: 1,
  specialRequirements: '',
  location: '',
  availableTimeSlots: []
});

// Car options for dropdown
const carOptions = computed((): SelectOption[] => {
  return userCars.value.map(car => ({
    label: `${car.brand} ${car.model} ${car.version || ''} (${car.yearProduction})`,
    value: car.carId,
    disabled: false
  }));
});

// Selected car details
const selectedCar = computed(() => {
  return userCars.value.find(car => car.carId === formData.value.carId);
});

// Form validation rules
const rules: FormRules = {
  carId: [
    { type: 'number', required: true, message: 'Please select a car', trigger: 'change' }
  ],
  title: [
    { required: true, message: 'Title is required', trigger: 'blur' },
    { min: 10, max: 150, message: 'Title should be between 10 and 150 characters', trigger: 'blur' }
  ],
  price: [
    { type: 'number', required: true, message: 'Price is required', trigger: 'blur' },
    { type: 'number', min: 1, max: 1000000, message: 'Price should be between 1 and 1,000,000 €/day', trigger: 'blur' }
  ],
  location: [
    { required: true, message: 'Location is required', trigger: 'blur' }
  ],
  description: [
    { max: 500, message: 'Description cannot exceed 500 characters', trigger: 'blur' }
  ],
  specialRequirements: [
    { max: 250, message: 'Special requirements cannot exceed 250 characters', trigger: 'blur' }
  ]
};

// Load user's cars
const loadUserCars = async () => {
  try {
    loadingCars.value = true;
    userCars.value = await carsApi.getCarsByOwner(currentUserId);
  } catch (error) {
    console.error('Failed to load cars:', error);
    if (error instanceof ApiError) {
      message.error(`Failed to load your cars: ${error.message}`);
    } else {
      message.error('Failed to load your cars. Please try again.');
    }
  } finally {
    loadingCars.value = false;
  }
};

// Add time slot
const addTimeSlot = () => {
  const now = Date.now();
  const tomorrow = now + 24 * 60 * 60 * 1000; // Default to tomorrow
  selectedTimeSlots.value.push(tomorrow);
};

// Remove time slot
const removeTimeSlot = (index: number) => {
  selectedTimeSlots.value.splice(index, 1);
};

// Update time slot
const updateTimeSlot = (index: number, value: number | null) => {
  if (value !== null) {
    selectedTimeSlots.value[index] = value;
  }
};

// Submit handler
const handleSubmit = async () => {
  if (!formRef.value) return;

  try {
    // Validate form
    await formRef.value.validate();
    
    // Validate time slots
    if (selectedTimeSlots.value.length === 0) {
      message.error('Please add at least one available time slot');
      return;
    }

    // Validate time slots are in the future
    const now = Date.now();
    const invalidSlots = selectedTimeSlots.value.filter(slot => slot <= now);
    if (invalidSlots.length > 0) {
      message.error('All time slots must be in the future');
      return;
    }

    saving.value = true;
    
    // Prepare post data
    const postData: PostCreateDto = {
      ...formData.value,
      availableTimeSlots: selectedTimeSlots.value.map(slot => formatDateForAPI(new Date(slot)))
    };
    
    // Create the post
    await postsApi.createPost(postData);
    
    message.success('Post created successfully!');
    router.push('/my-posts');
    
  } catch (error) {
    console.error('Create post error:', error);
    
    if (error instanceof ApiError) {
      message.error(`Failed to create post: ${error.message}`);
    } else {
      message.error('An unexpected error occurred. Please try again.');
    }
  } finally {
    saving.value = false;
  }
};

// Cancel handler
const handleCancel = () => {
  router.push('/my-posts');
};

// Navigate to add car
const navigateToAddCar = () => {
  router.push('/add-car');
};

// Load user cars on component mount
onMounted(async () => {
  await loadUserCars();
  
  // Add initial time slot if none exist
  if (selectedTimeSlots.value.length === 0) {
    addTimeSlot();
  }
});
</script>

<template>
  <div class="create-post-container">
    <NPageHeader @back="handleCancel">
      <template #title>Create New Post</template>
      <template #subtitle>List your vehicle for rental</template>
    </NPageHeader>

    <NSpin :show="loading || loadingCars" description="Loading...">
      <div v-if="!loading && !loadingCars" class="form-container">
        <!-- Check if user has cars -->
        <div v-if="userCars.length === 0" class="no-cars-state">
          <NEmpty description="You need to add a car before creating a post">
            <template #extra>
              <NButton type="primary" @click="navigateToAddCar">
                Add Your First Car
              </NButton>
            </template>
          </NEmpty>
        </div>

        <!-- Create post form -->
        <NCard v-else title="Post Details">
          <NForm
            ref="formRef"
            :model="formData"
            :rules="rules"
            label-placement="top"
            size="large"
            class="post-form"
          >
            <NGrid :cols="1" :x-gap="16" :y-gap="16">
              <!-- Car Selection -->
              <NGridItem>
                <div class="form-section">
                  <h3 class="section-title">Select Vehicle</h3>
                  <NFormItem label="Your Car" path="carId">
                    <NSelect 
                      v-model:value="formData.carId" 
                      :options="carOptions"
                      placeholder="Choose a car to rent out"
                      filterable
                    />
                  </NFormItem>
                  
                  <!-- Selected car preview -->
                  <div v-if="selectedCar" class="car-preview">
                    <div class="car-info">
                      <h4>{{ selectedCar.brand }} {{ selectedCar.model }}</h4>
                      <div class="car-details">
                        <NTag>{{ selectedCar.yearProduction }}</NTag>
                        <NTag>{{ selectedCar.fuelDisplay }}</NTag>
                        <NTag>{{ selectedCar.transmissionDisplay }}</NTag>
                        <NTag>{{ selectedCar.numberDoors }} doors</NTag>
                        <NTag>{{ selectedCar.numberSeats }} seats</NTag>
                      </div>
                    </div>
                  </div>
                </div>
              </NGridItem>

              <!-- Basic Information -->
              <NGridItem>
                <div class="form-section">
                  <h3 class="section-title">Post Information</h3>
                  <NGrid :cols="2" :x-gap="16" :y-gap="16" responsive="screen">
                    <NGridItem span="2">
                      <NFormItem label="Title" path="title">
                        <NInput 
                          v-model:value="formData.title" 
                          placeholder="e.g., Comfortable BMW X5 for weekend trips"
                          show-count
                          maxlength="150"
                        />
                      </NFormItem>
                    </NGridItem>
                    
                    <NGridItem span="2 s:1">
                      <NFormItem label="Price per Day (€)" path="price">
                        <NInputNumber 
                          v-model:value="formData.price" 
                          :min="1"
                          :max="1000000"
                          :step="5"
                          class="full-width"
                          placeholder="e.g., 50"
                        />
                      </NFormItem>
                    </NGridItem>
                    
                    <NGridItem span="2 s:1">
                      <NFormItem label="Location" path="location">
                        <NInput 
                          v-model:value="formData.location" 
                          placeholder="e.g., Downtown Munich, Germany"
                        />
                      </NFormItem>
                    </NGridItem>
                  </NGrid>
                </div>
              </NGridItem>

              <!-- Description and Requirements -->
              <NGridItem>
                <div class="form-section">
                  <h3 class="section-title">Additional Details</h3>
                  <NGrid :cols="1" :x-gap="16" :y-gap="16">
                    <NGridItem>
                      <NFormItem label="Description (Optional)" path="description">
                        <NInput 
                          v-model:value="formData.description" 
                          type="textarea"
                          placeholder="Describe your car, its features, and any important information for renters..."
                          :rows="4"
                          show-count
                          maxlength="500"
                        />
                      </NFormItem>
                    </NGridItem>
                    
                    <NGridItem>
                      <NFormItem label="Special Requirements (Optional)" path="specialRequirements">
                        <NInput 
                          v-model:value="formData.specialRequirements" 
                          placeholder="e.g., Valid driver's license for 2+ years, age 25+, etc."
                          show-count
                          maxlength="250"
                        />
                      </NFormItem>
                    </NGridItem>
                  </NGrid>
                </div>
              </NGridItem>

              <!-- Time Slots -->
              <NGridItem>
                <div class="form-section">
                  <h3 class="section-title">Available Time Slots</h3>
                  <div class="time-slots-container">
                    <div v-for="(_, index) in selectedTimeSlots" :key="index" class="time-slot-item">
                      <NDatePicker
                        v-model:value="selectedTimeSlots[index]"
                        type="datetime"
                        :is-date-disabled="(ts: number) => ts < Date.now() - 24 * 60 * 60 * 1000"
                        placeholder="Select date and time"
                        class="time-picker"
                        @update:value="(value) => updateTimeSlot(index, value)"
                      />
                      <NButton 
                        type="error" 
                        ghost 
                        @click="removeTimeSlot(index)"
                        :disabled="selectedTimeSlots.length === 1"
                      >
                        Remove
                      </NButton>
                    </div>
                    
                    <NButton type="primary" dashed @click="addTimeSlot">
                      Add Time Slot
                    </NButton>
                  </div>
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
                :disabled="userCars.length === 0"
              >
                Create Post
              </NButton>
            </NSpace>
          </template>
        </NCard>
      </div>
    </NSpin>
  </div>
</template>

<style scoped>
.create-post-container {
  max-width: 900px;
  margin: 0 auto;
  padding: 24px;
}

.form-container {
  margin-top: 24px;
}

.no-cars-state {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 400px;
}

.post-form {
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

.car-preview {
  margin-top: 16px;
  padding: 16px;
  background-color: #f8fafc;
  border-radius: 8px;
  border: 1px solid #e2e8f0;
}

.car-info h4 {
  margin: 0 0 12px 0;
  font-size: 1.1rem;
  font-weight: 600;
  color: #1f2937;
}

.car-details {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.time-slots-container {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.time-slot-item {
  display: flex;
  gap: 12px;
  align-items: center;
}

.time-picker {
  flex: 1;
  min-width: 200px;
}

.full-width {
  width: 100%;
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .create-post-container {
    padding: 16px;
  }
  
  .section-title {
    font-size: 1rem;
  }
  
  .time-slot-item {
    flex-direction: column;
    align-items: stretch;
  }
  
  .time-picker {
    min-width: unset;
  }
}
</style>