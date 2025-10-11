<script setup lang="ts">

import { ref } from 'vue'
import { SearchOutline } from '@vicons/ionicons5'
import { 
  NInput, 
  NIcon, 
  NButton } from 'naive-ui'
import type { InputProps } from 'naive-ui'

type InputThemeOverride = NonNullable<InputProps['themeOverrides']>
const inputThemeOverrides: InputThemeOverride = {
  borderRadius: '24px',
  border: '2px solid #e0e0e0',
  borderHover: '2px solid #1e86db',
  borderFocus: '2px solid #004d96',
  boxShadowFocus: 'none',
  paddingMedium: '0 12px',
  heightMedium: '40px',
  fontSizeMedium: '14px',
  textColor: '#333',
  placeholderColor: '#999',
  caretColor: '#054682'
}

// Search state
const searchQuery = ref('')
const isFocused = ref(false)

// Emits for parent component communication
const emit = defineEmits<{
  search: [query: string]
  clear: []
}>()

// Handle search submission
const handleSearch = () => {
  if (searchQuery.value.trim()) {
    emit('search', searchQuery.value.trim())
    console.log('Searching for:', searchQuery.value)
  }
}

// Handle search clear
const handleClear = () => {
  searchQuery.value = ''
  emit('clear')
}

// Handle Enter key press
const handleKeyPress = (event: KeyboardEvent) => {
  if (event.key === 'Enter') {
    handleSearch()
  }
}

// Handle focus states
const handleFocus = () => {
  isFocused.value = true
}

const handleBlur = () => {
  isFocused.value = false
}
</script>

<template>
  <div class="search-container">
    <NInput
      v-model:value="searchQuery"
      size="medium"
      round
      placeholder="Search cars, locations..."
      clearable
      :class="{ 'search-focused': isFocused }"
      class="search-input"
      @focus="handleFocus"
      @blur="handleBlur"
      @keypress="handleKeyPress"
      @clear="handleClear"
      :theme-overrides="inputThemeOverrides"
    >
      <template #prefix>
        <NIcon 
          class="search-icon"
          :class="{ 'icon-hidden': isFocused || searchQuery }"
        >
          <SearchOutline />
        </NIcon>
      </template>
      
      <template #suffix>
        <NButton
          v-if="searchQuery"
          quaternary
          circle
          size="small"
          @click="handleSearch"
          class="search-button"
        >
          <template #icon>
            <NIcon>
              <SearchOutline />
            </NIcon>
          </template>
        </NButton>
      </template>
    </NInput>
  </div>
</template>

<style scoped>
.search-container {
  width: 100%;
  max-width: 460;
  min-width: 280px;
  position: relative;
}

.search-input {
  width: 100%;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.search-icon {
  color: #666;
  margin-right: 8px;
  opacity: 1;
  transform: translateX(0);
  width: 20px;
  overflow: hidden;
} 

.search-icon.icon-hidden {
  opacity: 0;
  transform: translateX(-100%);
  transition: 
    transform 0.5s cubic-bezier(0.4, 0, 0.2, 1),
    opacity 0.7s cubic-bezier(0.4, 0, 0.2, 1),
    width 0.5s cubic-bezier(0.4, 0, 0.2, 1),
    margin 0.5s cubic-bezier(0.4, 0, 0.2, 1);
  width: 0;
  margin-left: 8px;
  margin-right: 0;
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .search-container {
    max-width: 240px;
    min-width: 200px;
  }
}

@media (max-width: 480px) {
  .search-container {
    max-width: 180px;
    min-width: 160px;
  }
}

</style>