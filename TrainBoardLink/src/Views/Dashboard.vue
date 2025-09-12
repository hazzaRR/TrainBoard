<template>
  <div class="container mb-5">
    <div class="card m-4">
      <div class="card-header">
        <h1 class="text-center card-title fw-bold my-auto">
          Trainboard Settings
        </h1>
      </div>
      <div class="card-body w-100 mx-auto">
        <div
          v-if="showAlert"
          class="alert alert-success alert-dismissible fade show"
          role="alert"
        >
          <strong>Matrix settings sent</strong> The board will update within the
          next 30 seconds
        </div>
        <div class="form-floating mb-3">
          <input
            type="number"
            class="form-control"
            id="floatingNumRows"
            min="1"
            max="10"
            disabled
            v-model="numRows"
          />
          <label for="floatingNumRows">Number of Departure rows</label>
        </div>
        <div :class="['form-floating', 'mb-3', 'input-group']">
          <div :class="['form-control', 'multiSelect']">
            <Multiselect
              id="crsStationMultiSelect"
              v-model="crs"
              :searchable="true"
              :custom-label="(station) => `${station.name} (${station.crs})`"
              :options="stations"
              :close-on-select="true"
              track-by="crs"
            />
            <button class="btn btn-primary" @click="() => (crs = null)">
              Clear
            </button>
          </div>
          <label for="crsStationMultiSelect" class="form-label"
            >Station CRS</label
          >
        </div>
        <div :class="['form-floating', 'mb-3', , 'input-group']">
          <div :class="['form-control', 'multiSelect']">
            <Multiselect
              id="filterCrsStationMultiSelect"
              v-model="filterCrs"
              :searchable="true"
              :custom-label="(station) => `${station.name} (${station.crs})`"
              :options="stations"
              :close-on-select="true"
              track-by="crs"
            />
            <button class="btn btn-primary" @click="() => (filterCrs = null)">
              Clear
            </button>
          </div>
          <label for="filterCrsStationMultiSelect" class="form-label"
            >Destination station CRS</label
          >
        </div>
        <div class="form-floating mb-3">
          <select
            class="form-control"
            id="floatingFilterCrs"
            min="1"
            v-model="filterType"
          >
            <option value="to">To</option>
            <option value="from">From</option>
          </select>
          <label for="floatingFilterCrs">Filter Type</label>
        </div>
        <div class="form-floating mb-3">
          <input
            type="number"
            class="form-control"
            id="floatingTimeOffset"
            min="1"
            max="120"
            v-model="timeOffset"
          />
          <label for="floatingTimeOffset">Time Offset</label>
        </div>
        <div class="form-floating mb-3">
          <input
            type="number"
            class="form-control"
            id="floatingTimeWindow"
            min="1"
            max="120"
            v-model="timeWindow"
          />
          <label for="floatingTimeWindow">Time Window</label>
        </div>
        <div class="form-floating mb-3">
          <input
            type="color"
            class="form-control"
            id="floatingStdColour"
            v-model="stdColour"
          />
          <label for="floatingStdColour">Departure Time Colour</label>
        </div>
        <div class="form-floating mb-3">
          <input
            type="color"
            class="form-control"
            id="floatingPlatformColour"
            v-model="platformColour"
          />
          <label for="floatingPlatformColour">Platform Colour</label>
        </div>
        <div class="form-floating mb-3">
          <input
            type="color"
            class="form-control"
            id="floatingDestinationColour"
            v-model="destinationColour"
          />
          <label for="floatingDestinationColour">Destination Colour</label>
        </div>
        <div class="form-floating mb-3">
          <input
            type="color"
            class="form-control"
            id="floatingCallingPointsColour"
            v-model="callingPointsColour"
          />
          <label for="floatingDestinationColour">Calling Points Colour</label>
        </div>
        <div class="form-floating mb-3">
          <input
            type="color"
            class="form-control"
            id="floatingCurrentTimeColour"
            v-model="currentTimeColour"
          />
          <label for="floatingDestinationColour">Current Time Colour</label>
        </div>
        <div class="form-floating mb-3">
          <input
            type="color"
            class="form-control"
            id="floatingOnTimeColour"
            v-model="onTimeColour"
          />
          <label for="floatingOnTimeColour">On Time Colour</label>
        </div>
        <div class="form-floating mb-3">
          <input
            type="color"
            class="form-control"
            id="floatingDelayColour"
            v-model="delayColour"
          />
          <label for="floatingDelayColour">Delay Colour</label>
        </div>
        <div class="form-check form-switch">
          <input
            class="form-check-input"
            type="checkbox"
            role="switch"
            id="switchCheckDefault"
            v-model="showCustomDisplay"
          />
          <label class="form-check-label" for="switchCheckDefault"
            >Custom display</label
          >
        </div>
        <MatrixDisplay
          v-if="showCustomDisplay"
          v-model:matrix-pixels="matrixPixels"
        />
      </div>
      <div class="card-footer text-end">
        <button
          @click="resetMatrixConfig"
          type="button"
          class="btn btn-secondary me-1"
        >
          Reset
        </button>
        <button
          @click="updateMatrixConfig"
          type="button"
          class="btn btn-primary"
        >
          Update
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from "vue";
import MatrixDisplay from "../components/MatrixDisplay.vue";
import Multiselect from "vue-multiselect";
import { intToHex, hexToInt } from "../utils/ColourConverter";
import stations from "../assets/national-rail-stations.json";
import { useMqttStore } from "../stores/MqttStore";

const mqttStore = useMqttStore();

onMounted(() => {
  if (mqttStore?.matrixConfig) {
    updateConfiguration(mqttStore?.matrixConfig);
  }
});

watch(mqttStore.matrixConfig, () => {
  if (mqttStore?.matrixConfig) {
    updateConfiguration(mqttStore?.matrixConfig);
  }
}, {deep: true});

const emit = defineEmits("publishConfig");

const showAlert = ref(false);
const showCustomDisplay = ref(false);
const numRows = ref(1);
const crs = ref({ name: "Colchester", crs: "COL", owner: "Greater Anglia", latitiude: "51.900711", longitude: "0.892598" });
const filterCrs = ref(null);
const filterType = ref("to");
const timeOffset = ref(2);
const timeWindow = ref(120);
const stdColour = ref("#ffa000");
const platformColour = ref("#ffa000");
const destinationColour = ref("#ffa000");
const callingPointsColour = ref("#ffa000");
const currentTimeColour = ref("#ffa000");
const delayColour = ref("#ff0f00");
const onTimeColour = ref("#00ff00");
const matrixPixels = ref([]);

const resetMatrixConfig = () => {
  numRows.value = 1;
  crs.value = { name: "Colchester", crs: "COL", owner: "Greater Anglia", latitiude: "51.900711", longitude: "0.892598" };
  filterCrs.value = null;
  filterType.value = "to";
  timeOffset.value = 2;
  timeWindow.value = 120;
  stdColour.value = "#ffa000";
  platformColour.value = "#ffa000";
  destinationColour.value = "#ffa000";
  callingPointsColour.value = "#ffa000";
  currentTimeColour.value = "#ffa000";
  delayColour.value = "#ff0f00";
  onTimeColour.value = "#00ff00";
};

const updateMatrixConfig = async () => {
  const newConfiguration = {
    numRows: numRows.value,
    crs: crs.value.crs.toUpperCase(),
    filterCrs: filterCrs.value?.crs?.toUpperCase() ?? '',
    filterType: filterType.value,
    timeOffset: timeOffset.value,
    timeWindow: timeWindow.value,
    stdColour: hexToInt(stdColour.value),
    destinationColour: hexToInt(destinationColour.value),
    platformColour: hexToInt(platformColour.value),
    callingPointsColour: hexToInt(callingPointsColour.value),
    currentTimeColour: hexToInt(currentTimeColour.value),
    delayColour: hexToInt(delayColour.value),
    onTimeColour: hexToInt(onTimeColour.value),
    showCustomDisplay: showCustomDisplay.value,
    matrixPixels: matrixPixels.value,
  };


  mqttStore.publishPayload("matrix/config", newConfiguration,
   "Matrix settings sent The board will update within the next 30 seconds");

  showAlert.value = true;
  setTimeout(() => {
    showAlert.value = false;
  }, 10000);

};

function updateConfiguration(config) {
  numRows.value = config.numRows;
  crs.value = stations.find((station) => station.crs === config.crs);
  filterCrs.value =
    stations.find((station) => station.crs === config.filterCrs) ?? null;
  filterType.value = config.filterType;
  timeOffset.value = config.timeOffset;
  timeWindow.value = config.timeWindow;
  stdColour.value = intToHex(config.stdColour);
  destinationColour.value = intToHex(config.destinationColour);
  platformColour.value = intToHex(config.platformColour);
  callingPointsColour.value = intToHex(config.callingPointsColour);
  currentTimeColour.value = intToHex(config.currentTimeColour);
  delayColour.value = intToHex(config.delayColour);
  onTimeColour.value = intToHex(config.onTimeColour);
  showCustomDisplay.value = config.showCustomDisplay;
  matrixPixels.value = config.matrixPixels;
  initialiseArray();
};

const initialiseArray = () => {
  if (matrixPixels.value == null || matrixPixels.value.length == 0) {
    matrixPixels.value = new Array(32);
  }
  for (let i = 0; i < matrixPixels.value.length; i++) {
    if (matrixPixels.value[i] == null || matrixPixels.value[i].length == 0) {
      matrixPixels.value[i] = new Array(64);
      for (let j = 0; j < matrixPixels.value[i].length; j++) {
        matrixPixels.value[i][j] = 0;
      }
    }
  }
};

initialiseArray();
</script>

<style scoped>
.multiSelect {
  height: fit-content;
  padding-top: 2rem !important;
  display: flex;
}
</style>
