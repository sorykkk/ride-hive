<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { 
  NCard, 
  NButton, 
  NSpace,
  NGrid,
  NGridItem,
  NTag,
  NPopconfirm,
  NSpin,
  NEmpty,
  useMessage,
  NText,
  NDivider
} from 'naive-ui';
import { postsApi, ApiError } from '@/api';
import type { PostResponseDto } from '@/api/types';
import { formatDateForDisplay, getAvailableDates } from '@/utils/dateUtils';

const router = useRouter();
const message = useMessage();

// State
const posts = ref<PostResponseDto[]>([]);
const loading = ref(false);

// TODO: Replace with actual authenticated user ID from auth service
const currentUserId = "1";

// Load user's posts
const loadPosts = async () => {
  try {
    loading.value = true;
    posts.value = await postsApi.getPostsByOwner(currentUserId);
  } catch (error) {
    console.error('Failed to load posts:', error);
    if (error instanceof ApiError) {
      message.error(`Failed to load posts: ${error.message}`);
    } else {
      message.error('Failed to load posts. Please try again.');
    }
  } finally {
    loading.value = false;
  }
};

// Delete post
const handleDeletePost = async (postId: number) => {
  try {
    await postsApi.deletePost(postId);
    message.success('Post deleted successfully!');
    await loadPosts(); // Reload posts
  } catch (error) {
    console.error('Failed to delete post:', error);
    if (error instanceof ApiError) {
      message.error(`Failed to delete post: ${error.message}`);
    } else {
      message.error('Failed to delete post. Please try again.');
    }
  }
};

// Navigate to post details
const viewPostDetails = (postId: number) => {
  router.push(`/post/${postId}`);
};

// Navigate to create post
const createNewPost = () => {
  router.push('/create-post');
};

// Get available slots count
const getAvailableSlotsCount = (timeSlots: string[]): number => {
  return getAvailableDates(timeSlots).length;
};

// Load posts on component mount
onMounted(() => {
  loadPosts();
});
</script>

<template>
  <div class="posts-container">
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">My Posts</h1>
        <p class="page-subtitle">Manage your rental listings</p>
      </div>
      <!-- <NButton type="primary" @click="createNewPost" class="create-button">
        🚗 Create New Post
      </NButton> -->
    </div>

    <NSpin :show="loading" description="Loading your posts...">
      <div v-if="!loading" class="posts-content">
        <NEmpty v-if="posts.length === 0" description="No posts yet">
          <template #extra>
            <NButton 
              type="primary" 
              @click="createNewPost"
            >
              Create Your First Post
            </NButton>
          </template>
        </NEmpty>

        <NGrid v-else :cols="1" :x-gap="16" :y-gap="16" responsive="screen">
          <NGridItem 
            v-for="post in posts" 
            :key="post.postId"
          >
            <NCard 
              hoverable 
              class="post-card"
              :class="{ 'unavailable': !post.available }"
            >
              <template #header>
                <div class="post-header">
                  <div class="post-title-section">
                    <h3 class="post-title">{{ post.title }}</h3>
                    <NTag 
                      :type="post.available ? 'success' : 'warning'"
                      size="small"
                    >
                      {{ post.available ? 'Available' : 'Unavailable' }}
                    </NTag>
                  </div>
                  <div class="post-price">
                    <span class="price-icon">€</span>
                    <span class="price-amount">{{ post.price }}/day</span>
                  </div>
                </div>
              </template>

              <div class="post-content">
                <div class="post-info">
                  <div class="info-row">
                    <span class="info-icon">📍</span>
                    <span>{{ post.location }}</span>
                  </div>
                  
                  <div class="info-row">
                    <span class="info-icon">📅</span>
                    <span>{{ getAvailableSlotsCount(post.availableTimeSlots) }} available time slots</span>
                  </div>

                  <div class="info-row">
                    <span class="info-icon">🚗</span>
                    <span>Car ID: {{ post.carId }}</span>
                  </div>
                </div>

                <div v-if="post.description" class="post-description">
                  <NText depth="3">{{ post.description }}</NText>
                </div>

                <NDivider />

                <div class="post-meta">
                  <NText depth="3" class="posted-date">
                    Posted: {{ formatDateForDisplay(post.postedAt) }}
                  </NText>
                </div>
              </div>

              <template #action>
                <NSpace justify="end">
                  <NButton 
                    type="primary" 
                    ghost
                    @click="viewPostDetails(post.postId)"
                  >
                    👁️ View Details
                  </NButton>
                  
                  <NPopconfirm
                    @positive-click="handleDeletePost(post.postId)"
                    positive-text="Delete"
                    negative-text="Cancel"
                  >
                    <template #trigger>
                      <NButton type="error" ghost>
                        🗑️ Delete
                      </NButton>
                    </template>
                    Are you sure you want to delete this post? This action cannot be undone.
                  </NPopconfirm>
                </NSpace>
              </template>
            </NCard>
          </NGridItem>
        </NGrid>
      </div>
    </NSpin>
  </div>
</template>

<style scoped>
.posts-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 24px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 32px;
  padding-bottom: 24px;
  border-bottom: 1px solid #f0f0f0;
}

.header-content h1.page-title {
  margin: 0;
  font-size: 2rem;
  font-weight: 600;
  color: #2c3e50;
}

.page-subtitle {
  margin: 4px 0 0 0;
  color: #64748b;
  font-size: 1rem;
}

.create-button {
  padding: 12px 24px;
  height: auto;
}

.posts-content {
  min-height: 400px;
}

.post-card {
  transition: all 0.3s ease;
  border: 1px solid #e2e8f0;
}

.post-card:hover {
  border-color: #667eea;
  box-shadow: 0 4px 16px rgba(102, 126, 234, 0.1);
}

.post-card.unavailable {
  opacity: 0.8;
  border-color: #fbbf24;
}

.post-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.post-title-section {
  display: flex;
  align-items: center;
  gap: 12px;
  flex: 1;
}

.post-title {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
  color: #1f2937;
}

.post-price {
  display: flex;
  align-items: center;
  gap: 4px;
  color: #059669;
  font-weight: 600;
  font-size: 1.1rem;
}

.price-icon {
  color: #059669;
}

.price-amount {
  color: #059669;
}

.post-content {
  padding: 0;
}

.post-info {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-bottom: 16px;
}

.info-row {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #6b7280;
}

.info-icon {
  color: #9ca3af;
}

.post-description {
  margin: 16px 0;
  padding: 12px;
  background-color: #f8fafc;
  border-radius: 6px;
  border-left: 3px solid #667eea;
}

.post-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.posted-date {
  font-size: 0.875rem;
}

/* Responsive design */
@media (max-width: 768px) {
  .posts-container {
    padding: 16px;
  }
  
  .page-header {
    flex-direction: column;
    gap: 16px;
    align-items: stretch;
  }
  
  .create-button {
    width: 100%;
  }
  
  .post-header {
    flex-direction: column;
    gap: 12px;
    align-items: flex-start;
  }
  
  .post-title-section {
    width: 100%;
  }
}
</style>