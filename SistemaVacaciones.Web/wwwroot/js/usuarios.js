// JavaScript para Usuarios y Roles (CRUD Asíncrono)

let tipoEntidadActual = '';

async function abrirModalUsuario(tipoEntidad, id, urlFormUsuario, urlFormRol) {
    tipoEntidadActual = tipoEntidad;
    const title = id === 0 ? `Nuevo ${tipoEntidad}` : `Editar ${tipoEntidad}`;
    const baseUrl = tipoEntidad === 'Usuario' ? urlFormUsuario : urlFormRol;
    const url = `${baseUrl}/${id}`;

    const titleEl = document.getElementById('modalTitle');
    if (titleEl) titleEl.innerText = title;

    try {
        const response = await fetch(url);
        const html = await response.text();
        const bodyEl = document.getElementById('modalBody');
        if (bodyEl) bodyEl.innerHTML = html;
        $('#modalGenerico').modal('show');
    } catch (error) {
        alert('Error al cargar el formulario');
    }
}

async function guardarUsuarioRol(urlSaveUsuario, urlSaveRol) {
    const form = document.getElementById('formDinamico');
    if (!form) return;
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const formData = new FormData(form);
    const url = tipoEntidadActual === 'Usuario' ? urlSaveUsuario : urlSaveRol;

    try {
        const response = await fetch(url, {
            method: 'POST',
            body: formData
        });
        const res = await response.json();

        if (res.success) {
            location.reload();
        } else {
            alert(res.message || 'Error al guardar');
        }
    } catch (error) {
        alert('Error en la comunicación con el servidor');
    }
}

async function eliminarUsuarioRol(tipoEntidad, id, urlDeleteUsuario, urlDeleteRol) {
    if (!confirm(`¿Estás seguro de que deseas eliminar este ${tipoEntidad}?`)) return;

    const url = tipoEntidad === 'Usuario' ? urlDeleteUsuario : urlDeleteRol;

    try {
        const response = await fetch(`${url}/${id}`, { method: 'POST' });
        const res = await response.json();

        if (res.success) {
            const row = document.getElementById(`fila-${tipoEntidad}-${id}`);
            if (row) row.remove();
        } else {
            alert(`No se pudo eliminar el ${tipoEntidad}. Verifica sus dependencias.`);
        }
    } catch (error) {
        alert('Error de conexión');
    }
}
