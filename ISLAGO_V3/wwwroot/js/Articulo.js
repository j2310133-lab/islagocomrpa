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
    cargarCategorias();
    cargarUmedidas();
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
                    ${a.Imagen
                        ? `<img src="${a.Imagen}" class="img-table">`
                        : `<span>Sin Imagen</span>`
                    }
                </td>

                <td>${a.nombre}</td>

                <td>
                    ${a.descripcionCorta || ""}
                    ${
                        a.descripcionCompleta && a.descripcionCompleta.length > 60
                        ? `<button class="btn-ver-mas" onclick="verDescripcion('${a.descripcionCompleta.replace(/'/g, "\\'")}')">Ver más</button>`
                        : ""
                    }
                </td>

                <td>$${a.precio}</td>
                <td>${a.stock}</td>
                <td>${a.umedidum}</td>

                <td>
                    <span class="${a.activo ? 'badge badge-success' : 'badge badge-danger'}">
                        ${a.activo ? 'Activo' : 'Inactivo'}
                    </span>
                </td>

                <td>
                    <button class="btn btn-sm btn-primary">Editar</button>
                    <button class="btn btn-sm btn-danger">Eliminar</button>
                </td>
            `;

            tbody.appendChild(tr);
        });

    }
    catch (error) {
        console.error("Error al listar :", error);
    }
}

// Cargar categorias en MODAL
async function cargarCategorias() {
    const res = await fetch("/Articulo/ObtenerCategorias");
    const data = await res.json();

    const select = document.getElementById("categoriaSelect");
    
    data.forEach(c => {
        const option = document.createElement("option");
        option.value = c.id;
        option.textContent = c.nombre;
        select.appendChild(option); 
    });
}

// Cargar Unidad Medida en MODAL
async function cargarUmedidas() {
    const res = await fetch("/Articulo/ObtenerUnidadesMedida");
    const data = await res.json();

    const select = document.getElementById("unidadMedida");

    data.forEach(u => {
        const option = document.createElement("option");
        option.value = u.id;
        option.textContent = u.nombre;
        select.appendChild(option);
    });
}

// =================================================
// Capturamos Imagenes y convertirla a string
// =================================================
let imagenesBase64 = [];

document.getElementById("inputImagenes").addEventListener("change", function () {
    const files = this.files;
    imagenesBase64 = [];

    const preview = document.getElementById("preview");
    preview.innerHTML = "";

    Array.from(files).forEach(file => {
        const reader = new FileReader();
        
        reader.onload = function(e) {

            imagenesBase64.push(e.target.result);

            const img = document.createElement("img");
            img.src = e.target.result;
            img.className = "preview-img"

            preview.appendChild(img);

        };

        reader.readAsDataURL(file);
    });
});

// ===============================
// Crear Articulo
// ===============================
document.getElementById("btnGuardar").addEventListener("click", async function () {

    try{

        const data = {
            nombre: document.getElementById("nombre").value,
            descripcion: document.getElementById("descripcion").value,
            precio: parseFloat(document.getElementById("precio").value),
            stock: parseInt(document.getElementById("stock").value),
            activo: document.getElementById("activo").value === "true",
            idumedida: parseInt(document.getElementById("unidadMedida").value),

            categoriasId: categorias,
            imagenesBase64: imagenesBase64
        };

        const res = await fetch("/Articulo/Crear", {
            method: "post",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(data)
        });

        const result = await res.json();

        alert(result.message);

        //cerrar modal
        $("#modalArticulo").modal("hide");

        //recargar tablas
        listarArticulos();

    }
    catch(error) {
        console.error("Error al crear:", error);
    }

});