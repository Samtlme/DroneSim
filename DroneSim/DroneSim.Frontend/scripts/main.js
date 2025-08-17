import { startSimulation, pauseSimulation, updateSimulation } from './api.js';

const responseEl = document.getElementById('response');

document.getElementById('start').onclick = async () => {
  const msg = await startSimulation();
  responseEl.textContent = msg;
};

document.getElementById('pause').onclick = async () => {
  const msg = await pauseSimulation();
  responseEl.textContent = msg;
};

document.getElementById('update').onclick = async () => {
  const msg = await updateSimulation();
  responseEl.textContent = msg + ' | Drones: ' + JSON.stringify(drones);
};
