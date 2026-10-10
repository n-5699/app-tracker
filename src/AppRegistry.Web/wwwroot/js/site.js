// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Dark / light mode toggle (button in the top nav bar). The choice is remembered in this browser.
(function () {
    const root = document.documentElement;
    const button = document.getElementById('theme-toggle');
    if (!button) return;
    const label = button.querySelector('.theme-label');

    function show(theme) {
        root.setAttribute('data-bs-theme', theme);
        label.textContent = theme === 'dark' ? 'Light' : 'Dark';
    }

    show(root.getAttribute('data-bs-theme') || 'dark');

    button.addEventListener('click', function () {
        const next = root.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark';
        show(next);
        try { localStorage.setItem('theme', next); } catch { }
    });
})();
