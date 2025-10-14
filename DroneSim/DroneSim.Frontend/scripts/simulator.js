import * as THREE from 'three';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';

export class Simulator {
    constructor(containerId)
    {
        this.container = document.getElementById(containerId);
        this.scene = new THREE.Scene();
        this.scene.background = new THREE.Color(0xa0a0a0);

        const width = this.container.clientWidth;
        const height = this.container.clientHeight;

        //camera(fov,ratio,min distance, max distance)
        this.camera = new THREE.PerspectiveCamera(50, window.innerWidth / window.innerHeight, 0.1, 1000);
        this.camera.position.set(0, 20, 50);

        //render options
        this.renderer = new THREE.WebGLRenderer({ antialias: true });
        this.renderer.setSize(width, height, false);
        this.container.appendChild(this.renderer.domElement);

        //Camera movement
        this.controls = new OrbitControls(this.camera, this.renderer.domElement);
        this.controls.enableDamping = true;
        this.controls.dampingFactor = 0.1;  //smooth movement
        this.controls.rotateSpeed = 0.3; 
        this.controls.zoomSpeed = 0.7;
        this.controls.panSpeed = 0.7;
        this.controls.screenSpacePanning = false;
        this.controls.maxPolarAngle = Math.PI / 2;   //Angle limit
        //zoom min-max
        this.controls.minDistance = 10;
        this.controls.maxDistance = 200;
        
        //provisional floor
        this.floorGeometry = new THREE.BoxGeometry(200, 1, 200);
        this.floorMaterial = new THREE.MeshPhongMaterial({ color: 0x808080 });
        this.floor = new THREE.Mesh(this.floorGeometry, this.floorMaterial);
        this.floor.position.y = -0.5;
        this.scene.add(this.floor);

        this.light = new THREE.DirectionalLight(0xffffff, 1);
        this.light.position.set(0, 30, 0); //light position
        this.scene.add(this.light);
        this.scene.add(new THREE.AmbientLight(0x888888));

        this.raycaster = new THREE.Raycaster();
        this.mouse = new THREE.Vector2();

        this.container.addEventListener('click', this.onClick.bind(this));
        this.resizeObserver = new ResizeObserver(this.onResize.bind(this));
        this.resizeObserver.observe(this.container);

        this.animate();
    }

    //Animation loop, future FPS counter implementation goes here
    animate(){
        requestAnimationFrame(this.animate.bind(this));
        this.controls.update(); //needed for damping
        this.renderer.render(this.scene, this.camera);
    }
    
    onResize() {
        const width = this.container.clientWidth;
        const height = this.container.clientHeight;
        this.camera.aspect = width / height;
        this.camera.updateProjectionMatrix();
        this.renderer.setSize(width, height);
    }

    onClick(event) {
        if (!event.ctrlKey) return; //Ctrl + click to move
        //normalize
        const rect = this.container.getBoundingClientRect();
        this.mouse.x = ((event.clientX - rect.left) / rect.width) * 2 - 1;
        this.mouse.y = -((event.clientY - rect.top) / rect.height) * 2 + 1;
        this.raycaster.setFromCamera(this.mouse, this.camera);

        const intersects = this.raycaster.intersectObjects(this.scene.children, true);
        if (intersects.length > 0) {

             fetch("https://localhost:7057/Api/Simulation/movetotarget", {
                method: "POST",
                headers: {
                "Content-Type": "application/json"
                },
                body: JSON.stringify(intersects[0].point)
            })
            .then(res => res.text())
            .then(console.log);
        }
    }
}
