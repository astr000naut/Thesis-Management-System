import Main from '@/views/Main.vue';
import Login from '@/views/account/Login.vue';

export default {
    path: '/setting',
    component: Main,
    children: [
        { path: '', component: Login},
    ]
};
