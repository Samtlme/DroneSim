const API_BASE = 'https://localhost:7057/Api/Simulation/'; //temp dev address

async function apiCall(endpoint) {
  try {
    const res = await fetch(`${API_BASE}${endpoint}`, { method: 'POST' });
    return await res.text();
  } catch (err) {
    return 'Error: ' + err;
  }
}

async function startSimulation() {
  return apiCall('Start');
}

async function pauseSimulation() {
  return apiCall('Pause');
}

async function squareFormation() {
  return apiCall('Square');
}

async function cubeFormation() {
  return apiCall('Cube');
}

async function resetFormation() {
  return apiCall('resetFormation');
}

export default{
  startSimulation,
  pauseSimulation,
  squareFormation,
  resetFormation,
  cubeFormation
}