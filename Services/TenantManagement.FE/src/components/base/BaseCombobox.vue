<template>
  <div
    class="cbox"
    :class="[
      isrequired ? 'field--required' : '',
      noti.length > 0 ? 'error-noti' : '',
    ]"
    v-on-click-outside="cboxOnClickOutside"
  >
    <div class="cbox__label label">{{ label }}</div>
    <div class="cbox__select">
      <div class="select__box">
        <div class="select__input">
          <input
            type="text"
            :placeholder="pholder"
            :value="text"
            ref="refInput"
            @input="$emit('update:text', $event.target.value)"
            @keyup="inputKeyupHandler"
            @keypress="inputKeyPressHandler"
            @keydown.down.prevent="inputArrowDownHandler"
            @keydown.up.prevent="inputArrowUpHandler"
            @keydown.enter.prevent="inputEnterHandler"
          />
        </div>
        <button
          class="select__button"
          tabindex="-1"
          @click="selectButtonOnClick"
        >
          <i
            :class="[
              cbox.isOptionboxOpen
                ? 'fas fa-chevron-up'
                : 'fas fa-chevron-down',
            ]"
          ></i>
        </button>
      </div>
      <div
        :class="[cbox.isOptionboxOpen ? '' : 'display--none']"
        class="select__optionbox"
      >
        <div class="loader__container" v-show="cbox.isLoading">
          <BaseLoader />
        </div>
        <div
          class="optionlist"
          v-show="!cbox.isLoading"
          :class="[optionListDisplay.length > 5 ? 'hascrollbar' : '']"
          :style="{ maxHeight: mxheight > 0 ? mxheight + 'px' : '308px' }"
        >
          <template v-for="item in optionListDisplay" :key="item[schema.id]">
            <div
              class="option__item"
              @click="optionOnClick($event, item[schema.id], item[schema.name])"
              :class="[
                item[schema.id] == selectedItemId ? 'item--selected' : '',
                cbox.cusorItemId != null &&
                item[schema.id] ==
                  optionListDisplay[cbox.cusorItemId][schema.id]
                  ? 'item--highlighted'
                  : '',
              ]"
            >
              <div class="option__text">{{ item[schema.name] }}</div>
              <div class="option__icon"></div>
            </div>
          </template>
          <div v-show="optionListDisplay.length == 0" class="option__item">
            <div class="option__text">
              Không có dữ liệu
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="cbox__noti noti">{{ noti }}</div>
  </div>
</template>

<script setup>
//#region import
import { ref, inject } from "vue";
import { vOnClickOutside } from "@vueuse/components";
//#endregion

//#region init
// Luu cac bien setTimeout
const typingTimers = [];
const timeoutVal = 500;
const optionIdHide = ref([]);
const optionListDisplay = ref([]);
const refInput = ref(null);

const props = defineProps({
  label: String,
  text: String,
  isrequired: Boolean,
  selectedItemId: String,
  optionList: Array,
  noti: String,
  pholder: String,
  schema: Object,
  mxheight: Number,
});

/*
schema = {
  id: "EmployeeId",
  name: "EmployeeName"
}
*/

const emits = defineEmits([
  "update:text",
  "update:noti",
  "update:selectedItemId",
]);
defineExpose({ refInput });

const cbox = ref({
  isOptionboxOpen: false,
  isLoading: false,
  cusorItemId: null,
  hasScrollbar: false,
});

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

/**
 * Hàm tìm kiếm đơn vị
 * @param {String} input Tên đơn vị
 *
 * Author: Dũng (08/05/2023)
 */
function filterData(input) {
  //const IdHideList = [];
  const optionDisplayList = [];
  for (let i = 0; i < props.optionList.length; ++i) {
    if (
      props.optionList[i][props.schema.name]
        .toLowerCase()
        .includes(input.toLowerCase().trim())
    ) {
      //IdHideList.push(i);
      optionDisplayList.push(props.optionList[i]);
    }
  }
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve(optionDisplayList);
    }, 200);
  });
}

//#endregion

//#region handle event

/**
 * Sự kiện click ouside combobox
 *
 * Author: Dũng (10/06/2023)
 */
function cboxOnClickOutside() {
  if (cbox.value.isOptionboxOpen) {
    cbox.value.isOptionboxOpen = false;
    if (props.text.length != 0 && props.selectedItemId.length == 0) {
      emits("update:text", "");
    }
  }
}

/**
 * Sự kiện nhấn Enter tại ô input
 *
 * Author: Dũng (03/06/2023)
 */
function inputEnterHandler() {
  if (cbox.value.cusorItemId != null) {
    // Update dữ liệu lên Form Object
    emits(
      "update:text",
      optionListDisplay.value[cbox.value.cusorItemId][props.schema.name]
    );
    emits("update:noti", "");

    // Đóng optionbox
    cbox.value.isOptionboxOpen = false;
    emits(
      "update:selectedItemId",
      optionListDisplay.value[cbox.value.cusorItemId][props.schema.id]
    );
  }
}

/**
 * Sự kiện nhấn arrow up tại ô input
 *
 * Author: Dũng (03/06/2023)
 */
function inputArrowUpHandler() {
  if (cbox.value.isOptionboxOpen && cbox.value.cusorItemId > 0)
    --cbox.value.cusorItemId;
}

/**
 * Focus vào một dòng trong optionbox
 *
 * Author: Dũng (03/06/2023)
 */
function focusOnARow() {
  for (let i = 0; i < optionListDisplay.value.length; ++i) {
    if (optionListDisplay.value[i][props.schema.id] == props.selectedItemId) {
      cbox.value.cusorItemId = i;
      break;
    }
  }
  // if (cbox.value.cusorItemId == null) cbox.value.cusorItemId = 0;
}

/**
 * Sự kiện nhấn arrow down tại ô input
 *
 * Author: Dũng (03/06/2023)
 */
function inputArrowDownHandler() {
  if (!cbox.value.isOptionboxOpen) {
    cbox.value.isLoading = false;
    cbox.value.cusorItemId = 0;
    cbox.value.isOptionboxOpen = true;
    optionListDisplay.value = props.optionList;
    focusOnARow();
  } else {
    if (cbox.value.cusorItemId == null) cbox.value.cusorItemId = 0;
    if (cbox.value.cusorItemId < optionListDisplay.value.length - 1)
      ++cbox.value.cusorItemId;
  }
}

/**
 * Sự kiện click vào item trong danh sách combobox option
 *
 * @param {Object} _$event biến sự kiện
 * @param {String} optionId Option Id
 * @param {String} optionName Option Name
 *
 * Author: Dũng (08/05/2023)
 */
function optionOnClick(_$event, optionId, optionName) {
  // Update dữ liệu lên Form Object
  emits("update:text", optionName);
  emits("update:noti", "");

  // Đóng optionbox
  cbox.value.isOptionboxOpen = false;
  emits("update:selectedItemId", optionId);
}

/**
 * Sự kiện click vào mũi tên mở rộng combobox
 *
 * Author: Dũng (08/05/2023)
 */
function selectButtonOnClick() {
  // Kiểm tra xem chọn đúng đơn vị trong danh sách
  if (cbox.value.isOptionboxOpen == true) {
    optionIdHide.value = [];
    optionListDisplay.value = props.optionList;
    cbox.value.isOptionboxOpen = false;
    if (props.text.length != 0 && props.selectedItemId.length == 0) {
      emits("update:text", "");
    }
  } else {
    cbox.value.isOptionboxOpen = true;
    cbox.value.isLoading = false;
    cbox.value.cusorItemId = null;
    optionListDisplay.value = props.optionList;
    refInput.value.focus();
    focusOnARow();
  }
}

/**
 * Sự kiện Keyup ô Input
 * @param {Object} $event biến sự kiện
 *
 * Author: Dũng (08/05/2023)
 */

function inputKeyupHandler($event) {
  if (!isNormalCharacterKey($event.key)) return;
  // Nếu combobox là requied thì khi xóa hết text sẽ thông báo lỗi
  if (props.isrequired) {
    if (props.text.length == 0 && $event.key == "Backspace") {
      emits("update:noti", `${props.label} không được để trống`);
    } else {
      // Khi typing thì tắt thông báo lỗi
      emits("update:noti", "");
    }
  }

  // 500ms sau khi Typing tự động mở optionBox
  if (cbox.value.isOptionboxOpen == false && $event.key != "Tab") {
    setTimeout(() => {
      cbox.value.isOptionboxOpen = true;
    }, 500);
  }
  // Xóa các timeout trước trong khi typing
  while (typingTimers.length > 0) {
    clearTimeout(typingTimers[0]);
    typingTimers.splice(0, 1);
  }

  typingTimers.push(
    setTimeout(() => {
      // Display loading
      cbox.value.isLoading = true;
      emits("update:selectedItemId", "");
      filterData(props.text).then((optionDisplayList) => {
        optionListDisplay.value = optionDisplayList;
        cbox.value.isLoading = false;
        if (optionDisplayList.length) cbox.value.cusorItemId = 0;
        else cbox.value.cusorItemId = null;
      });
    }, timeoutVal)
  );
}

/**
 * Sự kiện Typing vào ô input
 * Author: Dũng (08/05/2023)
 */
function inputKeyPressHandler($event) {
  if (!isNormalCharacterKey($event.key)) return;
  cbox.value.isLoading = true;
  // Xóa setTimeout đã tạo từ tước
  while (typingTimers.length > 0) {
    clearTimeout(typingTimers[0]);
    typingTimers.splice(0, 1);
  }
}

//#endregion
</script>

<style
  scoped
  lang="css"
  src="@/assets/css/components/base/base-combobox.css"
></style>
