// JavaScript para Departamentos (CRUD Asíncrono)

async function abrirModalDepartamento(id, createUrl, editUrl) {
    const modalElement = $('#modalDepartamento');
    const title = id === 0 ? "Nuevo Departamento" : "Editar Departamento";
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

async function guardarDepartamento(createUrl, editUrl) {
    const form = document.getElementById('formDepartamento');
    if (!form) return;
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

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

async function eliminarDepartamento(id, deleteUrl) {
    if (!confirm("¿Estás seguro de que deseas eliminar este departamento?")) return;

    try {
        const response = await fetch(`${deleteUrl}/${id}`, { method: 'POST' });
        const res = await response.json();

        if (res.success) {
            const row = document.getElementById(`fila-${id}`);
            if (row) row.remove();
        } else {
            alert("No se pudo eliminar el departamento. Verifica que no tenga dependencias.");
        }
    } catch (error) {
        alert('Error de comunicación con el servidor');
    }
}
