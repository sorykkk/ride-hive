<template>
  <div class="auth-page">
    <n-card class="auth-card" :bordered="true">
      <!-- Logo -->
      <div class="auth-logo">
        <img src="@/assets/logo/logo-1.png" alt="RideHive" />
        <h1>RideHive</h1>
      </div>

      <n-h2 class="text-center">Welcome Back</n-h2>
      <n-text depth="3" class="subtitle">Sign in to your account</n-text>

      <!-- Login Form -->
      <n-form
        ref="formRef"
        :model="form"
        :rules="rules"
        size="large"
        label-placement="top"
        require-mark-placement="right-hanging"
        class="auth-form"
      >
        <n-form-item label="Email" path="email">
          <n-input
            v-model:value="form.email"
            type="text"
            placeholder="your.email@example.com"
            :disabled="authStore.isLoading"
            @keydown.enter="handleLogin"
          />
        </n-form-item>

        <n-form-item label="Password" path="password">
          <n-input
            v-model:value="form.password"
            type="password"
            show-password-on="click"
            placeholder="Enter your password"
            :disabled="authStore.isLoading"
            @keydown.enter="handleLogin"
          />
        </n-form-item>

        <n-alert v-if="authStore.error" type="error" :show-icon="true" class="mb-4">
          {{ authStore.error }}
        </n-alert>

        <n-button
          type="primary"
          block
          size="large"
          :loading="authStore.isLoading"
          @click="handleLogin"
        >
          {{ authStore.isLoading ? 'Logging in...' : 'Login' }}
        </n-button>
      </n-form>

      <n-divider />

      <div class="auth-footer">
        <n-text>Don't have an account?</n-text>
        <n-button text type="primary" @click="router.push('/register')">
          Register
        </n-button>
      </div>
    </n-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../api/Auth'
import { 
  NCard, 
  NH2, 
  NText, 
  NForm, 
  NFormItem, 
  NInput, 
  NButton, 
  NAlert,
  NDivider,
  type FormInst,
  type FormRules
} from 'naive-ui'

const router = useRouter()
const authStore = useAuthStore()
const formRef = ref<FormInst | null>(null)

// Clear any previous errors when component mounts
onMounted(() => {
  authStore.error = null
})

const form = ref({
  email: '',
  password: ''
})

const rules: FormRules = {
  email: [
    { required: true, message: 'Email is required', trigger: 'blur' },
    { type: 'email', message: 'Invalid email format', trigger: 'blur' }
  ],
  password: [
    { required: true, message: 'Password is required', trigger: 'blur' }
  ]
}

const handleLogin = async () => {
  try {
    await formRef.value?.validate()
    const user = await authStore.login(form.value)
    router.push('/home') 
  } catch (error) {
    console.error('Login failed:', error)
  }
}
</script>

<style scoped>
.auth-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f7f9fc;
  padding: 1.5rem;
}

.auth-card {
  width: 100%;
  max-width: 400px;
}

.auth-logo {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 1.5rem;
}

.auth-logo img {
  width: 64px;
  height: 64px;
  margin-bottom: 0.5rem;
}

.auth-logo h1 {
  font-size: 1.5rem;
  color: #003d80;
  margin: 0;
}

.text-center {
  text-align: center;
  margin-bottom: 0.25rem;
}

.subtitle {
  display: block;
  text-align: center;
  margin-bottom: 1.5rem;
}

.auth-form {
  margin-bottom: 0;
}

.auth-footer {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding-top: 0.5rem;
}

.mb-4 {
  margin-bottom: 1rem;
}
</style>