<template>
  <div class="auth-page">
    <div class="auth-card">
      <!-- Logo -->
      <div class="auth-logo">
        <img src="@/assets/logo/logo-1.png" alt="RideHive" />
        <h1>RideHive</h1>
      </div>

      <h2>Welcome Back</h2>
      <p class="subtitle">Sign in to your account</p>

      <!-- Login Form -->
      <form @submit.prevent="handleLogin" class="auth-form">
        <div class="form-group">
          <label>Email</label>
          <input
            v-model="form.email"
            type="email"
            placeholder="your.email@example.com"
            required
            :disabled="authStore.isLoading"
          />
        </div>

        <div class="form-group">
          <label>Password</label>
          <input
            v-model="form.password"
            type="password"
            placeholder="••••••••"
            required
            :disabled="authStore.isLoading"
          />
        </div>

        <div v-if="authStore.error" class="error-message">
          {{ authStore.error }}
        </div>

        <button type="submit" class="btn-primary" :disabled="authStore.isLoading">
          {{ authStore.isLoading ? 'Logging in...' : 'Login' }}
        </button>
      </form>

      <div class="auth-footer">
        <p>Don't have an account?
          <router-link to="/register" class="link-primary">Register</router-link>
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../api/Auth'

const router = useRouter()
const authStore = useAuthStore()

const form = ref({
  email: '',
  password: ''
})

const handleLogin = async () => {
  try {
    const user = await authStore.login(form.value)
    router.push('/')
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
  background: white;
  border-radius: 12px;
  padding: 2.5rem;
  width: 100%;
  max-width: 400px;
  border: 1px solid #e5e8ef;
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

h2 {
  text-align: center;
  color: #333;
  font-size: 1.25rem;
  margin-bottom: 0.25rem;
}

.subtitle {
  text-align: center;
  color: #666;
  margin-bottom: 1.5rem;
}

.auth-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

label {
  display: block;
  font-weight: 500;
  margin-bottom: 0.25rem;
  color: #444;
}

input {
  width: 100%;
  padding: 0.75rem;
  border: 1.5px solid #d0d7e2;
  border-radius: 8px;
  font-size: 1rem;
  transition: border-color 0.2s;
}

input:focus {
  outline: none;
  border-color: #004d96;
}

.error-message {
  background: #fee;
  color: #b22;
  border-radius: 8px;
  padding: 0.75rem;
  text-align: center;
  font-size: 0.9rem;
}

.btn-primary {
  width: 100%;
  padding: 0.9rem;
  background: #004d96;
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s;
}

.btn-primary:hover:not(:disabled) {
  background: #0364be;
}

.btn-primary:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.auth-footer {
  text-align: center;
  margin-top: 1.5rem;
  font-size: 0.95rem;
  color: #000;
}

.link-primary {
  color: #004d96;
  text-decoration: none;
  font-weight: 600;
}

.link-primary:hover {
  text-decoration: underline;
}
</style>
