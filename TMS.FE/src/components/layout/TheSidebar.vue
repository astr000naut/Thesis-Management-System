<template>
  <div
    class="sidebar"
    :style="{ 'flex-basis': isSidebarBig ? '236px' : '78px' }"
  >
    <div class="sidebar__main">
      <div class="item__hoverbox"
        v-show="!isSidebarBig && tooltip.isDisplay"
        :style="{ top: tooltip.position + 'px' }"
      >
        <div class="hover__arrow"></div>
        <div class="hover__text">{{ tooltip.text }}</div>
      </div>
      <div
        v-for="(item, index) in sidebarItems"
      >
        <router-link 
          :to="item.link"
          v-if="item.link"
        >
          <div class="sidebar__item cg-2"
            @click="normalItemOnClick(item)"
            @mouseenter="itemOnMouseEnter($event, item)"
            @mouseleave="tooltip.isDisplay = false"
          >
          <Icon class="item__icon" :icon="item.icon" style="color: white;"/>
            <div class="item__text" v-show="isSidebarBig">{{ item.name }}</div>
          </div>
        </router-link>
        <div v-else class="item__parent flex flex-col">
          <div class="parent__node sidebar__item"
            @click="parentItemOnClick(item)"
            :class="item.expand ? 'expand' : '', item.active ? 'active' : ''"
            @mouseenter="itemOnMouseEnter($event, item)"
            @mouseleave="tooltip.isDisplay = false"
          >
            <div class="icon_and_text flex cg-2 al-center">
              <Icon class="item__icon" :icon="item.icon" style="color: white;"/>
              <div class="item__text" v-show="isSidebarBig">{{ item.name }}</div>
            </div>
            <div class="arrow mi mi-16 mi-arrow-up--white" v-show="isSidebarBig"></div>
          </div> 
          <div class="child__group flex flex-col"
            v-show="item.expand"
          >
            <router-link 
              v-for="(child, index) in item.children"
              :to="child.link"
              
              
              >
              <div class="child__item sidebar__item flex cg-4"
                @mouseenter="itemOnMouseEnter($event, child)"
                @mouseleave="tooltip.isDisplay = false"
              >
                  <Icon class="item__icon" :icon="child.icon" style="color: white;"/>
                  <div class="child__text" v-show="isSidebarBig">{{ child.name }}</div>
            
              </div>
            </router-link>
          </div>
        </div>

      </div>
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
import { manager_item, student_item, teacher_item } from "@/config/sidebarItems";
import { useAuthStore } from "@/stores";
import { Icon } from "@iconify/vue";
import { useRoute } from "vue-router";

const route = useRoute();
const authStore = useAuthStore();

const sidebarItems = ref([]);
const isSidebarBig = ref(true);
const displayExpandTooltip = ref(false);
const tooltip = ref(
  {
    isDisplay: false,
    text: "",
    position: 0,
  }
);

initSidebarItems();

// #endregion

// #region function

function initSidebarItems() {

  if (authStore.loginInfo?.user.role === "ADMIN") {
    sidebarItems.value = manager_item;
  } else if (authStore.loginInfo?.user.role === "Student") {
    sidebarItems.value = student_item;
  } else if (authStore.loginInfo?.user.role === "Teacher") {
    sidebarItems.value = teacher_item;
  }

  sidebarItems.value.forEach((item) => {
    item.displayLabel = false;
    item.labelPos = 0;
  });

  console.log(route.path);
  // check current route match with sidebar children url
  sidebarItems.value.forEach((item) => {
    if (item.children) {
      item.children.forEach((child) => {
        if (child.link === route.path) {
          item.expand = true;
        }
      });
    }
  });

}

function parentItemOnClick(item) {
  item.expand = !item.expand;
  item.active = false;
  if (item.children && !item.expand) {
    item.children.forEach((child) => {
      if (child.link === route.path) {
        item.active = true;
      }
    });
  }
}

function normalItemOnClick(item) {
  sidebarItems.value.forEach((i) => {
    i.active = false;
  });
}

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

function itemOnMouseEnter($event, item) {
  if (!isSidebarBig.value) {
    tooltip.value.isDisplay = true;
    tooltip.value.text = item.name;
    tooltip.value.position = $event.currentTarget.getBoundingClientRect().y - 58;
  }
}


// #endregion
</script>

<style
  scoped
  lang="css"
  src="@/assets/css/layout/the-sidebar.css"
></style>
