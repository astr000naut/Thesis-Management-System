import {defineStore} from 'pinia';
import { httpClient } from '@/helpers';
import {router} from '@/router';
import { useAlertStore } from './alert.store';
import { ref } from 'vue';
import { ElMessage } from "element-plus";

import $api from '@/api';

export const useAuthStore = defineStore('auth', {
    state: () => ({
        loginInfo: JSON.parse(localStorage.getItem('loginInfo')),
        returnUrl: null,
        tenantBaseInfo: JSON.parse(localStorage.getItem('tenantBaseInfo'))
    }),
    actions:{
        async getTenantBaseInfo() {
            try {
                const body = {
                    Domain: window.location.hostname
                };
                const response = await httpClient.post($api.tenant.getTenantInfoLite(), body);
                localStorage.setItem('tenantBaseInfo', JSON.stringify(response));
                this.tenantBaseInfo = response;
            } catch (error) {
                const alertStore = useAlertStore();
                alertStore.alert('error', error);
            }
        },
        async login(username, password) {
            try {
    
                const response = await httpClient.post($api.user.authenticate, {username, password});

                if (response && response.errorCode) {
                    return response.message;
                } else if (response && response.success && response.data != null) {
                    localStorage.setItem('loginInfo', JSON.stringify(response.data));
    
                    this.loginInfo = response.data;
        
                    this.returnUrl = null;
        
                    router.push(this.returnUrl || '/');
                } else {
                    ElMessage.error('Có lỗi xảy ra');
                }
                
            } catch (error) {
                const alertStore = useAlertStore();
                alertStore.alert('error', error);
            }
        },
    
        logout() {
            localStorage.removeItem('loginInfo');
    
            this.loginInfo = null;
    
            router.push('account/login');
        },
        async refreshToken() {
            try {
                const response = await httpClient.post($api.user.refreshToken, { 
                    accessToken: this.loginInfo.accessToken,
                    refreshToken: this.loginInfo.refreshToken
                });

                if (response.Error) {
                    this.logout();
                } else {
                    this.loginInfo = response;
                    localStorage.setItem('loginInfo', JSON.stringify(response));
                }
            } catch (error) {
                this.logout();
            }
        }
    }
    
});