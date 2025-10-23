<!-- Home page with posts grid -->
<script setup lang="ts">
import { ref, onMounted, computed, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import {
  NCard,
  NGrid,
  NGridItem,
  NSpin,
  NCarousel,
  NTag,
  NText,
  NButton,
  NEmpty,
  NSpace,
  useMessage
} from 'naive-ui';
import SearchBar from './navbar/SearchBar.vue';
import { postsApi, carsApi, usersApi, ApiError } from '@/api';
import { useAuthStore } from '@/api/Auth';
import type { PostResponseDto, CarResponseDto, BasicUserInfo } from '@/api/types';

const router = useRouter();
const message = useMessage();
const authStore = useAuthStore();

// State
const posts = ref<PostResponseDto[]>([]);
const cars = ref<Map<number, CarResponseDto>>(new Map());
const users = ref<Map<string, BasicUserInfo>>(new Map());
const loading = ref(false);
const windowWidth = ref(window.innerWidth);

// Responsive grid columns based on screen width
const gridCols = computed(() => {
  if (windowWidth.value >= 1200) return 4; // Large screens: 4 cards
  if (windowWidth.value >= 900) return 3;  // Medium-large: 3 cards
  if (windowWidth.value >= 600) return 2;  // Medium: 2 cards
  return 1; // Small screens: 1 card
});

// Window resize handler
const handleResize = () => {
  windowWidth.value = window.innerWidth;
};

// Load all posts for home page
const loadPosts = async () => {
  try {
    loading.value = true;
    const allPosts = await postsApi.getAllPosts();

    // Filter posts: only show available posts with time slots
    const availablePosts = allPosts.filter(post => {
      // Show post only if it's marked as available AND has time slots
      return post.available && post.availableTimeSlots && post.availableTimeSlots.length > 0;
    });

    // Sort posts by most recent first using postedAt date
    posts.value = availablePosts.sort((a, b) => new Date(b.postedAt).getTime() - new Date(a.postedAt).getTime());

    // Load car details for each AVAILABLE post only
    const carIds = [...new Set(availablePosts.map(post => post.carId))];
    for (const carId of carIds) {
      try {
        const car = await carsApi.getCarById(carId);
        cars.value.set(carId, car);
      } catch (error) {
        console.warn(`Failed to load car ${carId}:`, error);
      }
    }

    // Load user details for each AVAILABLE post only (for displaying owner names)
    const userIds = [...new Set(availablePosts.map(post => post.ownerId))];
    for (const userId of userIds) {
      try {
        const user = await usersApi.getBasicUserInfo(userId);
        users.value.set(userId, user);
      } catch (error) {
        console.warn(`Failed to load user ${userId}:`, error);
      }
    }
    
  } catch (error) {
    console.error('Failed to load posts:', error);
    if (error instanceof ApiError) {
      message.error(`Failed to load posts: ${error.message}`);
    } else {
      message.error('Failed to load posts');
    }
  } finally {
    loading.value = false;
  }
};



// Get car image URLs
const getCarImageUrls = (car: CarResponseDto): string[] => {
  if (!car?.carImages) return [];
  return carsApi.getCarImageUrls(car.carImages);
};

// Navigate to post details
const viewPostDetails = (postId: number) => {
  router.push(`/post/${postId}`);
};

// Handle search events
const handleSearch = (query: string) => {
  console.log('Search submitted:', query);
  // TODO: Implement search logic - navigate to search results or call search API
};

const handleSearchInput = (query: string) => {
  console.log('Search input changed:', query);
  // TODO: Implement live search suggestions or autocomplete
};

// Load data on component mount
onMounted(() => {
  loadPosts();
  window.addEventListener('resize', handleResize);
});

onUnmounted(() => {
  window.removeEventListener('resize', handleResize);
});
</script>

<template>
  <div class="container">
    <!-- Search Bar at the top -->
    <div class="search-section">
      <SearchBar @search="handleSearch" @input="handleSearchInput" />
    </div>

    <h1 class="welcome-text">Welcome {{ authStore.user?.name }}!</h1>

    <!-- Loading state -->
    <NSpin v-if="loading" size="large" style="margin: 40px 0;">
      <template #description>Loading available cars...</template>
    </NSpin>

    <!-- Posts grid -->
    <div v-else-if="posts.length > 0" class="posts-section">
      <NGrid 
        :cols="gridCols" 
        :x-gap="20" 
        :y-gap="20"
      >
        <NGridItem 
          v-for="post in posts" 
          :key="post.postId"
        >
          <NCard 
            class="post-card"
            hoverable
            @click="viewPostDetails(post.postId)"
          >
            <!-- Car Images Carousel -->
            <div class="car-images" @click.stop>
              <NCarousel 
                v-if="cars.get(post.carId)?.carImages?.length"
                :show-dots="true"
                :show-arrow="true"
                effect="card"
                style="height: 240px;"
                @click.stop
              >
                <img 
                  v-for="(image, index) in getCarImageUrls(cars.get(post.carId)!)"
                  :key="index"
                  :src="image"
                  class="carousel-img"
                  alt="Car image"
                />
              </NCarousel>
              <div v-else class="no-images">
                <span>📷</span>
                <NText depth="3">No images available</NText>
              </div>
            </div>

            <!-- Post Info -->
            <div class="post-info">
              <div class="post-header">
                <h3 class="post-title">{{ post.title }}</h3>
                <NButton
                  v-if="authStore.userRole === 'Client'"
                  type="success"
                  class="booking-button"
                  @click.stop="router.push(`/booking/${post.postId}`)"
                >
                  Book - ${{ post.price }}/day
                </NButton>
                <NTag v-else type="success" class="price-tag">
                  ${{ post.price }}/day
                </NTag>
              </div>

              <!-- Car Details -->
              <div v-if="cars.get(post.carId)" class="car-details">
                <NText class="car-name">
                  {{ cars.get(post.carId)?.brand }} {{ cars.get(post.carId)?.model }}
                  {{ cars.get(post.carId)?.version }}
                </NText>
                <NSpace class="car-specs">
                  <NTag size="small">{{ cars.get(post.carId)?.yearProduction }}</NTag>
                  <NTag size="small">{{ cars.get(post.carId)?.fuelDisplay }}</NTag>
                  <NTag size="small">{{ cars.get(post.carId)?.transmissionDisplay }}</NTag>
                </NSpace>
              </div>

              <!-- Location and Owner -->
              <div class="post-meta">
                <NText depth="2" class="location">📍 {{ post.location }}</NText>
                <NText depth="3" class="owner">
                  by {{ users.get(post.ownerId)?.fullName || 'Owner' }}
                </NText>
              </div>

              <!-- Spacer to push button to bottom -->
              <div class="spacer"></div>
              
              <!-- View Details Button -->
              <NButton 
                type="primary" 
                class="view-button"
                @click.stop="viewPostDetails(post.postId)"
              >
                View Details
              </NButton>
            </div>
          </NCard>
        </NGridItem>
      </NGrid>
    </div>

    <!-- Empty state -->
    <NEmpty v-else description="No cars available for rent at the moment" class="empty-state">
      <template #icon>
        <span style="font-size: 48px;">🚗</span>
      </template>
    </NEmpty>
  </div>
</template>

<style scoped>
.container {
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  align-items: center;
  min-height: 100vh;
  color: black;
  padding: 20px;
  max-width: 1600px;
  margin: 0 auto;
}

.search-section {
  width: 100%;
  max-width: 600px;
  margin-bottom: 40px;
  display: flex;
  justify-content: center;
}

.search-section:deep(.container) {
  text-align: left;
}

.welcome-text {
  text-align: center;
  width: 100%;
  margin-bottom: 40px;
  color: #333;
  font-size: 2.5rem;
  font-weight: 600;
}

.posts-section {
  width: 100%;
  max-width: 1400px;
}

.post-card {
  cursor: pointer;
  transition: all 0.3s ease;
  height: 100%;
  display: flex;
  flex-direction: column;
  min-width: 280px; /* Minimum card width to prevent over-shrinking */
}

.post-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12);
}

.post-card :deep(.n-card__content) {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.car-images {
  position: relative;
  margin-bottom: 16px;
}

.carousel-img {
  width: 100%;
  height: 240px;
  object-fit: cover;
  border-radius: 6px;
}

.no-images {
  height: 240px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: #f5f5f5;
  border-radius: 6px;
  color: #999;
}

.no-images span {
  font-size: 48px;
  margin-bottom: 8px;
}

.post-info {
  padding: 4px 0;
  display: flex;
  flex-direction: column;
  flex: 1;
}

.post-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 12px;
  gap: 12px;
}

.post-title {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
  color: #333;
  line-height: 1.4;
  flex: 1;
}

.price-tag {
  font-weight: 600;
  font-size: 1rem;
  flex-shrink: 0;
}

.booking-button {
  font-weight: 600;
  font-size: 0.95rem;
  flex-shrink: 0;
  white-space: nowrap;
}

.car-details {
  margin-bottom: 12px;
}

.car-name {
  font-weight: 500;
  font-size: 1.1rem;
  color: #555;
  display: block;
  margin-bottom: 8px;
}

.car-specs {
  display: flex;
  gap: 6px;
  flex-wrap: wrap;
}

.post-meta {
  margin: 12px 0 16px 0;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.location {
  font-size: 0.95rem;
  font-weight: 500;
}

.owner {
  font-size: 0.9rem;
}

.spacer {
  flex: 1;
  min-height: 12px;
}

.view-button {
  width: 100%;
  margin-top: 8px;
}

.empty-state {
  margin-top: 60px;
}

/* Responsive grid adjustments */
@media (max-width: 768px) {
  .container {
    padding: 16px;
  }
  
  .welcome-text {
    font-size: 2rem;
    margin-bottom: 30px;
  }
  
  .post-header {
    flex-direction: column;
    gap: 8px;
  }
  
  .price-tag {
    align-self: flex-start;
  }
}

@media (max-width: 480px) {
  .container {
    padding: 12px;
  }
  
  .welcome-text {
    font-size: 1.75rem;
  }
  
  .carousel-img {
    height: 200px;
  }
  
  .no-images {
    height: 200px;
  }
}
</style>
