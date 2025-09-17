const API_BASE = 'https://localhost:7057/Api/Simulation/'; //temp dev address

export async function startSimulation() {
  return apiCall('Start');
}

export async function pauseSimulation() {
  return apiCall('Pause');
}

export async function updateSimulation() {
  return apiCall('Update');
}

async function apiCall(endpoint) {
  try {
    const res = await fetch(`${API_BASE}${endpoint}`, { method: 'POST' });
    return await res.text();
  } catch (err) {
    return 'Error: ' + err;
  }
}