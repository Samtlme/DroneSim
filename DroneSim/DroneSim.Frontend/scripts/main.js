import 'bootstrap/dist/css/bootstrap.min.css';
import '../styles/main.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import * as api from './api.js';
import { Simulator } from './simulator.js';
import * as signalR from '@microsoft/signalr';
import DM from './droneManager.js';
import * as THREE from 'three';
import { initDrawingCanvas } from './canvas.js';
import { AddConfigSlider } from './Components/configSlider.js';

let isRecording = false;
let darkMode = false;
const Config = {
  lerpMultiplier: 0.5, //interpolation speed
};


initDrawingCanvas('canvas-container');

const simulation = new Simulator('simulator-container');
let lastUpdateTime = performance.now();
let connection;

document.getElementById('droneCount').addEventListener('input', function() {
    this.value = this.value.replace(/[^0-9]/g, '');
    if(this.value.length > 5) this.value = this.value.slice(0,4);
});


document.getElementById('square').onclick = async () => {await api.squareFormation();};
document.getElementById('cube').onclick = async () => {await api.cubeFormation();};
document.getElementById('mirrorVertical').onclick = async () => {await api.mirrorToVertical();};
document.getElementById('droneUp').onclick = async () => {await api.dronesUp();};
document.getElementById('droneDown').onclick = async () => {await api.dronesDown();};
document.getElementById('reset').onclick = async () => {await api.resetFormation();};

document.getElementById('toggleReplay').onclick = async () => {
  const btn = document.getElementById('toggleReplay');
  if (!isRecording) {
    await api.startReplay();
    isRecording = true;
    btn.innerHTML = 'Stop recording <i class="bi bi-stop-circle-fill ms-2 fs-5"></i>';
    btn.classList.add('btn-recording');
  } else {
    await api.stopReplay();
    isRecording = false;
    btn.innerHTML = 'Capture <i class="bi bi-record-circle-fill ms-2 fs-5"></i>';
    btn.classList.remove('btn-recording');
  }
};

document.getElementById('pause').onclick = async function() {
  this.classList.toggle("btn-simulate-hover-active");
  await api.pauseSimulation();
};

async function initSignalR(){
if(!connection)
  {
    connection = new signalR.HubConnectionBuilder()
    .withUrl( api.API_BASE + "Simulation/droneshub")
    .withAutomaticReconnect()
    .build();

    connection.on("UpdateDrones", (apiDrones) => {
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

  if (connection.state === signalR.HubConnectionState.Connected) {
    await connection.stop();
  }

  await connection.start();
};

document.getElementById('start').onclick = async () => {
  
  const dronecount = parseInt(document.getElementById('droneCount').value, 10) || 100;
  await api.startSimulation(dronecount);

  try {
    initSignalR();
    document.getElementById("start").innerHTML = 'Restart <i class="bi bi-arrow-counterclockwise ms-2 fs-4"></i>';
    console.log("SignalR connected");
  } catch (err) {
    console.error("Error connecting to hub:", err);
  }

};

function animateDrones() {
  const now = performance.now();
  const deltaTime = (now - lastUpdateTime) / 1000;
  lastUpdateTime = now;
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

//Configuration tab
AddConfigSlider('config-options', 'CohesionSpeedFactor', 'Cohesion Speed', 0, 3, 0.1,  0.3);
AddConfigSlider('config-options', 'SeparationSpeedFactor', 'Separation Speed', 0, 5, 0.1, 0.5);
AddConfigSlider('config-options', 'MaxDroneSpeedLimit', 'Max Drone Speed', 0, 5, 0.1,  1);
AddConfigSlider('config-options', 'SwarmSpeedMultiplier', 'Swarm Speed Multiplier', 0, 5, 1, 1);
AddConfigSlider('config-options', 'MinSeparationDistance', 'Min Separation Distance', 0, 10, 1, 3);
AddConfigSlider('config-options', 'WindForceFactor', 'Wind Force Factor', 0, 1, 0.1, 0.2 );
AddConfigSlider('config-options', 'TargetThreshold', 'Target Threshold Distance', 0, 100, 1, 10);

AddConfigSlider('simulator-options', 'SimulatorBallSize', 'Drones Size', 0, 10, 0.1, 1);
const droneSizeSlider = document.getElementById('SimulatorBallSize');

droneSizeSlider.addEventListener('input', () => {
    const value = parseFloat(droneSizeSlider.value);
    DM.ChangeDronesSize(value);
});

function ResetAllSliders(containerId) {
  const container = document.getElementById(containerId);
  if (!container) return;

  const sliders = container.querySelectorAll('input[type="range"]');
  sliders.forEach(slider => {
    slider.value = slider.defaultValue;
    const span = slider.parentElement.querySelector('span');
    if (span) span.textContent = slider.defaultValue;
  });
}
document.getElementById("resetDefaults").addEventListener("click",() => ResetAllSliders("config-options"));
document.getElementById('applyConfig').addEventListener("click",async () => 
{
  const container = document.getElementById('config-options');
    if (!container) return;

    const sliders = container.querySelectorAll('input[type="range"]');
    const configValues = {};
    sliders.forEach(slider => {
      configValues[slider.id] = parseFloat(slider.value);
    });
    await api.setConfig(configValues);
});

//Replays tab

async function loadReplays() {
  const list = document.getElementById('replayList');
  list.innerHTML = '';
  const replays = await api.listReplays();

replays.forEach(replay => {
  const li = document.createElement('li');
  li.className = 'list-group-item d-flex justify-content-between align-items-center';

  const info = document.createElement('span');
  info.textContent = `Replay ${replay.id.substring(0, 8)} — ${new Date(replay.startedAt).toLocaleString()}`;

  const btnGroup = document.createElement('div');
  btnGroup.className = 'btn-group gap-2';

  const deleteBtn = document.createElement('button');
  deleteBtn.className = 'btn btn-small';
  deleteBtn.innerHTML = '<i class="bi bi-trash3 fs-4"></i>';
  deleteBtn.title = 'Eliminar';
  deleteBtn.onclick = e => {
    e.stopPropagation();
    api.deleteReplay(replay.id);
    li.remove();
  };

  const playBtn = document.createElement('button');
  playBtn.className = 'btn btn-small';
  playBtn.innerHTML = '<i class="bi bi-play-fill fs-4"></i>';
  playBtn.title = 'Reproducir';
  playBtn.onclick = e => {
    e.stopPropagation();
    document.querySelectorAll('#replayList .list-group-item').forEach(el => el.classList.remove('active'));
    li.classList.add('active');
    initSignalR();
    api.playReplay(replay.id);
  };

  btnGroup.appendChild(playBtn);
  btnGroup.appendChild(deleteBtn);
  li.appendChild(info);
  li.appendChild(btnGroup);

  list.appendChild(li);
});

}

const replaysTab = document.getElementById('replays-tab');
replaysTab.addEventListener('shown.bs.tab', loadReplays);


function toggleDarkMode() {
    darkMode = !darkMode;

    //Background
    simulation.scene.background = new THREE.Color(darkMode ? 0x101010 : 0xb3b0bf);
    simulation.light.intensity = darkMode ? 0.2 : 1.4;
    simulation.scene.children.forEach(obj => {
        if (obj.type === "AmbientLight") {
            obj.intensity = darkMode ? 0.4 : 0.888;
        }
    });

    Object.values(DM.drones).forEach(drone => {

        if (darkMode) {
            drone.material.emissive = new THREE.Color(drone.material.color);
            drone.material.emissiveIntensity = 0.9;
            //Glow setup drones
            if (!drone.userData.glow) {
                const glowGeometry = new THREE.SphereGeometry(0.45, 8, 8);
                const glowMaterial = new THREE.MeshBasicMaterial({
                    color: drone.material.color,
                    transparent: true,
                    opacity: 0.25
                });
                const glow = new THREE.Mesh(glowGeometry, glowMaterial);
                glow.renderOrder = 1;//so we don't mess up rendering layers
                drone.add(glow);
                drone.userData.glow = glow;
            } else {
                drone.userData.glow.visible = true;
            }
        } else {
            drone.material.emissive = new THREE.Color(0x000000);
            drone.material.emissiveIntensity = 0;

            if (drone.userData.glow) {
                drone.userData.glow.visible = false;
            }
        }
        drone.material.needsUpdate = true;
    });

    if (simulation.glowGrid) {
      simulation.glowGrid.visible = darkMode;
    }

    if(simulation.floor){
      simulation.floorMaterial = darkMode ? new THREE.MeshPhongMaterial({ color: 0x434254 }) :
                                            new THREE.MeshPhongMaterial({ color: 0x747391 });
    }
}

const darkModeBtn = document.getElementById('nightModeToggle');
darkModeBtn.addEventListener('click', toggleDarkMode);