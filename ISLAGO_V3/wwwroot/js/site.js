// DROPDOWNS SIDEBAR
document.querySelectorAll(".menu-title").forEach(menu => {
    menu.addEventListener("click", () => {

        const parent = menu.parentElement;

        // cerrar otros (opcional)
        document.querySelectorAll(".menu-group").forEach(m => {
            if (m !== parent) m.classList.remove("active");
        });

        parent.classList.toggle("active");
    });
});

// TOGGLE SIDEBAR MOBILE
const toggleBtn = document.getElementById("toggleSidebar");
const sidebar = document.getElementById("sidebar");

if (toggleBtn) {
    toggleBtn.addEventListener("click", () => {
        sidebar.classList.toggle("show");
    });
}