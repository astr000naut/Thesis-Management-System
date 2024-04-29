<template>
  <div class="header">
    <div class="header__mainarea">
      <div class="header__leftarea">
        <Icon icon="mingcute:book-5-line" style="width: 32px; height: 32px; color: #409EFF;"/>
        <div class="header__title">
          <div class="header__brandname" style="font-weight: bold;">
            HỆ THỐNG QUẢN LÝ KHÓA LUẬN TỐT NGHIỆP - {{ authStore.tenantBaseInfo.tenantName }}
          </div>
        </div>
      </div>
      <div class="header__rightarea">
        <div class="header__user">
          <el-dropdown>
            <div class="el-dropdown-link" style="outline: unset;font-weight: bold;display: flex;column-gap: 4px;">
              <div class="name">{{ authStore.loginInfo.user.fullName }}</div>
              <div class="icon"><Icon class="item__icon" icon="ri:arrow-down-s-fill"/></div>
            </div>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item
                  @click="showChangePassword = true"
                >Đổi mật khẩu</el-dropdown-item>
                <el-dropdown-item divided
                  @click="authStore.logout"
                >Đăng xuất</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
      </div>
    </div>
    <ChangePasswordPopup
      v-model:visible="showChangePassword"
    />
  </div>
</template>

<script setup>
// #region import
import { ref, inject, onBeforeUnmount } from "vue";
import { useAuthStore } from "@/stores";
import ChangePasswordPopup from "./ChangePasswordPopup.vue";
const authStore = useAuthStore();
const showChangePassword = ref();

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
>
</style>
