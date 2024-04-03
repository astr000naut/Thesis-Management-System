import { ref } from 'vue';
import {httpClient} from '@/helpers/httpClient';
import {useAlertStore} from '@/stores';

export function useList(api) {
    const listItem = ref([]);
    const pageNumber = ref(1);
    const pageSize = ref(10);
    const totalItems = ref(0);
    const loading = ref(false);
    const keySearch = ref('');
    const alertStore = useAlertStore();

    const fetchList = async () => {
        loading.value = true;
        try {
            const response = await httpClient.get(api.filter((pageNumber.value - 1) * pageSize.value, pageSize.value, keySearch.value));
            listItem.value = response.data;
        } catch (error) {
            alertStore.alert('error', error);
        } finally {
            loading.value = false;
        }
    };

    const refreshList = () => {
        pageNumber.value = 1;
        fetchList();
    };

    const setPageSize = (newPageSize) => {
        pageSize.value = newPageSize;
        refreshList();
    };

    return {
        listItem,
        pageNumber,
        pageSize,
        totalItems,
        loading,
        fetchList,
        refreshList,
        setPageSize,
    };
}
