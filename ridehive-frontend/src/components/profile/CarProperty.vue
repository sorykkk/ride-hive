<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { NCard, NButton, NDivider, NEmpty, NGrid, NGridItem, NTag, NModal, NSpace, useMessage, NSpin } from 'naive-ui';
import { apiService } from '@/services/apiService';
import type { CarResponseDto } from '@/api/client';

const router = useRouter();
const message = useMessage();

// State
const cars = ref<CarResponseDto[]>([]);
const loading = ref(false);
const showDeleteModal = ref(false);
const carToDelete = ref<CarResponseDto | null>(null);

// Mock user ID - in real app this would come from auth store/context
const currentUserId = 1;

// Fetch user's cars
const fetchUserCars = async () => {
  try {
    loading.value = true;
    cars.value = await apiService.getCarsByOwner(currentUserId);
    console.log('Loaded cars from API:', cars.value);
  } catch (error) {
    console.error('Failed to fetch cars:', error);
    message.error('Failed to load your cars. Please make sure the API server is running.');
    // Optionally load mock data as fallback
    cars.value = [];
  } finally {
    loading.value = false;
  }
};

// Navigate to add car page
const addCar = () => {
  router.push('/add-car');
};

// Navigate to edit car page
const editCar = (car: CarResponseDto) => {
  router.push(`/edit-car/${car.carId}`);
};

// Show delete confirmation
const showDeleteConfirmation = (car: CarResponseDto) => {
  carToDelete.value = car;
  showDeleteModal.value = true;
};

// Delete car
const deleteCar = async () => {
  if (!carToDelete.value?.carId) return;
  
  try {
    await apiService.deleteCar(carToDelete.value.carId);
    message.success('Car deleted successfully');
    await fetchUserCars(); // Refresh the list
  } catch (error) {
    console.error('Failed to delete car:', error);
    message.error('Failed to delete car. Please try again.');
  } finally {
    showDeleteModal.value = false;
    carToDelete.value = null;
  }
};

// Cancel delete
const cancelDelete = () => {
  showDeleteModal.value = false;
  carToDelete.value = null;
};

// Load cars on component mount
onMounted(() => {
  fetchUserCars();
});
</script>

<template>
  <div class="car-property-section">
    <NDivider class="section-divider" />
    
    <div class="section-header">
      <h2>My Cars</h2>
      <NButton 
        v-if="cars.length > 0"
        type="primary" 
        @click="addCar"
        class="add-car-btn"
      >
        Add Car
      </NButton>
    </div>

    <NSpin :show="loading">
      <div v-if="cars.length === 0" class="empty-state">
        <NEmpty 
          description="You haven't added any cars yet"
          class="custom-empty"
        >
          <template #extra>
            <NButton type="primary" @click="addCar">
              Add Your First Car
            </NButton>
          </template>
        </NEmpty>
      </div>

      <div v-else class="cars-grid">
        <NGrid :cols="1" :x-gap="16" :y-gap="16" responsive="screen">
          <NGridItem 
            v-for="car in cars" 
            :key="car.carId"
            :span="1"
          >
            <NCard class="car-card" hoverable>
              <div class="car-content">
                <div class="car-header">
                  <h3 class="car-title">
                    {{ car.brand }} {{ car.model }}
                    <span v-if="car.version" class="car-version">{{ car.version }}</span>
                  </h3>
                  <div class="car-year">{{ car.yearProduction }}</div>
                </div>

                <div class="car-details">
                  <div class="detail-row">
                    <span class="label">Color:</span>
                    <NTag type="info" size="small">{{ car.color }}</NTag>
                  </div>
                  <div class="detail-row">
                    <span class="label">Fuel:</span>
                    <NTag type="success" size="small">{{ car.fuelDisplay }}</NTag>
                  </div>
                  <div class="detail-row">
                    <span class="label">Transmission:</span>
                    <NTag type="warning" size="small">{{ car.transmissionDisplay }}</NTag>
                  </div>
                  <div class="detail-row">
                    <span class="label">Drive:</span>
                    <NTag type="default" size="small">{{ car.driveDisplay }}</NTag>
                  </div>
                  <div class="detail-row">
                    <span class="label">Condition:</span>
                    <NTag 
                      :type="car.conditionDisplay === 'Excellent' ? 'success' : 
                             car.conditionDisplay === 'Good' ? 'info' :
                             car.conditionDisplay === 'Fair' ? 'warning' : 'error'"
                      size="small"
                    >
                      {{ car.conditionDisplay }}
                    </NTag>
                  </div>
                  <div class="detail-row">
                    <span class="label">Mileage:</span>
                    <span>{{ car.course?.toLocaleString() || 0 }} km</span>
                  </div>
                  <div class="detail-row">
                    <span class="label">Power:</span>
                    <span>{{ car.horsePower || 0 }} HP</span>
                  </div>
                </div>

                <!-- <div v-if="car.description" class="car-description">
                  <p>{{ car.description }}</p>
                </div> -->

                <div class="car-actions">
                  <NSpace>
                    <NButton 
                      type="info"
                      size="small"
                      @click="editCar(car)"
                    >
                      Edit
                    </NButton>
                    <NButton 
                      type="error"
                      size="small"
                      @click="showDeleteConfirmation(car)"
                    >
                      Delete
                    </NButton>
                  </NSpace>
                </div>
              </div>
            </NCard>
          </NGridItem>
        </NGrid>
      </div>
    </NSpin>

    <!-- Delete Confirmation Modal -->
    <NModal 
      v-model:show="showDeleteModal"
      preset="dialog"
      title="Delete Car"
      content="Are you sure you want to delete this car? This action cannot be undone."
      positive-text="Delete"
      negative-text="Cancel"
      @positive-click="deleteCar"
      @negative-click="cancelDelete"
    />
  </div>
</template>

<style scoped>
.car-property-section {
  margin-top: 2rem;
}

.section-divider {
  margin: 2rem 0;
  border-color: #e0e0e6;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.section-header h2 {
  margin: 0;
  color: #2c3e50;
  font-size: 1.5rem;
  font-weight: 600;
}

.add-car-btn {
  background: linear-gradient(135deg, #4285f4 0%, #1976d2 100%);
  border: none;
  box-shadow: 0 2px 8px rgba(66, 133, 244, 0.3);
  transition: all 0.3s ease;
}

.add-car-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(66, 133, 244, 0.4);
}

.empty-state {
  text-align: center;
  padding: 3rem 1rem;
}

.custom-empty {
  padding: 2rem;
}

.cars-grid {
  margin-top: 1rem;
}

.car-card {
  border-radius: 12px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
  border: 1px solid #f0f0f0;
}

.car-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12);
}

.car-content {
  padding: 0.5rem;
}

.car-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 1rem;
}

.car-title {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
  color: #2c3e50;
  line-height: 1.4;
}

.car-version {
  color: #7c3aed;
  font-weight: 500;
}

.car-year {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.875rem;
  font-weight: 500;
}

.car-details {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 0.75rem;
  margin-bottom: 1rem;
}

.detail-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.5rem 0;
  border-bottom: 1px solid #f5f5f5;
}

.detail-row:last-child {
  border-bottom: none;
}

.label {
  font-weight: 500;
  color: #64748b;
  font-size: 0.875rem;
}

.car-description {
  margin: 1rem 0;
  padding: 1rem;
  background: #f8fafc;
  border-radius: 8px;
  border-left: 4px solid #4285f4;
}

.car-description p {
  margin: 0;
  color: #475569;
  font-style: italic;
  line-height: 1.5;
}

.car-actions {
  margin-top: 1.5rem;
  padding-top: 1rem;
  border-top: 1px solid #f0f0f0;
  display: flex;
  justify-content: flex-end;
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .section-header {
    flex-direction: column;
    align-items: stretch;
    gap: 1rem;
  }
  
  .car-details {
    grid-template-columns: 1fr;
  }
  
  .detail-row {
    justify-content: space-between;
  }
  
  .car-actions {
    justify-content: center;
  }
}
</style>