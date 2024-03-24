<script setup>
import { useAuthStore } from '@/stores';
import { router } from '@/router';

// redirect home if already logged in
const authStore = useAuthStore();
if (authStore.loginInfo) {
    router.push('/');
}
</script>

<template>
    <div class="container">
        <router-view />
    </div>
</template>

<style scoped>
    .container {
        width: 100vw;
        height: 100vh;
    }
</style>
