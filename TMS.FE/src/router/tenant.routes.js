import Main from '@/views/Main.vue';
const TenantDetail = () => import('@/views/tenant/TenantDetail.vue');
const TenantList = () => import('@/views/tenant/TenantList.vue');

export default {
    path: '/tenant',
    component: Main,
    children: [ 
        { 
            path: '', 
            component: TenantList,
            children: [
                {
                    path: '',
                    component: null,
                },
                {
                    path: 'new',
                    component: TenantDetail,
                },
                {
                    path: 'view/:id',
                    component: TenantDetail,
                },
                {
                    path: 'edit/:id',
                    component: TenantDetail,
                },
            ]
        },
    ]
};