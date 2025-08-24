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
    <div class="mx-auto d-flex align-items-center me-4 mt-2 justify-content-end">
      <input type="color" class="form-control form-control-color" v-model="selectedColor" />
      <button @click="clearMatrix" class="btn btn-danger">Clear</button>
  </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { hexToInt, intToHex } from '@/utils/ColourConverter';
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
