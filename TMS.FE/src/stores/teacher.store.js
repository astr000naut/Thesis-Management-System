import {defineStore} from 'pinia';
import $api from '@/api';
import { useBaseStore } from './base.store';

export const useTeacherStore = defineStore('teacher', () => {
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
            API: $api.teacher, 
            filterColumns: ["teacherName", "teacherCode"],
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


        
