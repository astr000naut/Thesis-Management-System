import {defineStore} from 'pinia';
import $api from '@/api';
import { useBaseStore } from './base.store';
import {useAlertStore, useAuthStore} from '@/stores';
import { httpClient } from '@/helpers';

export const useThesisStore = defineStore('thesis', () => {
    const {
        entities,
        total,
        loading,
        keySearch,
        pageNumber,
        pageSize,
        filterColumns,
        removeOneEntity,
        fetchList,
        exportList,
        getById,
        insert,
        update,
        deleteOne,
        deleteMany,
        setPageSize,
        setPageNumber,
        setKeySearch,
        getNew
    } = useBaseStore(
        {
            API: $api.thesis, 
            filterColumns: ["thesisName", "thesisCode"],
            keyName: 'thesisId'
        }); 

    const authStore = useAuthStore();
    
    const fetchThesisGuidingList = async (teacherId) => {
        loading.value = true;
        try {
            await new Promise(resolve => setTimeout(resolve, 500));
            const response = await httpClient.get($api.thesis.fetchThesisGuidingList(teacherId));
            if (response.errorCode) {
                throw response.message;
            }
            entities.value = response.data;
            total.value = response.data.length;
        } catch (error) {
            const alertStore = useAlertStore();
            alertStore.alert('error', error);
        } finally {
            loading.value = false;
        }
    }   
    
    const fetchThesisGuidedList = async () => {
        loading.value = true;
        try {
            await new Promise(resolve => setTimeout(resolve, 500));
            let payload = {
                teacherId: authStore.loginInfo.user.userId,
                skip: (pageNumber.value - 1) * pageSize.value,
                take: pageSize.value,
                keySearch: keySearch.value,
            }
            const response = await httpClient.post($api.thesis.fetchThesisGuidedList(), payload);
            if (response.errorCode) {
                throw response.message;
            }
            entities.value = response.data;
            total.value = response.total;
        } catch (error) {
            const alertStore = useAlertStore();
            alertStore.alert('error', error);
        } finally {
            loading.value = false;
        }
    }   

    return {
        entities,
        total,
        loading,
        keySearch,
        pageNumber,
        pageSize,
        filterColumns,
        removeOneEntity,
        fetchList,
        exportList,
        fetchThesisGuidingList,
        fetchThesisGuidedList,
        getById,
        insert,
        update,
        deleteOne,
        deleteMany,
        setPageSize,
        setPageNumber,
        setKeySearch,
        getNew
    }
});


        
