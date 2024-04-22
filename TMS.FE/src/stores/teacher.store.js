import {defineStore} from 'pinia';
import { httpClient } from '@/helpers';
import {router} from '@/router';
import { useAlertStore } from './alert.store';
import { ref } from 'vue';
import $api from '@/api';
import { ElMessage } from 'element-plus'

const API = $api.teacher;


export const useTeacherStore = defineStore('teacher', {
    state: () => ({
        entities: [],
        total: 0,
        loading: false,
        keySearch: '',
        pageNumber: 1,
        pageSize: 20,
        filterColumns: ["teacherName", "teacherCode"]
    }),
    actions:{
        async fetchList() {
            this.loading = true;
            try {
                await new Promise(resolve => setTimeout(resolve, 500));
                const response = await httpClient.post(API.filter(), {
                    skip: (this.pageNumber - 1) * this.pageSize,
                    take: this.pageSize,
                    keySearch: this.keySearch,
                    filterColumns: this.filterColumns
                });

                this.entities = response.data ?? [];
                this.total = response.total ?? 0;
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
                const response = await httpClient.get(API.getById(id));
                
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
        },

        async update(entity) { 
        },

        async delete(id) {
            this.loading = true;
            try {
                const response = await httpClient.post(API.delete(), [id]);

                if (response.Error) {
                    throw response.Message;
                }

                this.entities = this.entities.filter(x => x.userId !== id);
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
        }

        
    }
});