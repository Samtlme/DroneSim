import * as THREE from 'three';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';

export function initSimulator(containerId) {
    const scene = new THREE.Scene();
    scene.background = new THREE.Color(0xa0a0a0);
    const container = document.getElementById(containerId);
    const width = container.clientWidth;
    const height = container.clientHeight;

    //camera(fov,ratio,min distance, max distance)
    const camera = new THREE.PerspectiveCamera(50, window.innerWidth / window.innerHeight, 0.1, 1000);
    camera.position.set(0, 20, 50);

    //render options
    const renderer = new THREE.WebGLRenderer({ antialias: true });
    renderer.setSize(width, height, false);
    container.appendChild(renderer.domElement);

    //provisional floor
    const geometry = new THREE.BoxGeometry(200, 1, 200);
    const material = new THREE.MeshPhongMaterial({ color: 0x808080 });
    const floor = new THREE.Mesh(geometry, material);
    floor.position.y = -0.5;
    scene.add(floor);

    const light = new THREE.DirectionalLight(0xffffff, 1);
    light.position.set(10, 20, 10); //light position
    scene.add(light);
    scene.add(new THREE.AmbientLight(0x404040));

    //Camera movement
    const controls = new OrbitControls(camera, renderer.domElement);
    controls.enableDamping = true;
    controls.dampingFactor = 0.1;  //smooth movement
    controls.rotateSpeed = 0.3; 
    controls.zoomSpeed = 0.7;
    controls.panSpeed = 0.7;
    controls.screenSpacePanning = false;
    controls.maxPolarAngle = Math.PI / 2;   //Angle limit
    //zoom min-max
    controls.minDistance = 10;
    controls.maxDistance = 200;

    //Animation loop, future FPS counter implementation goes here
    function animate() {
        requestAnimationFrame(animate);
        controls.update();  //needed for damping
        renderer.render(scene, camera);
    }

    animate();

    //Resize handling
    const resizeObserver = new ResizeObserver(() => {
        const width = container.clientWidth;
        const height = container.clientHeight;
        camera.aspect = width / height;
        camera.updateProjectionMatrix();
        renderer.setSize(width, height);
    });
    resizeObserver.observe(container);


    return { scene, camera, renderer, controls };
}
