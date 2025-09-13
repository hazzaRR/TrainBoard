<template>
  <nav class="navbar navbar-expand-lg sticky-top bg-primary">
    <div class="container-fluid d-flex align-items-center justify-content-between">
      <div class="d-flex align-items-center">
        <a class="navbar-brand d-flex align-items-center" href="/">
          <h3 class="text-white">Train Board</h3>
          <img src="/train.svg" alt="Logo" width="64" height="64" class="ms-2" />
        </a>
        <img
        v-if="isConnected > 0"
          :src="cloud"
          alt="Cloud"
          width="32"
          height="32"
          class="ms-3 d-lg-none cloud-icon"
          :class="[isConnected === 1 ? 'cloud-green' : isConnected === 2 ? 'cloud-red' : '', 'd-lg-none', 'ms-3']"
        />
      </div>

      <button
        class="navbar-toggler"
        type="button"
        data-bs-toggle="collapse"
        data-bs-target="#navbarNav"
        aria-controls="navbarNav"
        aria-expanded="false"
        aria-label="Toggle navigation"
      >
        <span class="navbar-toggler-icon"></span>
      </button>

      <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav ms-auto align-items-center">
          <li class="nav-item d-none d-lg-block">
            <img
            v-if="isConnected > 0"
              :src="cloud"
              alt="Cloud"
              width="32"
              height="32"
              :class="[isConnected === 1 ? 'cloud-green' : isConnected === 2 ? 'cloud-red' : '', 'me-3']"
            />
          </li>
          <li class="nav-item">
            <RouterLink class="nav-link fw-medium text-white" to="/">Train Settings</RouterLink>
          </li>
          <li class="nav-item">
            <RouterLink class="nav-link fw-medium text-white" to="/network">Networking</RouterLink>
          </li>
        </ul>
      </div>
    </div>
  </nav>
</template>

<script setup>
import cloud from '@/assets/cloud-check-solid-full.svg'
import { RouterLink } from 'vue-router'
import { useMqttStore } from '@/stores/MqttStore';
import { computed } from 'vue';

const mqttStore = useMqttStore();
const isConnected = computed(() => mqttStore.status)
</script>

<style scoped>
.navbar-brand svg {
    vertical-align: middle;
}

.cloud-green {
    filter: invert(48%) sepia(97%) saturate(745%) hue-rotate(85deg) brightness(110%) contrast(105%);
}

.cloud-red {
    filter: invert(23%) sepia(93%) saturate(7481%) hue-rotate(345deg) brightness(95%) contrast(110%);
}

</style>