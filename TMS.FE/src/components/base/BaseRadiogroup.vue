<template>
  <div class="input--radio">
    <div class="radio__label" v-if="label.length > 0">{{ label }}</div>
    <div class="radio__container">
      <div v-for="option in options" :key="option.value" class="radio__item">
        <input
          type="radio"
          name="gender"
          id=""
          :checked="option.value == radioValue"
          @click="$emit('update:radioValue', option.value)"
        />
        <label for="">{{ option.text }}</label>
      </div>
    </div>
  </div>
</template>

<script setup>
//#region init
defineProps({
  label: String,
  options: Array,
  radioValue: Number,
});
defineEmits(["update:radioValue"]);

//#endregion
</script>

<style
  scoped
  lang="css"
  src="@/assets/css/components/base/base-radiogroup.css"
></style>
