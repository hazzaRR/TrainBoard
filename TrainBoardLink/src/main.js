import './assets/main.css'
import '../scss/styles.scss'
import * as bootstrap from 'bootstrap'

import { createApp } from 'vue'
import { createPinia } from 'pinia';

import router from './router';
import App from './App.vue'

import { library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon, } from '@fortawesome/vue-fontawesome';

import {
    faCheck,
    faWifi
} from '@fortawesome/free-solid-svg-icons';

library.add(faCheck, faWifi);



const app = createApp(App)
app.component('font-awesome-icon', FontAwesomeIcon);
app.use(router);
app.use(createPinia());
app.mount('#app')