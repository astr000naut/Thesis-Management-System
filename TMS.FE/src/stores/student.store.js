import {defineStore} from 'pinia';
import $api from '@/api';
import { useBaseStore } from './base.store';

export const useStudentStore = defineStore('student', () => {
    const {
        entities,
        total,
        loading,
        keySearch,
        pageNumber,
        pageSize,
        filterColumns,
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
            API: $api.student, 
            filterColumns: ["studentName", "studentCode"],
            keyName: 'userId'
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
    }
});


        
