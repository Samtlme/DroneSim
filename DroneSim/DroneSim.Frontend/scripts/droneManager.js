import * as THREE from 'three';

const droneData = [];
const drones = {};

function spawnDrone(scene, id, x, y, z, color) {
    if (drones[id]) return drones[id];

    const geometry = new THREE.SphereGeometry(0.2, 12, 12);
    const material = new THREE.MeshStandardMaterial({ color: color , metalness : 0, roughness : 0});
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

function ChangeDronesSize(value) {
    Object.values(drones).forEach(drone => {
        drone.scale.set(value, value, value);
    });
}

const DroneManager = { drones, droneData, spawnDrone, updateDronePosition, ChangeDronesSize };
export default DroneManager;