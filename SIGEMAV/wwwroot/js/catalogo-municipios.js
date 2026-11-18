function abrirModalEditarMunicipio(id, nombre, cveMun, cveZona, zona, cveDeleg, delegacion, alerta, indigena, region, cpdh) {
    // 1. Asignar valores a campos de texto y ocultos
    document.getElementById('editMunicipioId').value = id;
    document.getElementById('editNombre').value = nombre;
    document.getElementById('editCveMun').value = cveMun !== 'null' ? cveMun : '';
    document.getElementById('editZona').value = zona !== 'null' ? zona : '';
    document.getElementById('editDelegacion').value = delegacion !== 'null' ? delegacion : '';
    document.getElementById('editTcmunicipioRegion').value = region !== 'null' ? region : '';

    // 2. Asignar valores seleccionados a los DropDowns (Selects)
    document.getElementById('editCveZona').value = (cveZona !== 'null' && cveZona !== '') ? cveZona : '';
    document.getElementById('editCveDeleg').value = (cveDeleg !== 'null' && cveDeleg !== '') ? cveDeleg : '';

    // 3. Mostrar el modal de Bootstrap
    const modalElement = document.getElementById('modalEditarMunicipio');
    const modal = bootstrap.Modal.getInstance(modalElement) || new bootstrap.Modal(modalElement);
    modal.show();
}