import * as THREE from 'three';

const droneData = [];
const drones = {};

function spawnDrone(scene, id, x, y, z, color) {
    if (drones[id]) return drones[id];

    const geometry = new THREE.SphereGeometry(0.3, 16, 16);
    const material = new THREE.MeshStandardMaterial({ color: color });
    const drone = new THREE.Mesh(geometry, material);
    drone.position.set(x, y, z);
    scene.add(drone);
    drones[id] = drone;
    return drone;
}

function updateDronePosition(id, x, y, z) {
    const drone = drones[id];
    if (!drone) return;
    drone.position.set(x, y, z);
}

const DroneManager = { drones, droneData, spawnDrone, updateDronePosition };
export default DroneManager;