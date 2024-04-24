<template>
    <div class="logout-container">...Signing out</div>
</template>

<script setup>
import * as Yup from 'yup';
import { ref } from 'vue';

import { useAuthStore } from '@/stores';
const authStore = useAuthStore();

authStore.logout();

</script>

<style scoped>
</style>
