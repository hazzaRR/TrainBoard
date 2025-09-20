import { createRouter, createWebHistory } from 'vue-router';

const routes = [
      {
        path: '/',
        name: 'Dashboard',
        component: () => import('../Views/Dashboard.vue'),
      },
      {
        path: '/network',
        name: 'Network',
        component: () => import('../Views/Network.vue'),
      },
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router