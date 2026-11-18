function abrirModalEditarColor(id, nombre, activo) {
    document.getElementById('editColorId').value = id;
    document.getElementById('editColorNombre').value = nombre;
    document.getElementById('editColorActivo').checked = activo;

    const modal = new bootstrap.Modal(document.getElementById('modalEditarColor'));
    modal.show();
}