<template>
  <div class="matrix-container mx-auto">
    <div class="pixel-matrix mx-auto" style="--cols: 64;">
        <div
          v-for="(pixel, index) in matrixPixels"
          :key="index"
          class="pixel"
          @click.left.exact="setPixelColor(index)"
          @click.right.prevent.exact="resetPixelColor(index)"
          @click.ctrl="resetPixelColor(index)"
          @mousemove.shift="setPixelColor(index)"
          :style="{ 'background-color': intToHex(matrixPixels[index]) }"
        ></div>
    </div>
    <div class="mx-auto d-flex align-items-center ms-4 mt-2 justify-content-start">
      <button class="btn btn-primary me-1" type="button" @click="open">
        Upload Image
      </button>
      <input type="color" class="form-control form-control-color me-1" v-model="selectedColor" />
      <button @click="clearMatrix" class="btn btn-danger">Clear</button>
  </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { hexToInt, intToHex } from '@/utils/ColourConverter';
import { useFileDialog } from '@vueuse/core'
const cols = 64;
const rows = 32;

const matrixPixels = defineModel('matrixPixels', {type: Array})

const selectedColor = ref("#ff0000");

const setPixelColor = (index) => {
    matrixPixels.value[index] = hexToInt(selectedColor.value);
}

const resetPixelColor = (index) => {
    matrixPixels.value[index] = 0;
}

const clearMatrix = () => {

  for (let index = 0; index < matrixPixels.value.length; index++) {
      matrixPixels.value[index] = 0;
  }
}

const { files, open, reset, onCancel, onChange } = useFileDialog({
  accept: 'image/*',
})

onChange((files) => {
  const file = files[0];
  if (!file) return;

  const reader = new FileReader();
  reader.onload = (e) => {
    const img = new Image();
    img.onload = () => {
      processImage(img);
    };
    img.src = e.target.result;
  };
  reader.readAsDataURL(file);
})

onCancel(() => {
  /** do something on cancel */
})

const processImage = (img) => {
  const canvas = document.createElement('canvas');
  canvas.width = cols;
  canvas.height = rows;
  const ctx = canvas.getContext('2d');

  ctx.drawImage(img, 0, 0, cols, rows);

  const imageData = ctx.getImageData(0, 0, cols, rows).data;

  for (let i = 0; i < imageData.length; i += 4) {
    const r = imageData[i];
    const g = imageData[i + 1];
    const b = imageData[i + 2];
    const a = imageData[i + 3];

    const intValue = (r << 16) | (g << 8) | b;

    const pixelIndex = i / 4;

    matrixPixels.value[pixelIndex] = intValue;
  }
};

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
