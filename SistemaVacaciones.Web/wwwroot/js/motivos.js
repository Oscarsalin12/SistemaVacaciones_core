// JavaScript para Motivos de Ausencia (CRUD Asíncrono)

async function abrirModalMotivo(id, createUrl, editUrl) {
    const modalElement = $('#modalMotivo');
    const title = id === 0 ? "Nuevo Motivo" : "Editar Motivo";
    const url = id === 0 ? createUrl : `${editUrl}/${id}`;

    const titleEl = document.getElementById('modalTitle');
    if (titleEl) titleEl.innerText = title;

    try {
        const response = await fetch(url);
        const html = await response.text();
        const bodyEl = document.getElementById('modalBody');
        if (bodyEl) bodyEl.innerHTML = html;
        modalElement.modal('show');
    } catch (error) {
        alert('Error al cargar el formulario.');
    }
}

async function guardarMotivo(createUrl, editUrl) {
    const form = document.getElementById('formMotivo');
    if (!form) return;

    const formData = new FormData(form);
    const id = formData.get("Id");
    const url = id == "0" || id == "" ? createUrl : editUrl;

    try {
        const response = await fetch(url, {
            method: 'POST',
            body: formData
        });

        const resultado = await response.json();
        if (resultado.success) {
            location.reload();
        } else {
            alert(resultado.message);
        }
    } catch (error) {
        alert('Error de comunicación con el servidor');
    }
}

async function eliminarMotivo(id, deleteUrl) {
    if (!confirm("¿Deseas eliminar este registro?")) return;

    try {
        const response = await fetch(`${deleteUrl}/${id}`, { method: 'POST' });
        const res = await response.json();

        if (res.success) {
            const row = document.getElementById(`fila-${id}`);
            if (row) row.remove();
        } else {
            alert('No se pudo eliminar el registro. Es posible que esté en uso.');
        }
    } catch (error) {
        alert('Error de comunicación con el servidor');
    }
}
