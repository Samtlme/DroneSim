import { startSimulation, pauseSimulation, updateSimulation } from './api.js';
import { Simulator } from './simulator.js';
import * as signalR from '@microsoft/signalr';
import DM from './droneManager.js';
import * as THREE from 'three';

const Config = {
  lerpMultiplier: 0.0004, //interpolation speed
};

const simulation = new Simulator('simulator-container');
let lastUpdateTime = performance.now();
const responseEl = document.getElementById('response');
let connection;

document.getElementById('start').onclick = async () => {
  var response = await startSimulation();
  
  if(!connection)
  {
    connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7057/Api/Simulation/droneshub")
    .withAutomaticReconnect()
    .build();

    connection.on("UpdateDrones", (apiDrones) => {
      const now = performance.now();
      const currentIdsSet = new Set(apiDrones.map(d => d.id));

      for (const id in DM.drones) {
        if (!currentIdsSet.has(Number(id))) {
            const drone = DM.drones[id];
            simulation.scene.remove(drone);
            drone.geometry.dispose();
            drone.material.dispose();
            delete DM.drones[id];
        }
      }

      const droneDataMap = {};
      DM.droneData.forEach(dr => { droneDataMap[dr.id] = dr; });

      apiDrones.forEach(d => {
        let drone = DM.drones[d.id];

        if (!drone) {
            drone = DM.spawnDrone(simulation.scene, d.id, d.x, d.y, d.z, d.Color || 0x00ff00);
            DM.drones[d.id] = drone;
        }

        if (droneDataMap[d.id]) {
          droneDataMap[d.id].x = d.x;
          droneDataMap[d.id].y = d.y;
          droneDataMap[d.id].z = d.z;
        } else {
          const newDrone = { id: d.id, x: d.x, y: d.y, z: d.z };
          DM.droneData.push(newDrone);
          droneDataMap[d.id] = newDrone;
        }
      });
    });
  }

  try {
    await connection.start();
    console.log("SignalR connected");
  } catch (err) {
    console.error("Error connecting to hub:", err);
  }

};

document.getElementById('pause').onclick = async () => {
  const msg = await pauseSimulation();
};

document.getElementById('update').onclick = async () => {
  const msg = await updateSimulation();
};

function animateDrones() {
  const now = performance.now();
  const deltaTime = (now - lastUpdateTime) / 1000;
  const lerpSpeed = Math.min(1, deltaTime * Config.lerpMultiplier);

  DM.droneData.forEach(d => {
      const mesh = DM.drones[d.id];
      if (!mesh) return;

      const target = new THREE.Vector3(d.x, d.y, d.z);
      mesh.position.lerp(target, lerpSpeed); 
  });

  requestAnimationFrame(animateDrones);
}

animateDrones();



