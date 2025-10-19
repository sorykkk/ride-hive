<script setup lang="ts">
import { ref } from 'vue';
import { NCard, NForm, NFormItem, NInput, NButton, NSpace, NAvatar, NUpload, NPageHeader, NGrid, NGridItem, useMessage } from 'naive-ui';
import type { FormInst } from 'naive-ui';
import { useRouter } from 'vue-router';
import defaultProfilePic from '@/assets/profile/default-profile-pic-1.png';

const router = useRouter();
const message = useMessage();
const formRef = ref<FormInst | null>(null);

// Mock user data - in real app this would come from auth store/API
const userForm = ref({
  name: 'John Doe',
  email: 'john.doe@example.com',
  phone: '+1 (555) 123-4567',
  location: 'San Francisco, CA',
  bio: 'Car enthusiast and ride-sharing advocate. I love exploring new places and meeting fellow travelers.',
  avatar: defaultProfilePic
});

const saveProfile = async () => {
  try {
    // Here you would call your API to save the profile
    message.success('Profile updated successfully!');
    router.push('/profile');
  } catch (error) {
    message.error('Failed to update profile. Please try again.');
  }
};

const goBack = () => {
  router.back();
};
</script>

<template>
  <div class="edit-profile-container">
    <NPageHeader @back="goBack">
      <template #title>Edit Profile</template>
      <template #subtitle>Update your personal information</template>
    </NPageHeader>

    <div class="form-container">
      <NCard title="Profile Information">
        <NForm ref="formRef" :model="userForm" label-placement="top">
          <NGrid :cols="1" :x-gap="24" :y-gap="16">
            <!-- Avatar Section -->
            <NGridItem>
              <div class="avatar-section">
                <NAvatar 
                  :size="120" 
                  :src="userForm.avatar"
                  class="profile-avatar"
                />
                <div class="avatar-actions">
                  <NUpload
                    accept="image/*"
                    :max="1"
                    :show-file-list="false"
                  >
                    <NButton>Change Photo</NButton>
                  </NUpload>
                </div>
              </div>
            </NGridItem>

            <!-- Form Fields -->
            <NGridItem>
              <NGrid :cols="2" :x-gap="16" :y-gap="16" responsive="screen">
                <NGridItem span="2 s:1">
                  <NFormItem label="Full Name" required>
                    <NInput v-model:value="userForm.name" placeholder="Enter your full name" />
                  </NFormItem>
                </NGridItem>
                
                <NGridItem span="2 s:1">
                  <NFormItem label="Email" required>
                    <NInput v-model:value="userForm.email" placeholder="Enter your email" />
                  </NFormItem>
                </NGridItem>
                
                <NGridItem span="2 s:1">
                  <NFormItem label="Phone">
                    <NInput v-model:value="userForm.phone" placeholder="Enter your phone number" />
                  </NFormItem>
                </NGridItem>
                
                <NGridItem span="2 s:1">
                  <NFormItem label="Location">
                    <NInput v-model:value="userForm.location" placeholder="Enter your location" />
                  </NFormItem>
                </NGridItem>
              </NGrid>
            </NGridItem>

            <NGridItem>
              <NFormItem label="Bio">
                <NInput 
                  v-model:value="userForm.bio" 
                  type="textarea" 
                  placeholder="Tell us about yourself"
                  :rows="4"
                />
              </NFormItem>
            </NGridItem>
          </NGrid>
        </NForm>

        <template #action>
          <NSpace justify="end">
            <NButton @click="goBack">Cancel</NButton>
            <NButton type="primary" @click="saveProfile">Save Changes</NButton>
          </NSpace>
        </template>
      </NCard>
    </div>
  </div>
</template>

<style scoped>
.edit-profile-container {
  max-width: 800px;
  margin: 0 auto;
  padding: 24px;
}

.form-container {
  margin-top: 24px;
}

.avatar-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;
  padding: 24px;
  text-align: center;
}

.profile-avatar {
  border: 4px solid #f0f0f0;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.avatar-actions {
  display: flex;
  gap: 12px;
}

@media (max-width: 768px) {
  .edit-profile-container {
    padding: 16px;
  }
}
</style>