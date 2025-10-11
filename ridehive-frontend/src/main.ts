import { createApp } from 'vue'
import './style.css'
import App from './App.vue'

// Router and state management
import router from './router'
import { createPinia } from 'pinia'

const app = createApp(App)

// Add plugins
app.use(createPinia())  // State management
app.use(router)         // Router for navigation

app.mount('#app')
