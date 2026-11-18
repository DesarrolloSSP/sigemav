function abrirModalEditarModelo(id, marcaId, nombre, activo) {
    document.getElementById('editModeloId').value = id;
    document.getElementById('editModeloMarcaId').value = marcaId;
    document.getElementById('editModeloNombre').value = nombre;
    document.getElementById('editModeloActivo').checked = activo;

    const modal = new bootstrap.Modal(document.getElementById('modalEditarModelo'));
    modal.show();
}