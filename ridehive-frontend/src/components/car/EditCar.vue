<script setup lang="ts">
import { useRouter } from 'vue-router';
import { NCard, NButton } from 'naive-ui';

const router = useRouter();

const handleCancel = () => {
  router.push('/profile');
};
</script>

<template>
  <div class="edit-car-container">
    <NCard title="Edit Car">
      <p>Edit car functionality will be available when the API server is running.</p>
      <NButton @click="handleCancel" type="primary">
        Back to Profile
      </NButton>
    </NCard>
  </div>
</template>

<style scoped>
.edit-car-container {
  max-width: 800px;
  margin: 0 auto;
  padding: 2rem;
}
</style>