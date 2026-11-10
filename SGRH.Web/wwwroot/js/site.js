// SGRH - comportamiento común de la capa de presentación.
(function () {
    // Filtros: al cambiar una lista con data-autoenviar se envía el formulario.
    document.querySelectorAll('[data-autoenviar]').forEach(function (select) {
        select.addEventListener('change', function () { select.form.submit(); });
    });

    // Confirmación antes de inactivar un registro.
    document.querySelectorAll('[data-confirmar]').forEach(function (boton) {
        boton.addEventListener('click', function (e) {
            if (!confirm(boton.dataset.confirmar)) e.preventDefault();
        });
    });

    // Listas dependientes: <select data-depende-de="HotelId" data-url="/Pisos/PorHotel">
    // recarga sus opciones cuando cambia la lista de la que depende (piso -> hotel, bloque -> hotel).
    document.querySelectorAll('select[data-depende-de]').forEach(function (hijo) {
        var padre = document.getElementById(hijo.dataset.dependeDe);
        if (!padre) return;
        var primera = hijo.options[0] ? hijo.options[0].cloneNode(true) : null;

        padre.addEventListener('change', function () {
            hijo.innerHTML = '';
            if (primera) hijo.appendChild(primera.cloneNode(true));
            if (!padre.value || padre.value === '0') return;

            fetch(hijo.dataset.url + '?hotelId=' + encodeURIComponent(padre.value))
                .then(function (r) { return r.json(); })
                .then(function (items) {
                    items.forEach(function (i) { hijo.add(new Option(i.texto, i.valor)); });
                })
                .catch(function () { alert('No se pudo cargar la lista. Intente de nuevo.'); });
        });
    });

    // Tipo de habitación: propone la tarifa base del tipo seleccionado.
    document.querySelectorAll('select[data-tarifa-destino]').forEach(function (select) {
        var destino = document.getElementById(select.dataset.tarifaDestino);
        select.addEventListener('change', function () {
            var tarifa = select.selectedOptions[0] && select.selectedOptions[0].dataset.tarifa;
            if (tarifa && destino) destino.value = tarifa;
        });
    });
})();
