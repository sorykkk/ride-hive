<script setup lang="ts">
import { ref } from 'vue';
import { NCard, NSpace, NAvatar, NButton, NTag } from 'naive-ui';
import CarProperty from './CarProperty.vue';
import defaultProfilePic from '@/assets/profile/default-profile-pic-1.png'

// Mock user data - in real app this would come from auth store
const user = ref({
  id: 1,
  name: 'John Doe',
  email: 'john.doe@example.com',
  joinDate: '2024-01-15',
  avatar: defaultProfilePic
});
</script>

<template>
  <div class="profile-container">
    <div class="profile-content">
      <!-- User Profile Section -->
      <NCard class="profile-card">
        <div class="profile-header">
          <NAvatar 
            :size="80" 
            :src="user.avatar || defaultProfilePic"
            class="profile-avatar"
          />
          <div class="profile-info">
            <h1 class="profile-name">{{ user.name }}</h1>
            <p class="profile-email">{{ user.email }}</p>
            <NTag type="info" size="small">
              Member since {{ new Date(user.joinDate).toLocaleDateString() }}
            </NTag>
          </div>
          <div class="profile-actions">
            <NSpace>
              <NButton type="primary">Edit Profile</NButton>
              <NButton>Settings</NButton>
            </NSpace>
          </div>
        </div>
      </NCard>

      <!-- Car Property Section -->
      <CarProperty />
    </div>
  </div>
</template>

<style scoped>
.profile-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem 1rem;
}

.profile-content {
  width: 100%;
}

.profile-card {
  margin-bottom: 2rem;
  border-radius: 12px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
}

.profile-header {
  display: flex;
  align-items: center;
  gap: 2rem;
  padding: 1rem;
}

.profile-avatar {
  flex-shrink: 0;
}

.profile-info {
  flex: 1;
}

.profile-name {
  margin: 0 0 0.5rem 0;
  font-size: 2rem;
  font-weight: 600;
  color: #2c3e50;
}

.profile-email {
  margin: 0 0 0.75rem 0;
  color: #64748b;
  font-size: 1rem;
}

.profile-actions {
  flex-shrink: 0;
}

/* Responsive design */
@media (max-width: 768px) {
  .profile-header {
    flex-direction: column;
    text-align: center;
    gap: 1.5rem;
  }
  
  .profile-info {
    flex: none;
  }
  
  .profile-name {
    font-size: 1.5rem;
  }
}
</style>