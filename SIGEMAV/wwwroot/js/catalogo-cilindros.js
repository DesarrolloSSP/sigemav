function abrirModalEditarCilindro(id, nombre, activo) {
    document.getElementById('editCilindroId').value = id;
    document.getElementById('editCilindroNombre').value = nombre;
    document.getElementById('editCilindroActivo').checked = activo;

    const modal = new bootstrap.Modal(document.getElementById('modalEditarCilindro'));
    modal.show();
}