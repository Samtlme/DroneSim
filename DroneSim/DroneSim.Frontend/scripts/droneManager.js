import * as THREE from 'three';

const drones = {};

function spawnDrone(scene, id, x, y, z, color = 0x0000ff) {
    if (drones[id]) return drones[id];

    const geometry = new THREE.SphereGeometry(0.5, 16, 16);
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

//mock positions for spanw test drones
/*
const droneData = [
    { id: 1, x: 0, y: 1, z: 0, color: 0xff0000 },
    { id: 2, x: 2, y: 1, z: -1, color: 0x00ff00 },
    { id: 33, x: -2, y: 1, z: 1, color: 0x0000ff }
];
*/

const DroneManager = { drones, spawnDrone, updateDronePosition };
export default DroneManager;