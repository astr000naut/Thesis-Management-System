<template>
  <div
    class="txtfield"
    :class="[
      isrequired ? 'field--required' : '',
      noti.length > 0 ? 'error-noti' : '',
    ]"
  >
    <div
      v-show="!hideLabel"
      class="txtfield__label label"
      @mouseenter="labelOnMouseEnter"
      @mouseleave="labelOnMouseOut"
    >
      {{ label }}
    </div>
    <div class="txtfield__tooltip" v-show="tooltip != null && showTooltip">
      {{ tooltip }}
    </div>
    <div class="txtfield__textbox">
      <input
        :disabled="blocked"
        :maxlength="type == 'money' ? 18 : 255"
        class="txtfield__input"
        :class="[textRight ? 'input-text-right' : '']"
        type="text"
        :placeholder="pholder"
        @focus="inputOnFocus"
        @blur="inputOnBlur"
        :value="text"
        ref="refInput"
        @input="textFieldOnInput($event)"
        @keyup="inputKeyupHandler"
        @keypress="inputKeyPressHandler"
        @keydown.shift.f8.prevent="autoFill"
      />
      <div class="txtfield__icon"></div>
      <div class="txtfield__noti noti">{{ noti }}</div>
    </div>
  </div>
</template>

<script setup>
//#region import
import { ref } from "vue";
import numberFormatter from "@/common/number_formatter";
//#endregion

//#region init
const props = defineProps({
  blocked: Boolean,
  pholder: String,
  label: String,
  text: String,
  isrequired: Boolean,
  tooltip: String,
  hideLabel: Boolean,
  noti: String,
  type: String,
  realTimeSearch: Boolean,
  doSearch: Function,
  autoFill: Function,
  autoFillMessage: String,
  textRight: Boolean,
});
const typingTimers = [];
const timeoutVal = 500;
const refInput = ref(null);
const showTooltip = ref(false);
const emits = defineEmits(["update:text", "update:noti"]);
defineExpose({ refInput });

//#endregion

//#region function

/**
 * Kiểm tra keypress có là ký tự text bình thường không
 * @param {String} key là $event.key
 *
 * Author: Dũng(12/05/2023)
 */
function isNormalCharacterKey(key) {
  return key.length == 1 || key == "Backspace" ? true : false;
}

//#endregion

//#region handle event
/**
 * Sự kiện focus vào ô input
 *
 * Author: Dũng(25/05/2023)
 */
function inputOnFocus() {
  if (refInput.value) refInput.value.placeholder = props.autoFillMessage ?? "";
}

/**
 * Sự kiện blur ô input
 *
 * Author: Dũng(25/05/2023)
 */
function inputOnBlur() {
  if (refInput.value) refInput.value.placeholder = props.pholder;
}

/**
 * Sự kiện keyup cho ô input
 *
 * @param {Object} $event biến sự kiện
 *
 * Author: Dũng (08/05/2023)
 */
function inputKeyupHandler($event) {
  if ($event.key.length == 1) emits("update:noti", "");
  if (props.isrequired) {
    if (props.text.length == 0 && $event.key == "Backspace") {
      emits("update:noti", `${props.label} không được để trống`);
    }
  } else {
    if (props.text.length == 0 && $event.key == "Backspace") {
      emits("update:noti", "");
    }
  }
  if (props.text.length != 0 && $event.key == "Backspace") {
    emits("update:noti", "");
  }
  if (props.type == "money") {
    let s = numberFormatter.format(
      $event.target.value.toString().replace(/[^\d]/g, "")
    );
    emits("update:text", s);
  }
  if (isNormalCharacterKey($event.key) && props.realTimeSearch) {
    // Xóa các timeout trước trong khi typing
    while (typingTimers.length > 0) {
      clearTimeout(typingTimers[0]);
      typingTimers.splice(0, 1);
    }

    typingTimers.push(
      setTimeout(() => {
        // Display loading
        props.doSearch(props.text);
      }, timeoutVal)
    );
  }
}

/**
 * Sự kiện Typing vào ô input
 * Author: Dũng (08/05/2023)
 */
function inputKeyPressHandler($event) {
  if (isNormalCharacterKey($event.key) && props.realTimeSearch) {
    // Xóa setTimeout đã tạo từ tước
    while (typingTimers.length > 0) {
      clearTimeout(typingTimers[0]);
      typingTimers.splice(0, 1);
    }
  }
}

function textFieldOnInput(_$event) {
  emits("update:text", _$event.target.value);
}

/**
 * Sự kiện mouse enter vào label
 *
 * Author: Dũng (08/05/2023)
 */
function labelOnMouseEnter() {
  showTooltip.value = true;
}

/**
 * Sự kiện mouse out khỏi label
 *
 * Author: Dũng (08/05/2023)
 */
function labelOnMouseOut() {
  showTooltip.value = false;
}

//#endregion
</script>

<style
  scoped
  lang="css"
  src="@/assets/css/components/base/base-textfield.css"
></style>
