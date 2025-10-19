<script setup lang="ts">
import { ref, h } from 'vue'
import type { Component } from 'vue'
import { useRouter } from 'vue-router'
import { NDropdown, NButton, NIcon, NAvatar, NSpace } from 'naive-ui'
import { 
  PersonOutline, 
  SettingsOutline, 
  LogOutOutline,
  ChevronDownOutline ,
  LayersOutline
} from '@vicons/ionicons5'
import axios from 'axios'
import { useAuthStore } from '@/api/Auth'
import defaultProfilePic from '@/assets/profile/default-profile-pic-1.png'

// Router setup
const router = useRouter()

function renderIcon(icon: Component) {
  return () => h(NIcon, null, { default: () => h(icon) })
}

// Profile dropdown options - icons need to be render functions for NDropdown
const profileOptions = [
  {
    label: 'My Profile',
    key: 'profile',
    icon: renderIcon(PersonOutline)
  },
  {
    label: 'Owned properties',
    key: 'owned-prop', 
    icon: renderIcon(LayersOutline)
  },
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
]

// Handle dropdown menu selection
const handleSelect = async (key: string) => {
  if (key === 'logout') {
    try {
      const authStore = useAuthStore()
      await authStore.logout()
      router.push('/') 
    } catch (error) {
      console.error('Logout error:', error)
    }
    return
  }

  router.push({ name: key })
}

// Mock user data (replace with real user data)
const user = ref({
  name: 'John Doe',
  avatar: undefined // undefined works better for fallback
})
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
          :src="user.avatar || defaultProfilePic"
        />
        <span class="profile-name" style="font-weight: 500;">{{ user.name }}</span>
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