<template>
  <ol-map
    id="map"
    ref="map"
    :load-tiles-while-animating="true"
    :load-tiles-while-interacting="true"
    class="map"
  >
    <ol-view ref="view" :center="center" :zoom="zoom" />

    <ol-tile-layer>
      <ol-source-osm />
    </ol-tile-layer>

    <ol-overlay
        v-if="toolTipLocation"
        :position="tooltipCoord"
        :offset="[0, -105]"
        positioning="top-center"
      >
        <div class="tooltip tooltip-info">
            <span class="fw-bold">Station: {{ toolTipStationName }}</span>
            <br>
            <span class="fw-normal">CRS: {{ toolTipStationCrs }}</span>
            <br>
            <span class="fw-normal">Owner: {{ toolTipOwner }}</span>
            <br>
            <span class="fw-normal">Coords: {{ `${toolTipLocation[0]}, ${toolTipLocation[1]}` }}</span>
        </div>
    </ol-overlay>

    <ol-vector-layer ref="vectorLayer">
      <ol-source-vector>
        <ol-feature v-for="station in stations" :key="station?.crs" :properties="station">
          <ol-geom-point :coordinates="[station?.longitude, station?.latitude]"></ol-geom-point>
        <ol-style v-if="selectedPoint.crs === station.crs">
        <ol-style-circle :radius="6">
            <ol-style-fill :color="'rgba(51, 136, 255, 1)'"></ol-style-fill>
            <ol-style-stroke color="#FFFFFF" :width="2"></ol-style-stroke>
        </ol-style-circle>
        </ol-style>
      <ol-style v-else>
        <ol-style-circle :radius="4">
          <ol-style-fill :color="'#ff0000'"></ol-style-fill>
          <ol-style-stroke :color="'#FFFFFF'" :width="1"></ol-style-stroke>
        </ol-style-circle>
      </ol-style>
    </ol-feature>
      </ol-source-vector>
    </ol-vector-layer>

    <ol-interaction-select
        ref="clickInteraction"
      :condition="selectCondition"
      :filter="selectInteractionFilter"
      @select="featureSelected"
      >
    </ol-interaction-select>

      <ol-interaction-select
        :condition="selectHoverCondition"
        :filter="selectInteractionFilter"
        @select="featureHovered"
      >
      <ol-style>
        <ol-style-circle :radius="4">
          <ol-style-fill :color="'#60A050'"></ol-style-fill>
          <ol-style-stroke color="#FFFFFF" :width="1"></ol-style-stroke>
        </ol-style-circle>
      </ol-style>
      </ol-interaction-select>
  </ol-map>
</template>

<script setup>
import { ref, inject, useTemplateRef, onMounted, watch, nextTick } from "vue";
import { Point } from "ol/geom";


const { stations } = defineProps({
    stations: {
        type: Array,
        default: () => []
    }
})

const selectedPoint = defineModel("selectedPoint");

const center = ref([-0.1, 51.5]);
const zoom = ref(12);
const map = useTemplateRef('map');
const clickInteraction = useTemplateRef('clickInteraction');
const vectorLayer = useTemplateRef('vectorLayer');

const tooltipCoord = ref(null);
const toolTipLocation = ref("");
const toolTipOwner = ref("");
const toolTipStationName = ref("");
const toolTipStationCrs = ref("");

const selectConditions = inject("ol-selectconditions");
const selectCondition = selectConditions.click;
const selectHoverCondition = selectConditions.pointerMove;

onMounted(async () => {
    centerMap(map?.value?.map, selectedPoint.value);
  map.value?.map.on("pointermove", (event) => {
    if (event.dragging) {
        return;
    }
    tooltipCoord.value = event.coordinate;
    });
});

watch((() => selectedPoint), () => {
    centerMap(map?.value?.map, selectedPoint.value);
}, {deep: true})


const selectInteractionFilter = (feature) => {
  return feature.getGeometry().getType() === "Point";
};

function featureSelected(event) {
    if (event?.selected.length > 0) {
        selectedPoint.value = event.selected[0].values_;
    }
}

const featureHovered = (event) => {
  if (event.selected[0]?.values_ !== undefined) {
    toolTipOwner.value = event.selected[0]?.values_?.owner;
    toolTipStationName.value = event.selected[0]?.values_?.name;
    toolTipStationCrs.value = event.selected[0]?.values_?.crs;
    toolTipLocation.value = event.selected[0].getGeometry().getCoordinates();
  }
  else {
    toolTipLocation.value = "";
  }
};

const centerMap = (mapRef, point) => {
    if (mapRef && point) {
      let geom = new Point([Number(point.longitude), Number(point.latitude)]);
      const extent = geom.getExtent();
      mapRef.updateSize();
      mapRef.getView().fit(extent, {padding: [20, 20, 20, 20], maxZoom: 18});  
    }
    else if (mapRef){
      mapRef.getView().setCenter([-0.1, 51.5]);
      mapRef.getView().setZoom(12);
    } 
  }

</script>

<style lang="scss" scoped>
.map {
  height: 100%;
  width: 100%;
}

.tooltip {
  position: relative;
  background: rgba(0, 0, 0, 0.5);
  border-radius: 4px;
  color: white;
  padding: 4px 8px;
  opacity: 0.8;
  white-space: nowrap;
  font-size: 12px;
  cursor: default;
  user-select: none;
}
.tooltip-info {
  background-color: white;
  color: black;
  border: 1px solid white;
  font-weight: bold;
}
</style>
