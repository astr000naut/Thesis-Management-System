<template>
  <div class="app__wrapper">
    <div class="app__tooltip"></div>
    <div class="mainarea">
      <TheSidebar />
      <div class="maincontent fl-1">
        <TheHeader />
        <router-view></router-view>
      </div>
    </div>
  </div>
</template>

<script setup>
import TheHeader from "./components/layout/TheHeader.vue";
import TheSidebar from "./components/layout/TheSidebar.vue";
</script>

<style scoped>
.mainarea {
  display: flex;
}

</style>
