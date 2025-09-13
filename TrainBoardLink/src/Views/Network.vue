<template>
    <div class="container">
        <div class="card m-4">
            <div class="card-header">
                <h1 class="text-center card-title fw-bold my-auto">
                    Available Networks
                </h1>
            </div>
            <div class="card-body w-100 mx-auto">
                <h5 class="text-center" v-if="!mqttStore.availableNetworks">No networks found!</h5>
                <div v-for="(network, key) in mqttStore.availableNetworks" :key="key" :class="[
                    'card',
                    'network',
                    'card-body',
                    'm-1',
                    network.isActive ? 'bg-primary text-white mb-2 activeNetwork' : ''
                ]" @click="networkClicked(network, key)">
                    <div class="d-flex justify-content-between align-items-center flex-wrap">
                        <div class="d-flex align-items-center flex-wrap">
                            <span v-if="network.isSaved" class="badge bg-success me-2">
                                Saved
                            </span>
                            <h4 class="mb-0">{{ network.ssid }}</h4>
                        </div>

                        <span class="mt-1 mt-sm-0 fw-medium">
                            <font-awesome-icon v-if="network.isActive" class="m-1" :icon="['fas', 'check']" />
                            <span v-else><font-awesome-icon :icon="['fas', 'wifi']" /> {{ network.signal }}
                                dBm</span>
                        </span>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal fade" id="manageNetwork" tabindex="-1" aria-labelledby="networkManageLabel"
            aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h1 class="modal-title fs-5" id="networkManageLabel">{{ selectedNetwork?.ssid }}</h1>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>

                    <div class="modal-body">
                        <div v-if="!selectedNetwork?.isSaved" class="form-floating mb-3">
                            <input type="password" class="form-control" id="floatingNumRows" v-model="password" />
                            <label for="floatingNumRows">Password</label>
                        </div>
                        <button type="button" @click="connectToNetwork"
                            :class="['btn', 'btn-primary', 'w-100']">
                            Connect
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref } from 'vue';
import { useMqttStore } from '@/stores/mqttStore';
import { Modal } from 'bootstrap';

const mqttStore = useMqttStore();

const password = ref(null);
const selectedNetwork = ref(null);
const selectedKey = ref(null);

const networkClicked = (network, key) => {
    if (network.isActive) {
        return;
    }
    selectedNetwork.value = network;
    selectedKey.value = key;

    const modalInstance = Modal.getOrCreateInstance('#manageNetwork');
    if (modalInstance) {
        modalInstance.show();
    }
}

const connectToNetwork = () => {
    const connectToNetwork = {
        key: selectedKey.value,
        UseSaved: selectedNetwork.value,
        password: password.value
    }
    mqttStore.publishPayload("network/manage", connectToNetwork, `Attempting to connect to ${selectedNetwork?.value?.ssid}`);

    const modalInstance = Modal.getOrCreateInstance('#manageNetwork');
    if (modalInstance) {
        modalInstance.hide();
    }

}

</script>

<style scoped>
.activeNetwork {
    border-left: 5px solid #0d6efd;
}

.network {
    cursor: pointer;
}

.network:hover {
    background-color: aliceblue;
    opacity: 90%;
}
</style>