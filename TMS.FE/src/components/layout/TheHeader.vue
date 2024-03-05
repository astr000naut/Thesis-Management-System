<template>
  <div class="header">
    <div class="header__mainarea">
      <div class="header__leftarea">
        <div class="header__sidebaric mi mi-24 mi-header-three-line"></div>
        <div class="header__title">
          <div class="header__brandname">
            TENANT NAME
          </div>
          <div class="header__title__arrow mi mi-14 mi-arrowdown"></div>
        </div>
      </div>
      <div class="header__rightarea">
        <div
          class="header__noti mi mi-24 mi-header-bell"
          v-tooltip="'Thông báo'"
        ></div>
        <div class="header__user">
          <div class="header__avatar"></div>
          <div class="header__username">Dũng Nguyễn</div>
          <div class="header__user__arrow mi mi-24 mi-arrowdown-small"></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
// #region import
import { ref, inject, onBeforeUnmount } from "vue";
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
