<script setup lang="ts">

import { ref, h, watch, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import type { Component } from 'vue'
import {
  HomeOutline,
  AddOutline,
  NotificationsOutline,
  InformationCircleOutline,
  Albums } from '@vicons/ionicons5'
import {
  NLayoutHeader,
  NMenu,
  NSpace,
  NIcon,
  NButton,
  NBadge } from 'naive-ui'
import type { MenuProps } from 'naive-ui'
import ProfileDropdown from './ProfileDropdown.vue'
import { useAuthStore } from '@/api/Auth'

// Router setup
const router = useRouter()
const route = useRoute()

// Auth store
const authStore = useAuthStore()

// builds the VNode rendering a naive-ui Component icon
function renderIcon(icon: Component) {
  return () => h(NIcon, null, { default: () => h(icon) })
}

// Override of default style
type MenuThemeOverrides = NonNullable<MenuProps['themeOverrides']>
const menuThemeOverrides: MenuThemeOverrides = {
  // text properties
  itemTextColorActiveHorizontal: '#004d96',
  itemTextColorHoverHorizontal: '#1e86db',
  itemTextColorActiveHoverHorizontal: '#1e86db',
  // icon properties
  itemIconColorActiveHorizontal: '#004d96',
  itemIconColorHoverHorizontal: '#1e86db',
  itemIconColorActiveHoverHorizontal: '#1e86db',
  // fallback options
  itemTextColorActive: '#004d96',
  itemTextColorHover: '#1e86db',
  itemIconColorActive: '#004d96',
  itemIconColorHover: '#1e86db',
}

const activeKey = ref<string | null>(route.name?.toString() || null) // holds currently selected menu
const isPostHovered = ref(false)

// Computed menu options based on user role
const menuOptions = computed(() => {
  const baseOptions = [
    {
      label: 'Home',
      key: 'home',
      icon: renderIcon(HomeOutline)
    },
    {
      label: 'About',
      key: 'about',
      icon: renderIcon(InformationCircleOutline)
    }
  ]

  // Add "My posts" only for Owners
  if (authStore.isOwner) {
    baseOptions.push({
      label: 'My posts',
      key: 'owner-posts',
      icon: renderIcon(Albums)
    })
  }

  return baseOptions
})

// Watch route changes to update active menu item
watch(() => route.name, (newRouteName) => {
  if (newRouteName && menuOptions.value.map(option => option.key).includes(newRouteName.toString())) {
    activeKey.value = newRouteName.toString()
  } else {
    // For routes that aren't in the menu (like create-post), clear selection
    activeKey.value = null
  }
})

// Handle menu selection and navigation
const handleMenuSelect = (key: string) => {
  activeKey.value = key
  router.push({ name: key })
}

// Handle post button click
const handlePostClick = () => {
  // Clear menu selection and navigate to create post page
  activeKey.value = null
  router.push({ name: 'create-post' })
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
    class="navbar-header"
  >
    <!-- Logo -->
    <div class="logo-container">
      <router-link to="/" style="text-decoration: none;">
        <img src="@/assets/logo/logo-1.png" class="logo" alt="ride-hive logo" />
      </router-link>
    </div>

    <!-- Left Side: Navigation Menu + Post Button -->
    <div class="nav-menu-container">
      <!-- Navigation Menu -->
      <div class="menu-wrapper">
        <NMenu
          v-model:value="activeKey"
          mode="horizontal"
          :options="menuOptions"
          :theme-overrides="menuThemeOverrides"
          @update:value="handleMenuSelect"
        />
        <!-- Post Button directly next to Home - Only visible for Owners -->
        <NButton
          v-if="authStore.isOwner"
          type="success"
          circle
          :class="{ 'post-button-expanded': isPostHovered }"
          @click="handlePostClick"
          @mouseenter="isPostHovered = true"
          @mouseleave="isPostHovered = false"
          class="post-button"
        >
          <template #icon>
            <NIcon><AddOutline /></NIcon>
          </template>
          <span v-if="isPostHovered" style="padding: 0 8px;">Post</span>
        </NButton>
      </div>
    </div>

    <!-- Right Side Actions: notifications + profile -->
    <div class="actions-container">
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
.navbar-header {
  height: 64px;
  padding: 0 24px;
  display: flex;
  align-items: center;
  background: white;
  position: sticky;
  top: 0;
  z-index: 100;
}

.logo-container {
  margin-right: 12px;
}

.logo {
  height: 40px;
  padding: 0 12px;
  will-change: filter;
  transition: filter 300ms;
}

.logo:hover {
  filter: drop-shadow(0 0 2em #646cffaa);
}

.nav-menu-container {
  display: flex;
  align-items: center;
  flex: 1;
}

.menu-wrapper {
  display: flex;
  align-items: center;
}

.post-button {
  transition: all 0.3s ease;
  min-width: 40px;
  margin-left: 24px;
}

.actions-container {
  display: flex;
  align-items: center;
  flex-shrink: 0;
}

/* Expandable Post Button Styles */
.post-button-expanded {
  border-radius: 20px !important;
  min-width: 80px !important;
}

:deep(.n-badge-sup) {
  background-color: #ff4757 !important;
  transform: translate(-6px, 6px);
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

/* Mobile responsive layout - Equally distributed icons */
@media (max-width: 768px) {
  .navbar-header {
    padding: 0 8px;
    justify-content: space-between;
  }

  /* Hide logo on mobile to save space */
  .logo-container {
    display: none;
  }

  /* Make nav menu take full width */
  .nav-menu-container {
    flex: 1;
    justify-content: center;
  }

  .menu-wrapper {
    width: 100%;
    justify-content: space-evenly;
  }

  /* Hide menu labels on mobile, show only icons */
  :deep(.n-menu-item-content__icon) {
    margin-right: 0 !important;
  }

  :deep(.n-menu-item-content-header) {
    display: none !important;
  }

  /* Make menu items equally spaced */
  :deep(.n-menu-item) {
    flex: 1;
    justify-content: center !important;
    padding: 0 !important;
  }

  /* Post button stays circular on mobile */
  .post-button {
    margin-left: 0;
    flex-shrink: 0;
  }

  .post-button-expanded {
    border-radius: 50% !important;
    min-width: 40px !important;
    max-width: 40px !important;
  }
  
  .post-button-expanded span {
    display: none !important;
  }
  
  .post-button-expanded :deep(.n-button__icon) {
    margin: 0 !important;
    position: absolute !important;
    left: 50% !important;
    top: 50% !important;
    transform: translate(-50%, -50%) !important;
  }

  /* Actions container with reduced spacing */
  .actions-container {
    margin-left: 8px;
  }

  :deep(.n-space) {
    gap: 4px !important;
  }
}

/* Extra small screens - further optimization */
@media (max-width: 480px) {
  .navbar-header {
    padding: 0 4px;
  }

  :deep(.n-button) {
    width: 36px !important;
    height: 36px !important;
  }

  :deep(.n-icon) {
    font-size: 18px !important;
  }
}
</style>