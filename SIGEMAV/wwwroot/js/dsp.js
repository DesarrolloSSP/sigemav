



const importe = document.getElementById('Importe');

// FORMATEAR MIENTRAS ESCRIBE
importe.addEventListener('input', function () {

    let valor =
        this.value.replace(/,/g, '');

    valor =
        valor.replace(/[^\d.]/g, '');

    let partes = valor.split('.');

    if (partes.length > 2) {

        valor =
            partes[0] + '.' + partes[1];
    }

    // LIMITE 200 BILLONES
    if (parseFloat(valor) > 200000000000000) {

        valor = "200000000000000";
    }

    if (valor !== '') {

        let numeros =
            valor.split('.');

        numeros[0] =
            parseInt(
                numeros[0] || 0
            ).toLocaleString('en-US');

        valor =
            numeros.join('.');
    }

    this.value = valor;
});


// LIMPIAR ANTES DE ENVIAR
document.querySelector('form')
    .addEventListener('submit',
        function () {

            importe.value =
                importe.value
                    .replace(/,/g, '');

        });

