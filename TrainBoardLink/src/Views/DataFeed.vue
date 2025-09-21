<template>
  <div class="container mb-5">
    <div class="card m-4">
      <div class="card-header">
        <h1 class="text-center card-title fw-bold my-auto">
          Data Feed Configuration
        </h1>
      </div>
      <div class="card-body w-100 mx-auto">
        <p class="mb-1">
            To access the live train feed for the national rail network,
             you'll need a <b>National Rail API key</b>. You can sign up for a key with the following the <a href="https://realtime.nationalrail.co.uk/OpenLDBWSRegistration">link</a>
        </p>
        <p class="mb-3">
            When you fill out the registration form, select <b>"Personal"</b> for the usage type and describe your intended use as a <b>"personal project matrix board for displaying departures."</b>
        </p>
        <div class="form-floating mb-3">
          <input type="text" class="form-control" id="floatingTimeOffset" min="1" max="120" v-model="apiKey" />
          <label for="floatingTimeOffset">LDBWS API Key</label>
        </div>
      </div>
      <div class="card-footer text-end">
        <button @click="updateApiKey" type="button" class="btn btn-primary">
          Submit
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import { useMqttStore } from "../stores/MqttStore";

const mqttStore = useMqttStore();

const apiKey = ref(null);


const updateApiKey = () => {
    const apiKeyToAdd = {
        apiKey: apiKey.value,
    }
    mqttStore.publishPayload("feed/update", apiKeyToAdd, `Setting Api Key`, 0);
}

</script>

<style scoped>

</style>
