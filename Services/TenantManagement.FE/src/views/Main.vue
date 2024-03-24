<template>
    <el-container>
        <el-header><TheHeader></TheHeader></el-header>
        <el-container>
            <TheSidebar></TheSidebar>
            <el-main>
                <router-view></router-view>
            </el-main>
        </el-container>
    </el-container>
</template>

<script setup>
    import { httpClient } from '@/helpers';
    import $api from '@/api';
    import { useAuthStore } from '@/stores';
    import { TheSidebar, TheHeader } from '@/components/layout';
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

<style scoped>
    .el-header {
        padding: 0px;
    }
    
    .el-container {
        flex: 1;
    }

    .el-main {
        max-height: calc(100vh - 60px);
    }
</style>