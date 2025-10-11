<script setup lang="ts">
// Global imports, router setup, etc.
import TopNavigationBar from './components/navbar/TopNavigationBar.vue'
import Footer from './components/Footer.vue'
import { ApiClient, ApiConfig } from './services/api'

</script>

<template>
  <div id="app">
    <TopNavigationBar />
    <!-- Main content area - now using router-view for dynamic pages -->
    <main>
      <router-view />
    </main>
    <Footer />
  </div>
</template>

<style>
/* Global reset for this app */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

#app {
  min-height: 100vh;
  width: 100%;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
}

main {
  flex: 1;
  padding: 20px;
  background-color: #f5f5f5;
}
</style>
