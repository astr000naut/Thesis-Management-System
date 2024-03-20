import {defineStore} from 'pinia';
import { ref } from 'vue';

export const useAlertStore = defineStore('alert', () => {
//#region states
    const alert = ref('');
//#endregion

//#region getters
//#endregion

//#region actions
    function success(message) {
        alert.value = message;
    }

    function error(message) {
        alert.value = message;
    };

    function clear() {
        alert.value = '';
    }
//#endregion

return {
    success,
    error,
    clear,
};
});