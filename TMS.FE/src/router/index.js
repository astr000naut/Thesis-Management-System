
import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore } from '@/stores';
import accountRoutes from './account.routes';
import studentRoutes from './student.routes';
import teacherRoutes from './teacher.routes';
import managerRoutes from "./manager.routes";

import NotFound from '@/components/common/NotFound.vue';


export const router = createRouter({
  history: createWebHistory(),
  linkActiveClass: 'active',
  routes: [
      {path: '/', redirect: '/account/login'},
      { ...accountRoutes },
      { ...managerRoutes},
      { ...studentRoutes},
      { ...teacherRoutes},
      {path: '/not-found', component: NotFound},
  ]
});

router.beforeEach(async (to) => {

  // redirect to login page if not logged in and trying to access a restricted page 
  const publicPages = ['/account/login', '/account/register', '/not-found'];
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

  if (authRequired) {
    const role = authStore.loginInfo.user.role ?? "";
    const isRoleMatched = to.meta.roles ? to.meta.roles.includes(role) : false;

    if (!isRoleMatched) {
        return '/not-found';
    };
  }
});
