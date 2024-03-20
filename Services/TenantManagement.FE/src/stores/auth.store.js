import {defineStore} from 'pinia';
import { httpClient } from '@/helpers';
import {router} from '@/router';
import { useAlertStore } from './alert.store';
import { ref } from 'vue';

import $api from '@/api';

export const useAuthStore = defineStore('auth', () => {
//#region states
    const user = ref(localStorage.getItem('user'));
    const returnUrl = ref(null);
//#endregion

//#region getters
//#endregion

//#region actions
    async function login(username, password) {
        try {

            const response = await httpClient.post($api.user.authenticate, {username, password});

            localStorage.setItem('user', JSON.stringify(response));

            user.value = response;

            returnUrl.value = null;

            router.push(this.returnUrl || '/');
        } catch (error) {
            const alertStore = useAlertStore();
            alertStore.error(error);
        }
    }

    function logout() {
        localStorage.removeItem('user');

        user.value = null;

        router.push('account/login');
    }


//#endregion


});