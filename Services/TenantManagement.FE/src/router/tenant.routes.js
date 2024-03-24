import Main from '@/views/Main.vue';
import Login from '@/views/account/Login.vue';

export default {
    path: '/tenant',
    component: Main,
    children: [
        { path: '', component: () => import('@/views/tenant/TenantList.vue') },
    ]
};
