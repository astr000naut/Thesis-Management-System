import {defineStore} from 'pinia';
import $api from '@/api';
import { useBaseStore } from './base.store';

export const useFacultyStore = defineStore('faculty', () => {
    const {
        entities,
        total,
        loading,
        keySearch,
        pageNumber,
        pageSize,
        filterColumns,
        fetchList,
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
            API: $api.faculty, 
            filterColumns: ["facultyName", "facultyCode"],
            keyName: 'facultyId'
        });  
    return {
        entities,
        total,
        loading,
        keySearch,
        pageNumber,
        pageSize,
        filterColumns,
        fetchList,
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


        
