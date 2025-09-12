import { ref } from 'vue'
import { defineStore } from 'pinia'
import mqtt from "mqtt";

export const useMqttStore = defineStore('mqtt', () => {

const status = ref(0);
const alertMessage = ref(null);
let client = null;
const host = "ws://pizero.local:9001";
const matrixConfigTopic = "matrix/config";
const availableNetworksTopic = "network/available";
const matrixConfig = ref(null);
const availableNetworks = ref();

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
    });

    client.on("message", (receivedTopic, payload) => {
      if (receivedTopic === matrixConfigTopic) {
        matrixConfig.value = JSON.parse(payload.toString());
      }
      if (receivedTopic === availableNetworksTopic) {
        availableNetworks.value = JSON.parse(payload.toString());
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

const publishPayload = async (topic, payload, alertMessage) => {
  client.publish(
    topic,
    JSON.stringify(payload),
    { qos: 0, retain: 1 },
    (error) => {
      if (error) {
        console.error(error);
      }
    }
  );

  alertMessage.value = alertMessage;

  showAlert.value = true;
  setTimeout(() => {
    showAlert.value = false;
    alertMessage.value = null;
  }, 10000);
};


return {
    status,
    matrixConfig,
    availableNetworks,
    connectToBroker,
    publishPayload
  }
})
