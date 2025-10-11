import { createRouter, createWebHistory } from 'vue-router'
import Home from '@/components/Home.vue'
import About from '@/components/About.vue'
import CreatePost from '@/components/post/CreatePost.vue'

const routes = [
  {
    path: '/',
    name: 'home',
    component: Home,
    meta: { title: 'RideHive - Home' }
  },
  {
    path: '/about',
    name: 'about',
    component: About,
    meta: { title: 'About RideHive' }
  },
  {
    path: '/create-post',
    name: 'create-post',
    component: CreatePost,

  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  // Scroll to top when route changes
  scrollBehavior() {
    return { top: 0 }
  }
})

// Navigation guard to update page title
router.beforeEach((to, _from, next) => {
  // Update page title based on route meta
  if (to.meta.title) {
    document.title = to.meta.title as string
  }
  next()
})

export default router