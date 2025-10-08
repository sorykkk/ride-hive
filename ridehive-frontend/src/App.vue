<script setup lang="ts">
import { ref } from 'vue'
import { ApiClient, ApiConfig } from './services/api'

// Types
interface TemperatureRequest {
  temperature: number
}

interface TemperatureResponse {
  temperature: number
  adjective: string
  message: string
}

const temperature = ref<number | null>(null)
const result = ref<string>('')
const loading = ref(false)
const error = ref<string>('')

const analyzeTemperature = async () => {
  if (temperature.value === null || temperature.value === undefined) {
    error.value = 'Please enter a temperature'
    return
  }

  loading.value = true
  error.value = ''
  result.value = ''

  try {
    const response = await ApiClient.post<TemperatureResponse>(
      ApiConfig.ENDPOINTS.WEATHER.ANALYZE,
      { temperature: temperature.value } as TemperatureRequest
    )
    
    result.value = response.message
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Error connecting to server. Make sure the backend is running.'
    console.error('Error:', err)
  } finally {
    loading.value = false
  }
}

const clearForm = () => {
  temperature.value = null
  result.value = ''
  error.value = ''
}
</script>

<template>
  <div class="container">
    <div class="weather-analyzer">
      <h1>🌡️ Weather Temperature Analyzer</h1>
      <p>Enter a temperature in Celsius and get a descriptive adjective!</p>
      
      <div class="form-group">
        <label for="temperature">Temperature (°C):</label>
        <input
          id="temperature"
          v-model.number="temperature"
          type="number"
          step="0.1"
          placeholder="Enter temperature in Celsius"
          class="temperature-input"
          @keyup.enter="analyzeTemperature"
        />
      </div>

      <div class="button-group">
        <button 
          @click="analyzeTemperature" 
          :disabled="loading || temperature === null"
          class="analyze-btn"
        >
          {{ loading ? 'Analyzing...' : 'Analyze Temperature' }}
        </button>
        
        <button 
          @click="clearForm"
          class="clear-btn"
        >
          Clear
        </button>
      </div>

      <div v-if="error" class="error">
        {{ error }}
      </div>

      <div v-if="result" class="result">
        <h3>🎯 Result:</h3>
        <p>{{ result }}</p>
      </div>

      <div class="temperature-guide">
        <h4>Temperature Guide:</h4>
        <div class="guide-grid">
          <div class="guide-item freezing">≤ -10°C: Freezing</div>
          <div class="guide-item very-cold">-9°C to 0°C: Very Cold</div>
          <div class="guide-item cold">1°C to 10°C: Cold</div>
          <div class="guide-item cool">11°C to 15°C: Cool</div>
          <div class="guide-item mild">16°C to 20°C: Mild</div>
          <div class="guide-item warm">21°C to 25°C: Warm</div>
          <div class="guide-item hot">26°C to 30°C: Hot</div>
          <div class="guide-item very-hot">31°C to 35°C: Very Hot</div>
          <div class="guide-item scorching">≥ 36°C: Scorching</div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
}

.weather-analyzer {
  background: white;
  border-radius: 20px;
  padding: 40px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
  max-width: 600px;
  width: 100%;
}

h1 {
  text-align: center;
  color: #333;
  margin-bottom: 10px;
  font-size: 2.5em;
}

p {
  text-align: center;
  color: #666;
  margin-bottom: 30px;
}

.form-group {
  margin-bottom: 25px;
}

label {
  display: block;
  margin-bottom: 8px;
  font-weight: 600;
  color: #333;
  font-size: 1.1em;
}

.temperature-input {
  width: 100%;
  padding: 15px;
  border: 2px solid #e1e5e9;
  border-radius: 10px;
  font-size: 1.1em;
  transition: border-color 0.3s ease;
  box-sizing: border-box;
}

.temperature-input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.button-group {
  display: flex;
  gap: 15px;
  margin-bottom: 25px;
}

.analyze-btn {
  flex: 1;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  padding: 15px 30px;
  border-radius: 10px;
  font-size: 1.1em;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.2s ease, opacity 0.2s ease;
}

.analyze-btn:hover:not(:disabled) {
  transform: translateY(-2px);
}

.analyze-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.clear-btn {
  background: #f8f9fa;
  color: #666;
  border: 2px solid #e1e5e9;
  padding: 15px 25px;
  border-radius: 10px;
  font-size: 1.1em;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
}

.clear-btn:hover {
  background: #e9ecef;
  border-color: #ced4da;
}

.error {
  background: #fee;
  color: #c33;
  padding: 15px;
  border-radius: 10px;
  margin-bottom: 20px;
  border-left: 4px solid #c33;
}

.result {
  background: #f0f8f0;
  padding: 20px;
  border-radius: 10px;
  margin-bottom: 30px;
  border-left: 4px solid #28a745;
}

.result h3 {
  margin: 0 0 10px 0;
  color: #155724;
}

.result p {
  margin: 0;
  font-size: 1.2em;
  font-weight: 600;
  color: #155724;
  text-align: left;
}

.temperature-guide {
  background: #f8f9fa;
  padding: 20px;
  border-radius: 10px;
  margin-top: 20px;
}

.temperature-guide h4 {
  margin: 0 0 15px 0;
  color: #333;
  text-align: center;
}

.guide-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 8px;
}

.guide-item {
  padding: 8px 12px;
  border-radius: 6px;
  font-size: 0.9em;
  font-weight: 500;
  text-align: center;
}

.freezing { background: #e3f2fd; color: #0d47a1; }
.very-cold { background: #e8f5e8; color: #1b5e20; }
.cold { background: #f3e5f5; color: #4a148c; }
.cool { background: #e0f2f1; color: #004d40; }
.mild { background: #fff3e0; color: #e65100; }
.warm { background: #fff8e1; color: #f57f17; }
.hot { background: #ffebee; color: #c62828; }
.very-hot { background: #fce4ec; color: #880e4f; }
.scorching { background: #ffcdd2; color: #b71c1c; }

@media (max-width: 768px) {
  .weather-analyzer {
    padding: 20px;
  }
  
  .button-group {
    flex-direction: column;
  }
  
  .guide-grid {
    grid-template-columns: 1fr;
  }
}
</style>
