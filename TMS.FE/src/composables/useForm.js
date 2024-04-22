import { ref } from 'vue';
import { useRoute } from 'vue-router';

export function useForm(entityName) {
    const route = useRoute();
    
    const form = ref({
        title: '',
        mode: '',
    })
    
    const entity = ref({});

    initState();

    function initState() {
        if (route.path.includes('new')) {
            form.value.title = `Thêm mới ${entityName}`;
            form.value.mode = 'add';
        } else if (route.path.includes('view')) {
            form.value.title = `Xem ${entityName}`;
            form.value.mode = 'view';
        } else {
            form.value.title = `Chỉnh sửa ${entityName}`;
            form.value.mode = 'edit';
        }
    }

    return {
        form,
        entity
    };
}
