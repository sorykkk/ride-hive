import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { apiClient } from './base'

import type {UserLoginDto, UserAuthResponseDto, UserRegisterDto } from './types';

export const useAuthStore = defineStore('auth', () => {
  // State
  const user = ref<UserAuthResponseDto | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const isAuthenticated = computed(() => !!user.value)
  const userRole = computed(() => user.value?.role || null)
  const isOwner = computed(() => user.value?.role === 'Owner')
  const isClient = computed(() => user.value?.role === 'Client')

  // Actions
  const login = async (credentials: UserLoginDto) => {
    isLoading.value = true
    error.value = null

    try {
      const response = await apiClient.post<UserAuthResponseDto>('/api/user/login', credentials)
      user.value = response
      
      // Salvează în localStorage
      localStorage.setItem('user', JSON.stringify(response))
      
      return response
    } catch (err: any) {
      error.value = err.data?.message || 'Login failed'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const register = async (data: UserRegisterDto) => {
    isLoading.value = true
    error.value = null

    try {
      const formData = new FormData()
      formData.append('name', data.name)
      formData.append('surname', data.surname)
      formData.append('email', data.email)
      formData.append('password', data.password)
      formData.append('role', data.role)
      
      if (data.age) formData.append('age', data.age.toString())
      if (data.phone) formData.append('phone', data.phone.toString())
      if (data.drivingLicenseImage) {
        formData.append('drivingLicenseImage', data.drivingLicenseImage)
      }

      const response = await apiClient.postFormData<{ message: string }>('/api/user/register', formData)
      return response
    } catch (err: any) {
      error.value = err.data?.message || err.data?.errors?.join(', ') || 'Registration failed'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const logout = async () => {
    try {
      await apiClient.post('/api/user/logout', {})
    } catch (err) {
      console.error('Logout error:', err)
    } finally {
      user.value = null
      localStorage.removeItem('user')
    }
  }

  const checkAuth = () => {
    const storedUser = localStorage.getItem('user')
    if (storedUser) {
      try {
        user.value = JSON.parse(storedUser)
      } catch {
        localStorage.removeItem('user')
      }
    }
  }

  return {
    user,
    isLoading,
    error,
    isAuthenticated,
    userRole,
    isOwner,
    isClient,
    login,
    register,
    logout,
    checkAuth
  }
})