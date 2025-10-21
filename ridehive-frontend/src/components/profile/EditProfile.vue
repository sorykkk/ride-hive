<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { NCard, NForm, NFormItem, NInput, NInputNumber, NButton, NSpace, NAvatar, NUpload, NPageHeader, NGrid, NGridItem, useMessage } from 'naive-ui';
import type { FormInst } from 'naive-ui';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/api/Auth';
import defaultProfilePic from '@/assets/profile/default-profile-pic-1.png';

const router = useRouter();
const message = useMessage();
const authStore = useAuthStore();
const formRef = ref<FormInst | null>(null);

// User form with data from auth store
const userForm = ref({
  name: '',
  surname: '',
  email: '',
  phone: null as number | null,
  age: null as number | null,
  bio: '',
  location: '',
  avatar: defaultProfilePic
});

// Load user data from auth store on component mount
onMounted(() => {
  if (authStore.user) {
    userForm.value.name = authStore.user.name;
    userForm.value.surname = authStore.user.surname;
    userForm.value.email = authStore.user.email;
    userForm.value.phone = authStore.user.phone || null;
    userForm.value.age = authStore.user.age || null;
    userForm.value.bio = authStore.user.bio || '';
    userForm.value.location = authStore.user.location || '';

    // Load profile image if exists
    if (authStore.user.hasProfileImage) {
      const baseUrl = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7000';
      userForm.value.avatar = `${baseUrl}/api/user/profile/image/${authStore.user.userId}`;
    }
  } else {
    // Redirect to login if user is not authenticated
    message.warning('Please login first');
    router.push('/login');
  }
});

const saveProfile = async () => {
  try {
    console.log('Updating profile with data:', {
      name: userForm.value.name,
      surname: userForm.value.surname,
      phone: userForm.value.phone,
      age: userForm.value.age,
      bio: userForm.value.bio,
      location: userForm.value.location
    });

    await authStore.updateProfile({
      name: userForm.value.name,
      surname: userForm.value.surname,
      phone: userForm.value.phone ?? undefined,
      age: userForm.value.age ?? undefined,
      bio: userForm.value.bio || undefined,
      location: userForm.value.location || undefined
    });

    message.success('Profile updated successfully!');
    router.push('/profile');
  } catch (error: any) {
    console.error('Profile update error:', error);
    console.error('Error status:', error.status);
    console.error('Error data:', error.data);

    let errorMessage = 'Failed to update profile.';
    if (error.status === 401) {
      errorMessage = 'You need to login again.';
    } else if (error.data?.message) {
      errorMessage = error.data.message;
    } else if (error.data?.errors) {
      errorMessage = Array.isArray(error.data.errors)
        ? error.data.errors.join(', ')
        : JSON.stringify(error.data.errors);
    }

    message.error(errorMessage);
  }
};

const goBack = () => {
  router.back();
};

const handleAvatarUpload = async ({ file }: any) => {
  try {
    // Show preview immediately
    const reader = new FileReader();
    reader.onload = (e) => {
      userForm.value.avatar = e.target?.result as string;
    };
    reader.readAsDataURL(file.file);

    // Upload to server
    const formData = new FormData();
    formData.append('image', file.file);

    const baseUrl = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7000';
    const response = await fetch(`${baseUrl}/api/user/profile/image`, {
      method: 'POST',
      credentials: 'include',
      body: formData
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Failed to upload image');
    }

    // Update auth store to reflect profile image
    if (authStore.user) {
      authStore.user.hasProfileImage = true;
      localStorage.setItem('user', JSON.stringify(authStore.user));
    }

    message.success('Profile photo updated successfully!');
  } catch (error: any) {
    message.error(error.message || 'Failed to upload profile photo');
    userForm.value.avatar = defaultProfilePic;
  }

  return false; // Prevent default upload behavior
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
                    :custom-request="handleAvatarUpload"
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
                  <NFormItem label="First Name" required>
                    <NInput v-model:value="userForm.name" placeholder="Enter your first name" />
                  </NFormItem>
                </NGridItem>
                
                <NGridItem span="2 s:1">
                  <NFormItem label="Last Name" required>
                    <NInput v-model:value="userForm.surname" placeholder="Enter your last name" />
                  </NFormItem>
                </NGridItem>
                
                <NGridItem span="2 s:1">
                  <NFormItem label="Email" required>
                    <NInput v-model:value="userForm.email" placeholder="Enter your email" disabled />
                  </NFormItem>
                </NGridItem>
                
                <NGridItem span="2 s:1">
                  <NFormItem label="Phone">
                    <NInputNumber v-model:value="userForm.phone" placeholder="Enter your phone number" :show-button="false" class="full-width" />
                  </NFormItem>
                </NGridItem>

                <NGridItem span="2 s:1">
                  <NFormItem label="Age">
                    <NInputNumber v-model:value="userForm.age" placeholder="Enter your age" :min="0" :max="120" class="full-width" />
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
                  maxlength="500"
                  show-count
                />
              </NFormItem>
            </NGridItem>

            <NGridItem>
              <NFormItem label="Location">
                <NInput
                  v-model:value="userForm.location"
                  placeholder="Enter your location (e.g., San Francisco, CA)"
                  maxlength="200"
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

.full-width {
  width: 100%;
}
</style>