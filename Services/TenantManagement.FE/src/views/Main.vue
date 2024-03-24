<template>
    <el-container>
        <el-header>Header</el-header>
        <el-container>
            <TheSidebar></TheSidebar>
            <el-main style="background-color: azure;">
                <div>{{ loginInfo }}</div>
                <button @click="btnTestOnClick">Test</button>
                <button @click="btnLogoutOnClick">Logout</button>
            </el-main>
        </el-container>
    </el-container>
</template>

<script setup>
    import { httpClient } from '@/helpers';
    import $api from '@/api';
    import { useAuthStore } from '@/stores';
    import { TheSidebar } from '@/components/layout';
    import { storeToRefs } from 'pinia';
    const authStore = useAuthStore();
    const {loginInfo} = storeToRefs(authStore);


    async function btnTestOnClick() {
        const res = await httpClient.get($api.user.test);
        console.log(res);
    }

    function btnLogoutOnClick() {     
        authStore.logout();
    }

</script>

<style scoped></style>