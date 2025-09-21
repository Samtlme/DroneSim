import { startSimulation, pauseSimulation, updateSimulation } from './api.js';
import { initSimulator } from './simulator.js';
import * as signalR from '@microsoft/signalr';
import DM from './droneManager.js';
import * as THREE from 'three';

const simulation = initSimulator('simulator-container');
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
          apiDrones.forEach(d => {

            if(!DM.drones[d.id]){
              DM.drones[d.id] = DM.spawnDrone(simulation.scene, d.id, d.x, d.y, d.z, d.Color || 0x00ff00);
            }

            const existing = DM.droneData.find(dr => dr.id === d.id);
            if(existing){
              existing.x = d.x;
              existing.y = d.y;
              existing.z = d.z;
            }else{
              DM.droneData.push({ id: d.id, x: d.x, y: d.y, z: d.z });
            }
          });
          
          moveDrones(DM.droneData, 1000);
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
    DM.droneData.forEach(d => {
        const mesh = DM.drones[d.id];
        if (!mesh) return;

        const target = new THREE.Vector3(d.x, d.y, d.z);
        mesh.position.lerp(target, 0.01); 
    });

    requestAnimationFrame(animateDrones);
}

animateDrones();

