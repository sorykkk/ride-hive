<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { 
  NCard, 
  NButton, 
  NSpace,
  NGrid,
  NGridItem,
  NTag,
  NSpin,
  NPageHeader,
  NText,
  NDivider,
  NImage,
  NImageGroup,
  NEmpty,
  useMessage
} from 'naive-ui';
import { postsApi, carsApi, ApiError } from '@/api';
import { useAuthStore } from '@/api/Auth';
import type { PostResponseDto, CarResponseDto } from '@/api/types';
import { formatDateForDisplay, getAvailableDates, groupTimeSlots } from '@/utils/dateUtils';

const router = useRouter();
const route = useRoute();
const message = useMessage();
const authStore = useAuthStore();

// State
const post = ref<PostResponseDto | null>(null);
const car = ref<CarResponseDto | null>(null);
const loading = ref(false);
const loadingCar = ref(false);

// Post ID from route parameter
const postId = computed(() => {
  const id = route.params.id;
  return typeof id === 'string' ? parseInt(id) : 0;
});

// Check if current user is the owner
const isOwner = computed(() => {
  return post.value?.ownerId === authStore.user?.userId;
});

// Get available time slots grouped into ranges
const availableSlots = computed(() => {
  if (!post.value) return [];
  const availableDates = getAvailableDates(post.value.availableTimeSlots);
  return groupTimeSlots(availableDates);
});

// Get car image URLs
const carImageUrls = computed(() => {
  if (!car.value?.carImages) return [];
  return carsApi.getCarImageUrls(car.value.carImages);
});

// Load post data
const loadPost = async () => {
  try {
    loading.value = true;
    post.value = await postsApi.getPostById(postId.value);
    
    // Load car details
    if (post.value.carId) {
      await loadCarDetails(post.value.carId);
    }
  } catch (error) {
    console.error('Failed to load post:', error);
    if (error instanceof ApiError) {
      message.error(`Failed to load post: ${error.message}`);
    } else {
      message.error('Failed to load post. Please try again.');
    }
    router.push('/');
  } finally {
    loading.value = false;
  }
};

// Load car details
const loadCarDetails = async (carId: number) => {
  try {
    loadingCar.value = true;
    car.value = await carsApi.getCarById(carId);
  } catch (error) {
    console.error('Failed to load car details:', error);
    // Don't show error for car loading, just log it
  } finally {
    loadingCar.value = false;
  }
};

// Navigate back
const goBack = () => {
  router.back();
};

// Navigate to edit post (if owner)
// const editPost = () => {
//   if (post.value && isOwner.value) {
//     router.push(`/edit-post/${post.value.postId}`);
//   }
// };

// Contact owner
const contactOwner = () => {
  message.info('Contact functionality will be implemented with user authentication');
};

// Load post on component mount
onMounted(() => {
  loadPost();
});
</script>

<template>
  <div class="post-details-container">
    <NPageHeader @back="goBack">
      <template #title>Post Details</template>
      <template #subtitle v-if="post">{{ post.title }}</template>
      <template #extra v-if="post && isOwner">
        <!-- <NButton type="primary" ghost @click="editPost">
          Edit Post
        </NButton> -->
      </template>
    </NPageHeader>

    <NSpin :show="loading" description="Loading post details...">
      <div v-if="!loading && post" class="content-container">
        <NGrid :cols="1" :x-gap="24" :y-gap="24" responsive="screen">
          <!-- Main Post Information -->
          <NGridItem>
            <NCard>
              <template #header>
                <div class="post-header">
                  <div class="title-section">
                    <h1 class="post-title">{{ post.title }}</h1>
                    <div class="tags-container">
                      <NTag 
                        :type="post.available ? 'success' : 'warning'"
                        size="medium"
                      >
                        {{ post.available ? 'Available' : 'Unavailable' }}
                      </NTag>
                    </div>
                  </div>
                  <div class="price-section">
                    <!-- Price as clickable button for clients -->
                    <NButton
                      v-if="!isOwner && post.available && authStore.userRole === 'Client'"
                      type="success"
                      size="large"
                      strong
                      class="price-book-button"
                      @click="router.push(`/booking/${post.postId}`)"
                    >
                      <span class="price-icon">€</span>
                      <span class="price-amount">{{ post.price }}</span>
                      <span class="price-unit">/day</span>
                    </NButton>

                    <!-- Regular price display for owners or unavailable posts -->
                    <div v-else class="price-display">
                      <span class="price-icon">€</span>
                      <span class="price-amount">{{ post.price }}</span>
                      <span class="price-unit">/day</span>
                    </div>
                  </div>
                </div>
              </template>

              <div class="post-content">
                <!-- Location and Posted Date -->
                <div class="info-grid">
                  <div class="info-item">
                    <span class="info-icon">📍</span>
                    <div class="info-content">
                      <div class="info-label">Location</div>
                      <div class="info-value">{{ post.location }}</div>
                    </div>
                  </div>

                  <div class="info-item">
                    <span class="info-icon">🕒</span>
                    <div class="info-content">
                      <div class="info-label">Posted</div>
                      <div class="info-value">{{ formatDateForDisplay(post.postedAt) }}</div>
                    </div>
                  </div>
                </div>

                <!-- Description -->
                <div v-if="post.description" class="description-section">
                  <h3 class="section-title">Description</h3>
                  <NText class="description-text">{{ post.description }}</NText>
                </div>

                <!-- Special Requirements -->
                <div v-if="post.specialRequirements" class="requirements-section">
                  <h3 class="section-title">Special Requirements</h3>
                  <div class="requirements-content">
                    <NText>{{ post.specialRequirements }}</NText>
                  </div>
                </div>
              </div>
            </NCard>
          </NGridItem>

          <!-- Car Details -->
          <NGridItem>
            <NCard title="Vehicle Information">
              <NSpin :show="loadingCar" description="Loading car details...">
                <div v-if="!loadingCar && car" class="car-details">
                  <div class="car-header">
                    <h2 class="car-title">{{ car.brand }} {{ car.model }}</h2>
                    <div class="car-tags">
                      <NTag>{{ car.yearProduction }}</NTag>
                      <NTag>{{ car.fuelDisplay }}</NTag>
                      <NTag>{{ car.transmissionDisplay }}</NTag>
                      <NTag>{{ car.driveDisplay }}</NTag>
                      <NTag>{{ car.bodyDisplay }}</NTag>
                    </div>
                  </div>

                  <NDivider />

                  <div class="car-specs">
                    <NGrid :cols="3" :x-gap="16" :y-gap="12" responsive="screen">
                      <NGridItem span="3 s:1">
                        <div class="spec-item">
                          <span class="spec-label">Color:</span>
                          <span class="spec-value">{{ car.color }}</span>
                        </div>
                      </NGridItem>
                      
                      <NGridItem span="3 s:1">
                        <div class="spec-item">
                          <span class="spec-label">Version:</span>
                          <span class="spec-value">{{ car.version || 'Standard' }}</span>
                        </div>
                      </NGridItem>

                      <NGridItem span="3 s:1">
                        <div class="spec-item">
                          <span class="spec-label">Doors:</span>
                          <span class="spec-value">{{ car.numberDoors }}</span>
                        </div>
                      </NGridItem>

                      <NGridItem span="3 s:1">
                        <div class="spec-item">
                          <span class="spec-label">Seats:</span>
                          <span class="spec-value">{{ car.numberSeats }}</span>
                        </div>
                      </NGridItem>

                      <NGridItem span="3 s:1">
                        <div class="spec-item">
                          <span class="spec-label">Mileage:</span>
                          <span class="spec-value">{{ car.course.toLocaleString() }} km</span>
                        </div>
                      </NGridItem>

                      <NGridItem span="3 s:1">
                        <div class="spec-item">
                          <span class="spec-label">Engine Size:</span>
                          <span class="spec-value">{{ car.displacement }} cm³</span>
                        </div>
                      </NGridItem>

                      <NGridItem span="3 s:1">
                        <div class="spec-item">
                          <span class="spec-label">Power:</span>
                          <span class="spec-value">{{ car.horsePower }} HP</span>
                        </div>
                      </NGridItem>

                      <NGridItem span="3 s:1">
                        <div class="spec-item">
                          <span class="spec-label">Consumption:</span>
                          <span class="spec-value">{{ car.consumption || 'N/A' }} L/100km</span>
                        </div>
                      </NGridItem>

                      <NGridItem span="3 s:1">
                        <div class="spec-item">
                          <span class="spec-label">Condition:</span>
                          <span class="spec-value">{{ car.conditionDisplay }}</span>
                        </div>
                      </NGridItem>
                    </NGrid>
                  </div>

                  <!-- Car Images -->
                  <div v-if="carImageUrls.length > 0" class="car-images">
                    <h3 class="section-title">Vehicle Photos</h3>
                    <NImageGroup>
                      <NSpace>
                        <NImage
                          v-for="(imageUrl, index) in carImageUrls"
                          :key="index"
                          :src="imageUrl"
                          width="200"
                          height="150"
                          object-fit="cover"
                          class="car-image"
                          :alt="`Car image ${index + 1}`"
                        />
                      </NSpace>
                    </NImageGroup>
                  </div>
                </div>

                <NEmpty v-else-if="!loadingCar" description="Car details not available" />
              </NSpin>
            </NCard>
          </NGridItem>

          <!-- Available Time Slots (Only visible for owner) -->
          <NGridItem v-if="isOwner">
            <NCard>
              <template #header>
                <div class="slots-header">
                  <span class="slots-icon">📅</span>
                  <span>Available Dates</span>
                </div>
              </template>

              <div v-if="availableSlots.length > 0" class="time-slots">
                <NGrid :cols="1" :x-gap="12" :y-gap="8" responsive="screen">
                  <NGridItem 
                    v-for="(timeRange, index) in availableSlots" 
                    :key="index"
                  >
                    <div class="time-slot">
                      <span class="slot-icon">✓</span>
                      <span class="slot-time">{{ timeRange.display }}</span>
                    </div>
                  </NGridItem>
                </NGrid>
              </div>

              <NEmpty v-else description="No available dates" />
            </NCard>
          </NGridItem>

          <!-- Contact Section (for non-owners) -->
           <!-- TODO: to modify this part -->
          <NGridItem v-if="!isOwner">
            <NCard title="Interested in this rental?">
              <div class="contact-section">
                <NText>Contact the owner to make a reservation or ask questions.</NText>
                <NSpace style="margin-top: 16px;">
                  <NButton type="primary" @click="contactOwner">
                    Contact Owner
                  </NButton>
                  <NButton>Save to Favorites</NButton>
                </NSpace>
              </div>
            </NCard>
          </NGridItem>
        </NGrid>
      </div>

      <NEmpty v-else-if="!loading && !post" description="Post not found" />
    </NSpin>
  </div>
</template>

<style scoped>
.post-details-container {
  max-width: 1000px;
  margin: 0 auto;
  padding: 24px;
}

.content-container {
  margin-top: 24px;
}

.post-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 24px;
}

.title-section {
  flex: 1;
}

.post-title {
  margin: 0 0 12px 0;
  font-size: 1.75rem;
  font-weight: 600;
  color: #1f2937;
  line-height: 1.3;
}

.tags-container {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.price-section {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
}

.price-display {
  display: flex;
  align-items: center;
  gap: 4px;
  background: linear-gradient(135deg, #059669, #10b981);
  color: white;
  padding: 12px 20px;
  border-radius: 12px;
  font-weight: 600;
  box-shadow: 0 4px 12px rgba(5, 150, 105, 0.3);
}

.price-icon {
  color: white;
}

.price-amount {
  font-size: 1.5rem;
  font-weight: 700;
}

.price-unit {
  font-size: 1rem;
  opacity: 0.9;
}

.price-book-button {
  cursor: pointer;
  transition: all 0.3s ease;
}

.price-book-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(5, 150, 105, 0.4) !important;
}

.price-book-button .price-icon,
.price-book-button .price-amount,
.price-book-button .price-unit {
  color: white !important;
}

.price-book-button .price-amount {
  font-size: 1.5rem;
  font-weight: 700;
}

.price-book-button .price-unit {
  font-size: 1rem;
  opacity: 0.9;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 16px;
  margin-bottom: 24px;
}

.info-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 16px;
  background: #f8fafc;
  border-radius: 8px;
  border: 1px solid #e2e8f0;
}

.info-icon {
  color: #667eea;
  flex-shrink: 0;
}

.info-content {
  flex: 1;
}

.info-label {
  font-size: 0.875rem;
  color: #6b7280;
  margin-bottom: 2px;
}

.info-value {
  font-weight: 500;
  color: #1f2937;
}

.section-title {
  margin: 0 0 16px 0;
  font-size: 1.1rem;
  font-weight: 600;
  color: #374151;
  border-left: 4px solid #667eea;
  padding-left: 12px;
}

.description-section {
  margin: 24px 0;
}

.description-text {
  font-size: 1rem;
  line-height: 1.6;
  color: #4b5563;
}

.requirements-section {
  margin: 24px 0;
}

.requirements-content {
  padding: 16px;
  background: #fef3c7;
  border: 1px solid #f59e0b;
  border-radius: 8px;
  color: #92400e;
}

.car-details {
  padding: 8px 0;
}

.car-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.car-title {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 600;
  color: #1f2937;
}

.car-tags {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.car-specs {
  margin: 16px 0;
}

.spec-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 0;
}

.spec-label {
  font-weight: 500;
  color: #6b7280;
}

.spec-value {
  font-weight: 600;
  color: #1f2937;
}

.car-images {
  margin-top: 24px;
}

.car-image {
  border-radius: 8px;
  cursor: pointer;
  transition: transform 0.2s ease;
}

.car-image:hover {
  transform: scale(1.05);
}

.slots-header {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 600;
}

.time-slots {
  padding: 8px 0;
}

.time-slot {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 16px;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  transition: all 0.2s ease;
}

.time-slot:hover {
  background: #f1f5f9;
  border-color: #cbd5e1;
}

.slot-icon {
  color: #667eea;
}

.slot-time {
  font-weight: 500;
  color: #1f2937;
}

.contact-section {
  text-align: center;
  padding: 16px;
}

/* Responsive design */
@media (max-width: 768px) {
  .post-details-container {
    padding: 16px;
  }
  
  .post-header {
    flex-direction: column;
    gap: 16px;
  }
  
  .price-display {
    font-size: 1.2rem;
    padding: 10px 16px;
  }
  
  .price-amount {
    font-size: 1.3rem;
  }
  
  .info-grid {
    grid-template-columns: 1fr;
  }
  
  .car-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }
}
</style>