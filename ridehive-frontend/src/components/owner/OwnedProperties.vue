<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import { NCard, NButton, NEmpty, NGrid, NGridItem, NTag, NModal, useMessage, NSpin, NCarousel, NIcon } from 'naive-ui';
import { AddOutline, CreateOutline, TrashOutline, CalendarOutline, SpeedometerOutline, CarOutline } from '@vicons/ionicons5';
import { carsApi, ApiError } from '@/api';
import { useAuthStore } from '@/api/Auth';
import type { CarResponseDto, CarImageData } from '@/api';

const router = useRouter();
const message = useMessage();
const authStore = useAuthStore();

// State
const cars = ref<CarResponseDto[]>([]);
const loading = ref(false);
const showDeleteModal = ref(false);
const carToDelete = ref<CarResponseDto | null>(null);

// Get current user ID from auth store
const currentUserId = computed(() => authStore.user?.userId || '');

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
    await carsApi.deleteCar(carToDelete.value.carId);
    message.success('Car deleted successfully');
    await fetchUserCars(); // Refresh the list
  } catch (error) {
    console.error('Failed to delete car:', error);
    if (error instanceof ApiError) {
      message.error(`Failed to delete car: ${error.message}`);
    } else {
      message.error('Failed to delete car. Please try again.');
    }
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

// Fetch user's cars
const fetchUserCars = async () => {
  if (!currentUserId.value) {
    message.warning('Please login to view your cars');
    router.push('/login');
    return;
  }

  try {
    loading.value = true;
    cars.value = await carsApi.getCarsByOwner(currentUserId.value);
    console.log('Loaded cars from API:', cars.value);
    
    // Debug: Check if cars have images
    cars.value.forEach((car, index) => {
      console.log(`Car ${index + 1} (${car.brand} ${car.model}):`, {
        carId: car.carId,
        hasImages: car.carImages && car.carImages.length > 0,
        imageCount: car.carImages?.length || 0,
        imageTypes: car.carImages?.map(img => img.imageContentType) || [],
        firstImagePath: car.carImages?.[0]?.imagePath || 'N/A'
      });
    });
  } catch (error) {
    console.error('Failed to fetch cars:', error);
    if (error instanceof ApiError) {
      message.error(`Failed to load your cars: ${error.message}`);
    } else {
      message.error('Failed to load your cars. Please make sure the API server is running.');
    }
    // Optionally load mock data as fallback
    cars.value = [];
  } finally {
    loading.value = false;
  }
};

// Helper function to get image URLs from car images
const getCarImageUrls = (carImages: CarImageData[]): string[] => {
  return carsApi.getCarImageUrls(carImages);
};

// Load cars on component mount
onMounted(() => {
  fetchUserCars();
});
</script>

<template>
  <div class="owned-properties-container">
    <!-- Custom Page Header -->
    <div class="page-header">
      <div class="header-content">
        <div class="title-section">
          <h2 class="page-title">My Cars</h2>
          <p class="page-subtitle">Manage your registered vehicles and properties</p>
        </div>
        <div class="header-actions">
          <NButton 
            v-if="cars.length > 0"
            type="primary" 
            @click="addCar"
            size="large"
          >
            <template #icon>
              <NIcon><AddOutline /></NIcon>
            </template>
            Add Car
          </NButton>
        </div>
      </div>
    </div>

    <NSpin :show="loading">
      <!-- Empty State -->
      <div v-if="cars.length === 0 && !loading" class="empty-state">
        <NEmpty 
          description="You haven't added any cars yet"
          class="custom-empty"
        >
          <template #extra>
            <NButton type="primary" size="large" @click="addCar">
              <template #icon>
                <NIcon><AddOutline /></NIcon>
              </template>
              Add Your First Car
            </NButton>
          </template>
        </NEmpty>
      </div>

      <!-- Cars Grid -->
      <div v-if="cars.length > 0" class="cars-grid">
        <NGrid :cols="1" :x-gap="24" :y-gap="24" responsive="screen">
          <NGridItem 
            v-for="car in cars" 
            :key="car.carId"
            span="1 s:1 m:1 l:1 xl:1 2xl:1"
          >
            <NCard class="car-card" hoverable>
              <div class="car-content">
                <!-- Car Images Carousel -->
                <div class="car-images">
                  <div v-if="car.carImages && car.carImages.length > 0" class="carousel-container">
                    <NCarousel 
                      v-if="car.carImages.length > 1"
                      show-dots
                      show-arrow
                      :style="{ height: '100%', width: '100%' }"
                      class="car-carousel"
                    >
                      <img 
                        v-for="(imageUrl, index) in getCarImageUrls(car.carImages)"
                        :key="index"
                        :src="imageUrl" 
                        :alt="`${car.brand} ${car.model} - Image ${index + 1}`"
                        class="car-image"
                        @error="($event.target as HTMLImageElement).src = '/api/placeholder/300/200'"
                      />
                    </NCarousel>
                    <img 
                      v-else
                      :src="getCarImageUrls(car.carImages)[0]" 
                      :alt="`${car.brand} ${car.model}`"
                      class="car-image single-image"
                      @error="($event.target as HTMLImageElement).src = '/api/placeholder/300/200'"
                    />
                  </div>
                  <div v-else class="no-image-placeholder">
                    <NIcon size="48" color="#d0d7de">
                      <CarOutline />
                    </NIcon>
                    <span>No images</span>
                  </div>
                </div>

                <!-- Car Details -->
                <div class="car-details">
                  <div class="car-header">
                    <h3 class="car-title">{{ car.brand }} {{ car.model }}</h3>
                    <div class="car-actions">
                      <NButton 
                        size="small" 
                        @click="editCar(car)"
                        type="primary"
                        quaternary
                      >
                        <template #icon>
                          <NIcon><CreateOutline /></NIcon>
                        </template>
                        Edit
                      </NButton>
                      <NButton 
                        size="small" 
                        @click="showDeleteConfirmation(car)"
                        type="error"
                        quaternary
                      >
                        <template #icon>
                          <NIcon><TrashOutline /></NIcon>
                        </template>
                        Delete
                      </NButton>
                    </div>
                  </div>

                  <div class="car-version" v-if="car.version">
                    <span class="version-label">Version:</span> {{ car.version }}
                  </div>

                  <!-- Car Specifications -->
                  <div class="car-specs">
                    <div class="spec-grid">
                      <div class="spec-item">
                        <NIcon size="16"><CalendarOutline /></NIcon>
                        <span>{{ car.yearProduction }}</span>
                      </div>
                      <div class="spec-item">
                        <NIcon size="16"><SpeedometerOutline /></NIcon>
                        <span>{{ car.course.toLocaleString() }} km</span>
                      </div>
                      <div class="spec-item">
                        <span class="spec-label">Fuel:</span>
                        <span>{{ car.fuelDisplay }}</span>
                      </div>
                      <div class="spec-item">
                        <span class="spec-label">Transmission:</span>
                        <span>{{ car.transmissionDisplay }}</span>
                      </div>
                      <div class="spec-item">
                        <span class="spec-label">Engine:</span>
                        <span>{{ car.displacement }}cm³ - {{ car.horsePower }}HP</span>
                      </div>
                      <div class="spec-item" v-if="car.consumption">
                        <span class="spec-label">Consumption:</span>
                        <span>{{ car.consumption }}L/100km</span>
                      </div>
                    </div>
                  </div>

                  <!-- Additional Info Tags -->
                  <div class="car-tags">
                    <NTag size="small" type="info">{{ car.color }}</NTag>
                    <NTag size="small" type="default">{{ car.bodyDisplay }}</NTag>
                    <NTag size="small" type="success">{{ car.conditionDisplay }}</NTag>
                    <NTag size="small" type="warning">{{ car.numberDoors }} doors</NTag>
                    <NTag size="small" type="default">{{ car.numberSeats }} seats</NTag>
                  </div>
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
      :content="`Are you sure you want to delete ${carToDelete?.brand} ${carToDelete?.model}? This action cannot be undone.`"
      positive-text="Delete"
      negative-text="Cancel"
      @positive-click="deleteCar"
      @negative-click="cancelDelete"
    />
  </div>
</template>

<style scoped>
.owned-properties-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 24px;
}

/* Custom Page Header Styles */
.page-header {
  margin-bottom: 24px;
  padding-bottom: 16px;
  border-bottom: 1px solid #e2e8f0;
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 24px;
}

.title-section {
  flex: 1;
}

.page-title {
  font-size: 2rem;
  font-weight: 700;
  margin: 0 0 8px 0;
  color: #1a202c;
}

.page-subtitle {
  font-size: 1rem;
  color: #64748b;
  margin: 0;
  line-height: 1.5;
}

.header-actions {
  flex-shrink: 0;
}

.page-title {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 1.5rem;
  font-weight: 600;
}

.title-icon {
  color: #667eea;
}

.empty-state {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 400px;
  padding: 40px;
}

.custom-empty {
  text-align: center;
}

.cars-grid {
  margin-top: 24px;
}

.car-card {
  border-radius: 12px;
  border: 1px solid #e2e8f0;
  transition: all 0.3s ease;
  overflow: hidden;
}

.car-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
}

.car-content {
  display: flex;
  gap: 24px;
}

.car-images {
  flex: 0 0 350px;
  height: 300px;
  position: relative;
  border-radius: 8px;
  overflow: hidden;
}

.carousel-container {
  width: 100%;
  height: 100%;
  position: relative;
}

.car-carousel {
  border-radius: 8px;
  overflow: hidden;
}

.car-image {
  width: 100%;
  height: 300px;
  object-fit: cover;
  display: block;
}

.single-image {
  width: 100%;
  height: 300px;
  object-fit: cover;
  border-radius: 8px;
}

.no-image-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
  background-color: #f8fafc;
  border: 2px dashed #d0d7de;
  border-radius: 8px;
  color: #64748b;
}

.car-details {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.car-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.car-title {
  font-size: 1.5rem;
  font-weight: 700;
  margin: 0;
  color: #1a202c;
}

.car-actions {
  display: flex;
  gap: 8px;
}

.car-version {
  font-size: 1rem;
  color: #64748b;
}

.version-label {
  font-weight: 600;
}

.car-specs {
  flex: 1;
}

.spec-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 12px;
}

.spec-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  background-color: #f8fafc;
  border-radius: 6px;
  font-size: 0.875rem;
}

.spec-label {
  font-weight: 600;
  color: #4a5568;
}

.car-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  margin-top: auto;
}

/* Responsive Design */
@media (max-width: 1024px) {
  .spec-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .owned-properties-container {
    padding: 16px;
  }

  .header-content {
    flex-direction: column;
    align-items: stretch;
    gap: 16px;
  }

  .title-section {
    text-align: center;
  }

  .header-actions {
    align-self: center;
  }

  .car-content {
    flex-direction: column;
    gap: 16px;
  }

  .car-images {
    flex: none;
    height: 250px;
  }

  .spec-grid {
    grid-template-columns: 1fr;
  }

  .car-header {
    flex-direction: column;
    gap: 12px;
  }

  .car-actions {
    align-self: flex-end;
  }
}

@media (max-width: 480px) {
  .car-images {
    height: 200px;
  }

  .car-title {
    font-size: 1.25rem;
  }
}
</style>