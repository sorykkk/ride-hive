<script setup lang="ts">
import { computed, h } from 'vue'
import type { Component } from 'vue'
import { useRouter } from 'vue-router'
import { NDropdown, NButton, NIcon, NAvatar, NSpace, useMessage } from 'naive-ui'
import { 
  PersonOutline, 
  SettingsOutline, 
  LogOutOutline,
  ChevronDownOutline,
  LayersOutline
} from '@vicons/ionicons5'
import { useAuthStore } from "@/api/Auth"
import defaultProfilePic from '@/assets/profile/default-profile-pic-1.png'

// Router setup
const router = useRouter()
const authStore = useAuthStore()
const message = useMessage()

function renderIcon(icon: Component) {
  return () => h(NIcon, null, { default: () => h(icon) })
}

// Profile dropdown options - computed to show different options based on role
const profileOptions = computed(() => {
  const baseOptions: Array<any> = [
    {
      label: 'My Profile',
      key: 'profile',
      icon: renderIcon(PersonOutline)
    }
  ]

  // Add "Owned properties" only for Owners
  if (authStore.isOwner) {
    baseOptions.push({
      label: 'Owned properties',
      key: 'owned-prop',
      icon: renderIcon(LayersOutline)
    })
  }

  // Add remaining common options
  baseOptions.push(
    {
      label: 'Settings',
      key: 'settings',
      icon: renderIcon(SettingsOutline)
    },
    {
      type: 'divider',
      key: 'divider1'
    },
    {
      label: 'Log Out',
      key: 'logout',
      icon: renderIcon(LogOutOutline)
    }
  )

  return baseOptions
})

// Computed properties for user data from auth store
const userDisplayName = computed(() => {
  if (authStore.user) {
    return `${authStore.user.name} ${authStore.user.surname}`
  }
  return 'Guest'
})

const userAvatar = computed(() => {
  if (authStore.user?.hasProfileImage) {
    const baseUrl = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7000';
    return `${baseUrl}/api/user/profile/image/${authStore.user.userId}`;
  }
  return defaultProfilePic;
})

// Handle dropdown menu selection
const handleSelect = async (key: string) => {
  if (key === 'logout') {
    try {
      await authStore.logout()
      message.success('You have been logged out.')
      router.push('/')
    } catch (error) {
      message.error('Logout failed!')
    }
    return
  }

  router.push({ name: key })
}
</script>

<template>
  <NDropdown 
    :options="profileOptions" 
    @select="handleSelect"
    trigger="click"
    placement="bottom-end"
  >
    <NButton quaternary>
      <NSpace align="center" :size="8">
        <NAvatar 
          round
          :size="32" 
          :src="userAvatar"
        />
        <span class="profile-name" style="font-weight: 500;">{{ userDisplayName }}</span>
        <NIcon class="profile-chevron" size="16">
          <ChevronDownOutline />
        </NIcon>
      </NSpace>
    </NButton>
  </NDropdown>
</template>

<style scoped>
/* Responsive profile button - hide name on smaller screens */
@media (max-width: 768px) {
  .profile-name {
    display: none;
  }
  
  /* Bring chevron closer to profile pic and make button more compact */
  .n-space {
    gap: 2px !important;
  }
  
  .n-button {
    padding: 2px 6px !important;
    min-height: 30px !important;
  }
  
  .profile-chevron {
    font-size: 14px !important;
  }
}

/* Extra small screens - make avatar slightly smaller */
@media (max-width: 480px) {
  :deep(.n-avatar) {
    width: 28px !important;
    height: 28px !important;
  }
}
</style>