import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/api/Auth'
import Home from '@/components/Home.vue'
import About from '@/components/About.vue'
import CreatePost from '@/components/owner/CreatePost.vue'
import Profile from '@/components/profile/Profile.vue'
import EditProfile from '@/components/profile/EditProfile.vue'
import Settings from '@/components/profile/Settings.vue'
import OwnedProperties from '@/components/owner/OwnedProperties.vue'
import AddCar from '@/components/car/AddCar.vue'
import EditCar from '@/components/car/EditCar.vue'
import LoginPage from "@/components/AuthPage/LoginPage.vue"
import RegisterPage from '@/components/AuthPage/RegisterPage.vue'
import OwnerPosts from '@/components/owner/Posts.vue'

const routes = [
  // Public routes 
  {
    path: '/',
    name: 'login',
    component: LoginPage,
    meta: { 
      title: 'Log in RideHive',
      requiresAuth: false,
      hideForAuth: true // if logged redirect
    }
  },
  {
    path: '/register',
    name: 'register',
    component: RegisterPage,
    meta: { 
      title: 'Register',
      requiresAuth: false,
      hideForAuth: true
    }
  },

  // Protected routes 
  {
    path: '/home',
    name: 'home',
    component: Home,
    meta: { 
      title: 'RideHive - Home',
      requiresAuth: true 
    }
  },
  {
    path: '/about',
    name: 'about',
    component: About,
    meta: { 
      title: 'About RideHive',
      requiresAuth: true 
    }
  },
  {
    path: '/create-post',
    name: 'create-post',
    component: CreatePost,
    meta: {
      requiresAuth: true 
    }
  },
  {
    path: '/profile',
    name: 'profile',
    component: Profile,
    meta: { 
      title: 'My Profile - RideHive',
      requiresAuth: true 
    }
  },
  {
    path: '/edit-profile',
    name: 'edit-profile',
    component: EditProfile,
    meta: { title: 'Edit Profile - RideHive', requiresAuth: true }
  },
  {
    path: '/settings',
    name: 'settings',
    component: Settings,
    meta: { title: 'Settings - RideHive', requiresAuth: true }
  },
  {
    path: '/owned-properties',
    name: 'owned-prop',
    component: OwnedProperties,
    meta: { 
      title: 'Owned Properties - RideHive', 
      requiresAuth: true, 
      requiresRole: 'Owner'
    }
  },
  {
    path: '/add-car',
    name: 'add-car',
    component: AddCar,
    meta: { 
      title: 'Add Car - RideHive',
      requiresAuth: true, 
      requiresRole: 'Owner'
    }
  },
  {
    path: '/edit-car/:id',
    name: 'edit-car',
    component: EditCar,
    meta: { 
      title: 'Edit Car - RideHive',
      requiresAuth: true, 
      requiresRole: 'Owner'
    }
  },
  {
    path: '/my-posts',
    name: 'owner-posts',
    component: OwnerPosts,
    meta: {
      title: "My Posts - RideHive",
      requiresAuth: true, 
      requiresRole: 'Owner'
    }
  },
  {
    path: '/post/:id',
    name: 'post-details',
    component: () => import('@/components/post/PostDetails.vue'),
    meta: {title: "Post Details - RideHive", requiresAuth: true}
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior() {
    return { top: 0 }
  }
})


router.beforeEach((to, _from, next) => {
  // Update page title
  if (to.meta.title) {
    document.title = to.meta.title as string
  }

  // Get auth store 
  const authStore = useAuthStore()
  
  // Check authentication
  authStore.checkAuth()
  
  const isAuthenticated = authStore.isAuthenticated
  const userRole = authStore.userRole

  // 1. Dacă ruta necesită autentificare și userul NU e logat
  if (to.meta.requiresAuth && !isAuthenticated) {
    console.log('Access denied: Not authenticated. Redirecting to login...')
    return next({ name: 'login' })
  }

  // 2. Dacă userul e logat și încearcă să acceseze login/register
  if (to.meta.hideForAuth && isAuthenticated) {
    console.log('Already authenticated. Redirecting to home...')
    return next({ name: 'home' })
  }

  // 3. Verifică role-based access
  if (to.meta.requiresRole && userRole !== to.meta.requiresRole) {
    console.log(`Access denied: Role mismatch. Required: ${to.meta.requiresRole}, User: ${userRole}`)
    
    // Redirect la pagina potrivită pentru rolul lor
    if (userRole === 'Owner') {
      return next({ name: 'home' })
    } else if (userRole === 'Client') {
      return next({ name: 'home' })
    } else {
      return next({ name: 'login' })
    }
  }

  // 4. Toate verificările au trecut - permite accesul
  next()
})

export default router