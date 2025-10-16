<script setup lang="ts">
import { ref } from 'vue';
import { NCard, NForm, NFormItem, NSwitch, NButton, NSpace, NPageHeader, NGrid, NGridItem, NSelect, NInput, NDivider, useMessage } from 'naive-ui';
import { useRouter } from 'vue-router';

const router = useRouter();
const message = useMessage();

// Settings state
const settings = ref({
  // Privacy Settings
  profileVisibility: true,
  showEmail: false,
  showPhone: false,
  
  // Notification Settings
  emailNotifications: true,
  pushNotifications: true,
  smsNotifications: false,
  marketingEmails: false,
  
  // App Settings
  language: 'en',
  theme: 'light',
  autoSave: true,
  
  // Security Settings
  twoFactorAuth: false,
});

const languageOptions = [
  { label: 'English', value: 'en' },
  { label: 'Spanish', value: 'es' },
  { label: 'French', value: 'fr' },
  { label: 'German', value: 'de' }
];

const themeOptions = [
  { label: 'Light', value: 'light' },
  { label: 'Dark', value: 'dark' },
  { label: 'Auto', value: 'auto' }
];

const saveSettings = async () => {
  try {
    // Here you would call your API to save the settings
    message.success('Settings saved successfully!');
  } catch (error) {
    message.error('Failed to save settings. Please try again.');
  }
};

const goBack = () => {
  router.back();
};

const resetSettings = () => {
  settings.value = {
    profileVisibility: true,
    showEmail: false,
    showPhone: false,
    emailNotifications: true,
    pushNotifications: true,
    smsNotifications: false,
    marketingEmails: false,
    language: 'en',
    theme: 'light',
    autoSave: true,
    twoFactorAuth: false,
  };
  message.info('Settings reset to defaults');
};
</script>

<template>
  <div class="settings-container">
    <NPageHeader @back="goBack">
      <template #title>Settings</template>
      <template #subtitle>Manage your application preferences</template>
    </NPageHeader>

    <div class="settings-content">
      <NGrid :cols="1" :y-gap="24">
        <!-- Privacy Settings -->
        <NGridItem>
          <NCard title="Privacy Settings">
            <NForm :model="settings" label-placement="left" label-width="180">
              <NFormItem label="Profile Visibility">
                <NSwitch v-model:value="settings.profileVisibility" />
                <template #feedback>
                  <span class="setting-description">Make your profile visible to other users</span>
                </template>
              </NFormItem>
              
              <NFormItem label="Show Email">
                <NSwitch v-model:value="settings.showEmail" />
                <template #feedback>
                  <span class="setting-description">Display your email address on your profile</span>
                </template>
              </NFormItem>
              
              <NFormItem label="Show Phone">
                <NSwitch v-model:value="settings.showPhone" />
                <template #feedback>
                  <span class="setting-description">Display your phone number on your profile</span>
                </template>
              </NFormItem>
            </NForm>
          </NCard>
        </NGridItem>

        <!-- Notification Settings -->
        <NGridItem>
          <NCard title="Notification Settings">
            <NForm :model="settings" label-placement="left" label-width="180">
              <NFormItem label="Email Notifications">
                <NSwitch v-model:value="settings.emailNotifications" />
                <template #feedback>
                  <span class="setting-description">Receive important updates via email</span>
                </template>
              </NFormItem>
              
              <NFormItem label="Push Notifications">
                <NSwitch v-model:value="settings.pushNotifications" />
                <template #feedback>
                  <span class="setting-description">Receive push notifications in your browser</span>
                </template>
              </NFormItem>
              
              <NFormItem label="SMS Notifications">
                <NSwitch v-model:value="settings.smsNotifications" />
                <template #feedback>
                  <span class="setting-description">Receive notifications via SMS</span>
                </template>
              </NFormItem>
              
              <NFormItem label="Marketing Emails">
                <NSwitch v-model:value="settings.marketingEmails" />
                <template #feedback>
                  <span class="setting-description">Receive promotional and marketing emails</span>
                </template>
              </NFormItem>
            </NForm>
          </NCard>
        </NGridItem>

        <!-- App Settings -->
        <NGridItem>
          <NCard title="Application Settings">
            <NForm :model="settings" label-placement="left" label-width="180">
              <NFormItem label="Language">
                <NSelect 
                  v-model:value="settings.language" 
                  :options="languageOptions"
                  style="width: 200px;"
                />
              </NFormItem>
              
              <NFormItem label="Theme">
                <NSelect 
                  v-model:value="settings.theme" 
                  :options="themeOptions"
                  style="width: 200px;"
                />
              </NFormItem>
              
              <NFormItem label="Auto Save">
                <NSwitch v-model:value="settings.autoSave" />
                <template #feedback>
                  <span class="setting-description">Automatically save your work</span>
                </template>
              </NFormItem>
            </NForm>
          </NCard>
        </NGridItem>

        <!-- Security Settings -->
        <NGridItem>
          <NCard title="Security Settings">
            <NForm :model="settings" label-placement="left" label-width="180">
              <NFormItem label="Two-Factor Auth">
                <NSwitch v-model:value="settings.twoFactorAuth" />
                <template #feedback>
                  <span class="setting-description">Add an extra layer of security to your account</span>
                </template>
              </NFormItem>
            </NForm>
            
            <NDivider />
            
            <div class="security-actions">
              <h4>Account Actions</h4>
              <NSpace vertical :size="12">
                <NButton type="warning" ghost>Change Password</NButton>
                <NButton type="error" ghost>Delete Account</NButton>
              </NSpace>
            </div>
          </NCard>
        </NGridItem>
      </NGrid>

      <!-- Action Buttons -->
      <div class="settings-actions">
        <NSpace>
          <NButton @click="resetSettings">Reset to Defaults</NButton>
          <NButton @click="goBack">Cancel</NButton>
          <NButton type="primary" @click="saveSettings">Save Settings</NButton>
        </NSpace>
      </div>
    </div>
  </div>
</template>

<style scoped>
.settings-container {
  max-width: 800px;
  margin: 0 auto;
  padding: 24px;
}

.settings-content {
  margin-top: 24px;
}

.setting-description {
  font-size: 0.875rem;
  color: #64748b;
  margin-top: 4px;
}

.security-actions {
  margin-top: 16px;
}

.security-actions h4 {
  margin: 0 0 12px 0;
  color: #374151;
  font-weight: 600;
}

.settings-actions {
  margin-top: 32px;
  padding: 24px;
  background-color: #f8fafc;
  border-radius: 8px;
  border: 1px solid #e2e8f0;
  display: flex;
  justify-content: center;
}

@media (max-width: 768px) {
  .settings-container {
    padding: 16px;
  }

  .settings-actions {
    padding: 16px;
  }

  .settings-actions :deep(.n-space) {
    width: 100%;
    justify-content: center;
    flex-wrap: wrap;
  }
}
</style>