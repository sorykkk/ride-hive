<script setup lang="ts">
import { computed } from 'vue';
import { useRouter } from 'vue-router';
import { NCard, NSpace, NAvatar, NButton, NTag, NGrid, NGridItem, NIcon } from 'naive-ui';
import { SettingsOutline, CreateOutline, CalendarOutline, MailOutline, CallOutline, LocationOutline } from '@vicons/ionicons5';
import { useAuthStore } from '@/api/Auth';
import defaultProfilePic from '@/assets/profile/default-profile-pic-1.png'

const router = useRouter();
const authStore = useAuthStore();

// Computed user data from auth store
const user = computed(() => {
  const baseUrl = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7000';
  const profileImageUrl = authStore.user?.hasProfileImage
    ? `${baseUrl}/api/user/profile/image/${authStore.user.userId}`
    : defaultProfilePic;

  return {
    id: authStore.user?.userId,
    name: `${authStore.user?.name || ''} ${authStore.user?.surname || ''}`.trim() || 'User',
    email: authStore.user?.email || '',
    phone: authStore.user?.phone ? `+${authStore.user.phone}` : 'Not provided',
    location: authStore.user?.location || 'Not provided',
    joinDate: authStore.user?.registeredAt || new Date().toISOString(),
    avatar: profileImageUrl,
    bio: authStore.user?.bio || 'No bio provided',
    verificationStatus: 'verified', // TODO: Add verification status to user profile
    rating: 0, // TODO: Add rating to user profile
    totalRides: 0, // TODO: Add total rides to user profile
    membershipType: authStore.user?.role || 'Client'
  };
});

// Navigate to edit profile
const editProfile = () => {
  router.push('/edit-profile');
};

// Navigate to settings
const goToSettings = () => {
  router.push('/settings');
};
</script>

<template>
  <div class="user-info-container">
    <!-- Main Profile Card -->
    <NCard class="profile-card">
      <div class="profile-header">
        <NAvatar 
          :size="120" 
          :src="user.avatar || defaultProfilePic"
          class="profile-avatar"
        />
        <div class="profile-info">
          <div class="name-section">
            <h1 class="profile-name">{{ user.name }}</h1>
            <NTag 
              :type="user.verificationStatus === 'verified' ? 'success' : 'warning'"
              size="small"
              class="verification-tag"
            >
              {{ user.verificationStatus === 'verified' ? '✓ Verified' : 'Pending Verification' }}
            </NTag>
          </div>
          
          <p class="profile-bio">{{ user.bio }}</p>
          
          <div class="profile-stats">
            <div class="stat-item">
              <span class="stat-value">{{ user.rating }}</span>
              <span class="stat-label">Rating</span>
            </div>
            <div class="stat-item">
              <span class="stat-value">{{ user.totalRides }}</span>
              <span class="stat-label">Total Rides</span>
            </div>
            <div class="stat-item">
              <span class="stat-value">{{ user.membershipType }}</span>
              <span class="stat-label">Membership</span>
            </div>
          </div>
        </div>
        
        <div class="profile-actions">
          <NSpace vertical>
            <NButton type="primary" @click="editProfile">
              <template #icon>
                <NIcon><CreateOutline /></NIcon>
              </template>
              Edit Profile
            </NButton>
            <NButton @click="goToSettings">
              <template #icon>
                <NIcon><SettingsOutline /></NIcon>
              </template>
              Settings
            </NButton>
          </NSpace>
        </div>
      </div>
    </NCard>

    <!-- Contact Information -->
    <NCard title="Contact Information" class="contact-card">
      <NGrid :cols="1" :y-gap="16">
        <NGridItem>
          <div class="contact-item">
            <NIcon size="20" class="contact-icon">
              <MailOutline />
            </NIcon>
            <div class="contact-details">
              <span class="contact-label">Email</span>
              <span class="contact-value">{{ user.email }}</span>
            </div>
          </div>
        </NGridItem>
        
        <NGridItem>
          <div class="contact-item">
            <NIcon size="20" class="contact-icon">
              <CallOutline />
            </NIcon>
            <div class="contact-details">
              <span class="contact-label">Phone</span>
              <span class="contact-value">{{ user.phone }}</span>
            </div>
          </div>
        </NGridItem>
        
        <NGridItem>
          <div class="contact-item">
            <NIcon size="20" class="contact-icon">
              <LocationOutline />
            </NIcon>
            <div class="contact-details">
              <span class="contact-label">Location</span>
              <span class="contact-value">{{ user.location }}</span>
            </div>
          </div>
        </NGridItem>
        
        <NGridItem>
          <div class="contact-item">
            <NIcon size="20" class="contact-icon">
              <CalendarOutline />
            </NIcon>
            <div class="contact-details">
              <span class="contact-label">Member Since</span>
              <span class="contact-value">{{ new Date(user.joinDate).toLocaleDateString('en-US', { 
                year: 'numeric', 
                month: 'long', 
                day: 'numeric' 
              }) }}</span>
            </div>
          </div>
        </NGridItem>
      </NGrid>
    </NCard>
  </div>
</template>

<style scoped>
.user-info-container {
  max-width: 1000px;
  margin: 0 auto;
  padding: 24px;
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.profile-card {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
}

.profile-card :deep(.n-card__content) {
  padding: 32px;
}

.profile-header {
  display: flex;
  align-items: flex-start;
  gap: 32px;
}

.profile-avatar {
  border: 4px solid rgba(255, 255, 255, 0.3);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
}

.profile-info {
  flex: 1;
  min-width: 0;
}

.name-section {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 12px;
}

.profile-name {
  font-size: 2.5rem;
  font-weight: 700;
  margin: 0;
  color: white;
}

.verification-tag {
  font-weight: 600;
}

.profile-bio {
  font-size: 1.1rem;
  margin: 0 0 24px 0;
  opacity: 0.9;
  line-height: 1.6;
}

.profile-stats {
  display: flex;
  gap: 32px;
}

.stat-item {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.stat-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: white;
}

.stat-label {
  font-size: 0.9rem;
  opacity: 0.8;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.profile-actions {
  display: flex;
  flex-direction: column;
}

.profile-actions :deep(.n-button) {
  min-width: 140px;
}

.contact-card {
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
}

.contact-card :deep(.n-card-header__main) {
  font-size: 1.25rem;
  font-weight: 600;
  color: #2d3748;
}

.contact-item {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 12px 0;
}

.contact-icon {
  color: #667eea;
  background: rgba(102, 126, 234, 0.1);
  padding: 8px;
  border-radius: 8px;
  min-width: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.contact-details {
  display: flex;
  flex-direction: column;
  flex: 1;
}

.contact-label {
  font-size: 0.875rem;
  color: #718096;
  margin-bottom: 2px;
}

.contact-value {
  font-size: 1rem;
  color: #2d3748;
  font-weight: 500;
}

/* Responsive Design */
@media (max-width: 768px) {
  .user-info-container {
    padding: 16px;
  }

  .profile-header {
    flex-direction: column;
    text-align: center;
    gap: 24px;
  }

  .profile-stats {
    justify-content: center;
    gap: 24px;
  }

  .profile-name {
    font-size: 2rem;
  }

  .name-section {
    flex-direction: column;
    gap: 8px;
  }

  .profile-actions :deep(.n-space) {
    width: 100%;
  }

  .profile-actions :deep(.n-button) {
    width: 100%;
  }
}

@media (max-width: 480px) {
  .profile-stats {
    flex-direction: column;
    gap: 16px;
  }

  .profile-name {
    font-size: 1.75rem;
  }
}
</style>