import { ref } from 'vue'
import { defineStore } from 'pinia'
import mqtt from "mqtt";

export const useMqttStore = defineStore('mqtt', () => {

const status = ref(0);
const alert = ref({
  status: 0,
  show: false,
  message: '',
});

let client = null;
const host = "ws://pizero.local:9001";
const matrixConfigTopic = "matrix/config";
const imageEncodedTopic = "image/encoded";
const availableNetworksTopic = "network/available";
const networkOutcomeTopic = "network/outcome";
const matrixConfig = ref({
  numRows: 1,
  crs: "COL",
  filterCrs: null,
  filterType: "to",
  timeOffset: 2,
  timeWindow: 120,
  stdColour: 16758784,
  platformColour: 16758784,
  destinationColour: 16758784,
  callingPointsColour: 16758784,
  currentTimeColour: 16758784,
  delayColour: 16711680,
  onTimeColour: 65280,
  matrixFrames: [],
  brightness: 50,
});
const availableNetworks = ref(null);

const connectToBroker = () => {
  try {
    client = mqtt.connect(host);
    client.on("connect", () => {
      status.value = 1;
      console.log("Connected to MQTT broker!");

      client.subscribe(matrixConfigTopic, (err) => {
        if (!err) {
          console.log(`Subscribed to topic: ${matrixConfigTopic}`);
        }
      });
      client.subscribe(availableNetworksTopic, (err) => {
        if (!err) {
          console.log(`Subscribed to topic: ${availableNetworksTopic}`);
        }
      });
      client.subscribe(networkOutcomeTopic, (err) => {
        if (!err) {
          console.log(`Subscribed to topic: ${networkOutcomeTopic}`);
        }
      });
      client.subscribe(imageEncodedTopic, (err) => {
        if (!err) {
          console.log(`Subscribed to topic: ${imageEncodedTopic}`);
        }
      });
    });

    client.on("message", (receivedTopic, payload) => {
      if (receivedTopic === matrixConfigTopic) {
        matrixConfig.value = JSON.parse(payload.toString());
      }
      if (receivedTopic === availableNetworksTopic) {
        availableNetworks.value = JSON.parse(payload.toString());
      }
      if (receivedTopic === imageEncodedTopic) {
        matrixConfig.value.matrixFrames = JSON.parse(payload.toString());
      }
      if (receivedTopic === networkOutcomeTopic) {
        updateAlert(1, true, JSON.parse(payload.toString()));
        setTimeout(() => dismissAlert(), 10000);
      }
    });

    client.on("error", (err) => {
      status.value = 2;
      console.error("MQTT error:", err);
    });

    client.on("close", () => {
      status.value = 2;
      console.log("Disconnected from MQTT broker.");
    });
  } catch (err) {
    status.value = 2;
    console.error("Connection failed:", err);
  }
};

const publishPayload = async (topic, payload, alertMessage, retain) => {
  client.publish(
    topic,
    JSON.stringify(payload),
    { qos: 0, retain: retain },
    (error) => {
      if (error) {
        console.error(error);
      }
    }
  );

  updateAlert(1, true, alertMessage);
  setTimeout(() => dismissAlert(), 10000);
};

function updateAlert(status, show, message) {
    alert.value = {
      status,
      show,
      message,
    }
}

function dismissAlert() {
  alert.value = {
      status: 0,
      show: false,
      message: '',
    }
}

return {
    alert,
    status,
    matrixConfig,
    availableNetworks,
    connectToBroker,
    publishPayload,
    updateAlert,
    dismissAlert
  }
})
