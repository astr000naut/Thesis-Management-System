import {defineStore} from 'pinia';
import { httpClient } from '@/helpers';
import {router} from '@/router';
import { useAlertStore } from './alert.store';
import { ref } from 'vue';
import $api from '@/api';


export const useTenantStore = defineStore('tenant', {
    state: () => ({
        tenants: [],
        total: 0,
        loading: false,
        keySearch: '',
        pageNumber: 1,
        pageSize: 20,
        filterColumns: ["TenantName", "TenantCode", "Domain"]
    }),
    actions:{
        async fetchList() {
            this.loading = true;
            try {
                const response = await httpClient.post($api.tenant.filter(), {
                    skip: (this.pageNumber - 1) * this.pageSize,
                    take: this.pageSize,
                    keySearch: this.keySearch
                });

                this.tenants = response.data;
                this.total = response.total;
            } catch (error) {
                const alertStore = useAlertStore();
                alertStore.alert('error', error);
            } finally {
                this.loading = false;
            }
        },
        async getById(id) {
            this.loading = true;
            try {
                const response = await httpClient.get($api.tenant.getById(id));
                return response.data;
            } catch (error) {
                const alertStore = useAlertStore();
                alertStore.alert('error', error);
            } finally {
                this.loading = false;
            }
        },
        async reload() {
            this.pageNumber = 1;
            await this.fetchList();
        },
        async insert(entity) {
            this.loading = true;
            try {
                const response = await httpClient.post($api.tenant.insert(), entity);

                if (response.Error) {
                    throw response.Message;
                }
                // await this.reload();
                this.tenants.unshift(entity);
                ++ this.total;
                router.push('/tenant');
            } catch (error) {
                const alertStore = useAlertStore();
                alertStore.alert('error', error);
            } finally {
                this.loading = false;
            }
        },

        async update(entity) {
            this.loading = true;
            try {
                const response = await httpClient.put($api.tenant.update(entity.id), entity);

                if (response.Error) {
                    throw response.Message;
                }
                // await this.reload();
                const index = this.tenants.findIndex(x => x.id === entity.id);
                this.tenants[index] = entity;
                router.push('/tenant');
            } catch (error) {
                const alertStore = useAlertStore();
                alertStore.alert('error', error);
            } finally {
                this.loading = false;
            }
        },

        async delete(id) {
            this.loading = true;
            try {
                await httpClient.delete($api.tenant.delete(id));
                this.tenants = this.tenants.filter(x => x.id !== id);
                -- this.total;
            } catch (error) {
                const alertStore = useAlertStore();
                alertStore.alert('error', error);
            } finally {
                this.loading = false;
            }
        },

        async setPageSize(newPageSize) {
            this.pageSize = newPageSize;
            await this.reload();
        },

        async setPageNumber(newPageNumber) {
            this.pageNumber = newPageNumber;
            await this.fetchList();
        },

        async setKeySearch(newKeySearch) {
            this.keySearch = newKeySearch;
            await this.reload();
        }
    }
});