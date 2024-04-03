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
                ElMessage.success('Thêm mới khách hàng thành công');
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
                const response = await httpClient.put($api.tenant.update(entity.tenantId), entity);

                if (response.Error) {
                    throw response.Message;
                }
                
                const index = this.tenants.findIndex(x => x.tenantId === entity.tenantId);
                this.tenants[index] = entity;
                ElMessage.success('Cập nhật thông tin khách hàng thành công');
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
                DBName: entity.dbName
            }
            try {
                const connectDBResponse = await httpClient.post($api.tenant.checkDBConnection(), body);
            
                if (connectDBResponse.Error) {
                    throw connectDBResponse.Message;
                }

                if (connectDBResponse === false) {
                    return "Không thể kết nối đến cơ sở dữ liệu.";
                }
            } catch {
                return "Không thể kết nối đến cơ sở dữ liệu.";
            }
            
            
            body = {
                AutoCreateMinio: entity.autoCreateMinio,
                Endpoint: entity.minioEndpoint,
                Port: entity.minioPort,
                AccessKey: entity.minioAccessKey,
                SecretKey: entity.minioSecretKey,
                BucketName: entity.minioBucketName
            }
            try {
                const connectMinioReseponse = await httpClient.post($api.fileService.checkMinioConnection(), body);
            
                if (connectMinioReseponse.Error) {
                    throw connectMinioReseponse.Message;
                }
    
                if (connectMinioReseponse === false) {
                    
                    return "Không thể kết nối đến Minio.";
                }
                
            } catch {
                return "Không thể kết nối đến Minio.";
            }   

            
            return "";
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
                ElMessage.success('Kích hoạt khách hàng thành công');

            } catch (error) {
                const alertStore = useAlertStore();
                alertStore.alert('error', error);
            } finally {
                this.loading = false;
            }
        }

        
    }
});