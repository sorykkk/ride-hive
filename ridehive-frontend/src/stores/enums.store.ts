//This is used for state handling of enums
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { EnumCollections } from '@/api/types'
import { enumsApi } from '@/api'

export const useEnumStore = defineStore('enums', () => {
  // State
  const enums = ref<EnumCollections>({
    fuelTypes: [],
    driveTypes: [],
    transmissionTypes: [],
    bodyTypes: [],
    conditionTypes: []
  })
  
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Getters (computed)
  const fuelOptions = computed(() => enums.value.fuelTypes)
  const driveOptions = computed(() => enums.value.driveTypes)
  const transmissionOptions = computed(() => enums.value.transmissionTypes)
  const bodyOptions = computed(() => enums.value.bodyTypes)
  const conditionOptions = computed(() => enums.value.conditionTypes)

  // Actions
  const fetchEnums = async () => {
    if (loading.value) return // Prevent duplicate calls
    
    loading.value = true
    error.value = null
    
    try {
      // Use the new API instead of direct fetch
      const data = await enumsApi.getAllEnums()
      
      enums.value = {
        fuelTypes: data.fuelTypes || [],
        driveTypes: data.driveTypes || [],
        transmissionTypes: data.transmissionTypes || [],
        bodyTypes: data.bodyTypes || [],
        conditionTypes: data.conditionTypes || []
      }
    } catch (err: any) {
      error.value = err.message || 'Unknown error occurred'
      console.error('Error fetching enums:', err)
    } finally {
      loading.value = false
    }
  }

  // Initialize enums on store creation if not already loaded
  const initializeEnums = async () => {
    if (enums.value.fuelTypes.length === 0) {
      await fetchEnums()
    }
  }

  return {
    // State
    enums,
    loading,
    error,
    
    // Getters
    fuelOptions,
    driveOptions,
    transmissionOptions,
    bodyOptions,
    conditionOptions,
    
    // Actions
    fetchEnums,
    initializeEnums
  }
})