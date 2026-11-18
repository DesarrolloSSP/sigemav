function abrirModalEditarTransmision(id, nombre, activo) {
    document.getElementById('editTransmisionId').value = id;
    document.getElementById('editTransmisionNombre').value = nombre;
    document.getElementById('editTransmisionActivo').checked = activo;

    const modal = new bootstrap.Modal(document.getElementById('modalEditarTransmision'));
    modal.show();
}

