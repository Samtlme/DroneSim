export const API_BASE = import.meta.env.VITE_API_BASE + "/Api/";

async function apiCall(endpoint, data = null) {
  try {
    const res = await fetch(`${API_BASE}${endpoint}`,
       { method: 'POST' ,
         headers: {
            'Content-Type': 'application/json'
         },
          body: data ? JSON.stringify(data) : undefined
       });
    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    return await res.json();
  } catch (err) {
      return [];
  }
}

export async function startSimulation(droneCount) { return apiCall('Simulation/Start', droneCount); }
export async function pauseSimulation() { return apiCall('Simulation/Pause'); }
export async function squareFormation() { return apiCall('Simulation/Square'); }
export async function cubeFormation() { return apiCall('Simulation/Cube'); }
export async function resetFormation() { return apiCall('Simulation/resetFormation'); }
export async function sendFormation(points) { return apiCall('Simulation/customformation', points); }
export async function mirrorToVertical() { return apiCall('Simulation/MirrorToVertical'); }
export async function dronesUp() { return apiCall('Simulation/dronesUp'); }
export async function dronesDown() { return apiCall('Simulation/dronesDown'); }
export async function startReplay() { return apiCall('replay/start'); }
export async function stopReplay() { return apiCall('replay/stop'); }

export async function listReplays() { return apiCall('replay/getReplays'); }
export async function playReplay(replayId) { return apiCall('replay/playReplay',replayId); }
export async function deleteReplay(replayId) { return apiCall('replay/deleteReplay',replayId); }


export async function setConfig(configValues) { return apiCall('Simulation/setConfig',configValues); }
