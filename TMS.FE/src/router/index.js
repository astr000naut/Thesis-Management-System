
import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore } from '@/stores';
import accountRoutes from './account.routes';
import studentRoutes from './student.routes';
import managerRoutes from "./manager.routes";


export const router = createRouter({
  history: createWebHistory(),
  linkActiveClass: 'active',
  routes: [
      { ...accountRoutes },
      { ...managerRoutes},
      { ...studentRoutes}
  ]
});

router.beforeEach(async (to) => {

  // redirect to login page if not logged in and trying to access a restricted page 
  const publicPages = ['/account/login', '/account/register'];
  const authRequired = !publicPages.includes(to.path);

  const authStore = useAuthStore();

  // check current tenant info
  if (!authStore.tenantBaseInfo) {
      await authStore.getTenantBaseInfo();  
  }


  if (authRequired && !authStore.loginInfo) {
      authStore.returnUrl = to.fullPath;
      return '/account/login';
  }
});
