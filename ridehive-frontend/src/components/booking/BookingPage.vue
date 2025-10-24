<template>
  <div class="booking-container">
    <NPageHeader @back="router.back()" class="page-header">
      <template #title>Book Your Ride</template>
    </NPageHeader>

    <NSpin :show="loading" size="large">
      <div v-if="post && car" class="booking-content">
        <!-- Car and Post Info Section -->
        <NCard class="info-card" hoverable>
          <div class="car-info-section">
            <!-- Car Images Carousel -->
            <div v-if="car.images && car.images.length > 0" class="car-images">
              <NCarousel
                :show-dots="true"
                :autoplay="false"
                effect="card"
                :draggable="true"
              >
                <img
                  v-for="(image, index) in car.images"
                  :key="index"
                  :src="image"
                  class="carousel-image"
                  alt="Car image"
                />
              </NCarousel>
            </div>

            <!-- Car Details -->
            <div class="car-details">
              <h2 class="car-title">
                {{ car.brand }} {{ car.model }} {{ car.version }}
              </h2>
              <NSpace class="car-specs" :size="12">
                <NTag type="info" :bordered="false">
                  <template #icon>
                    <NIcon><CalendarOutline /></NIcon>
                  </template>
                  {{ car.yearProduction }}
                </NTag>
                <NTag type="info" :bordered="false">
                  <template #icon>
                    <NIcon><FlameOutline /></NIcon>
                  </template>
                  {{ car.fuelDisplay }}
                </NTag>
                <NTag type="info" :bordered="false">
                  <template #icon>
                    <NIcon><SettingsOutline /></NIcon>
                  </template>
                  {{ car.transmissionDisplay }}
                </NTag>
              </NSpace>

              <!-- Post Info -->
              <div class="post-details">
                <h3 class="post-title">{{ post.title }}</h3>
                <NText v-if="post.description" class="post-description">
                  {{ post.description }}
                </NText>

                <div class="location-section">
                  <NText depth="2">
                    <NIcon><LocationOutline /></NIcon>
                    {{ post.location }}
                  </NText>
                </div>

                <div v-if="post.specialRequirements" class="requirements-section">
                  <NText depth="2" strong>Special Requirements:</NText>
                  <NText>{{ post.specialRequirements }}</NText>
                </div>

                <div class="price-section">
                  <NText strong class="price-label">Price:</NText>
                  <NTag type="success" size="large" :bordered="false" class="price-tag">
                    ${{ post.price }}/day
                  </NTag>
                </div>
              </div>
            </div>
          </div>
        </NCard>

        <!-- Time Slots Selection Section -->
        <NCard title="Select Your Dates" class="booking-card" hoverable>
          <template #header-extra>
            <NText depth="3">{{ selectedSlots.length }} day(s) selected</NText>
          </template>

          <div v-if="availableSlots.length > 0" class="time-slots-container">
            <NText depth="2" class="selection-hint">
              Click on dates to select/deselect them for your booking
            </NText>

            <div class="time-slots-grid">
              <NCard
                v-for="slot in availableSlots"
                :key="slot.iso"
                :class="['time-slot-card', { selected: isSlotSelected(slot.iso) }]"
                hoverable
                @click="toggleSlot(slot.iso)"
              >
                <div class="slot-content">
                  <div class="slot-date">
                    <NText strong>{{ slot.day }}</NText>
                    <NText depth="3" class="slot-month">{{ slot.month }}</NText>
                  </div>
                  <div class="slot-weekday">
                    <NText depth="2">{{ slot.weekday }}</NText>
                  </div>
                  <div v-if="isSlotSelected(slot.iso)" class="selected-indicator">
                    <NIcon size="24" color="#18a058">
                      <CheckmarkCircleOutline />
                    </NIcon>
                  </div>
                </div>
              </NCard>
            </div>
          </div>

          <NEmpty
            v-else
            description="No available time slots for this car"
            class="empty-slots"
          />
        </NCard>

        <!-- Booking Summary and Confirm Section -->
        <NCard v-if="selectedSlots.length > 0" title="Booking Summary" class="summary-card" hoverable>
          <div class="summary-content">
            <div class="summary-row">
              <NText>Selected Dates:</NText>
              <NText strong>{{ selectedSlots.length }} day(s)</NText>
            </div>
            <div class="summary-row">
              <NText>Price per day:</NText>
              <NText strong>${{ post.price }}</NText>
            </div>
            <NDivider />
            <div class="summary-row total">
              <NText strong class="total-label">Total Price:</NText>
              <NTag type="success" size="large" strong>
                ${{ totalPrice }}
              </NTag>
            </div>

            <NButton
              type="primary"
              size="large"
              block
              strong
              class="confirm-button"
              @click="confirmBooking"
              :disabled="selectedSlots.length === 0"
            >
              <template #icon>
                <NIcon><CheckmarkOutline /></NIcon>
              </template>
              Confirm Booking
            </NButton>
          </div>
        </NCard>
      </div>

      <NEmpty
        v-else-if="!loading && !post"
        description="Post not found"
        class="empty-state"
      />
    </NSpin>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  NPageHeader,
  NCard,
  NSpin,
  NCarousel,
  NSpace,
  NTag,
  NText,
  NIcon,
  NButton,
  NEmpty,
  NDivider,
  useMessage
} from 'naive-ui'
import {
  CalendarOutline,
  FlameOutline,
  SettingsOutline,
  LocationOutline,
  CheckmarkCircleOutline,
  CheckmarkOutline
} from '@vicons/ionicons5'
import { useAuthStore } from '@/api/Auth'
import { carsApi } from '@/api/cars'

interface Post {
  postId: number
  ownerId: string
  carId: number
  title: string
  description?: string
  price: number
  specialRequirements?: string
  location: string
  availableTimeSlots: string[]
  postedAt: string
  available: boolean
}

interface Car {
  carId: number
  brand: string
  model: string
  version: string
  yearProduction: number
  fuelDisplay: string
  transmissionDisplay: string
  images: string[]
}

interface TimeSlot {
  iso: string
  day: string
  month: string
  weekday: string
  date: Date
}

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
const message = useMessage()

const baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5030'
const loading = ref(true)
const post = ref<Post | null>(null)
const car = ref<Car | null>(null)
const selectedSlots = ref<string[]>([])

// Process available time slots into a more readable format
const availableSlots = computed<TimeSlot[]>(() => {
  if (!post.value?.availableTimeSlots) return []

  return post.value.availableTimeSlots.map((slot) => {
    const date = new Date(slot)
    return {
      iso: slot,
      day: date.getDate().toString().padStart(2, '0'),
      month: date.toLocaleString('en-US', { month: 'short' }),
      weekday: date.toLocaleString('en-US', { weekday: 'short' }),
      date: date
    }
  }).sort((a, b) => a.date.getTime() - b.date.getTime())
})

// Calculate total price based on selected slots
const totalPrice = computed(() => {
  if (!post.value) return 0
  return (post.value.price * selectedSlots.value.length).toFixed(2)
})

// Check if a slot is selected
const isSlotSelected = (slotIso: string): boolean => {
  return selectedSlots.value.includes(slotIso)
}

// Toggle slot selection
const toggleSlot = (slotIso: string) => {
  const index = selectedSlots.value.indexOf(slotIso)
  if (index > -1) {
    selectedSlots.value.splice(index, 1)
  } else {
    selectedSlots.value.push(slotIso)
  }
}

// Confirm booking
const confirmBooking = async () => {
  if (selectedSlots.value.length === 0) {
    message.warning('Please select at least one date')
    return
  }

  if (!post.value || !authStore.user?.userId) {
    message.error('Missing required information')
    return
  }

  try {
    loading.value = true

    // Convert selected slot ISO strings to Date objects (preserve the date without timezone conversion)
    const requestedDates = selectedSlots.value.map(slot => {
      const date = new Date(slot)
      // Extract just the date part in YYYY-MM-DD format at midnight UTC
      const year = date.getFullYear()
      const month = String(date.getMonth() + 1).padStart(2, '0')
      const day = String(date.getDate()).padStart(2, '0')
      return `${year}-${month}-${day}T00:00:00.000Z`
    })

    const requestData = {
      userId: authStore.user.userId,
      postId: post.value.postId,
      requestedDates: requestedDates
    }

    console.log('=== BOOKING REQUEST DEBUG ===')
    console.log('Selected slots:', selectedSlots.value)
    console.log('Requested dates:', requestedDates)
    console.log('Request data:', requestData)
    console.log('Post available time slots:', post.value.availableTimeSlots)

    const response = await fetch(`${baseUrl}/api/requests`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      credentials: 'include',
      body: JSON.stringify(requestData)
    })

    console.log('Response status:', response.status, response.statusText)

    if (!response.ok) {
      // Read the response body as text first
      const responseText = await response.text()
      let errorMessage = 'Failed to create booking request'

      try {
        // Try to parse as JSON
        const errorData = JSON.parse(responseText)
        errorMessage = errorData.message || errorData || responseText || errorMessage
      } catch {
        // If not JSON, use the text directly
        errorMessage = responseText || errorMessage
      }

      throw new Error(errorMessage)
    }

    await response.json()

    message.success(
      `Booking request submitted successfully! Total: $${totalPrice.value}`,
      {
        duration: 5000
      }
    )

    // Redirect to user's bookings page after a short delay
    setTimeout(() => {
      router.push('/home')
    }, 2000)
  } catch (error: any) {
    console.error('Error creating booking:', error)
    message.error(error.message || 'Failed to create booking request')
  } finally {
    loading.value = false
  }
}

// Fetch post and car data
const fetchPostData = async () => {
  try {
    loading.value = true
    const postId = route.params.id

    // Fetch post data
    const postResponse = await fetch(`${baseUrl}/api/Posts/${postId}`, {
      credentials: 'include'
    })

    if (!postResponse.ok) {
      throw new Error('Failed to fetch post')
    }

    post.value = await postResponse.json()

    // Fetch car data
    if (post.value?.carId) {
      const carResponse = await fetch(
        `${baseUrl}/api/Cars/${post.value.carId}`,
        {
          credentials: 'include'
        }
      )

      if (carResponse.ok) {
        const carData = await carResponse.json()
        // Process car images to get full URLs
        const imageUrls = carsApi.getCarImageUrls(carData.carImages || [])
        car.value = {
          ...carData,
          images: imageUrls
        }
      }
    }
  } catch (error) {
    console.error('Error fetching data:', error)
    message.error('Failed to load booking information')
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchPostData()
})
</script>

<style scoped>
.booking-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.page-header {
  margin-bottom: 24px;
  background: white;
  padding: 16px;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

.booking-content {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

/* Info Card Styles */
.info-card {
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
}

.car-info-section {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 32px;
}

.car-images {
  width: 100%;
  border-radius: 8px;
  height: 400px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f5f5f5;
}

.carousel-image {
  width: 100%;
  max-height: 400px;
  object-fit: contain;
  border-radius: 8px;
}

.car-details {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.car-title {
  font-size: 1.8rem;
  margin: 0;
  color: #333;
}

.car-specs {
  display: flex;
  flex-wrap: wrap;
}

.post-details {
  display: flex;
  flex-direction: column;
  gap: 16px;
  padding-top: 16px;
  border-top: 1px solid #e8e8e8;
}

.post-title {
  font-size: 1.4rem;
  margin: 0;
  color: #18a058;
}

.post-description {
  line-height: 1.6;
  color: #666;
}

.location-section,
.requirements-section {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.price-section {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-top: 8px;
}

.price-label {
  font-size: 1.1rem;
}

.price-tag {
  font-size: 1.2rem !important;
  padding: 8px 16px;
}

/* Time Slots Styles */
.booking-card {
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
}

.time-slots-container {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.selection-hint {
  text-align: center;
  padding: 12px;
  background: #f6f9ff;
  border-radius: 6px;
}

.time-slots-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(140px, 1fr));
  gap: 16px;
}

.time-slot-card {
  cursor: pointer;
  transition: all 0.3s ease;
  border: 2px solid transparent;
  position: relative;
}

.time-slot-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.12);
}

.time-slot-card.selected {
  border-color: #18a058;
  background: #f6ffed;
  box-shadow: 0 4px 16px rgba(24, 160, 88, 0.2);
}

.slot-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  padding: 8px;
  position: relative;
}

.slot-date {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 4px;
}

.slot-date .n-text:first-child {
  font-size: 2rem;
  line-height: 1;
}

.slot-month {
  text-transform: uppercase;
  font-size: 0.85rem;
}

.slot-weekday {
  text-transform: uppercase;
  font-size: 0.75rem;
  letter-spacing: 0.5px;
}

.selected-indicator {
  position: absolute;
  top: 8px;
  right: 8px;
}

/* Summary Card Styles */
.summary-card {
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
  position: sticky;
  top: 20px;
}

.summary-content {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.summary-row.total {
  font-size: 1.2rem;
  padding-top: 8px;
}

.total-label {
  font-size: 1.2rem;
}

.confirm-button {
  margin-top: 16px;
  height: 48px;
  font-size: 1.1rem;
}

.empty-slots,
.empty-state {
  padding: 60px 20px;
}

/* Responsive Design */
@media (max-width: 968px) {
  .car-info-section {
    grid-template-columns: 1fr;
    gap: 24px;
  }

  .carousel-image {
    height: 300px;
  }

  .time-slots-grid {
    grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
    gap: 12px;
  }
}

@media (max-width: 640px) {
  .booking-container {
    padding: 12px;
  }

  .car-title {
    font-size: 1.5rem;
  }

  .post-title {
    font-size: 1.2rem;
  }

  .time-slots-grid {
    grid-template-columns: repeat(auto-fill, minmax(100px, 1fr));
    gap: 10px;
  }

  .carousel-image {
    height: 250px;
  }
}
</style>
