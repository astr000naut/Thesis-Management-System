<template>
  <div
    class="sbox"
    :class="[
      isrequired ? 'field--required' : '',
      noti.length > 0 ? 'error-noti' : '',
      isActive ? '' : 'disabl',
    ]"
    v-on-click-outside="sboxOnClickOutside"
  >
    <div class="sbox__label label" v-if="label.length != 0">{{ label }}</div>
    <div class="sbox__select">
      <div class="select__box">
        <div class="select__input">
          {{
            isActive
              ? selectedItemId != -1
                ? optionList[selectedItemId]
                : ""
              : ""
          }}
        </div>
        <button
          :disabled="blocked"
          class="select__button"
          tabindex="-1"
          @click="selectButtonOnClick"
        >
          <i
            :class="[
              sbox.isOptionboxOpen
                ? 'fas fa-chevron-up'
                : 'fas fa-chevron-down',
            ]"
          ></i>
        </button>
      </div>
      <div
        :class="[sbox.isOptionboxOpen ? '' : 'display--none']"
        class="select__optionbox"
      >
        <div class="loader__container" v-show="sbox.isLoading">
          <BaseLoader />
        </div>
        <div
          class="optionlist"
          v-show="!sbox.isLoading"
          :class="[sbox.hasScrollbar ? 'hascrollbar' : '']"
        >
          <template v-for="(optionName, index) in optionList" :key="index">
            <div
              class="option__item"
              @click="optionOnClick($event, index, optionName)"
              :class="[index == selectedItemId ? 'item--selected' : '']"
            >
              <div class="option__text">{{ optionName }}</div>
              <div class="option__icon"></div>
            </div>
          </template>
        </div>
      </div>
    </div>
    <div class="sbox__noti noti">{{ noti }}</div>
  </div>
</template>

<script setup>
//#region import
import { ref } from "vue";
import { vOnClickOutside } from "@vueuse/components";
//#endregion

//#region init

const props = defineProps({
  blocked: Boolean,
  isActive: Boolean,
  label: String,
  isrequired: Boolean,
  selectedItemId: Number,
  optionList: Array,
  noti: String,
  pholder: String,
});

const emits = defineEmits(["update:noti", "update:selectedItemId"]);

const sbox = ref({
  isOptionboxOpen: false,
  isLoading: false,
  cusorItemId: null,
  hasScrollbar: false,
});

//#endregion

//#region function
//#endregion

//#region handle event

/**
 * Sự kiện click ouside combobox
 *
 * Author: Dũng (10/06/2023)
 */
function sboxOnClickOutside() {
  if (sbox.value.isOptionboxOpen) {
    sbox.value.isOptionboxOpen = false;
  }
}

/**
 * Sự kiện click vào item trong danh sách combobox option
 *
 * @param {Object} _$event biến sự kiện
 * @param {String} index Option Id
 * @param {String} optionName Option Name
 *
 * Author: Dũng (08/05/2023)
 */
function optionOnClick(_$event, index) {
  // Đóng optionbox
  sbox.value.isOptionboxOpen = false;
  emits("update:selectedItemId", index);
}

/**
 * Sự kiện click vào mũi tên mở rộng combobox
 *
 * Author: Dũng (08/05/2023)
 */
function selectButtonOnClick() {
  if (props.isActive) {
    sbox.value.isOptionboxOpen = !sbox.value.isOptionboxOpen;
  }
}
//#endregion
</script>

<style
  scoped
  lang="css"
  src="@/assets/css/components/base/base-selectbox.css"
></style>
