<script setup lang="ts">
import { ref, h } from 'vue'
import { NLayoutHeader, NMenu, NSpace, NIcon, NButton, NBadge } from 'naive-ui'
import { HomeOutline, AddOutline, NotificationsOutline, InformationCircleOutline } from '@vicons/ionicons5'
import ProfileDropdown from './ProfileDropdown.vue'

const activeKey = ref('home') // holds currently selected menu
const isPostHovered = ref(false)

// Only include primary navigation items here. Profile is rendered on the right
const menuOptions = [
  {
    label: 'Home',
    key: 'home',
    icon: () => h(NIcon, null, { default: () => h(HomeOutline) }) // builds the VNode rendering <NIcon>
  },
  {
    label: 'About',
    key: 'about',
    icon: () => h(NIcon, null, { default: () => h(InformationCircleOutline) })
  }
]

// Handle post button click
//@todo: handle post click logic
const handlePostClick = () => {
  console.log('Post button clicked!')
  // TODO: Open post creation modal or navigate to post page
}

// Handle notifications click
//@todo: handle notification click logic
const handleNotificationsClick = () => {
  console.log('Notifications clicked!')
  // TODO: Open notifications dropdown or navigate to notifications page
}
</script>

<template>
  <NLayoutHeader 
    bordered
    style="height: 64px; padding: 0 24px; display: flex; align-items: center; background: white; position: sticky; top: 0; z-index: 100;"
  >
    <!-- Logo -->
    <div style="margin-right: 12px;">
      <a href="/">
        <img src="@/assets/logo/logo-1.png" class="logo" alt="ride-hive logo" />
      </a>
    </div>

    <!-- Left Side: Navigation Menu + Post Button -->
    <div style="display: flex; align-items: center; flex: 1;">
      <!-- Navigation Menu -->
      <div style="display: flex; align-items: center;">
        <!-- :options -> column before optionsinterpret menuOptions as value not as string -->
        <!-- v-model:value -> two way binding between component property and local variable, here we
          bind value prop of NMenu to activeKey, when active key is changed the prop will change, and 
          when user interacts with menu and changes section, activeKey will automatically update -->
        <NMenu
          v-model:value="activeKey"
          mode="horizontal"
          :options="menuOptions"
        />
        <!-- Post Button directly next to Home -->
        <!-- @ -> shorthand for v-on, is used to listen for DOM events and does something if it is true -->
        <NButton
          type="success"
          circle
          :class="{ 'post-button-expanded': isPostHovered }"
          @click="handlePostClick"
          @mouseenter="isPostHovered = true"
          @mouseleave="isPostHovered = false"
          style="transition: all 0.3s ease; min-width: 40px; margin-left: 24px;"
        >
          <!-- Insert this <NIcon> into the slot named icon of the parent component, only if isPostHovered is false -->
          <template #icon v-if="!isPostHovered">
            <NIcon><AddOutline /></NIcon>
          </template>
          <span v-if="isPostHovered" style="padding: 0 8px;">Post</span>
          <template #icon v-if="isPostHovered">
            <NIcon><AddOutline /></NIcon>
          </template>
        </NButton>
      </div>
    </div>

    <!-- Right Side Actions: notifications + profile -->
    <div style="display: flex; align-items: center; flex-shrink: 0;">
      <NSpace align="center" :size="12">
        <NBadge :value="3" dot type="error" :offset="[-6, 6]">
          <NButton 
            quaternary 
            circle 
            @click="handleNotificationsClick"
            aria-label="Notifications"
          >
            <template #icon>
              <NIcon size="20"><NotificationsOutline /></NIcon>
            </template>
          </NButton>
        </NBadge>
          
        <!-- Profile Dropdown -->
        <ProfileDropdown />
      </NSpace>
    </div>
  </NLayoutHeader>
</template>

<style scoped>
.logo {
  height: 40px;        /* Fits within 64px header */
  padding: 0 12px;     /* Reasonable padding */
  will-change: filter;
  transition: filter 300ms;
}
.logo:hover {
  filter: drop-shadow(0 0 2em #646cffaa);
}

/* Expandable Post Button Styles */
.post-button-expanded {
  border-radius: 20px !important;
  min-width: 80px !important;
}

/* Notification Badge Styles */
:deep(.n-badge-sup) {
  background-color: #ff4757 !important;
  transform: translate(-6px, 6px); /* more left, more down */
}

/* Smooth transitions for all interactive elements */
.n-button {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

/* Hover effects for notification button */
.n-button[aria-label="Notifications"]:hover {
  background-color: #f0f0f0;
  transform: scale(1.05);
}

/* Home menu colors to match SearchBar */
:deep(.n-menu-item--selected) {
  color: #054682 !important;
}

:deep(.n-menu-item--selected .n-menu-item-content-header) {
  color: #054682 !important;
}

:deep(.n-menu-item:hover) {
  color: #1e86db !important;
}

:deep(.n-menu-item:hover .n-menu-item-content-header) {
  color: #1e86db !important;
}

:deep(.n-menu-item--selected .n-icon) {
  color: #054682 !important;
}

:deep(.n-menu-item:hover .n-icon) {
  color: #1e86db !important;
}

/* Reduce spacing between menu items to bring About closer to Home */
/* :deep(.n-menu-item) {
  margin-right: -20px !important;
}

:deep(.n-menu-item:last-child) {
  margin-right: 0 !important;
} */
</style>