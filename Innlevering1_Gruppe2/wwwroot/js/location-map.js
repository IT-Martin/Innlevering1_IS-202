// Velger og viser plassering for ressurser og hjelpebehov.
document.querySelectorAll('[data-picker-map], [data-detail-map]').forEach(element => {
    if (!window.L) { element.textContent = 'Kartet kunne ikke lastes. Prøv å laste siden på nytt.'; return; }
    const form = element.closest('form');
    const latitude = form?.querySelector('[data-latitude]');
    const longitude = form?.querySelector('[data-longitude]');
    const initialLatitude = Number.parseFloat(latitude?.value || element.dataset.lat);
    const initialLongitude = Number.parseFloat(longitude?.value || element.dataset.lng);
    const selected = Number.isFinite(initialLatitude) && Number.isFinite(initialLongitude) && Math.abs(initialLatitude) <= 90 && Math.abs(initialLongitude) <= 180;
    const map = L.map(element).setView(selected ? [initialLatitude, initialLongitude] : [58.1467,7.9956], selected ? 14 : 12);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',{maxZoom:19,attribution:'© OpenStreetMap-bidragsytere'}).addTo(map);
    let marker = selected ? L.marker([initialLatitude,initialLongitude]).addTo(map) : null;
    const status = form?.querySelector('[data-picker-status], #map-message');
    const button = form?.querySelector('#confirm-position');
    const currentLocationButton = form?.querySelector('[data-use-current-location]');
    const setPosition = point => {
        if (marker) marker.setLatLng(point); else marker = L.marker(point).addTo(map);
        latitude.value = point.lat;
        longitude.value = point.lng;
        map.setView(point, 14);
        if (status) status.textContent = 'Plassering valgt.';
        if (button) button.disabled = false;
        ['latitude','latitudeDisplay'].forEach(id => { const node = form.querySelector('#'+id); if (node) node.textContent = point.lat.toFixed(6); });
        ['longitude','longitudeDisplay'].forEach(id => { const node = form.querySelector('#'+id); if (node) node.textContent = point.lng.toFixed(6); });
    };
    if (button && selected) button.disabled = false;
    if (selected) {
        if (status) status.textContent = 'Plasseringen er valgt. Du kan flytte markøren ved å klikke på kartet.';
        ['latitude','latitudeDisplay'].forEach(id => { const node = form?.querySelector('#'+id); if (node) node.textContent = initialLatitude.toFixed(6); });
        ['longitude','longitudeDisplay'].forEach(id => { const node = form?.querySelector('#'+id); if (node) node.textContent = initialLongitude.toFixed(6); });
    }
    if (currentLocationButton) {
        currentLocationButton.addEventListener('click', () => {
            if (!navigator.geolocation) {
                if (status) status.textContent = 'Nettleseren støtter ikke posisjon.';
                return;
            }

            currentLocationButton.disabled = true;
            if (status) status.textContent = 'Henter nåværende posisjon ...';
            navigator.geolocation.getCurrentPosition(
                position => {
                    setPosition(L.latLng(position.coords.latitude, position.coords.longitude));
                    currentLocationButton.disabled = false;
                },
                () => {
                    if (status) status.textContent = 'Kunne ikke hente posisjonen. Tillat posisjon i nettleseren og prøv igjen.';
                    currentLocationButton.disabled = false;
                }
            );
        });
    }
    if (element.hasAttribute('data-detail-map')) {
        const expandButton = element.parentElement.querySelector('[data-expand-detail-map]');
        if (expandButton) {
            expandButton.hidden = false;
            expandButton.addEventListener('click', () => {
                const expanded = element.classList.toggle('map-picker-expanded');
                expandButton.setAttribute('aria-expanded', String(expanded));
                expandButton.textContent = expanded ? 'Vis mindre kart' : 'Åpne i større kart';
                map.invalidateSize();
            });
        }
        return;
    }
    map.on('click', event => {
        setPosition(event.latlng);
    });
});
