import './assets/main.css'
import '../scss/styles.scss'
import * as bootstrap from 'bootstrap'

import { createApp } from 'vue'
import { createPinia } from 'pinia';

import router from './router';
import App from './App.vue'

import { library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';

import {
    faCheck,
    faWifi,
    faChevronLeft,
    faChevronRight,
    faPlay,
    faPause,
    faUpload,
    faClone,
    faPlus,
    faTrash,
    faSquareMinus
} from '@fortawesome/free-solid-svg-icons';

library.add(faCheck, faWifi, faChevronLeft, faChevronRight, faPlay, faPause, faUpload, faClone, faPlus, faTrash, faSquareMinus);

import "vue3-openlayers/styles.css";
import OpenLayersMap from "vue3-openlayers";
import { useGeographic } from 'ol/proj.js';
useGeographic();

const app = createApp(App)
app.component('font-awesome-icon', FontAwesomeIcon);
app.use(router);
app.use(createPinia());
app.use(OpenLayersMap);
app.mount('#app')