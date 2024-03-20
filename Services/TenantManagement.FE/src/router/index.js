
import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore, useAlertStore } from '@/stores';
import accountRoutes from './account.routes';
import Home from "@/views/Home.vue";

export const router = createRouter({
  history: createWebHistory(),
  linkActiveClass: 'active',
  routes: [
      { path: '/', component: Home },
      { ...accountRoutes },
      // catch all redirect to home page
      { path: '/:pathMatch(.*)*', redirect: '/' }
  ]
});

router.beforeEach(async (to) => {
  // clear alert on route change
  const alertStore = useAlertStore();
  alertStore.clear();

  // redirect to login page if not logged in and trying to access a restricted page 
  const publicPages = ['/account/login', '/account/register'];
  const authRequired = !publicPages.includes(to.path);

  const authStore = useAuthStore();

  if (authRequired && !authStore.user) {
      authStore.returnUrl = to.fullPath;
      return '/account/login';
  }
});
