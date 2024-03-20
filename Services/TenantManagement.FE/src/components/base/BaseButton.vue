<template>
  <button :disabled="blocked" ref="refBtn">{{ bname }}</button>
</template>

<script setup>
//#region import
import { ref } from "vue";
//#endregion

//#region  init
defineProps({
  blocked: Boolean,
  // Ten button
  bname: String,
  type: String,
});
// Template reference
const refBtn = ref(null);
defineExpose({ refBtn });
//#endregion
</script>

<style
  scoped
  lang="css"
  src="@/assets/css/components/base/base-button.css"
></style>
