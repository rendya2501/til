<template>
  <div id="app">
    <router-view />
  </div>
</template>

<style>
:root {
  --header-height: 50px;
}
body {
  margin: 0;
}
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  /** ダブルタップによるズームを防ぐ */
  touch-action: manipulation;
  /* IE用 フォールバック */
  height: 90vmax;
  /* フォールバック */
  height: 90vh;
  height: -moz-available;
  height: -webkit-fill-available;
  height: fill-available;
  min-height: 90vh;
}
</style>
