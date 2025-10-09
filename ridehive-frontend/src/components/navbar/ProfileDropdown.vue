<script setup lang="ts">
import { ref, h } from 'vue'
import { NDropdown, NButton, NIcon, NAvatar, NSpace } from 'naive-ui'
import { 
  PersonOutline, 
  SettingsOutline, 
  DocumentTextOutline, 
  LogOutOutline,
  ChevronDownOutline 
} from '@vicons/ionicons5'
import defaultProfilePic from '@/assets/profile/default-profile-pic-1.png'

// Profile dropdown options - icons need to be render functions for NDropdown
const profileOptions = [
  {
    label: 'My Profile',
    key: 'profile',
    icon: () => h(NIcon, null, { default: () => h(PersonOutline) })
  },
  {
    label: 'Settings',
    key: 'settings', 
    icon: () => h(NIcon, null, { default: () => h(SettingsOutline) })
  },
  {
    label: 'Documents',
    key: 'documents',
    icon: () => h(NIcon, null, { default: () => h(DocumentTextOutline) })
  },
  {
    type: 'divider',
    key: 'divider1'
  },
  {
    label: 'Log Out',
    key: 'logout',
    icon: () => h(NIcon, null, { default: () => h(LogOutOutline) })
  }
]

// Handle dropdown menu selection
const handleSelect = (key: string) => {
  console.log('Selected:', key)
  // TODO: Add navigation logic here
  switch (key) {
    case 'profile':
      // Navigate to profile page
      break
    case 'settings':
      // Navigate to settings page  
      break
    case 'documents':
      // Navigate to documents page
      break
    case 'logout':
      // Handle logout logic
      break
  }
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
        <span style="font-weight: 500;">{{ user.name }}</span>
        <NIcon size="16">
          <ChevronDownOutline />
        </NIcon>
      </NSpace>
    </NButton>
  </NDropdown>
</template>

<style scoped>
/* Add any component-specific styles here */
</style>