<template>
  <div class="dialog">
    <div class="dialog__bar">
      <div class="dialog__heading">{{ title }}</div>
      <BaseButton
        ref="closeBtn"
        bname=""
        class="mi-36 btn--close"
        @click="closeOnClick"
        @keydown.shift.tab.prevent="closeBtnOnShiftTab"
      />
    </div>
    <div class="dialog__content">
      {{ message }}
    </div>
    <div class="dialog__footer">
      <BaseButton
        ref="noBtn"
        bname="Không"
        class="btn--secondary"
        @click="noOnClick"
      />
      <BaseButton
        ref="yesBtn"
        bname="Có"
        class="btn--primary"
        @click="yesOnClick"
        @keydown.tab.prevent="yesBtnOnTab"
        @keydown.shift.tab.prevent="yesBtnOnShiftTab"
      />
    </div>
  </div>
</template>

<script setup>
//#region import
import { ref } from "vue";
const yesBtn = ref(null);
const closeBtn = ref(null);
const noBtn = ref(null);
//#endregion
//#region init
defineProps({
  title: String,
  message: String,
  closeOnClick: Function,
  yesOnClick: Function,
  noOnClick: Function,
});
defineExpose({ yesBtn });
//#endregion

//#region handle event
/**
 * Sự kiện tab vào nút yes
 * Author: Duxng(03/06/2023)
 */
function yesBtnOnTab() {
  closeBtn.value.refBtn.focus();
}

/**
 * Sự kiện shift tab vào nút close
 * Author: Duxng(03/06/2023)
 */
function closeBtnOnShiftTab() {
  yesBtn.value.refBtn.focus();
}

/**
 * Sự kiện shift tab vào nút yes
 * Author: Duxng(03/06/2023)
 */
function yesBtnOnShiftTab() {
  noBtn.value.refBtn.focus();
}
//#endregion
</script>

<style
  scoped
  lang="css"
  src="@/assets/css/components/base/base-dialog.css"
></style>
