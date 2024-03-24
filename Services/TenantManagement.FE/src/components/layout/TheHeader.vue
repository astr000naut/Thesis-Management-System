<template>
  <div class="header">
    <div class="header__mainarea">
      <div class="header__leftarea">
        <Icon icon="eos-icons:admin-outlined" style="width: 32px; height: 32px; color: #409EFF;"/>
        <div class="header__title">
          <div class="header__brandname" style="font-weight: bold;">
            HỆ THỐNG QUẢN LÝ KHÓA LUẬN TỐT NGHIỆP - ADMIN
          </div>
        </div>
      </div>
      <div class="header__rightarea">
        <div class="header__user">
          <a href="" style="color: blue; text-decoration: underline;" @click="authStore.logout()">Đăng xuất</a>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
// #region import
import { ref, inject, onBeforeUnmount } from "vue";
import { useAuthStore } from "@/stores";
const authStore = useAuthStore();
// #endregion

// #region init

// #endregion

// #region hook


onBeforeUnmount(() => {
});
// #endregion
</script>

<style
  scoped
  lang="css"
  src="@/assets/css/layout/the-header.css"
></style>
