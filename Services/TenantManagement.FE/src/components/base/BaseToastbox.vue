<template>
  <div class="toast__box">
    <transition-group name="list">
      <div
        v-for="toast in toastList"
        :key="toast.id"
        class="toast"
        :class="`toast--${toast.type}`"
      >
        <div class="left__area">
          <div class="minc mi-24"></div>
          <div class="toast__title">
            <strong>{{
              toast.type == "success"
                ? "Thành công!"
                : toast.type == "alert"
                ? "Cảnh báo!"
                : "Lỗi!"
            }}</strong>
          </div>
          <div class="toast__message">{{ toast.message }}</div>
        </div>
        <div class="right__area">
          <div
            class="mi mi-16 mi-close-small"
            @click="removeToast(toast.id)"
          ></div>
        </div>
      </div>
    </transition-group>
  </div>
</template>

<script setup>
//#region init
defineProps({
  toastList: Array,
  removeToast: Function,
});
//#endregion
</script>

<style
  scoped
  lang="css"
  src="@/assets/css/components/base/base-toastbox.css"
></style>
