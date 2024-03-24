import {defineStore} from 'pinia';
import { ref } from 'vue';

export const useAlertStore = defineStore('alert', {
    state: () => ({
        alerts: [],
        id: 0,
    }),
    actions: {
        alert(type, message) {
            let id = ++ this.id;
            this.alerts.push({ id: id, message: message, type: type });
            setTimeout(() => {
                console.log('remove', id);
                this.remove(id);
            }, 4000);
        },
        clear() {
            this.alerts = [];
        },
        remove(id) {
            const index = this.alerts.findIndex(x => x.id === id);
            if (index !== -1) {
                this.alerts.splice(index, 1);
            }
        }
    },
});