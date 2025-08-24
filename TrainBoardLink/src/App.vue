<template>
  <div class="container mb-5">
    <div class="card m-4">
      <div class="card-header">
        <h1 class="text-center card-title fw-bold my-auto">
          Trainboard Settings
        </h1>
      </div>
      <div class="card-body w-100 mx-auto">
        <div v-if="showAlert" class="alert alert-success alert-dismissible fade show" role="alert">
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
            v-model="numRows"
          />
          <label for="floatingNumRows">Number of Departure rows</label>
        </div>
        <div class="input-group mb-3">
          <div class="form-floating flex-grow-1">
            <input
              list="stations"
              class="form-control"
              id="floatingCrs"
              v-model="crs"
            />
            <label for="floatingCrs">Station CRS</label>
          </div>
          <button class="btn btn-primary" @click="() => crs = ''">
            Clear
          </button>
        </div>
        <div class="input-group mb-3">
          <div class="form-floating flex-grow-1">
            <input
              list="stations"
              class="form-control form-control-lg"
              id="floatingFilterCrs"
              min="1"
              v-model="filterCrs"
            />
            <label for="floatingCrs">Destination station CRS</label>
          </div>
          <button
            class="btn btn-primary"
            @click="() => (filterCrs = '')"
          >
            Clear
          </button>
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
        <MatrixDisplay v-if="showCustomDisplay" v-model:matrix-pixels="matrixPixels" />
      </div>
      <div class="card-footer text-end">
        <button
          @click="resetMatrixConfig"
          type="button"
          class="btn btn-secondary"
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

      <datalist id="stations">
        <option v-for="station in stations" :key="station.crs" :value="station.Crs">{{`${station.name} (${station.crs})`}}</option>
      </datalist>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue';
import MatrixDisplay from './components/MatrixDisplay.vue';
import mqtt from 'mqtt';
import { intToHex, hexToInt } from './utils/ColourConverter';

const showAlert = ref(false);
const showCustomDisplay = ref(false);
const numRows = ref(1);
const crs = ref("COL");
const filterCrs = ref("");
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
const stations = ref([]);
const matrixPixels = ref([]);

const status = ref('Connecting...');
const message = ref('');
let client = null; // Use a regular variable for the client instance
const host = 'ws://pizero.local:9001';
const topic = 'matrix_config';

const connectToBroker = () => {
  
  try {
    client = mqtt.connect(host);

    client.on('connect', () => {
      status.value = 'Connected!';
      console.log('Connected to MQTT broker!');
      
      client.subscribe(topic, (err) => {
        if (!err) {
          console.log(`Subscribed to topic: ${topic}`);
        }
      });
    });

    client.on('message', (receivedTopic, payload) => {
      if (receivedTopic === topic) {
        message.value = payload.toString();
        console.log(`Received message: ${message.value}`);
        updateConfiguration(JSON.parse(payload.toString()));
      }
    });

    client.on('error', (err) => {
      status.value = `Error: ${err.message}`;
      console.error('MQTT error:', err);
    });

    client.on('close', () => {
      status.value = 'Disconnected.';
      console.log('Disconnected from MQTT broker.');
    });

  } catch (err) {
    status.value = `Failed to connect: ${err.message}`;
    console.error('Connection failed:', err);
  }
};

onMounted(() => {
  connectToBroker();
});

onBeforeUnmount(() => {
  if (client) {
    client.end();
  }
});

const resetMatrixConfig = () => {
  numRows.value = 1;
  crs.value = "COL";
  filterCrs.value = "";
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
}

const updateMatrixConfig = async () => {
  const newConfiguration = {
    "numRows": numRows.value,
    "crs": crs.value.toUpperCase(),
    "filterCrs": filterCrs.value.toUpperCase(),
    "filterType": filterType.value,
      "timeOffset": timeOffset.value,
      "timeWindow": timeWindow.value,
      "stdColour": hexToInt(stdColour.value),
      "destinationColour": hexToInt(destinationColour.value),
      "platformColour": hexToInt(platformColour.value),
      "callingPointsColour": hexToInt(callingPointsColour.value),
      "currentTimeColour": hexToInt(currentTimeColour.value),
      "delayColour": hexToInt(delayColour.value),
      "onTimeColour": hexToInt(onTimeColour.value),
      "showCustomDisplay": showCustomDisplay.value,
      "matrixPixels": matrixPixels.value
    };
    
    client.publish(topic, JSON.stringify(newConfiguration), { qos: 0, retain: 1 }, (error) => {
      if (error) {
        console.error(error);
      }
    })
    
  showAlert.value = true;
  setTimeout(() => {
    showAlert.value = false;
  }, 10000);
}

const updateConfiguration = (config) => {
  numRows.value = config.numRows;
  crs.value = config.crs;
  filterCrs.value = config.filterCrs;
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
}

const initialiseArray = () => {
    if (matrixPixels.value == null || matrixPixels.value.length == 0)
    {
      matrixPixels.value = new Array(32); 
    }    
    for (let i = 0; i < matrixPixels.value.length; i++)
    {
      if (matrixPixels.value[i] == null || matrixPixels.value[i].length == 0)
      {
        matrixPixels.value[i] = new Array(64); 
        for (let j = 0; j < matrixPixels.value[i].length; j++)
        {
          matrixPixels.value[i][j] = 0;
        }
      }
    }
  }
  
  initialiseArray();
</script>
<style scoped>
</style>
