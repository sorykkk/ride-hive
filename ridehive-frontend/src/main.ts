import { createApp } from 'vue'
import './style.css'
import App from './App.vue'

// Additional plugins/libraries
import { createRouter, createWebHistory } from 'vue-router'
import { createPinia } from 'pinia'

import HomeComponent from './components/Home.vue'
import AboutComponent from './components/About.vue'
import CreatePostComponent from './components/post/CreatePost.vue'

// Define routes
const routes = [
    {
        path: '/',
        name: 'Home',
        component: HomeComponent
    },
    {
        path: '/about',
        name: 'About',
        component: AboutComponent
    },
    {
        path: '/create-post',
        name: 'Create post',
        component: CreatePostComponent
    }
]

const app = createApp(App)

// Add plugins
app.use(createPinia())  // State management
app.use(createRouter({
    history: createWebHistory(),
    routes
}))

app.mount('#app')
