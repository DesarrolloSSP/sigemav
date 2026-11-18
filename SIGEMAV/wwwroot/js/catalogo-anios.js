function abrirModalEditarAnio(id, descripcion, activo) {
    document.getElementById('editAnioId').value = id;
    document.getElementById('editAnioDescripcion').value = descripcion;
    document.getElementById('editAnioActivo').checked = activo;

    const modal = new bootstrap.Modal(document.getElementById('modalEditarAnio'));
    modal.show();
}