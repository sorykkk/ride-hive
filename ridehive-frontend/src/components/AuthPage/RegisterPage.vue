<template>
  <div class="auth-page">
    <n-card class="auth-card" :bordered="true">
      <!-- Logo -->
      <div class="auth-logo">
        <img src="@/assets/logo/logo-1.png" alt="RideHive" />
        <h1>RideHive</h1>
      </div>

      <n-h2 class="text-center">Create Account</n-h2>
      <n-text depth="3" class="subtitle">Join RideHive today</n-text>

      <!-- Register Form -->
      <n-form
        ref="formRef"
        :model="form"
        :rules="rules"
        size="large"
        label-placement="top"
        require-mark-placement="right-hanging"
      >
        <!-- Name & Surname -->
        <n-grid :cols="2" :x-gap="12">
          <n-gi>
            <n-form-item label="First Name" path="name">
              <n-input
                v-model:value="form.name"
                placeholder="John"
                :disabled="authStore.isLoading"
              />
            </n-form-item>
          </n-gi>
          <n-gi>
            <n-form-item label="Last Name" path="surname">
              <n-input
                v-model:value="form.surname"
                placeholder="Doe"
                :disabled="authStore.isLoading"
              />
            </n-form-item>
          </n-gi>
        </n-grid>

        <!-- Email -->
        <n-form-item label="Email" path="email">
          <n-input
            v-model:value="form.email"
            type="text"
            placeholder="john.doe@example.com"
            :disabled="authStore.isLoading"
          />
        </n-form-item>

        <!-- Password -->
        <n-form-item label="Password" path="password">
          <n-input
            v-model:value="form.password"
            type="password"
            show-password-on="click"
            placeholder="Min 8 characters"
            :disabled="authStore.isLoading"
          />
          <template #feedback>
            <n-text depth="3" style="font-size: 12px;">
              8+ chars, uppercase, lowercase, digit, special char
            </n-text>
          </template>
        </n-form-item>

        <!-- Role Selection -->
        <n-form-item label="I want to register as:" path="role">
          <n-radio-group v-model:value="form.role" class="role-selection">
            <n-space vertical>
              <n-radio value="Client">
                <div class="role-option">
                  <span class="role-icon">🚗</span>
                  <div>
                    <n-text strong>Client</n-text>
                    <n-text depth="3" style="display: block; font-size: 13px;">
                      Rent cars
                    </n-text>
                  </div>
                </div>
              </n-radio>
              <n-radio value="Owner">
                <div class="role-option">
                  <span class="role-icon">🏢</span>
                  <div>
                    <n-text strong>Owner</n-text>
                    <n-text depth="3" style="display: block; font-size: 13px;">
                      List cars for rent
                    </n-text>
                  </div>
                </div>
              </n-radio>
            </n-space>
          </n-radio-group>
        </n-form-item>

        <!-- Optional fields -->
        <n-grid :cols="2" :x-gap="12">
          <n-gi>
            <n-form-item label="Age (optional)" path="age">
              <n-input-number
                v-model:value="form.age"
                placeholder="25"
                :min="18"
                :disabled="authStore.isLoading"
                style="width: 100%;"
              />
            </n-form-item>
          </n-gi>
          <n-gi>
            <n-form-item label="Phone (optional)" path="phone">
              <n-input-number
                v-model:value="form.phone"
                placeholder="0712345678"
                :show-button="false"
                :disabled="authStore.isLoading"
                style="width: 100%;"
              />
            </n-form-item>
          </n-gi>
        </n-grid>

        <!-- Driving License (Client only - Required) -->
        <n-form-item 
          v-if="form.role === 'Client'" 
          label="Driving License"
          path="drivingLicenseImage"
        >
          <n-upload
            :max="1"
            accept="image/jpeg,image/jpg,image/png"
            :custom-request="handleFileUpload"
            :disabled="authStore.isLoading"
            @before-upload="beforeUpload"
          >
            <n-button :disabled="authStore.isLoading">
              {{ form.drivingLicenseImage ? 'Change License' : 'Upload License' }}
            </n-button>
          </n-upload>
          <template #feedback>
            <n-text depth="3" style="font-size: 12px;">
              Required for clients - JPG, JPEG, PNG (max 5MB)
            </n-text>
          </template>
        </n-form-item>

        <!-- Error Message -->
        <n-alert v-if="authStore.error" type="error" :show-icon="true" class="mb-4">
          {{ authStore.error }}
        </n-alert>

        <!-- Success Message -->
        <n-alert v-if="successMessage" type="success" :show-icon="true" class="mb-4">
          {{ successMessage }}
        </n-alert>

        <!-- Submit Button -->
        <n-button
          type="primary"
          block
          size="large"
          :loading="authStore.isLoading"
          @click="handleRegister"
        >
          {{ authStore.isLoading ? 'Creating account...' : 'Create Account' }}
        </n-button>
      </n-form>

      <n-divider />

      <!-- Login Link -->
      <div class="auth-footer">
        <n-text>Already have an account?</n-text>
        <n-button text type="primary" @click="router.push('/')">
          Login here
        </n-button>
      </div>
    </n-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/api/Auth'
import {
  NCard,
  NH2,
  NText,
  NForm,
  NFormItem,
  NInput,
  NInputNumber,
  NButton,
  NAlert,
  NDivider,
  NRadioGroup,
  NRadio,
  NSpace,
  NGrid,
  NGi,
  NUpload,
  useMessage,
  type FormInst,
  type FormRules,
  type UploadCustomRequestOptions
} from 'naive-ui'

const router = useRouter()
const authStore = useAuthStore()
const message = useMessage()
const formRef = ref<FormInst | null>(null)

// Clear any previous errors when component mounts
onMounted(() => {
  authStore.error = null
})

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

const rules: FormRules = {
  name: [
    { required: true, message: 'First name is required', trigger: 'blur' }
  ],
  surname: [
    { required: true, message: 'Last name is required', trigger: 'blur' }
  ],
  email: [
    { required: true, message: 'Email is required', trigger: 'blur' },
    { type: 'email', message: 'Invalid email format', trigger: 'blur' }
  ],
  password: [
    { required: true, message: 'Password is required', trigger: 'blur' },
    { min: 8, message: 'Password must be at least 8 characters', trigger: 'blur' }
  ],
  role: [
    { required: true, message: 'Please select a role', trigger: 'change' }
  ],
  drivingLicenseImage: [
    { 
      required: true, 
      message: 'Driving license is required for client registration', 
      trigger: 'change',
      validator: (_rule: any, value: any) => {
        // Only validate if user selected Client role
        if (form.value.role === 'Client') {
          return !!value;
        }
        return true;
      }
    }
  ]
}

const beforeUpload = (options: { file: { file: File | null } }) => {
  const file = options.file.file
  if (file && file.size > 5 * 1024 * 1024) {
    message.error('Image size cannot exceed 5MB')
    return false
  }
  return true
}

const handleFileUpload = (options: UploadCustomRequestOptions) => {
  const file = options.file.file
  if (file) {
    form.value.drivingLicenseImage = file as File
    options.onFinish()
  }
}

const handleRegister = async () => {
  try {
    await formRef.value?.validate()
    successMessage.value = ''
    
    // Additional check for driving license if client role
    if (form.value.role === 'Client' && !form.value.drivingLicenseImage) {
      message.error('Please upload your driving license image')
      return
    }
    
    await authStore.register(form.value)
    
    successMessage.value = 'Account created successfully! Redirecting to login...'
    
    setTimeout(() => {
      router.push('/')
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
  background: #f7f9fc;
  padding: 2rem 1rem;
}

.auth-card {
  width: 100%;
  max-width: 500px;
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

.text-center {
  text-align: center;
  margin-bottom: 0.25rem;
}

.subtitle {
  display: block;
  text-align: center;
  margin-bottom: 1.75rem;
}

.role-selection {
  width: 100%;
}

.role-option {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.role-icon {
  font-size: 1.5rem;
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

@media (max-width: 640px) {
  .auth-card {
    max-width: 100%;
  }
}
</style>