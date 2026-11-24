// JavaScript para el módulo de Permisos

function confirmarCambioEstado(mensaje, formId) {
    if (confirm(mensaje)) {
        const form = document.getElementById(formId);
        if (form) form.submit();
    }
}
