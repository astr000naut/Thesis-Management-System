<template>
  <div
    class="sidebar"
    :style="{ 'flex-basis': isSidebarBig ? '200px' : '78px' }"
  >
    <div class="sidebar__main">
      <router-link
        v-for="(item, index) in sidebarItems"
        :key="item.name"
        :to="item.link"
        active-class="active"
      >
        <div
          class="sidebar__item"
          @mouseover="sideBarItemOnMouseOver($event, item)"
          @mouseleave="item.displayLabel = !isSidebarBig & false"
        >
          <div class="item__icon mi mi-24" :class="item.icon"></div>
          <div v-show="isSidebarBig" class="item__text">
            {{ item.name }}
          </div>
          <div
            v-show="item.displayLabel"
            :style="{ top: item.labelPos + 'px' }"
            class="item__hoverbox"
          >
            <div class="hover__arrow"></div>
            <div class="hover__text">{{ item.name }}</div>
          </div>
        </div>
        <hr size="1px" v-if="index == 14" />
      </router-link>
    </div>
    <div
      class="sidebar__footer"
      @click="resizeSidebar"
      @mouseenter="displayExpandTooltip = true"
      @mouseleave="displayExpandTooltip = false"
    >
      <div
        class="item__icon mi mi-24"
        :class="
          isSidebarBig ? 'mi-sidebar-left-arrow' : 'mi-sidebar-right-arrow'
        "
      ></div>
      <div v-show="isSidebarBig" class="item__text" data-text="Thu gọn">
        Thu gọn
      </div>
      <div
        v-show="!isSidebarBig && displayExpandTooltip"
        class="item__hoverbox"
      >
        <div class="hover__arrow"></div>
        <div class="hover__text">Mở rộng</div>
      </div>
    </div>
  </div>
</template>

<script setup>
// #region import
import { ref, inject } from "vue";
const $common = inject("$common");

// #endregion

// #region init
const sidebarItems = ref($common.sidebarItems);
const isSidebarBig = ref(true);
sidebarItems.value.forEach((item) => {
  item.displayLabel = false;
  item.labelPos = 0;
});
const displayExpandTooltip = ref(false);
// #endregion

// #region function
/**
 * Thay đổi kích cỡ sidebar
 *
 * Author: Dũng (27/05/2023)
 */
function resizeSidebar() {
  isSidebarBig.value = !isSidebarBig.value;
}
// #endregion

// #region handle event


/**
 * Sự kiện mouseover qua sidebar item
 *
 * Author: Dũng (27/05/2023)
 */
function sideBarItemOnMouseOver($event, item) {
  item.labelPos = $event.currentTarget.getBoundingClientRect().y - 56;
  item.displayLabel = !isSidebarBig.value & true;
}

// #endregion
</script>

<style
  scoped
  lang="css"
  src="@/assets/css/layout/the-sidebar.css"
></style>
