import {Login, Register } from '@/views/account';

export default {
    path: '/account',
    children: [
        { path: '', redirect: 'login' },
        { path: 'login', component: Login },
    ]
};
