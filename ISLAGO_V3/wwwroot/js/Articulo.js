let categorias = [];

document.getElementById("categoriaSelect").addEventListener("change", function () {
    const id = this.value;
    const text = this.options[this.selectedIndex].text;

    if (!id || categorias.includes(id)) return;

    categorias.push(id);

    const tag = document.createElement("div");
    tag.className = "categoria-tag";
    tag.setAttribute("data-id", id);

    tag.innerHTML = `
        ${text}
        <i class="fas fa-times"></i>
    `;

    tag.querySelector("i").addEventListener("click", function () {
        categorias = categorias.filter(c => c !== id);
        tag.remove();
    });

    document.getElementById("categoriasSeleccionadas").appendChild(tag);

    this.value = "";
});

// ===================================
// Funcionalidad del proyecto
// ===================================
document.addEventListener("DOMContentLoaded", () => {
    listarArticulos();
});

// ===================================
// LISTAR TABLA ARTICULO
// Y MOSTRAR LA ULTIMA IMAGEN
// PUBLICADA
// ===================================

async function listarArticulos() {
    try {

        const res = await fetch("/Articulo/Listar");
        const data = await res.json();

        const tbody = document.getElementById("tablaArticulos");
        tbody.innerHTML = "";

        data.forEach(a => {
            const tr = document.createElement("tr");

            tr.innerHTML = `
                <td>
                    ${a.imagen
                    ? `<img src="${a.imagen}" class="img-table">`
                    : `<span>Sin Imagen</span>`
                    }
                </td>

                <td> ${a.nombre} </td>

                <td>
                    ${a.descripcionCorta || ""}
                    $
            `
        })

    }
    catch (error) {
        console.error("Error al listar :", error);
    }
}