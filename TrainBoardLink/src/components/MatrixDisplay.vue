<template>
  <div class="matrix-container mx-auto">
    <div class="pixel-matrix mx-auto">
      <div
        v-for="(rowPixels, row) in matrixPixels"
        :key="row"
        class="container d-flex flex-row w-100 p-0"
      >
        <div
          v-for="(pixelColor, col) in rowPixels"
          :key="`${row}-${col}`"
          class="pixel"
          @click.left.exact="setPixelColor(row, col)"
          @click.right.prevent.exact="resetPixelColor(row, col)"
          @click.ctrl="resetPixelColor(row, col)"
          @mousemove.shift="setPixelColor(row, col)"
          :style="{ 'background-color': intToHex(matrixPixels[row][col]) }"
        ></div>
    </div>
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

const setPixelColor = (row, col) => {
    matrixPixels.value[row][col] = hexToInt(selectedColor.value);
}

const resetPixelColor = (row, col) => {
    matrixPixels.value[row][col] = 0;
}

const clearMatrix = () => {

  for (let row = 0; row < matrixPixels.value.length; row++) {
      for (let col = 0; col < matrixPixels.value[row].length; col++) {
        matrixPixels.value[row][col] = 0;
    }
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

  const pixels = [];
  for (let i = 0; i < imageData.length; i += 4) {
    const r = imageData[i];
    const g = imageData[i + 1];
    const b = imageData[i + 2];
    const a = imageData[i + 3];

    const intValue = (r << 16) | (g << 8) | b;

    // Calculate the row and column from the 1D index
    const pixelIndex = i / 4;
    const row = Math.floor(pixelIndex / cols);
    const col = pixelIndex % cols;

    matrixPixels.value[row][col] = intValue;
  }
};

</script>

<style lang="scss" scoped>
.pixel-matrix {
  display: grid;
  // grid-template-columns: repeat(var(--cols), 1fr);
  width: fit-content;
//   width: calc((15px * var(--cols)) + (2px * (var(--cols) - 1)) + 2px);
//   grid-gap: 1px;
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
