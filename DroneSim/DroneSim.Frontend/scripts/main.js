import { startSimulation, pauseSimulation, updateSimulation } from './api.js';
import { initSimulator } from './simulator.js';
import * as signalR from '@microsoft/signalr';
import DM from './droneManager.js';


const simulation = initSimulator('simulator-container');
const responseEl = document.getElementById('response');

document.getElementById('start').onclick = async () => {
  const msg = await startSimulation();
  responseEl.textContent = msg;
  
  DM.droneData.forEach(d => DM.spawnDrone(simulation.scene,d.id, d.x, d.y, d.z, d.color));  //testing drone spawn
};

document.getElementById('pause').onclick = async () => {
  const msg = await pauseSimulation();
  responseEl.textContent = msg;
};

document.getElementById('update').onclick = async () => {
  const msg = await updateSimulation();
  responseEl.textContent = msg + ' | Drones: ' + JSON.stringify(DM.drones);
  //moveDrones
};


/*
for example for moveDrones->
const targetPositions = [
    { id: 1, x: 0, y: 5, z: 0 },
    { id: 2, x: 2, y: 4, z: -1 },
    { id: 3, x: -2, y: 6, z: 1 }
];
*/ 

function moveDrones(targetPositions, duration = 1000) { //duration = total time to move
    targetPositions.forEach(pos => {
        const drone = DM.drones[pos.id];
        if (!drone) return;

        const startPos = drone.position.clone();
        const targetPos = new THREE.Vector3(pos.x, pos.y, pos.z);
        const startTime = performance.now();

        function animate() {
            const now = performance.now();
            const t = Math.min((now - startTime) / duration, 1);
            drone.position.lerpVectors(startPos, targetPos, t);

            if (t < 1) requestAnimationFrame(animate);
        }

        animate();
    });
}
