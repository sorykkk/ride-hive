<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { NPopover, NButton, NIcon, NBadge, NSpace, useMessage, NScrollbar, NDivider, NSpin } from 'naive-ui'
import { NotificationsOutline, CheckmarkOutline, CloseOutline, CheckmarkCircleOutline, CloseCircleOutline } from '@vicons/ionicons5'
import { useAuthStore } from '@/api/Auth'
import * as notificationsApi from '@/api/notifications'
import type { Notification } from '@/api/notifications'

// Auth store
const authStore = useAuthStore()
const message = useMessage()

// State
const notifications = ref<Notification[]>([])
const isLoading = ref(false)
const error = ref<string | null>(null)

// Computed: has notifications
const hasNotifications = computed(() => notifications.value.length > 0)

// Computed: notification count
const notificationCount = computed(() => notifications.value.length)

// Fetch notifications from API
const fetchNotifications = async () => {
  if (!authStore.user?.userId) {
    return
  }

  isLoading.value = true
  error.value = null

  try {
    const data = await notificationsApi.getNotifications(authStore.user.userId)
    notifications.value = data
  } catch (err: any) {
    error.value = 'Failed to load notifications'
    console.error('Error fetching notifications:', err)
  } finally {
    isLoading.value = false
  }
}

// Handle accept booking (Owner only)
const handleAccept = async (notification: Notification) => {
  if (notification.type !== 'booking_request') return

  console.log('Accepting request:', notification.requestId)
  console.log('Full notification:', notification)

  try {
    await notificationsApi.acceptRequest(notification.requestId)
    message.success(`Accepted booking from ${notification.userName}`)

    // Refresh notifications to get updated list
    await fetchNotifications()
  } catch (err: any) {
    const errorMessage = err.message || 'Failed to accept booking'
    message.error(`Error: ${errorMessage}`)
    console.error('Error accepting request:', err)
    console.error('Full error object:', JSON.stringify(err, null, 2))
  }
}

// Handle decline booking (Owner only)
const handleDecline = async (notification: Notification) => {
  if (notification.type !== 'booking_request') return

  console.log('Declining request:', notification.requestId)
  console.log('Full notification:', notification)

  try {
    await notificationsApi.declineRequest(notification.requestId)
    message.warning(`Declined booking from ${notification.userName}`)

    // Refresh notifications to get updated list
    await fetchNotifications()
  } catch (err: any) {
    const errorMessage = err.message || 'Failed to decline booking'
    message.error(`Error: ${errorMessage}`)
    console.error('Error declining request:', err)
    console.error('Full error object:', JSON.stringify(err, null, 2))
  }
}

// Handle dismiss notification (Client only)
const handleDismiss = async (notification: Notification) => {
  try {
    await notificationsApi.deleteNotification(notification.notificationId)

    // Remove notification from list
    notifications.value = notifications.value.filter(n => n.notificationId !== notification.notificationId)
  } catch (err: any) {
    message.error('Failed to dismiss notification')
    console.error('Error dismissing notification:', err)
  }
}

// Type guards
const isBookingRequest = (notification: Notification): boolean => {
  return notification.type === 'booking_request'
}

const isBookingResponse = (notification: Notification): boolean => {
  return notification.type === 'booking_accepted' || notification.type === 'booking_rejected'
}

// Format dates for display
const formatDates = (dates: string[]): string => {
  if (!dates || dates.length === 0) return ''

  const firstDate = dates[0]
  if (!firstDate) return ''

  if (dates.length === 1) {
    return new Date(firstDate).toLocaleDateString()
  }

  const lastDate = dates[dates.length - 1]
  if (!lastDate) return new Date(firstDate).toLocaleDateString()

  const first = new Date(firstDate).toLocaleDateString()
  const last = new Date(lastDate).toLocaleDateString()
  return `${first} - ${last}`
}

// Load notifications on mount
onMounted(() => {
  fetchNotifications()
})

// Watch for user changes and reload notifications
watch(() => authStore.user?.userId, (newUserId) => {
  if (newUserId) {
    fetchNotifications()
  } else {
    notifications.value = []
  }
})

// Refresh notifications every 30 seconds
setInterval(() => {
  if (authStore.user?.userId) {
    fetchNotifications()
  }
}, 30000)
</script>

<template>
  <NPopover trigger="click" placement="bottom-end" @update:show="(show) => show && fetchNotifications()">
    <template #trigger>
      <NBadge
        :show="hasNotifications"
        :value="notificationCount"
        type="error"
        :offset="[-5, 5]"
      >
        <NButton
          quaternary
          circle
          aria-label="Notifications"
        >
          <template #icon>
            <NIcon size="20">
              <NotificationsOutline />
            </NIcon>
          </template>
        </NButton>
      </NBadge>
    </template>

    <!-- Dropdown content with multiple notifications -->
    <div class="notifications-container">
      <h3 class="notifications-header">
        {{ authStore.isOwner ? 'Booking Requests' : 'Booking Responses' }} ({{ notificationCount }})
      </h3>

      <!-- Loading state -->
      <div v-if="isLoading" class="notifications-loading">
        <NSpin size="small" />
        <span>Loading notifications...</span>
      </div>

      <!-- Error state -->
      <div v-else-if="error" class="notifications-error">
        <p>{{ error }}</p>
        <NButton text type="primary" size="small" @click="fetchNotifications">
          Retry
        </NButton>
      </div>

      <!-- Empty state -->
      <div v-else-if="!hasNotifications" class="notifications-empty">
        <p>No notifications</p>
      </div>

      <!-- Notifications list -->
      <NScrollbar v-else style="max-height: 400px">
        <div class="notifications-list">
          <template v-for="(notification, index) in notifications" :key="notification.notificationId">
            <!-- Booking Request (for Owners) -->
            <div v-if="isBookingRequest(notification)" class="notification-card">
              <div class="notification-content">
                <div class="notification-title">{{ notification.title }}</div>
                <div class="notification-message">{{ notification.message }}</div>
                <div class="booking-dates">{{ formatDates(notification.requestedDates) }}</div>
              </div>

              <NSpace :size="12" style="margin-top: 12px; width: 100%">
                <NButton
                  type="success"
                  size="small"
                  style="flex: 1"
                  @click="() => handleAccept(notification)"
                >
                  <template #icon>
                    <NIcon><CheckmarkOutline /></NIcon>
                  </template>
                  Accept
                </NButton>
                <NButton
                  type="error"
                  size="small"
                  style="flex: 1"
                  @click="() => handleDecline(notification)"
                >
                  <template #icon>
                    <NIcon><CloseOutline /></NIcon>
                  </template>
                  Decline
                </NButton>
              </NSpace>
            </div>

            <!-- Booking Response (for Clients) -->
            <div v-else-if="isBookingResponse(notification)" class="notification-card">
              <div class="response-header" :class="notification.type">
                <NIcon :size="20" :color="notification.type === 'booking_accepted' ? '#18a058' : '#d03050'">
                  <CheckmarkCircleOutline v-if="notification.type === 'booking_accepted'" />
                  <CloseCircleOutline v-else />
                </NIcon>
                <span class="response-title">{{ notification.title }}</span>
              </div>

              <div class="notification-content">
                <div class="notification-message">{{ notification.message }}</div>
                <div class="booking-dates">{{ formatDates(notification.requestedDates) }}</div>
              </div>

              <NButton
                text
                size="small"
                type="primary"
                style="margin-top: 8px; width: 100%"
                @click="() => handleDismiss(notification)"
              >
                Dismiss
              </NButton>
            </div>

            <NDivider v-if="index < notifications.length - 1" style="margin: 12px 0" />
          </template>
        </div>
      </NScrollbar>
    </div>
  </NPopover>
</template>

<style scoped>
.notifications-container {
  width: 360px;
  max-width: 90vw;
}

.notifications-header {
  margin: 0;
  padding: 16px;
  font-size: 16px;
  font-weight: 600;
  color: #333;
  border-bottom: 1px solid #e5e5e5;
}

.notifications-loading,
.notifications-error,
.notifications-empty {
  padding: 40px 20px;
  text-align: center;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12px;
}

.notifications-loading {
  color: #666;
}

.notifications-error {
  color: #d03050;
}

.notifications-empty {
  color: #999;
}

.notifications-list {
  padding: 0;
}

.notification-card {
  padding: 16px;
}

.notification-content {
  margin-bottom: 8px;
}

.notification-title {
  font-size: 15px;
  font-weight: 600;
  color: #333;
  margin-bottom: 6px;
}

.notification-message {
  font-size: 14px;
  color: #666;
  margin-bottom: 6px;
  line-height: 1.5;
}

.booking-dates {
  font-size: 13px;
  color: #999;
  margin-top: 4px;
}

/* Client notification styles */
.response-header {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 12px;
}

.response-title {
  font-size: 15px;
  font-weight: 600;
}

.response-header.booking_accepted .response-title {
  color: #18a058;
}

.response-header.booking_rejected .response-title {
  color: #d03050;
}

.response-content {
  padding-left: 4px;
}

/* Mobile responsive */
@media (max-width: 480px) {
  .notifications-container {
    width: 300px;
  }

  .notification-card {
    padding: 12px;
  }
}
</style>
