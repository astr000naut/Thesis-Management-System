<template>
  <div class="notibox">
    <div class="notibox__content">
      <div
        class="content__logo"
        :class="[type == 'alert' ? 'logo--alert' : '']"
      ></div>
      <div class="content__text">{{ message }}</div>
    </div>
    <hr />
    <div class="notibox__footer">
      <BaseButton
        ref="yesBtn"
        bname="Đồng ý"
        class="btn--primary"
        @click="yesOnClick"
        @keydown.enter.stop
        @keydown.tab.prevent
      />
    </div>
  </div>
</template>

<script setup>
//#region import
import { ref } from "vue";
//#endregion
//#region init
const yesBtn = ref(null);
defineProps({
  type: String,
  message: String,
  yesOnClick: Function,
});
defineExpose({ yesBtn });
//#endregion
</script>

<style
  scoped
  lang="css"
  src="@/assets/css/components/base/base-notibox.css"
></style>
