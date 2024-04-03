import Main from '@/views/Main.vue';
import TenantDetail from '@/views/tenant/TenantDetail.vue';
import TenantList from '@/views/tenant/TenantList.vue';

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