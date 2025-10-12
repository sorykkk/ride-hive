<script setup lang="ts">
import { ref, h } from 'vue'
import type { Component } from 'vue'
import { NDropdown, NButton, NIcon, NAvatar, NSpace } from 'naive-ui'
import { 
  PersonOutline, 
  SettingsOutline, 
  LogOutOutline,
  ChevronDownOutline ,
  SendOutline
} from '@vicons/ionicons5'
import defaultProfilePic from '@/assets/profile/default-profile-pic-1.png'

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
    label: 'Requested rents',
    key: 'req-rents', 
    icon: renderIcon(SendOutline)
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
const handleSelect = (key: string) => {
  console.log('Selected:', key)
  // TODO: Add navigation logic here
  switch (key) {
    case 'profile':
      // Navigate to profile page
      break
    case 'req-rents':
      // Navigate to settings page  
      break
    case 'settings':
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