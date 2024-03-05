<template>
  <div class="ecb">
    <div class="ecb__label">{{ label }}</div>
    <div class="ecb__main" :class="[isTableOpen ? 'active' : '']">
      <div class="main__selected">
        <div class="selected__input">
          <input
            type="text"
            ref="refInput"
            :value="selectedItemName"
            @input="$emit('update:selectedItemName', $event.target.value)"
            @keyup="inputKeyupHandler"
            @keypress="inputKeyPressHandler"
          />
        </div>
      </div>
      <div class="main__action">
        <div class="btn__add">
          <div class="btn__add__icon mi mi-12 mi-add--green"></div>
        </div>
        <button class="btn__dropdown" @click="btnDropDownOnClick">
          <i class="fas fa-chevron-down"></i>
        </button>
      </div>
    </div>
    <div class="ecb__table" v-show="isTableOpen">
      <div class="table__header">
        <table class="t__header">
          <thead>
            <tr>
              <th v-for="tSchema in tableSchema" :key="tSchema.prop">
                <div class="th__text align--left">{{ tSchema.name }}</div>
              </th>
            </tr>
          </thead>
        </table>
      </div>
      <div
        class="table__content"
        @scroll="tableContentOnScroll($event)"
        ref="talbeContentRef"
      >
        <div class="t__loader" v-show="isLoadingData">
          <BaseLoader />
        </div>
        <table class="t__content" v-show="!isLoadingData">
          <colgroup>
            <col
              v-for="tSchema in tableSchema"
              :key="tSchema.prop"
              span="1"
              :style="{ width: tSchema.width + '%' }"
            />
          </colgroup>
          <tbody>
            <tr
              v-for="(item, index) in itemList"
              :key="item[itemSchema[0]]"
              @click="trOnClick(index)"
              :class="[item[itemSchema[0]] == selectedItemId ? 'selected' : '']"
            >
              <td v-for="tSchema in tableSchema" :key="tSchema.prop">
                <div class="td__text align--left">
                  {{ item[tSchema.prop] }}
                </div>
              </td>
            </tr>
            <tr v-show="itemList.length == 0">
              <div class="no__data">Không có dữ liệu</div>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, inject, nextTick } from "vue";
const refInput = ref(null);
const isTableOpen = ref(false);
const $axios = inject("$axios");
const itemList = ref([]);
const typingTimers = [];
const timeoutVal = 500;
const isLoadingData = ref(false);
const talbeContentRef = ref(null);
const selectedItemIndex = ref(0);

const props = defineProps({
  label: String,
  selectedItemId: String,
  selectedItemName: String,
  fetchApi: String,
  itemSchema: Object,
  tableSchema: Object,
});

/*
  tableSchema: [
    {
      name: "Mã nhân viên",
      prop: "employeeCode",
    },
    {
      name: "Tên nhân viên",
      prop: "employeeFullName",
    }
  ]
*/
/*
  itemSchema: [
    "employeeId",
    "employeeCode",
    "employeeFullName",
  ]
*/
const emits = defineEmits(["update:selectedItemId", "update:selectedItemName"]);

async function fetchNewItem(skip, take, keySearch, reload) {
  const response = await $axios.get(props.fetchApi, {
    params: {
      skip: skip,
      take: take,
      keySearch: keySearch,
    },
  });
  if (reload) itemList.value = [];
  for (const item of response.data.filteredList) {
    itemList.value.push(item);
  }
  // console.log(itemList.value);
}

async function tableContentOnScroll(e) {
  let {
    target: { scrollTop, clientHeight, scrollHeight },
  } = e;
  if (scrollTop + clientHeight >= scrollHeight) {
    await fetchNewItem(
      itemList.value.length,
      10,
      props.selectedItemName,
      false
    );
  }
}

async function btnDropDownOnClick() {
  if (!isTableOpen.value && itemList.value.length == 0) {
    await fetchNewItem(
      itemList.value.length,
      10,
      props.selectedItemName,
      false
    );
  }
  isTableOpen.value = !isTableOpen.value;
  if (isTableOpen.value && props.selectedEmployeeId != "") {
    await nextTick();
    talbeContentRef.value.scrollTo({
      top: 38 * selectedItemIndex.value,
      behavior: "smooth",
    });
  }
}

function inputKeyupHandler($event) {
  if (!isNormalCharacterKey($event.key)) return;

  // 500ms sau khi Typing tự động mở optionBox
  if (isTableOpen.value == false && $event.key != "Tab") {
    setTimeout(() => {
      isTableOpen.value = true;
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
      isLoadingData.value = true;
      emits("update:selectedItemId", "");
      fetchNewItem(0, 20, props.selectedItemName, true);
      isLoadingData.value = false;
    }, timeoutVal)
  );
}

/**
 * Sự kiện Typing vào ô input
 * Author: Dũng (08/05/2023)
 */
function inputKeyPressHandler($event) {
  if (!isNormalCharacterKey($event.key)) return;
  isLoadingData.value = true;
  // Xóa setTimeout đã tạo từ tước
  while (typingTimers.length > 0) {
    clearTimeout(typingTimers[0]);
    typingTimers.splice(0, 1);
  }
}

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

function trOnClick(index) {
  emits("update:selectedItemName", itemList.value[index][props.itemSchema[2]]);
  emits("update:selectedItemId", itemList.value[index][props.itemSchema[0]]);
  selectedItemIndex.value = index;
  isTableOpen.value = false;
}
</script>

<style
  scoped
  lang="css"
  src="@/assets/css/components/base/base-combobox-table.css"
></style>
