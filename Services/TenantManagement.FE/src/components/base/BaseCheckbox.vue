<template>
  <div class="checkbox__main">
    <div
      class="checkbox mi mi-18"
      :class="[checked ? 'mi-checkbox-checked' : '']"
      @click="checkboxOnClick"
    ></div>
    <div class="checkbox__label">{{ label }}</div>
  </div>
</template>

<script setup>
const props = defineProps({
  label: String,
  checked: Boolean,
});
const emits = defineEmits(["update:checked"]);

function checkboxOnClick() {
  emits("update:checked", !props.checked);
}
</script>

<style
  scoped
  lang="css"
  src="@/assets/css/components/base/base-checkbox.css"
></style>
