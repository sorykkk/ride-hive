<!-- mock implementation -->
<script setup lang="ts">
import SearchBar from './navbar/SearchBar.vue'

// Handle search events
const handleSearch = (query: string) => {
  console.log('Search submitted:', query)
  // TODO: Implement search logic - navigate to search results or call search API
}

const handleSearchInput = (query: string) => {
  console.log('Search input changed:', query)
  // TODO: Implement live search suggestions or autocomplete
}
</script>

<template>
  <div class="container">
    <!-- Search Bar at the top -->
    <div class="search-section">
      <SearchBar @search="handleSearch" @input="handleSearchInput" />
    </div>

    <h1 class="welcome-text">Welcome @user.name!</h1>

    <div class="card">
      <p>Here is mock implementation</p>
    </div>
  </div>
</template>

<style scoped>
.container {
  display: flex;
  flex-direction: column;    /* stack vertically */
  justify-content: flex-start; /* start from top instead of center */
  align-items: center;       /* horizontal centering */
  min-height: 100vh;         /* full viewport height */
  /* text-align: center; */
  color: black;              /* make all text black */
  padding: 20px 20px;        /* add some padding */
}

.search-section {
  width: 100%;
  max-width: 600px;          /* limit max width */
  margin-bottom: 40px;       /* space below search bar */
  display: flex;
  justify-content: center;
}
.search-section:deep(.container) {
  text-align:left;
}

.card {
  margin-top: 20px;
}

.welcome-text {
  text-align: center;
  width: 100%;
}
</style>
