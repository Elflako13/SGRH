(function () {
    document.querySelectorAll('[data-autoenviar]').forEach(function (select) {
        select.addEventListener('change', function () { select.form.submit(); });
    });

    document.querySelectorAll('[data-confirmar]').forEach(function (boton) {
        boton.addEventListener('click', function (e) {
            if (!confirm(boton.dataset.confirmar)) e.preventDefault();
        });
    });

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

    document.querySelectorAll('select[data-tarifa-destino]').forEach(function (select) {
        var destino = document.getElementById(select.dataset.tarifaDestino);
        select.addEventListener('change', function () {
            var tarifa = select.selectedOptions[0] && select.selectedOptions[0].dataset.tarifa;
            if (tarifa && destino) destino.value = tarifa;
        });
    });
})();

(function () {
    var resaltar = document.body.dataset.resaltar;
    if (resaltar) {
        var objetivo = resaltar === 'nuevo'
            ? document.querySelector('.tabla tbody tr[data-id], .grid-hoteles [data-id]')
            : document.querySelector('[data-id="' + resaltar + '"]');

        if (objetivo) {
            objetivo.classList.add('resaltado');
            if (resaltar === 'nuevo') {
                var destino = objetivo.querySelector('b, .hotel-nombre');
                if (destino) destino.insertAdjacentHTML('beforeend', '<span class="etiqueta-nuevo">Nuevo</span>');
            }
            objetivo.scrollIntoView({ block: 'center', behavior: 'smooth' });
        }
    }

    var arriba = document.querySelector('[data-arriba]');
    if (arriba) {
        window.addEventListener('scroll', function () {
            arriba.classList.toggle('visible', window.scrollY > 300);
        });
        arriba.addEventListener('click', function () { window.scrollTo({ top: 0, behavior: 'smooth' }); });
    }

    document.querySelectorAll('.fila-elegible').forEach(function (fila) {
        var radio = fila.querySelector('input[type=radio]');
        if (!radio) return;
        var marcar = function () {
            document.querySelectorAll('.fila-elegible.elegida').forEach(function (f) { f.classList.remove('elegida'); });
            fila.classList.add('elegida');
        };
        if (radio.checked) marcar();
        fila.addEventListener('click', function () { radio.checked = true; marcar(); });
    });
})();
