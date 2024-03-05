<template>
  <div
    class="dpicker"
    :class="[noti.length > 0 ? 'error-noti' : '']"
    v-on-click-outside="dpickerOnClickOutside"
  >
    <div class="dpicker__label">{{ label }}</div>
    <div class="dpicker__selector">
      <div class="dpicker__input">
        <input
          :disabled="blocked"
          type="text"
          :placeholder="pholder.length > 0 ? pholder : $formatter.dateFormat"
          :value="inputText"
          ref="refInput"
          @input="$emit('update:inputText', $event.target.value)"
          @keyup="inputKeyupHandler"
        />
        <div
          class="dpicker__icon mi mi-24 mi-calendar"
          @click="miCalendarOnClick"
        ></div>
      </div>
    </div>
    <div v-show="isBoxOpen" class="dpicker__box">
      <div class="dpicker__header">
        <div class="dp__header__left">
          <div class="dp__date">{{ boxText }}</div>
          <div
            class="dp__expandic mi mi-14"
            :class="boxStatus == 0 ? 'mi-arrowdown' : 'mi-arrowup'"
            @click="expandIcOnClick"
          ></div>
        </div>
        <div class="dp__header__right">
          <div
            class="dp__prev mi mi-24 mi-arrowleft"
            @click="prevOnClick"
          ></div>
          <div
            class="dp__next mi mi-24 mi-arrowright"
            @click="nextOnClick"
          ></div>
        </div>
      </div>
      <div class="dpicker__body">
        <div v-show="boxStatus == 2" class="dpicker__yearlist">
          <div class="yearlist__tablebox">
            <table>
              <tbody>
                <tr v-for="i in 3" :key="i">
                  <td
                    v-for="j in 4"
                    :key="j"
                    @click="
                      yearItemOnClick(
                        $event,
                        yearRangeNow + 4 * (i - 1) + j - 1
                      )
                    "
                  >
                    <div class="year-item">
                      {{ yearRangeNow + 4 * (i - 1) + j - 1 }}
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div class="dpicker__cancelbox">
            <button class="dp__cancel" @click="cancelBtnOnClick">
              Hủy
            </button>
          </div>
        </div>
        <div v-show="boxStatus == 1" class="dpicker__yearlist">
          <div class="yearlist__tablebox">
            <table>
              <tbody>
                <tr v-for="i in 3" :key="i">
                  <td
                    v-for="j in 4"
                    :key="j"
                    @click="monthItemOnClick($event, 4 * (i - 1) + j)"
                  >
                    <div class="month-item">
                      Tháng {{ 4 * (i - 1) + j }}
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div class="dpicker__cancelbox">
            <button class="dp__cancel" @click="cancelBtnOnClick">
              Hủy
            </button>
          </div>
        </div>
        <div v-show="boxStatus == 0" class="dpicker__daylist">
          <div class="daylist__table">
            <table>
              <thead>
                <tr>
                  <th>T2</th>
                  <th>T3</th>
                  <th>T4</th>
                  <th>T5</th>
                  <th>T6</th>
                  <th>T7</th>
                  <th>CN</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="i in calendar.rowAmount" :key="i">
                  <td
                    v-for="j in 7"
                    :key="j"
                    :class="{
                      chooseable: cell[7 * (i - 1) + j - 1] > 0,
                      selected:
                        cell[7 * (i - 1) + j - 1] == curDay &&
                        curMonth == realMonth &&
                        curYear == realYear,
                    }"
                    @click="dateItemOnClick($event, 7 * (i - 1) + j - 1)"
                  >
                    <div class="date__item">
                      {{
                        cell[7 * (i - 1) + j - 1] > 0
                          ? cell[7 * (i - 1) + j - 1]
                          : ""
                      }}
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div class="dpicker__curdate">
            <button class="dp__now" @click="todayBtnOnClick">
              Hôm nay
            </button>
          </div>
        </div>
      </div>
    </div>
    <div class="dpicker__noti noti">{{ noti }}</div>
  </div>
</template>

<script setup>
//#region import
import { ref, inject } from "vue";
import $formatter from "@/common/formatter";
import { vOnClickOutside } from "@vueuse/components";

//#endregion

//#region init
const refInput = ref(null);
const props = defineProps({
  blocked: Boolean,
  label: String,
  pholder: String,
  inputText: String,
  noti: String,
});

const emits = defineEmits(["update:inputText", "update:noti"]);
defineExpose({ refInput });
const isBoxOpen = ref(false);
const calendar = ref({
  rowAmount: 0,
});

// 0 -> Day 1-> Month 2 -> Year
const boxStatus = ref(0);
var date = new Date();
const yearRangeNow = ref(date.getFullYear());
const boxText = ref("");
const curYear = ref(0);
const curMonth = ref(0);
const curDay = ref(-1);
const realYear = ref(-1);
const realMonth = ref(-1);
const realDay = ref(-1);
const cell = ref([]);
const selectedIdx = ref(-1);
const maxYearLimit = date.getFullYear() + 12;
const minYearLimit = date.getFullYear() - 150;

//#endregion

//#region function

/**
 * Gán ngày hiển thị bằng ngày đã chọn
 *
 * Author: Dũng (08/05/2023)
 */
function assignRealtoCur() {
  curDay.value = realDay.value;
  curMonth.value = realMonth.value;
  curYear.value = realYear.value;
}

/**
 * Gán ngày đã chọn bằng ngày hiển thị
 *
 * Author: Dũng (08/05/2023)
 */
function assignCurToReal() {
  realDay.value = curDay.value;
  realMonth.value = curMonth.value;
  realYear.value = curYear.value;
}

/**
 * Cập nhật lại giá trị ngày cho cell
 * @param {Number} year Id Đơn vị
 * @param {Number} month Tên đơn vị
 *
 * Author: Dũng (08/05/2023)
 */
function updateCell(year, month) {
  const fd = new Date(year, month - 1, 1);

  //0-7
  const firstDay = fd.getDay();

  const dc = new Date(year, month, 0);
  const dateCount = dc.getDate();
  cell.value = [];
  let day = 0;
  let i = 0;
  calendar.value.rowAmount = 0;
  while (day < dateCount) {
    if (i >= firstDay - 1) ++day;
    cell.value[i] = day;
    if (i % 7 == 0) ++calendar.value.rowAmount;
    ++i;
  }
}

//#endregion

//#region handle event

/**
 * Sự kiện click outside dpicker box
 *
 * Author: Dũng (11/06/2023)
 */
function dpickerOnClickOutside() {
  if (isBoxOpen.value) isBoxOpen.value = false;
}

/**
 * Sự kiện nhập vào ô input
 *
 * Author: Dũng (08/05/2023)
 */
function inputKeyupHandler() {
  emits("update:noti", "");
}

/**
 * Sự kiện click vào chọn ngày hôm nay
 *
 * Author: Dũng (08/05/2023)
 */
function todayBtnOnClick() {
  date = new Date();
  realYear.value = date.getFullYear();
  realMonth.value = date.getMonth() + 1;
  realDay.value = date.getDate();
  assignRealtoCur();
  // Update ngày
  updateCell(realYear.value, realMonth.value);
  yearRangeNow.value = date.getFullYear();
  boxText.value = `Tháng ${realMonth.value}, ${realYear.value}`;
  isBoxOpen.value = false;
  emits(
    "update:inputText",
    $formatter.formatDate(realDay.value, realMonth.value, realYear.value)
  );
}

/**
 * Sự kiện click vào btn hủy chọn ngày
 * Author: Dũng (08/05/2023)
 */
function cancelBtnOnClick() {
  assignRealtoCur();
  boxStatus.value = 0;
  boxText.value = `Tháng ${realMonth.value}, ${realYear.value}`;
}

/**
 * Sự kiện click vào ngày trong lịch
 *
 * @param {Object} _e biến sự kiện
 * @param {String} dateChooseIdx Index của ngày được chọn trong cells
 *
 * Author: Dũng (08/05/2023)
 */
function dateItemOnClick(_e, dateChoosedIdx) {
  if (cell[dateChoosedIdx] == 0) {
    return;
  }
  curDay.value = cell.value[dateChoosedIdx];
  assignCurToReal();
  isBoxOpen.value = false;
  emits(
    "update:inputText",
    $formatter.formatDate(realDay.value, realMonth.value, realYear.value)
  );
}

/**
 * Sự kiện click vào tháng trong lịch
 * @param {Object} _e biến sự kiện
 * @param {Number} monthChoosed tháng được chọn
 *
 * Author: Dũng (08/05/2023)
 */
function monthItemOnClick(_e, monthChoosed) {
  curMonth.value = monthChoosed;
  boxText.value = `Tháng ${curMonth.value}, ${curYear.value}`;
  updateCell(curYear.value, curMonth.value);
  boxStatus.value = 0;
}

/**
 * Sự kiện click vào năm trong lịch
 * @param {Object} _ biến sự kiện
 * @param {String} yearChoosed năm được chọn
 *
 * Author: Dũng (08/05/2023)
 */
function yearItemOnClick(_e, yearChoosed) {
  curYear.value = yearChoosed;
  boxText.value = `Năm ${yearChoosed}`;
  boxStatus.value = 1;
}

/**
 * Sự kiện click vào item calendar
 *
 * Author: Dũng (08/05/2023)
 */
function miCalendarOnClick() {
  if (props.blocked == true) return;
  // Init
  if (!isBoxOpen.value) {
    emits("update:noti", "");
    if ($formatter.isValidDate(props.inputText)) {
      // Nếu ngày tháng nhập vào hợp lệ
      // Reset ngày tháng năm về ngày tháng đã nhập
      const { day, month, year } = $formatter.stringToDmy(props.inputText);
      realDay.value = day;
      realMonth.value = month;
      realYear.value = year;
      assignRealtoCur();
      // Update ngày
      yearRangeNow.value = curYear.value;
      boxText.value = `Tháng ${realMonth.value}, ${realYear.value}`;
      boxStatus.value = 0;
      emits(
        "update:inputText",
        $formatter.formatDate(realDay.value, realMonth.value, realYear.value)
      );
      updateCell(realYear.value, realMonth.value);
    } else {
      // Nếu ngày tháng nhập vào không hợp lệ hoặc để trống
      // Reset input text
      emits("update:inputText", "");
      // Reset ngày tháng năm về hiện tại
      realYear.value = date.getFullYear();
      realMonth.value = date.getMonth() + 1;
      realDay.value = date.getDate();
      assignRealtoCur();
      // Update ngày
      updateCell(realYear.value, realMonth.value);
      yearRangeNow.value = date.getFullYear();
      boxText.value = `Tháng ${realMonth.value}, ${realYear.value}`;
      boxStatus.value = 0;
    }
  }
  isBoxOpen.value = !isBoxOpen.value;
}

/**
 * Sự kiện click vào nút chọn năm, tháng
 *
 * Author: Dũng (08/05/2023)
 */
function expandIcOnClick() {
  // Lịch -> Năm
  if (boxStatus.value == 0) {
    boxText.value = `${yearRangeNow.value} - ${yearRangeNow.value + 11}`;
    boxStatus.value = 2;
  } else {
    // Năm, Tháng -> Lịch
    assignRealtoCur();
    boxText.value = `Tháng ${realMonth.value}, ${realYear.value}`;
    boxStatus.value = 0;
  }
}

/**
 * Sự kiện click vào nút prev
 *
 * Author: Dũng (08/05/2023)
 */
function prevOnClick() {
  // Back Năm
  if (boxStatus.value == 2) {
    if (yearRangeNow.value > minYearLimit) {
      yearRangeNow.value -= 12;
      boxText.value = `${yearRangeNow.value} - ${yearRangeNow.value + 11}`;
    }
  }
  if (boxStatus.value == 0) {
    // Back Lịch
    if (curMonth.value == 1) {
      curMonth.value = 12;
      curYear.value -= 1;
    } else {
      curMonth.value -= 1;
    }
    boxText.value = `Tháng ${curMonth.value}, ${curYear.value}`;
    selectedIdx.value = -1;
    updateCell(curYear.value, curMonth.value);
  }
  if (boxStatus.value == 1) {
    if (curYear.value > minYearLimit) {
      curYear.value -= 1;
      boxText.value = `Năm ${curYear.value}`;
    }
  }
}

/**
 * Sự kiện click vào nút next
 *
 * Author: Dũng (08/05/2023)
 */
function nextOnClick() {
  if (boxStatus.value == 2) {
    if (yearRangeNow.value < maxYearLimit) {
      yearRangeNow.value += 12;
      boxText.value = `${yearRangeNow.value} - ${yearRangeNow.value + 11}`;
    }
  }
  if (boxStatus.value == 0) {
    // Next lịch
    if (curMonth.value == 12) {
      if (curYear.value < maxYearLimit) {
        curMonth.value = 1;
        curYear.value += 1;
      }
    } else {
      curMonth.value += 1;
    }
    boxText.value = `Tháng ${curMonth.value}, ${curYear.value}`;
    selectedIdx.value = -1;
    updateCell(curYear.value, curMonth.value);
  }
  if (boxStatus.value == 1) {
    if (curYear.value < maxYearLimit) {
      curYear.value += 1;
      boxText.value = `Năm ${curYear.value}`;
    }
  }
}

//#endregion
</script>

<style
  scoped
  lang="css"
  src="@/assets/css/components/base/base-datepicker.css"
></style>@/common/formatter
