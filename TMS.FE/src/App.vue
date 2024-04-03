<template>
  <div class="app__wrapper">
    <div class="app__tooltip"></div>
    <div class="app__alert">
      <BaseToastbox/>
    </div>
    <div class="mainarea">
      <RouterView></RouterView>
    </div>
  </div>
</template>

<script setup>
  import {BaseToastbox} from '@/components/base';
</script>

<style scoped>
.mainarea {
  display: flex;
  width: 100vw;
  height: 100vh;
}

.app__wrapper {
  position: relative;
}

.app__alert {
  right: 10px;
  top: 10px;
  position: absolute;
  z-index: 99999;

}

</style>
