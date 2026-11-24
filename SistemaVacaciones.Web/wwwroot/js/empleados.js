// JavaScript para el módulo de Empleados (Modales y Acciones)

async function abrirModalEmpleado(url, titulo) {
    const modalEl = document.getElementById('modalEmpleado');
    if (!modalEl) return;

    let modal = bootstrap.Modal.getInstance(modalEl);
    if (!modal) modal = new bootstrap.Modal(modalEl);

    const titleEl = document.getElementById('modalEmpleadoTitulo');
    const bodyEl = document.getElementById('modalEmpleadoCuerpo');

    if (titleEl) titleEl.innerText = titulo;
    if (bodyEl) bodyEl.innerHTML = '<div class="text-center p-4"><div class="spinner-border text-primary"></div></div>';

    modal.show();

    try {
        const res = await fetch(url);
        if (!res.ok) throw new Error("Error al cargar");
        const html = await res.text();
        if (bodyEl) bodyEl.innerHTML = html;
    } catch (err) {
        if (bodyEl) bodyEl.innerHTML = `<div class="alert alert-danger m-3">${err.message}</div>`;
    }
}

async function guardarEmpleado(url) {
    const form = document.getElementById('formEmpleado');
    if (!form) return;
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const formData = new FormData(form);

    try {
        const res = await fetch(url, {
            method: 'POST',
            body: formData
        });
        const data = await res.json();
        if (data.success) {
            location.reload();
        } else {
            alert(data.message || 'Error al guardar el empleado.');
        }
    } catch (err) {
        alert('Error de conexión con el servidor');
    }
}
