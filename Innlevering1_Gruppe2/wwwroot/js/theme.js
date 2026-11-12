(() => {
    const key = 'beredskapssentralen.theme';
    const root = document.documentElement;
    // Lys er standard. Bare et lagret valg av Mørk slår på mørk modus.
    try {
        root.dataset.bsTheme = localStorage.getItem(key) === 'dark' ? 'dark' : 'light';
    } catch {
        root.dataset.bsTheme = 'light';
    }

    document.addEventListener('DOMContentLoaded', () => {
        const picker = document.querySelector('[data-theme-choice]');
        if (!picker) return;

        picker.value = root.dataset.bsTheme;
        picker.closest('label').hidden = false;
        picker.addEventListener('change', () => {
            root.dataset.bsTheme = picker.value;
            try {
                localStorage.setItem(key, picker.value);
            } catch {
                // Valget virker på siden selv om nettleseren blokkerer lagring.
            }
        });
    });
})();
