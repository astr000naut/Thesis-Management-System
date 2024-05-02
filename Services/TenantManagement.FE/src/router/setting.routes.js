import Main from '@/views/Main.vue';
import Login from '@/views/account/Login.vue';
import UnderContruction from '@/components/common/UnderContruction.vue';

export default {
    path: '/setting',
    component: Main,
    children: [
        { path: '', component: UnderContruction},
    ]
};
