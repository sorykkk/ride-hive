import { createRouter, createWebHistory } from 'vue-router'
import Home from '@/components/Home.vue'
import About from '@/components/About.vue'
import CreatePost from '@/components/owner/CreatePost.vue'
import Profile from '@/components/profile/Profile.vue'
import EditProfile from '@/components/profile/EditProfile.vue'
import Settings from '@/components/profile/Settings.vue'
import OwnedProperties from '@/components/owner/OwnedProperties.vue'
import AddCar from '@/components/car/AddCar.vue'
import EditCar from '@/components/car/EditCar.vue'
import OwnerPosts from '@/components/owner/Posts.vue'

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
  },
  {
    path: '/profile',
    name: 'profile',
    component: Profile,
    meta: { title: 'My Profile - RideHive' }
  },
  {
    path: '/edit-profile',
    name: 'edit-profile',
    component: EditProfile,
    meta: { title: 'Edit Profile - RideHive' }
  },
  {
    path: '/settings',
    name: 'settings',
    component: Settings,
    meta: { title: 'Settings - RideHive' }
  },
  {
    path: '/owned-properties',
    name: 'owned-prop',
    component: OwnedProperties,
    meta: { title: 'Owned Properties - RideHive' }
  },
  {
    path: '/add-car',
    name: 'add-car',
    component: AddCar,
    meta: { title: 'Add Car - RideHive' }
  },
  {
    path: '/edit-car/:id',
    name: 'edit-car',
    component: EditCar,
    meta: { title: 'Edit Car - RideHive' }
  },
  {
    path: '/my-posts',
    name: 'owner-posts',
    component: OwnerPosts,
    meta: {title: "My Posts - RideHive"}
  },
  {
    path: '/post/:id',
    name: 'post-details',
    component: () => import('@/components/post/PostDetails.vue'),
    meta: {title: "Post Details - RideHive"}
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