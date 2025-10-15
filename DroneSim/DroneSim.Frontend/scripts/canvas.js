import * as api from './api.js';

export function initDrawingCanvas(containerId) {
  const container = document.getElementById(containerId);

  const canvas = document.createElement("canvas");
  const clearBtn = document.createElement("button");
  clearBtn.textContent = "Reset";
  const sendBtn = document.createElement("button");
  sendBtn.textContent = "Send";

  container.appendChild(canvas);
  container.appendChild(document.createElement("br"));
  container.appendChild(clearBtn);
  container.appendChild(sendBtn);

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

