<template>
  <div class="notification">
    <p>404 - Trang không tồn tại</p>
    <el-button type="primary" @click="goBack">Quay lại</el-button>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router';

const router = useRouter();

const goBack = () => {
  router.go(-1);
};
</script>

<style scoped>
.notification {
  display: flex;
  justify-content: center;
  align-items: center;
  flex-direction: column;
  height: 100%;
  width: 100%;
  font-size: 2em;
  font-weight: bold;
  color: #333;
  background-color: #d9d9d9;
  text-transform: uppercase;
  letter-spacing: 1px;
}

</style>