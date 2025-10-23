import { apiClient } from './base'

// Notification interface matching backend NotificationResponseDto
export interface Notification {
  notificationId: number
  userId: string
  type: 'booking_request' | 'booking_accepted' | 'booking_rejected'
  requestId: number
  postId: number
  title: string
  message: string
  isRead: boolean
  createdAt: string
  userName?: string
  userAvatar?: string
  carName?: string
  requestedDates: string[]
}

// Get all notifications for a user
export const getNotifications = async (userId: string): Promise<Notification[]> => {
  try {
    const response = await apiClient.get<Notification[]>(`/api/notifications?userId=${userId}`)
    return response
  } catch (error) {
    console.error('Error fetching notifications:', error)
    throw error
  }
}

// Get a specific notification by ID
export const getNotification = async (notificationId: number): Promise<Notification> => {
  try {
    const response = await apiClient.get<Notification>(`/api/notifications/${notificationId}`)
    return response
  } catch (error) {
    console.error('Error fetching notification:', error)
    throw error
  }
}

// Mark a notification as read
export const markAsRead = async (notificationId: number): Promise<void> => {
  try {
    await apiClient.put(`/api/notifications/${notificationId}/read`, {})
  } catch (error) {
    console.error('Error marking notification as read:', error)
    throw error
  }
}

// Mark all notifications as read for a user
export const markAllAsRead = async (userId: string): Promise<void> => {
  try {
    await apiClient.put(`/api/notifications/read-all?userId=${userId}`, {})
  } catch (error) {
    console.error('Error marking all notifications as read:', error)
    throw error
  }
}

// Delete a notification
export const deleteNotification = async (notificationId: number): Promise<void> => {
  try {
    await apiClient.delete(`/api/notifications/${notificationId}`)
  } catch (error) {
    console.error('Error deleting notification:', error)
    throw error
  }
}

// Get unread notification count
export const getUnreadCount = async (userId: string): Promise<number> => {
  try {
    const response = await apiClient.get<{ count: number }>(`/api/notifications/unread-count?userId=${userId}`)
    return response.count
  } catch (error) {
    console.error('Error fetching unread count:', error)
    throw error
  }
}

// Accept a booking request (owner only)
export const acceptRequest = async (requestId: number): Promise<void> => {
  try {
    console.log(`Calling API: PUT /api/requests/${requestId}/accept`)
    await apiClient.put(`/api/requests/${requestId}/accept`, {})
    console.log(`Successfully accepted request ${requestId}`)
  } catch (error: any) {
    console.error('Error accepting request:', error)
    console.error('Error details:', error.response || error.message)
    throw error
  }
}

// Decline a booking request (owner only)
export const declineRequest = async (requestId: number): Promise<void> => {
  try {
    console.log(`Calling API: PUT /api/requests/${requestId}/decline`)
    await apiClient.put(`/api/requests/${requestId}/decline`, {})
    console.log(`Successfully declined request ${requestId}`)
  } catch (error: any) {
    console.error('Error declining request:', error)
    console.error('Error details:', error.response || error.message)
    throw error
  }
}
