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

let listaArticulos = [];

async function listarArticulos() {
    try {

        const res = await fetch("/Articulo/Listar");
        const data = await res.json();

        listaArticulos = data;

        renderArticulos(data);

    }
    catch (error) {

        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: error.message || "Algo salió mal"
        });

    }
}

// Ver descripcion
function verDescripcion(desc) {
    Swal.fire({
        title: 'Descripción completa',
        html: `<div style="text-align:left">${desc}</div>`,
        width: 600,
        confirmButtonText: 'Cerrar'
    });
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

// ===================================
// BUSCADOR
// ===================================

//Creamos render antes de buscar para listar mientras se busca el articulo.
async function renderArticulos(data) {
    const tbody = document.getElementById("tablaArticulos");
    tbody.innerHTML = "";

    // Mensaje por si no hay resultado.
    if (!data || data.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="8" class="text-center text-muted">
                    <div style="padding:20px;">
                        <i class="fas fa-search" style="font-size:20px; opacity:0.6;"></i>
                        <p style="margin-top:10px;">
                            No se encontraron artículos con ese criterio
                        </p>
                    </div>
                </td>
            </tr>
        `;
        return;
    }

    data.forEach(a => {
        const tr = document.createElement("tr");

        tr.innerHTML = `
            <td>
                ${a.imagen
                ? `<img src="${a.imagen}" class="img-table">`
                : `<span>Sin Imagen</span>`}
            </td>

            <td>${a.nombre}</td>

            <td>
                ${a.descripcionCorta || ""}
                ${a.descripcionCompleta && a.descripcionCompleta.length > 60
                ? `<button class="btn btn-sm btn-info ms-2" onclick="verDescripcion('${a.descripcionCompleta.replace(/'/g, "\\'")}')">
                        <i class="fas fa-eye"></i>
                   </button>`
                : ""}
            </td>

            <td>C$ ${a.precio}</td>
            <td>${a.stock}</td>
            <td>${a.umedidum}</td>

            <td>
                <span class="badge ${a.activo ? 'bg-success' : 'bg-danger'}">
                    ${a.activo ? 'Activo' : 'Inactivo'}
                </span>
            </td>

            <td>

            <button
                class="btn btn-sm btn-primary"
                onclick="editarArticulo(${a.id})">

                <i class="fas fa-edit"></i>

            </button>

            <button
                class="btn btn-sm btn-danger"
                onclick="cambiarEstado(${a.id}, ${a.activo})">

                <i class="fas fa-trash"></i>

            </button>

        </td>
        `;

        tbody.appendChild(tr);
    });
}

// Creamos funcion de buscar
document.getElementById("buscadorArticulos").addEventListener("input", function () {
    const filtro = this.value.toLowerCase().trim();

    if (!filtro) {
        renderArticulos(listaArticulos);
        return;
    }

    const filtrados = listaArticulos.filter(a =>
        (a.nombre && a.nombre.toLowerCase().includes(filtro)) ||
        (a.descripcionCompleta && a.descripcionCompleta.toLowerCase().includes(filtro))
    );

    renderArticulos(filtrados);
});

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

        if (!res.ok) {
            return Swal.fire({
                icon: 'error',
                title: 'Error',
                text: result.message || 'Error al guardar.',
            });
        }

        await Swal.fire({
            icon: 'success',
            title: 'Éxito',
            text: 'Se ha guardado con exito el articulo',
            timer: 2000,
            showConfirmButton: false
        });

        //cerrar modal
        $("#modalArticulo").modal("hide");

        //recargar tablas
        listarArticulos();

    }
    catch(error) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'Error al tratar de guardar revise bien los campos o comente con soporte.'
        });
    }

});

// ==========================
// EDITAR
// ==========================
//function editarArticulo(id) {

//    const articulo = listaArticulos.find(a => a.id == id);

//    if (!articulo) return;

//    // Abrir modal
//    $("#modalArticulo").modal("show");

//    // Llenar campos
//    document.getElementById("nombre").value = articulo.nombre;
//    document.getElementById("descripcion").value = articulo.descripcionCompleta;
//    document.getElementById("precio").value = articulo.precio;
//    document.getElementById("stock").value = articulo.stock;
//    document.getElementById("activo").value = articulo.activo.toString();
//    document.getElementById("imagen").value = articulo.imagen;
//}

async function cambiarEstado(id, activoActual) {

    const nuevoEstado = !activoActual;

    const confirmacion = await Swal.fire({
        title: nuevoEstado ? 'Restaurar artículo' : 'Eliminar artículo',

        text: nuevoEstado
            ? '¿Deseas restaurar este artículo?'
            : '¿Deseas eliminar este artículo?',

        icon: 'warning',
        showCancelButton: true,

        confirmButtonText: nuevoEstado
            ? 'Sí, restaurar'
            : 'Sí, eliminar',

        cancelButtonText: 'Cancelar',

        confirmButtonColor: nuevoEstado
            ? '#28a745'
            : '#d33'
    });

    if (!confirmacion.isConfirmed) return;

    try {

        const res = await fetch(`/Articulo/CambiarEstado?id=${id}&activo=${nuevoEstado}`, {
            method: 'PUT'
        });

        const result = await res.json();

        if (!res.ok) {
            throw new Error(result.message);
        }

        await Swal.fire({
            icon: 'success',

            title: 'Éxito',

            text: nuevoEstado
                ? 'Artículo restaurado correctamente'
                : 'Artículo eliminado correctamente',

            timer: 1800,
            showConfirmButton: false
        });

        listarArticulos();

    }
    catch (error) {

        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: error.message
        });

    }
}