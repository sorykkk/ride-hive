<script setup lang="ts">
import UserInfo from './UserInfo.vue';
</script>

<template>
  <div class="profile-container">
    <UserInfo />
  </div>
</template>

<style scoped>
.profile-container {
  min-height: 100vh;
  background-color: #f8fafc;
  padding: 24px 0;
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .profile-container {
    padding: 16px 0;
  }
}
</style>
