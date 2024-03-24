<template>
    <div>Hi</div>
    <button @click="btnTestOnClick">Test</button>
    <button @click="btnLogoutOnClick">Logout</button>
</template>

<script setup>
    import { httpClient } from '@/helpers';
    import $api from '@/api';
    import { useAuthStore } from '@/stores';


    async function btnTestOnClick() {
        const res = await httpClient.get($api.user.test);
        console.log(res);
    }

    function btnLogoutOnClick() {
        const authStore = useAuthStore();
        authStore.logout();
    }

</script>

<style scoped></style>