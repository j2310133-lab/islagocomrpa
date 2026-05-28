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

/* ========================================= */
/* =============== LOADER ================== */
/* ========================================= */

window.addEventListener("load", () => {

    const loader = document.getElementById("page-loader");

    const percentText = document.getElementById("loader-percent");

    const progressCircle = document.querySelector(".progress-ring-fill");

    const loaderText = document.getElementById("loader-text");

    if (!loader) return;

    const radius = 60;

    const circumference = 2 * Math.PI * radius;

    let progress = 0;

    const messages = [
        "Inicializando módulo...",
        "Cargando recursos...",
        "Procesando información...",
        "Preparando interfaz...",
        "Finalizando carga..."
    ];

    const interval = setInterval(() => {

        progress += Math.floor(Math.random() * 8) + 3;

        if (progress > 100)
            progress = 100;

        percentText.textContent = `${progress}%`;

        const offset =
            circumference - (progress / 100) * circumference;

        progressCircle.style.strokeDashoffset = offset;

        // Cambiar mensajes dinámicamente
        if (progress < 25)
            loaderText.textContent = messages[0];

        else if (progress < 45)
            loaderText.textContent = messages[1];

        else if (progress < 70)
            loaderText.textContent = messages[2];

        else if (progress < 90)
            loaderText.textContent = messages[3];

        else
            loaderText.textContent = messages[4];

        // FINALIZAR
        if (progress >= 100) {

            clearInterval(interval);

            setTimeout(() => {

                loader.classList.add("hide");

            }, 400);
        }

    }, 80);

});