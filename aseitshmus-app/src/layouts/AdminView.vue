<script setup>
</script>
<template>
  <div>
    <base-card class="container">
      <router-view />
    </base-card>
  </div>
</template>
<style scoped>
  .container {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    margin-bottom: 1rem;
  }
</style>