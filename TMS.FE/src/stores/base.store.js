import {ref} from 'vue';
import {httpClient} from '@/helpers/httpClient';
import {useAlertStore} from '@/stores';

// props = {
//     API: null,
//     filterColumns: [],
//     keyName: 'id'
// }

export function useBaseStore(props) {
    const entities = ref([]);
    const total = ref(0);
    const loading = ref(false);
    const keySearch = ref('');
    const pageNumber = ref(1);
    const pageSize = ref(20);
    const filterColumns = props.filterColumns;
    const keyName = props.keyName;
    const API = props.API;

    const fetchList = async (customWhere) => {
        loading.value = true;
        try {
            await new Promise(resolve => setTimeout(resolve, 500));
            const response = await httpClient.post(API.filter(), {
                skip: (pageNumber.value - 1) * pageSize.value,
                take: pageSize.value,
                keySearch: keySearch.value,
                filterColumns: filterColumns,
                customWhere: customWhere
            });

            if (response.errorCode) {
                throw response.message;
            } else {
                entities.value = response?.data ?? [];
                total.value = response?.total ?? 0;
            }
        } catch (error) {
            const alertStore = useAlertStore();
            alertStore.alert('error', error);
        } finally {
            loading.value = false;
        }
    };

    const removeOneEntity = (id) => {
        entities.value = entities.value.filter(x => x[keyName] !== id);
        -- total.value;
    }

    const getById = async (id) => {
        loading.value = true;
        try {
            const response = await httpClient.get(API.getById(id));
            
            if (response.errorCode) {
                throw response.message;
            }
            
            return response;
        } catch (error) {
            const alertStore = useAlertStore();
            alertStore.alert('error', error);
        } finally {
            loading.value = false;
        }
    };

    const insert = async (entity) => {
        loading.value = true;
        try {
            const response = await httpClient.post(API.insert(), entity);

            if (response.errorCode) {
                throw response.message;
            }
            entities.value.unshift(entity);
            ++ total.value;
            return true;
        } catch (error) {
            const alertStore = useAlertStore();
            alertStore.alert('error', error);
            return false;
        } finally {
            loading.value = false;
        }
    }

    const update = async (entity) => {
        loading.value = true;
        try {
            const response = await httpClient.put(API.update(entity[keyName]), entity);

            if (response.errorCode) {
                throw response.message;
            }
            entities.value = entities.value.map(x => x[keyName] === entity[keyName] ? entity : x);
            return true;
        } catch (error) {
            const alertStore = useAlertStore();
            alertStore.alert('error', error);
            return false;
        } finally {
            loading.value = false;
        }
    };

    const deleteOne = async (id) => {
        loading.value = true;
        try {
            const response = await httpClient.post(API.delete(), [id]);

            if (response.errorCode) {
                throw response.message;
            }

            entities.value = entities.value.filter(x => x[keyName] !== id);
            -- total.value;
            return true;
        } catch (error) {
            const alertStore = useAlertStore();
            alertStore.alert('error', error);
            return false;
        } finally {
            loading.value = false;
        }
    };

    const deleteMany = async (ids) => {
    }

    const setPageSize = async (newPageSize) => {
        if (pageSize.value === newPageSize) {
            return;
        }

        pageSize.value = newPageSize;
        await fetchList();
    }

    const setPageNumber = async (newPageNumber) => {
        if (pageNumber.value === newPageNumber) {
            return;
        }

        pageNumber.value = newPageNumber;
        await fetchList();
    }

    const setKeySearch = async (newKeySearch) => {
        keySearch.value = newKeySearch;
        await fetchList();
    }

    const getNew = async () => {
        loading.value = true;
        try {
            const response = await httpClient.get(API.getNew());

            if (response.errorCode) {
                throw response.message;
            }

            return response;
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

}