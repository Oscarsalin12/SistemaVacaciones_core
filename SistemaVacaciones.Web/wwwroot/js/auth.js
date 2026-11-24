// JavaScript para el módulo de Autenticación, Perfil y Modales Dinámicos

async function cargarFormulario(url, titulo) {
    const modalElement = document.getElementById('modalGeneral');
    if (!modalElement) return;

    let modal = bootstrap.Modal.getInstance(modalElement);
    if (!modal) {
        modal = new bootstrap.Modal(modalElement);
    }

    const cuerpo = document.getElementById('modalCuerpo');
    const tituloEl = document.getElementById('modalTitulo');

    if (tituloEl) tituloEl.innerText = titulo;
    if (cuerpo) cuerpo.innerHTML = '<div class="text-center p-4"><div class="spinner-border text-primary" role="status"></div></div>';

    modal.show();

    try {
        const respuesta = await fetch(url, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });
        if (!respuesta.ok) throw new Error("Error al cargar");
        const html = await respuesta.text();
        if (cuerpo) cuerpo.innerHTML = html;
    } catch (error) {
        if (cuerpo) cuerpo.innerHTML = `<div class="alert alert-danger m-3">Error al cargar el formulario: ${error.message}</div>`;
    }
}

document.addEventListener('submit', async (e) => {
    if (e.target && e.target.id === 'formGenerico') {
        e.preventDefault();
        const btn = e.target.querySelector('button[type="submit"]');
        if (btn) {
            btn.disabled = true;
            btn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Guardando...';
        }

        try {
            const formData = new FormData(e.target);
            const res = await fetch(e.target.action, {
                method: 'POST',
                body: formData,
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });

            if (res.ok) {
                location.reload();
            } else {
                const htmlError = await res.text();
                const cuerpo = document.getElementById('modalCuerpo');
                if (cuerpo) cuerpo.innerHTML = htmlError;
            }
        } catch (error) {
            if (typeof toastr !== 'undefined') {
                toastr.error('Error de conexión con el servidor');
            } else {
                alert('Error de conexión con el servidor');
            }
            if (btn) {
                btn.disabled = false;
                btn.innerHTML = 'Guardar';
            }
        }
    }
});
