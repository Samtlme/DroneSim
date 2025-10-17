import * as api from './api.js';

export function initDrawingCanvas(containerId) {
  const container = document.getElementById(containerId);

  const canvas = document.createElement("canvas");
  canvas.width = 465;
  canvas.height = 230;
  const btnOptions = document.createElement("div");
  btnOptions.style.display = "flex";
  btnOptions.style.gap = "10px";
  btnOptions.style.justifyContent = "center";
  const clearBtn = document.createElement("button");
  clearBtn.classList.add("btn", "btn-primary");
  clearBtn.textContent = "Clear";
  const sendBtn = document.createElement("button");
  sendBtn.classList.add("btn");
  sendBtn.textContent = "Send";

  container.appendChild(canvas);
  container.appendChild(document.createElement("br"));
  btnOptions.appendChild(clearBtn);
  btnOptions.appendChild(sendBtn);
  container.appendChild(btnOptions);

  const ctx = canvas.getContext("2d");
  ctx.lineWidth = 5;
  const points = [];
  let drawing = false;

  canvas.onmousedown = () => {
    drawing = true;
    ctx.beginPath();
  };

  canvas.onmouseup = () => {
    drawing = false;
    ctx.beginPath();
  };

  canvas.onmousemove = e => {
    if (!drawing) return;
    const rect = canvas.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;
    points.push({ x, y });

    ctx.lineTo(x, y);
    ctx.stroke();
  };

  clearBtn.onclick = () => {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    points.length = 0;
  };

  sendBtn.onclick = () => {
    api.sendFormation(points);
  };
}

