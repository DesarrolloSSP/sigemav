function abrirModalEditarArea(id, nombre, activo, activoFinancieros) {
    document.getElementById('editAreaId').value = id;
    document.getElementById('editAreaNombre').value = nombre;
    document.getElementById('editAreaActivo').checked = activo;
    // document.getElementById('editAreaActivoFinancieros').checked = activoFinancieros;

    const modal = new bootstrap.Modal(document.getElementById('modalEditarArea'));
    modal.show();
}