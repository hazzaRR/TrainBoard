<template>
  <div>
    <svg xmlns="http://www.w3.org/2000/svg" class="d-none">
      <symbol id="check-circle-fill" viewBox="0 0 16 16">
        <path
          d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zm-3.97-3.03a.75.75 0 0 0-1.08.022L7.477 9.417 5.384 7.323a.75.75 0 0 0-1.06 1.06L6.97 11.03a.75.75 0 0 0 1.079-.02l3.992-4.99a.75.75 0 0 0-.01-1.05z"
        />
      </symbol>
      <symbol id="exclamation-triangle-fill" viewBox="0 0 16 16">
        <path
          d="M8.982 1.566a1.13 1.13 0 0 0-1.96 0L.165 13.233c-.457.778.091 1.767.98 1.767h13.713c.889 0 1.438-.99.98-1.767L8.982 1.566zM8 5c.535 0 .954.462.9.995l-.35 3.507a.552.552 0 0 1-1.1 0L7.1 5.995A.905.905 0 0 1 8 5zm.002 6a1 1 0 1 1 0 2 1 1 0 0 1 0-2z"
        />
      </symbol>
    </svg>
    <Navbar />
    <RouterView />
    <keep-alive>
      <div
        v-show="mqttStore.alert.show"
        :class="[
          'alert',
          mqttStore.alert.status === 2
            ? 'alert-info'
            : mqttStore.alert.status === 1
            ? 'alert-success'
            : 'alert-warning',
          'alertContainer',
        ]"
        role="alert"
      >
        <span class="alertText">{{ mqttStore.alert.message }}</span>
        <button
          type="button"
          class="btn-close"
          @click="mqttStore.dismissAlert"
        />
      </div>
    </keep-alive>
  </div>
</template>

<script setup>
import { onMounted, onBeforeUnmount } from "vue";
import { RouterView } from "vue-router";
import { useMqttStore } from "./stores/MqttStore";
import Navbar from "./components/Navbar.vue";

const mqttStore = useMqttStore();

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
.alertContainer {
  min-width: 25%;
  display: flex;
  text-align: center;
  position: fixed;
  top: 93%;
  left: 50%;
  transform: translate(-50%, -50%);
  margin-bottom: 10px;
  z-index: 5000;
}

#borderAnimation {
  width: 100%;
  position: absolute;
  left: 0;
  bottom: 0;
  animation: border_anim 10s linear forwards;
}

.alertIcon {
  position: relative;
  align-self: center;
  margin-right: 10px;
}

.alertText {
  margin-right: 10px;
}

@media only screen and (max-width: 767px) {
  .alertContainer {
    min-width: 90%;
    word-break: break-all;
  }
}
</style>
