import {defineStore} from 'pinia';
import $api from '@/api';
import { useBaseStore } from './base.store';

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


        
