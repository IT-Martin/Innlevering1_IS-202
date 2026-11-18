// Samme søk og filter styrer listen og kartmarkørene på kartsiden.
document.querySelectorAll('[data-collection]').forEach(collection => {
    const items = Array.from(collection.querySelectorAll('[data-item]'));
    const search = collection.querySelector('[data-search]');
    const category = collection.querySelector('[data-category]');
    const sort = collection.querySelector('[data-sort]');
    const kind = collection.querySelector('select[data-kind]');
    const reset = collection.querySelector('[data-reset]');
    const categories = [...new Set(items.map(item => item.dataset.category))].filter(Boolean).sort((a,b) => a.localeCompare(b,'nb'));
    for (const name of categories) { const option = document.createElement('option'); option.value = name; option.textContent = name; category?.append(option); }
    function update() {
        let visible = 0;
        for (const item of items) {
            const text = (item.dataset.name + ' ' + item.dataset.category + ' ' + item.dataset.place).toLocaleLowerCase('nb');
            item.hidden = !!search?.value && !text.includes(search.value.toLocaleLowerCase('nb')) || !!category?.value && item.dataset.category !== category.value || !!kind?.value && item.dataset.kind !== kind.value;
            if (!item.hidden) visible++;
        }
        const sorted = [...items].sort((a,b) => {
            const key = sort?.value === 'category' ? 'category' : 'name';
            const result = (a.dataset[key] || '').localeCompare(b.dataset[key] || '', 'nb');
            return sort?.value === 'name-desc' ? -result : result;
        });
        const parent = collection.querySelector('[data-items]');
        sorted.forEach(item => parent?.append(item));
        const count = collection.querySelector('[data-count]'); if (count) count.textContent = visible + ' av ' + items.length + ' treff';
        const empty = collection.querySelector('[data-empty]');
        if (empty) {
            empty.hidden = visible !== 0;
            const filtered = !!search?.value || !!category?.value || !!kind?.value;
            empty.textContent = items.length === 0 && !filtered
                ? (empty.dataset.emptyInitial || 'Ingen registrert ennå.')
                : 'Ingen treff med disse filtrene. Endre søket eller nullstill filtrene.';
        }
        if (reset) reset.hidden = !search?.value && !category?.value && !kind?.value && (!sort?.value || sort.value === 'name');
        collection.dispatchEvent(new Event('collectionchange'));
    }
    reset?.addEventListener('click', () => {
        if (search) search.value = '';
        if (category) category.value = '';
        if (kind) kind.value = '';
        if (sort) sort.value = 'name';
        update();
        search?.focus();
    });
    [search, category, sort, kind].forEach(input => input?.addEventListener('input', update));
    update();
});

// Behold menytilhørigheten også ved registrering og redigering.
let activePath = location.pathname.toLowerCase();
const ownResources = document.querySelector('.main-nav a[href="/Resources/MyContributions"]');
if (activePath.startsWith('/resources') || activePath.startsWith('/availability')) {
    activePath = ownResources ? '/resources/mycontributions' : '/map';
} else if (activePath.startsWith('/needs')) {
    activePath = '/needs';
} else if (activePath.startsWith('/volunteers')) {
    activePath = ownResources ? '/volunteers/edit' : '/volunteers';
}
document.querySelectorAll('.main-nav a[href]').forEach(link => {
    if (link.getAttribute('href') === '#') return;
    const path = new URL(link.href).pathname.toLowerCase();
    if (path === activePath) {
        link.classList.add('active');
        link.setAttribute('aria-current', 'page');
        link.closest('.dropdown')?.querySelector('.dropdown-toggle')?.classList.add('active');
    }
});
