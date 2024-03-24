<template>
  <div class="alert__box">
    <transition-group name="list">
      <div
        v-for="alert in alerts"
        :key="alert.id"
        class="alert"
        :class="`alert--${alert.type}`"
      >
        <div class="left__area">
          <div class="minc mi-24"></div>
          <div class="alert__title">
            <strong>{{
              alert.type == "success"
                ? "Thành công!"
                : alert.type == "alert"
                ? "Cảnh báo!"
                : "Lỗi!"
            }}</strong>
          </div>
          <div class="alert__message">{{ alert.message }}</div>
        </div>
        <div class="right__area">
          <div
            class="mi mi-16 mi-close-small"
            @click="alertStore.remove(alert.id)"
          ></div>
        </div>
      </div>
    </transition-group>
  </div>
</template>

<script setup>
//#region init
  import { useAlertStore } from '@/stores';
import { storeToRefs } from 'pinia';
  const alertStore = useAlertStore();
  const { alerts } = storeToRefs(alertStore);
//#endregion
</script>

<style
  scoped
  lang="css"
  src="@/assets/css/components/base/base-toastbox.css"
></style>
