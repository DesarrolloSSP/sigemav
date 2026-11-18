function abrirModalEditarMarca(id, nombre, activo) {
    document.getElementById('editMarcaId').value = id;
    document.getElementById('editMarcaNombre').value = nombre;
    document.getElementById('editMarcaActivo').checked = activo;

    const modal = new bootstrap.Modal(document.getElementById('modalEditarMarca'));
    modal.show();
}