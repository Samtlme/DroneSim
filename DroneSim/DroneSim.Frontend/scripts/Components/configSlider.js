export function AddConfigSlider(containerId, id, labelText, min=0, max=100, step=1, initialValue=null) {
    const container = document.getElementById(containerId);
    if (!container) return;

    if (initialValue === null) initialValue = Math.floor((min + max) / 2);

    const div = document.createElement('div');
    div.style.display = 'flex';
    div.style.alignItems = 'center';
    div.style.gap = '0.5rem';
    div.style.padding = '0.3rem 0.5rem';
    const index = container.children.length;
    if (index % 2 === 0) {
        div.style.background = 'linear-gradient(to right, rgba(0,0,0,0) 0%, rgba(0,0,0,0.6) 50%, rgba(0,0,0,0) 100%)';
    }

    const label = document.createElement('label');
    label.htmlFor = id;
    label.className = 'form-label mb-0';
    label.style.minWidth = "150px"
    label.style.width = '60%';
    label.style.fontSize = '0.8rem';
    label.textContent = labelText;
    div.appendChild(label);

    const input = document.createElement('input');
    input.type = 'range';
    input.className = 'form-range';
    input.id = id;
    input.min = min;
    input.max = max;
    input.step = step;
    input.value = initialValue;
    input.defaultValue = initialValue;
    input.style.flexGrow = '1';
    div.appendChild(input);

    const span = document.createElement('span');
    span.className = 'ms-2 fw-bold';
    span.textContent = initialValue;
    span.style.display = 'inline-block';
    span.style.minWidth = '4ch';
    span.style.textAlign = 'right';
    span.style.overflow = 'hidden';
    span.style.whiteSpace = 'nowrap';
    div.appendChild(span);

    input.addEventListener('input', () => {
        span.textContent = input.value;
    });

  container.appendChild(div);
}