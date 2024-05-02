import {defineStore} from 'pinia';
import $api from '@/api';
import { httpClient } from '@/helpers';
import {useAlertStore} from '@/stores';

export const useSettingStore = defineStore('setting', {
    state: () => ({
        appSetting: {},
        loading: false,
    }),
    actions: {
        async fetchListSetting() {
            try {
                await new Promise(resolve => setTimeout(resolve, 500));
                const response = await httpClient.get($api.setting.getSetting());
                this.appSetting = response.data;
                
            } catch (error) {
                const alertStore = useAlertStore();
                alertStore.alert('error', error);
            } finally {
                this.loading = false;
            }
        },
        async updateSetting(entity) {
            try {
                await new Promise(resolve => setTimeout(resolve, 500));
                const response = await httpClient.put($api.setting.updateSetting(entity.id), entity);
                if (response) {
                    this.appSetting = entity;
                }
                return response
                
            } catch (error) {
                const alertStore = useAlertStore();
                alertStore.alert('error', error);
            } finally {
                this.loading = false;
            }
        },
    },
});