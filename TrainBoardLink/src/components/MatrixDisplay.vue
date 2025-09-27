<template>
  <div class="matrix-container mx-auto">
    <div class="d-flex align-items-center gap-2 my-2 justify-content-end">
      <button v-if="matrixFrames.length > 1" :class="['btn', !isPlaying ? 'btn-primary' : 'btn-warning', 'me-1']" type="button"
        @click="() => !isPlaying ? playFrames() : stopPlaying()">
      <font-awesome-icon class="m-1" :icon="['fas', !isPlaying ? 'play': 'pause']" />
      </button>
      <p class="mb-0 px-2 py-1 border rounded bg-light text-primary fw-bold">
        {{ `${currentFrame + 1} / ${matrixFrames.length}` }}
      </p>
      <button class="btn btn-secondary me-1" type="button" :disabled="currentFrame === 0" @click="currentFrame--">
        <font-awesome-icon class="m-1" :icon="['fas', 'chevron-left']" />
      </button>
      <button class="btn btn-secondary me-1" type="button" :disabled="currentFrame === matrixFrames.length - 1"
        @click="currentFrame++">
        <font-awesome-icon class="m-1" :icon="['fas', 'chevron-right']" />
      </button>
    </div>

    <div class="pixel-matrix mx-auto" style="--cols: 64;">
      <div v-for="(pixel, index) in matrixFrames[currentFrame].pixels" :key="index" class="pixel"
        @click.left.exact="setPixelColor(index)" @click.right.prevent.exact="resetPixelColor(index)"
        @click.ctrl="resetPixelColor(index)" @mousemove.shift="setPixelColor(index)"
        :style="{ 'background-color': intToHex(matrixFrames[currentFrame].pixels[index]) }"></div>
    </div>
    <div v-if="matrixFrames.length > 1" class="form-floating my-2">
          <input type="number" class="form-control" id="floatingFrameDelay" min="1" v-model="matrixFrames[currentFrame].delay" />
          <label for="floatingFrameDelay">Frame Delay (milliseconds)</label>
    </div>
    <div class="mx-auto d-flex align-items-center mt-2 justify-content-start my-1">
      <input type="color" class="form-control form-control-color me-1" v-model="selectedColor" />
      <button :disabled="isConnected !== 1" class="btn btn-primary me-1" type="button" @click="open">
        <font-awesome-icon class="m-1" :icon="['fas', 'upload']" />
      </button>
      <button class="btn btn-primary me-1" type="button" @click="duplicateCurrentFrame">
        <font-awesome-icon class="m-1" :icon="['fas', 'clone']" />
      </button>
      <button class="btn btn-primary me-1" type="button" @click="addNewBlankFrame">
        <font-awesome-icon class="m-1" :icon="['fas', 'plus']" />
      </button>
      <button @click="clearFrame" class="btn btn-danger me-1">
        <font-awesome-icon class="m-1" :icon="['fas', 'square-minus']" />
      </button>
      <button @click="resetAll" class="btn btn-danger">
        <font-awesome-icon class="m-1" :icon="['fas', 'trash']" />
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { hexToInt, intToHex } from '@/utils/ColourConverter';
import { useFileDialog } from '@vueuse/core'
const cols = 64;
const rows = 32;
let timeoutId;

const emit = defineEmits(['processImage'])

const matrixFrames = defineModel('matrixFrames', { type: Array })

const {isConnected} = defineProps({
  isConnected: {
    type: Number,
    default: 0
  }
})

const currentFrame = ref(0);
const selectedColor = ref("#ff0000");
const isPlaying = ref(false);

const setPixelColor = (index) => {
  matrixFrames.value[currentFrame.value].pixels[index] = hexToInt(selectedColor.value);
}

const resetPixelColor = (index) => {
  matrixFrames.value[currentFrame.value].pixels[index] = 0;
}

const clearFrame = () => {
  for (let index = 0; index < matrixFrames.value[currentFrame.value].pixels.length; index++) {
    matrixFrames.value[currentFrame.value].pixels[index] = 0;
  }
}

const resetAll = () => {
  currentFrame.value = 0;
  while (matrixFrames.value.length > 1) {
    matrixFrames.value.pop();
  }
  for (let index = 0; index < matrixFrames.value[currentFrame.value].pixels.length; index++) {
    matrixFrames.value[currentFrame.value].pixels[index] = 0;
  }
}

const playFrames = () => {
  isPlaying.value = true;
  nextFrame();
}

  const nextFrame = () => {
    if (!isPlaying.value) return

    currentFrame.value = (currentFrame.value + 1) % matrixFrames.value.length

    const delay = matrixFrames.value[currentFrame.value].delay
    timeoutId = setTimeout(nextFrame, delay)
  }


const stopPlaying = () => {
  clearTimeout(timeoutId)
  timeoutId = null;
  currentFrame.value = 0;
  isPlaying.value = false;
}

const { files, open, reset, onCancel, onChange } = useFileDialog({
  accept: 'image/*',
})

// onChange((files) => {
//   const file = files[0];
//   if (!file) return;

//   const reader = new FileReader();
//   reader.onload = (e) => {
//     const img = new Image();
//     img.onload = () => {
//       processImage(img);
//     };
//     img.src = e.target.result;
//   };
//   reader.readAsDataURL(file);
// })

onChange((files) => {
  const file = files[0];
  if (!file) return;

  const reader = new FileReader();
  reader.onload = (e) => {
    const arrayBuffer = e.target.result;
    const byteArray = new Uint8Array(arrayBuffer);
    emit('processImage', byteArray);
  };

  reader.readAsArrayBuffer(file);
});


onCancel(() => {
  /** do something on cancel */
})

// const processImage = (img) => {
//   const canvas = document.createElement('canvas');
//   canvas.width = cols;
//   canvas.height = rows;
//   // const ctx = canvas.getContext('2d');

//   // ctx.drawImage(img, 0, 0, cols, rows);

//   const imageData = ctx.getImageData(0, 0, cols, rows).data;

//   for (let i = 0; i < imageData.length; i += 4) {
//     const r = imageData[i];
//     const g = imageData[i + 1];
//     const b = imageData[i + 2];
//     const a = imageData[i + 3];

//     const intValue = (r << 16) | (g << 8) | b;

//     const pixelIndex = i / 4;

//     matrixFrames.value[currentFrame].pixels[pixelIndex] = intValue;
//   }
// };


function addNewBlankFrame() {
  const newFrame = {
    pixels: new Array(32 * 64),
    delay: 1000
  };

  for (let i = 0; i < newFrame.pixels.length; i++) {
    newFrame.pixels[i] = 0;
  }

  matrixFrames.value.push(newFrame);
}

</script>

<style lang="scss" scoped>
.pixel-matrix {
  display: grid;
  grid-template-columns: repeat(var(--cols), 1fr);
  width: fit-content;
  grid-gap: 1px;
  border: solid #000000;
  padding: 1px;
}

.pixel {
  width: 1rem !important;
  height: 1rem !important;
  margin: 1px;
  aspect-ratio: 1 / 1;
  border: 1px solid #ccc;
  cursor: pointer;
}

.pixel:hover {
  cursor: pointer;
}

.matrix-container {
  max-width: 100%;
  overflow-x: auto;
}
</style>
