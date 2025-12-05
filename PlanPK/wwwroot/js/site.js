// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', () => {
  const toggles = document.querySelectorAll('.tree-toggle');

  toggles.forEach((toggle) => {
    toggle.addEventListener('click', () => {
      const node = toggle.closest('.issue-node');

      if (!node) {
        return;
      }

      const isExpanded = toggle.getAttribute('aria-expanded') === 'true';
      toggle.setAttribute('aria-expanded', String(!isExpanded));

      const icon = toggle.querySelector('.toggle-icon');
      if (icon) {
        icon.textContent = isExpanded ? '+' : '−';
      }

      node.classList.toggle('collapsed', isExpanded);
    });
  });
});
