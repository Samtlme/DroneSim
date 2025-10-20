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

export async function startSimulation(droneCount) { return apiCall('Start', droneCount); }
export async function pauseSimulation() { return apiCall('Pause'); }
export async function squareFormation() { return apiCall('Square'); }
export async function cubeFormation() { return apiCall('Cube'); }
export async function resetFormation() { return apiCall('resetFormation'); }
export async function sendFormation(points) { return apiCall('customformation', points); }
export async function mirrorToVertical() { return apiCall('MirrorToVertical'); }
export async function dronesUp() { return apiCall('dronesUp'); }
export async function dronesDown() { return apiCall('dronesDown'); }

export async function setConfig(configValues) { return apiCall('setConfig',configValues); }
