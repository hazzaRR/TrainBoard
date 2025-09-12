<template>
  <div>
    <Navbar />
    <RouterView />
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from "vue";
import { RouterView } from 'vue-router'
import { useMqttStore } from "./stores/MqttStore";
import Navbar from "./components/Navbar.vue";

const mqttStore = useMqttStore();
const status = ref("Connecting...");


onMounted(() => {
  mqttStore.connectToBroker();
});

onBeforeUnmount(() => {
  if (client) {
    client.end();
  }
});

</script>
<style scoped>
</style>
