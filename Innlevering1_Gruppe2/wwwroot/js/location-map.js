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
    if (button && selected) button.disabled = false;
    if (status && selected) status.textContent = 'Plasseringen er valgt. Du kan flytte markøren ved å klikke på kartet.';
    if (element.hasAttribute('data-detail-map')) return;
    map.on('click', event => {
        const point = event.latlng;
        if (marker) marker.setLatLng(point); else marker = L.marker(point).addTo(map);
        latitude.value = point.lat; longitude.value = point.lng;
        if (status) status.textContent = 'Plassering valgt.';
        if (button) button.disabled = false;
        ['latitude','latitudeDisplay'].forEach(id => { const node = form.querySelector('#'+id); if (node) node.textContent = point.lat.toFixed(6); });
        ['longitude','longitudeDisplay'].forEach(id => { const node = form.querySelector('#'+id); if (node) node.textContent = point.lng.toFixed(6); });
    });
});
