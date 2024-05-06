import {defineStore} from 'pinia';
import { httpClient } from '@/helpers';
import {router} from '@/router';
import { useAlertStore } from './alert.store';
import { ref } from 'vue';
import $api from '@/api';
import { ElMessage } from 'element-plus'


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
                await new Promise(resolve => setTimeout(resolve, 800));
                const response = await httpClient.post($api.tenant.filter(), {
                    skip: (this.pageNumber - 1) * this.pageSize,
                    take: this.pageSize,
                    keySearch: this.keySearch,
                    filterColumns: this.filterColumns
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
                
                if (response.Error) {
                    throw response.Message;
                }
                
                return response;
            } catch (error) {
                const alertStore = useAlertStore();
                alertStore.alert('error', error);
            } finally {
                this.loading = false;
            }
        },
        
        async insert(entity) {
            this.loading = true;
            try {
                const response = await httpClient.post($api.tenant.insert(), entity);

                if (response.Error) {
                    throw response.Message;
                }
                
                this.tenants.unshift(entity);
                ++ this.total;
                return "";
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
                const response = await httpClient.put($api.tenant.update(entity.tenantId), entity);

                if (response.Error) {
                    return response.Message;
                }
                
                const index = this.tenants.findIndex(x => x.tenantId === entity.tenantId);
                this.tenants[index] = entity;
                return "";
            } catch (error) {
                const alertStore = useAlertStore();
                return "Có lỗi xảy ra! Vui lòng thử lại sau."
            } finally {
                this.loading = false;
            }
        },

        async removeResource(entity) {
            this.loading = true;
            try {
                const response = await httpClient.post($api.tenant.removeResource(), entity);

                if (response.Error) {
                    throw response.Message;
                }
                
                const index = this.tenants.findIndex(x => x.tenantId === entity.tenantId);
                entity.status = 0;
                this.tenants[index] = entity;
                return "";
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
                const response = await httpClient.post($api.tenant.delete(), [id]);

                if (response.Error) {
                    throw response.Message;
                }

                this.tenants = this.tenants.filter(x => x.tenantId !== id);
                -- this.total;
                return true;
            } catch (error) {
                const alertStore = useAlertStore();
                alertStore.alert('error', error);
                return false;
            } finally {
                this.loading = false;
            }
        },

        async setPageSize(newPageSize) {
            if (this.pageSize === newPageSize) {
                return;
            }

            this.pageSize = newPageSize;
            await this.fetchList();
        },

        async setPageNumber(newPageNumber) {

            if (this.pageNumber === newPageNumber) {
                return;
            }

            this.pageNumber = newPageNumber;
            await this.fetchList();
        },

        async setKeySearch(newKeySearch) {
            this.keySearch = newKeySearch;
            await this.fetchList();
        },

        async getNew() {
            this.loading = true;
            try {
                const response = await httpClient.get($api.tenant.getNew());

                if (response.Error) {
                    throw response.Message;
                }

                return response;
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
                
                if (response.Error) {
                    throw response.Message;
                }
                
                return response;
            } catch (error) {
                const alertStore = useAlertStore();
                alertStore.alert('error', error);
            } finally {
                this.loading = false;
            }
        },

        async checkConnection(entity) {
            let body = {
                AutoCreateDB: entity.autoCreateDB,
                ConnectionString: entity.dbConnection,
                DBName: entity.dbName,

                AutoCreateMinio: entity.autoCreateMinio,
                MinioEndpoint: entity.minioEndpoint,
                MinioAccessKey: entity.minioAccessKey,
                MinioSecretKey: entity.minioSecretKey,
                MinioBucketName: entity.minioBucketName
            }
            try {
                const checkConnectionResponse = await httpClient.post($api.tenant.checkConnection(), body);
                return checkConnectionResponse;
            } catch(error) {
                const alertStore = useAlertStore();
                alertStore.alert('error', error);
            }
        },

        async activeTenant(entity) {
            this.loading = true;
            try {
                const response = await httpClient.post($api.tenant.activeTenant(), {
                    TenantId: entity.tenantId
                });

                if (response.Error) {
                    throw response.Message;
                }
                
                const index = this.tenants.findIndex(x => x.tenantId === entity.tenantId);
                this.tenants[index] = response;
                return  "";
            } catch (error) {
                const alertStore = useAlertStore();
                alertStore.alert('error', error);
            } finally {
                this.loading = false;
            }
        }

        
    }
});