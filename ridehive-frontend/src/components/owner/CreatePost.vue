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
import { useAuthStore } from '@/api/Auth';
import type { PostCreateDto, CarResponseDto } from '@/api/types';

// Form interface allowing null carId initially
interface PostFormData extends Omit<PostCreateDto, 'carId'> {
  carId: number | null;
}

const router = useRouter();
const message = useMessage();
const authStore = useAuthStore();

// Form reference
const formRef = ref<FormInst | null>(null);

// Loading states
const loading = ref(false);
const saving = ref(false);
const loadingCars = ref(false);

// User's cars for selection
const userCars = ref<CarResponseDto[]>([]);

// Time slot ranges (start-end pairs with dates only, no hours)
interface TimeSlotRange {
  id: number;
  startDate: number | null;
  endDate: number | null;
}

const timeSlotRanges = ref<TimeSlotRange[]>([]);
const nextRangeId = ref(1);

// Form data
const formData = ref<PostFormData>({
  ownerId: authStore.user?.userId || '',
  carId: null,
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
    { 
      required: true, 
      message: 'Please select a car or register one in your owned properties', 
      trigger: 'change',
      validator: (_rule: any, value: any) => {
        return value != null && value !== 0;
      }
    }
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
    if (!authStore.user?.userId) {
      throw new Error('User not authenticated');
    }
    userCars.value = await carsApi.getCarsByOwner(authStore.user.userId);
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

// Add time slot range
const addTimeSlotRange = () => {
  timeSlotRanges.value.push({
    id: nextRangeId.value++,
    startDate: null,
    endDate: null
  });
};

// Remove time slot range
const removeTimeSlotRange = (id: number) => {
  const index = timeSlotRanges.value.findIndex(range => range.id === id);
  if (index > -1) {
    timeSlotRanges.value.splice(index, 1);
  }
};

// Generate all dates between start and end (inclusive, day by day without hours)
const generateDateRange = (startDate: number, endDate: number): string[] => {
  const dates: string[] = [];
  const start = new Date(startDate);
  const end = new Date(endDate);
  
  // Set time to midnight (00:00:00) for date-only comparison
  start.setHours(0, 0, 0, 0);
  end.setHours(0, 0, 0, 0);
  
  // Ensure start is not after end
  if (start > end) {
    return [];
  }
  
  // Generate dates day by day
  const currentDate = new Date(start);
  
  while (currentDate <= end) {
    // Format as ISO date string (YYYY-MM-DD) at midnight UTC
    const isoDate = currentDate.toISOString();
    dates.push(isoDate);
    // Add 1 day for the next slot
    currentDate.setDate(currentDate.getDate() + 1);
  }
  
  return dates;
};

// Get all dates from all ranges
const getAllTimeSlots = (): string[] => {
  const allDates: string[] = [];
  
  for (const range of timeSlotRanges.value) {
    if (range.startDate && range.endDate) {
      const rangeDates = generateDateRange(range.startDate, range.endDate);
      allDates.push(...rangeDates);
    }
  }
  
  // Remove duplicates and sort
  return [...new Set(allDates)].sort();
};

// Submit handler
const handleSubmit = async () => {
  if (!formRef.value) return;

  try {
    // Validate form
    await formRef.value.validate();
    
    // Validate time slot ranges
    if (timeSlotRanges.value.length === 0) {
      message.error('Please add at least one available time slot range');
      return;
    }

    // Validate all ranges have both start and end dates
    const incompleteRanges = timeSlotRanges.value.filter(range => !range.startDate || !range.endDate);
    if (incompleteRanges.length > 0) {
      message.error('Please complete all time slot ranges with both start and end dates');
      return;
    }

    // Validate ranges are in the future
    const now = Date.now();
    const invalidRanges = timeSlotRanges.value.filter(range => 
      range.startDate! <= now || range.endDate! <= now
    );
    if (invalidRanges.length > 0) {
      message.error('All time slots must be in the future');
      return;
    }

    // Validate start dates are before or equal to end dates
    const invalidOrderRanges = timeSlotRanges.value.filter(range => 
      range.startDate! > range.endDate!
    );
    if (invalidOrderRanges.length > 0) {
      message.error('Start date must be before end date');
      return;
    }

    saving.value = true;
    
    // Generate all time slots from ranges
    const allTimeSlots = getAllTimeSlots();
    
    // Validate carId before submitting
    if (formData.value.carId === null) {
      message.error('Please select a car');
      return;
    }
    
    // Prepare post data
    const postData: PostCreateDto = {
      ...formData.value,
      carId: formData.value.carId, // TypeScript now knows this is not null
      availableTimeSlots: allTimeSlots
    };
    
    console.log('Sending post data:', postData);
    console.log('Generated time slots:', allTimeSlots);
    
    // Create the post
    const response = await postsApi.createPost(postData);
    console.log('Post creation response:', response);
    
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
  
  // Add initial time slot range if none exist
  if (timeSlotRanges.value.length === 0) {
    addTimeSlotRange();
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
                  <h3 class="section-title">Available Time Slot Ranges</h3>
                  <div class="time-slots-container">
                    <div v-for="range in timeSlotRanges" :key="range.id" class="time-slot-range">
                      <div class="range-header">
                        <span class="range-label">Time Slot Range {{ range.id }}</span>
                        <NButton 
                          type="error" 
                          ghost 
                          size="small"
                          @click="removeTimeSlotRange(range.id)"
                          :disabled="timeSlotRanges.length === 1"
                        >
                          Remove Range
                        </NButton>
                      </div>
                      
                      <div class="date-range-inputs">
                        <div class="datetime-input-group">
                          <label class="date-label">Start Date</label>
                          <NDatePicker
                            v-model:value="range.startDate"
                            type="date"
                            :is-date-disabled="(ts: number) => ts < Date.now() - 24 * 60 * 60 * 1000"
                            placeholder="Select start date"
                            class="datetime-picker"
                            clearable
                          />
                        </div>
                        
                        <div class="datetime-input-group">
                          <label class="date-label">End Date</label>
                          <NDatePicker
                            v-model:value="range.endDate"
                            type="date"
                            :is-date-disabled="(ts: number) => ts < (range.startDate || Date.now() - 24 * 60 * 60 * 1000)"
                            placeholder="Select end date"
                            class="datetime-picker"
                            clearable
                          />
                        </div>
                      </div>
                      
                      <div v-if="range.startDate && range.endDate" class="range-preview">
                        <span class="preview-text">
                          📅 {{ new Date(range.startDate).toLocaleDateString() }} - {{ new Date(range.endDate).toLocaleDateString() }}
                          <br>
                          📆 {{ generateDateRange(range.startDate, range.endDate).length }} days will be available
                        </span>
                      </div>
                    </div>
                    
                    <NButton type="primary" dashed @click="addTimeSlotRange">
                      Add Time Slot Range
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
  gap: 20px;
}

.time-slot-range {
  padding: 20px;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  background-color: #f8fafc;
}

.range-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.range-label {
  font-weight: 600;
  color: #374151;
  font-size: 1rem;
}

.date-range-inputs {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
  margin-bottom: 12px;
}

.datetime-input-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.date-input-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.date-label {
  font-size: 0.875rem;
  font-weight: 500;
  color: #4b5563;
}

.date-picker {
  width: 100%;
}

.datetime-picker {
  width: 100%;
}

.range-preview {
  margin-top: 12px;
  padding: 8px 12px;
  background-color: #e0f2fe;
  border: 1px solid #0284c7;
  border-radius: 6px;
}

.preview-text {
  font-size: 0.875rem;
  color: #0c4a6e;
  font-weight: 500;
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
  
  .date-range-inputs {
    grid-template-columns: 1fr;
  }
  
  .range-header {
    flex-direction: column;
    gap: 8px;
    align-items: stretch;
  }
  
  .date-picker {
    min-width: unset;
  }
  
  .datetime-picker {
    min-width: unset;
  }
}
</style>