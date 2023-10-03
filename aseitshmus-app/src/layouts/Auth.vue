<script setup>

</script>
<template>
  <div>
    <base-card class="container2">
      <router-view />
    </base-card>
  </div>
</template>

<style scoped>
  .container2 {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    height: 100%;
    margin-bottom: 1rem;
  }
</style>