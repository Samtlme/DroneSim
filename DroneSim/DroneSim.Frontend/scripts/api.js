const API_BASE = 'https://localhost:7057/Api/Simulation/'; //temp dev address

async function apiCall(endpoint, data = null) {
  try {
    const res = await fetch(`${API_BASE}${endpoint}`,
       { method: 'POST' ,
         headers: {
            'Content-Type': 'application/json'
         },
          body: data ? JSON.stringify(data) : undefined
       });
    return await res.text();
  } catch (err) {
    return 'Error: ' + err;
  }
}

// api.js
export async function startSimulation() { return apiCall('Start'); }
export async function pauseSimulation() { return apiCall('Pause'); }
export async function squareFormation() { return apiCall('Square'); }
export async function cubeFormation() { return apiCall('Cube'); }
export async function resetFormation() { return apiCall('resetFormation'); }
export async function sendFormation(points) { return apiCall('customformation', points); }
