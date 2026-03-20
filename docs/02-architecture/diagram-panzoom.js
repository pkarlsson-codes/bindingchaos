window.setupPanZoom = function setupPanZoom() {
  const container = document.querySelector('.diagram-container');
  const el = document.querySelector('.mermaid');
  if (container && el) {
    let scale = 1, tx = 0, ty = 0, dragging = false, startX, startY, startTx, startTy;
    el.style.transformOrigin = '0 0';
    function apply() { el.style.transform = `translate(${tx}px,${ty}px) scale(${scale})`; }
    container.addEventListener('mousedown', e => {
      dragging = true; startX = e.clientX; startY = e.clientY; startTx = tx; startTy = ty;
      e.preventDefault();
    });
    window.addEventListener('mousemove', e => {
      if (!dragging) return;
      tx = startTx + (e.clientX - startX); ty = startTy + (e.clientY - startY); apply();
    });
    window.addEventListener('mouseup', () => { dragging = false; });
    container.addEventListener('wheel', e => {
      e.preventDefault();
      const r = container.getBoundingClientRect();
      const mx = e.clientX - r.left, my = e.clientY - r.top;
      const factor = e.deltaY > 0 ? 0.9 : 1.1;
      const newScale = Math.max(0.1, Math.min(10, scale * factor));
      tx = mx - (mx - tx) * (newScale / scale);
      ty = my - (my - ty) * (newScale / scale);
      scale = newScale; apply();
    }, { passive: false });
  }
}
