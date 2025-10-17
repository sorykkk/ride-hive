<template>
  <div class="auth-page">
    <div class="auth-container">
      <div class="auth-card">
        <!-- Logo -->
        <div class="auth-logo">
          <img src="@/assets/logo/logo-1.png" alt="RideHive" />
          <h1>RideHive</h1>
        </div>

        <h2>Create Account</h2>
        <p class="subtitle">Join RideHive today</p>

        <!-- Register Form -->
        <form @submit.prevent="handleRegister" class="auth-form">
          <!-- Name & Surname -->
          <div class="form-row">
            <div class="form-group">
              <label for="name">First Name *</label>
              <input
                id="name"
                v-model="form.name"
                type="text"
                placeholder="John"
                required
                :disabled="authStore.isLoading"
              />
            </div>

            <div class="form-group">
              <label for="surname">Last Name *</label>
              <input
                id="surname"
                v-model="form.surname"
                type="text"
                placeholder="Doe"
                required
                :disabled="authStore.isLoading"
              />
            </div>
          </div>

          <!-- Email -->
          <div class="form-group">
            <label for="email">Email *</label>
            <input
              id="email"
              v-model="form.email"
              type="email"
              placeholder="john.doe@example.com"
              required
              :disabled="authStore.isLoading"
            />
          </div>

          <!-- Password -->
          <div class="form-group">
            <label for="password">Password *</label>
            <input
              id="password"
              v-model="form.password"
              type="password"
              placeholder="Min 8 characters"
              required
              minlength="8"
              :disabled="authStore.isLoading"
            />
            <small>8+ chars, uppercase, lowercase, digit, special char</small>
          </div>

          <!-- Role Selection -->
          <div class="form-group">
            <label>I want to register as: *</label>
            <div class="role-selection">
              <label class="role-option">
                <input
                  type="radio"
                  v-model="form.role"
                  value="Client"
                  required
                  :disabled="authStore.isLoading"
                />
                <div class="role-card">
                  <span class="role-icon">🚗</span>
                  <h4>Client</h4>
                  <p>Rent cars</p>
                </div>
              </label>

              <label class="role-option">
                <input
                  type="radio"
                  v-model="form.role"
                  value="Owner"
                  required
                  :disabled="authStore.isLoading"
                />
                <div class="role-card">
                  <span class="role-icon">🏢</span>
                  <h4>Owner</h4>
                  <p>List cars for rent</p>
                </div>
              </label>
            </div>
          </div>

          <!-- Optional fields -->
          <div class="form-row">
            <div class="form-group">
              <label for="age">Age</label>
              <input
                id="age"
                v-model.number="form.age"
                type="number"
                placeholder="25"
                min="18"
                :disabled="authStore.isLoading"
              />
            </div>

            <div class="form-group">
              <label for="phone">Phone</label>
              <input
                id="phone"
                v-model.number="form.phone"
                type="tel"
                placeholder="0712345678"
                :disabled="authStore.isLoading"
              />
            </div>
          </div>

          <!-- Driving License (Client only) -->
          <div v-if="form.role === 'Client'" class="form-group">
            <label for="license">Driving License (optional)</label>
            <input
              id="license"
              type="file"
              accept="image/jpeg,image/jpg,image/png"
              @change="handleFileUpload"
              :disabled="authStore.isLoading"
            />
            <small>JPG, JPEG, PNG (max 5MB)</small>
          </div>

          <!-- Error Message -->
          <div v-if="authStore.error" class="error-message">
            {{ authStore.error }}
          </div>

          <!-- Success Message -->
          <div v-if="successMessage" class="success-message">
            {{ successMessage }}
          </div>

          <!-- Submit Button -->
          <button 
            type="submit" 
            class="btn-primary"
            :disabled="authStore.isLoading"
          >
            {{ authStore.isLoading ? 'Creating account...' : 'Create Account' }}
          </button>
        </form>

        <!-- Login Link -->
        <div class="auth-footer">
          <p>Already have an account?</p>
          <router-link to="/" class="link-primary">
            Login here
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/api/Auth'

const router = useRouter()
const authStore = useAuthStore()

const form = ref({
  name: '',
  surname: '',
  email: '',
  password: '',
  role: 'Client' as 'Client' | 'Owner',
  age: undefined as number | undefined,
  phone: undefined as number | undefined,
  drivingLicenseImage: undefined as File | undefined
})

const successMessage = ref('')

const handleFileUpload = (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  
  if (file) {
    if (file.size > 5 * 1024 * 1024) {
      authStore.error = 'Image size cannot exceed 5MB'
      target.value = ''
      return
    }
    form.value.drivingLicenseImage = file
  }
}

const handleRegister = async () => {
  successMessage.value = ''
  
  try {
    await authStore.register(form.value)
    
    successMessage.value = 'Account created successfully! Redirecting to login...'
    
    // Redirect to login after 2 seconds
    setTimeout(() => {
      router.push('/login')
    }, 2000)
  } catch (error) {
    console.error('Registration failed:', error)
  }
}
</script>
<style scoped>
.auth-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f7f9fc; /* simplu, fără gradient */
  padding: 2rem 1rem;
}

.auth-container {
  width: 100%;
  max-width: 480px;
}

.auth-card {
  background: #fff;
  border: 1px solid #ddd;
  border-radius: 12px;
  padding: 2.5rem 2rem;
}

.auth-logo {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 2rem;
}

.auth-logo img {
  width: 72px;
  height: 72px;
  margin-bottom: 0.75rem;
}

.auth-logo h1 {
  color: #004d96;
  font-size: 1.6rem;
  font-weight: 700;
  margin: 0;
}

h2 {
  color: #222;
  font-size: 1.3rem;
  text-align: center;
  margin-bottom: 0.25rem;
}

.subtitle {
  text-align: center;
  color: #666;
  margin-bottom: 1.75rem;
  font-size: 0.9rem;
}

.auth-form {
  margin-bottom: 1.5rem;
}

.form-group {
  margin-bottom: 1rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.75rem;
}

label {
  display: block;
  color: #333;
  font-weight: 500;
  margin-bottom: 0.4rem;
  font-size: 0.9rem;
}

input[type="text"],
input[type="email"],
input[type="password"],
input[type="number"],
input[type="tel"],
input[type="file"] {
  width: 100%;
  padding: 0.6rem 0.75rem;
  border: 1px solid #ccc;
  border-radius: 6px;
  font-size: 0.95rem;
  transition: border-color 0.2s ease;
}

input:focus {
  outline: none;
  border-color: #004d96;
}

input:disabled {
  background: #f0f0f0;
  cursor: not-allowed;
}

small {
  display: block;
  margin-top: 0.25rem;
  color: #888;
  font-size: 0.8rem;
}

.role-selection {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.75rem;
  margin-top: 0.5rem;
}

.role-option {
  cursor: pointer;
}

.role-option input[type="radio"] {
  display: none;
}

.role-card {
  border: 1px solid #ccc;
  border-radius: 8px;
  padding: 1rem;
  text-align: center;
  transition: border-color 0.2s ease, background 0.2s ease;
}

.role-option input[type="radio"]:checked + .role-card {
  border-color: #004d96;
  background: #f2f8ff;
}

.role-icon {
  font-size: 2rem;
  margin-bottom: 0.3rem;
}

.role-card h4 {
  margin: 0;
  font-size: 1rem;
  color: #222;
}

.role-card p {
  margin: 0.25rem 0 0 0;
  color: #666;
  font-size: 0.85rem;
}

.error-message {
  background: #fdeaea;
  color: #b22;
  padding: 0.7rem;
  border-radius: 6px;
  text-align: center;
  font-size: 0.9rem;
  margin-bottom: 1rem;
}

.success-message {
  background: #e9fbe9;
  color: #257a25;
  padding: 0.7rem;
  border-radius: 6px;
  text-align: center;
  font-size: 0.9rem;
  margin-bottom: 1rem;
}

.btn-primary {
  width: 100%;
  padding: 0.8rem;
  background: #004d96;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s ease;
}

.btn-primary:hover:not(:disabled) {
  background: #063c74;
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.auth-footer {
  text-align: center;
  padding-top: 1.25rem;
  border-top: 1px solid #e0e0e0;
  font-size: 0.9rem;
  color: #000; /* 🔥 negru clar pe alb */
}

.auth-footer p {
  margin: 0 0 0.25rem 0;
}

.link-primary {
  color: #004d96;
  text-decoration: none;
  font-weight: 600;
}

.link-primary:hover {
  text-decoration: underline;
}

@media (max-width: 640px) {
  .form-row,
  .role-selection {
    grid-template-columns: 1fr;
  }

  .auth-card {
    padding: 2rem 1.25rem;
  }
}
</style>
