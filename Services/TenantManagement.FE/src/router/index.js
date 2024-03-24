
import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore, useAlertStore } from '@/stores';
import accountRoutes from './account.routes';
import tenantRoutes from "./tenant.routes";
import settingRoutes from "./setting.routes";

export const router = createRouter({
  history: createWebHistory(),
  linkActiveClass: 'active',
  routes: [
      { path: '/', redirect: '/tenant' },
      { ...accountRoutes },
      { ...tenantRoutes },
      { ...settingRoutes },


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

  if (authRequired && !authStore.loginInfo) {
      authStore.returnUrl = to.fullPath;
      return '/account/login';
  }
});
