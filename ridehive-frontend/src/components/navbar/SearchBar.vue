<script setup lang="ts">
import { ref } from 'vue'
import { NInput, NIcon, NButton } from 'naive-ui'
import { SearchOutline } from '@vicons/ionicons5'

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
      placeholder="Search rides, locations..."
      clearable
      :class="{ 'search-focused': isFocused }"
      class="search-input"
      @focus="handleFocus"
      @blur="handleBlur"
      @keypress="handleKeyPress"
      @clear="handleClear"
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
            <NIcon><SearchOutline /></NIcon>
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

.search-input :deep(.n-input__input-el) {
  font-size: 14px;
  padding: 0;
  text-align: left;
}

.search-input :deep(.n-input__input-el)::placeholder {
  text-align: left;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.search-focused :deep(.n-input__input-el)::placeholder,
.search-input:has(input:not(:placeholder-shown)) :deep(.n-input__input-el)::placeholder {
  transform: translateX(-20px);
}

.search-input :deep(.n-input__border) {
  border-radius: 24px;
  border: 2px solid #e0e0e0;
  transition: all 0.3s ease;
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
  margin-left: 4px;
  margin-right: 0;
}

.search-focused .search-icon {
  color: #054682;
}

.search-button {
  margin-right: 4px;
}

.search-input :deep(.n-input__suffix) {
  margin-right: 8px;
}

.search-input :deep(.n-input__prefix) {
  margin-left: 8px;
  transition: all 2.5s cubic-bezier(0.4, 0, 0.2, 1);
  overflow: hidden;
}

/* When focused or typing, reduce prefix space since icon is hidden */
.search-focused :deep(.n-input__prefix) {
  margin-left: 0px;
}

.search-focused :deep(.n-input__border) {
  border-color: #054682 !important;
  box-shadow: none !important;
}

.search-input:hover :deep(.n-input__border) {
  border-color: #1e86db !important;
  box-shadow: none !important;
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

/* Remove all default Naive UI focus/hover indicators */
/* applies to all components which has .search-input class
    :deep -> style applies and for components outsided of the current scope
        but that are also child of .search-input
    (.n-input) -> applies to .n-input child of .search-input
*/
/* __state-border is according BEM (Block Element Modifier) naming convention
    so the block is .n-input, the element that is part of the block is separated by double _, __state-border
    the modifier, would have double hyphens: --active */
.search-input :deep(.n-input__state-border) {
  display: none !important;
}
</style>