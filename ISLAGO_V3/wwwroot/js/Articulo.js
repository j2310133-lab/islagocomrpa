let categorias = [];
let editImagenesBase64 = [];

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

        html: `
            <div class="descripcion-modal">

                <div class="descripcion-icon">

                    <i class="fas fa-align-left"></i>

                </div>

                <h3>
                    Descripción del artículo
                </h3>

                <div class="descripcion-content">
                    ${desc}
                </div>

            </div>
        `,

        showConfirmButton: true,

        confirmButtonText: 'Cerrar',

        customClass: {
            popup: 'descripcion-popup',
            confirmButton: 'descripcion-btn'
        }

    });

}

// Cargar categorias en MODAL
async function cargarCategorias() {

    const res = await fetch("/Articulo/ObtenerCategorias");
    const data = await res.json();

    // =========================
    // MODAL CREAR
    // =========================
    const select = document.getElementById("categoriaSelect");

    select.innerHTML = `
        <option value="">-- Seleccionar categoría --</option>
    `;

    // =========================
    // MODAL EDITAR
    // =========================
    const editSelect = document.getElementById("editCategoriaSelect");

    editSelect.innerHTML = `
        <option value="">-- Seleccionar categoría --</option>
    `;

    data.forEach(c => {

        // CREATE
        const option = document.createElement("option");
        option.value = c.id;
        option.textContent = c.nombre;
        select.appendChild(option);

        // EDIT
        const optionEdit = document.createElement("option");
        optionEdit.value = c.id;
        optionEdit.textContent = c.nombre;
        editSelect.appendChild(optionEdit);

    });

}

// Cargar Unidad Medida en MODAL
async function cargarUmedidas() {

    const res = await fetch("/Articulo/ObtenerUnidadesMedida");
    const data = await res.json();

    // CREAR
    const select = document.getElementById("unidadMedida");

    select.innerHTML = `
        <option value="">-- Seleccionar unidad --</option>
    `;

    // EDITAR
    const editSelect = document.getElementById("editUnidadMedida");

    editSelect.innerHTML = `
        <option value="">-- Seleccionar unidad --</option>
    `;

    data.forEach(u => {

        // CREAR
        const option = document.createElement("option");
        option.value = u.id;
        option.textContent = u.nombre;
        select.appendChild(option);

        // EDITAR
        const optionEdit = document.createElement("option");
        optionEdit.value = u.id;
        optionEdit.textContent = u.nombre;
        editSelect.appendChild(optionEdit);

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

                <div class="actions-dropdown">

                    <button
                        class="actions-btn"
                        onclick="toggleActionsMenu(this)">

                        <i class="fas fa-ellipsis-v"></i>

                    </button>

                    <div class="actions-menu">

                        ${a.descripcionCompleta && a.descripcionCompleta.length > 60
                            ? `
                            <button
                                class="actions-item"
                                onclick="verDescripcion('${a.descripcionCompleta.replace(/'/g, "\\'")}')">

                                <i class="fas fa-eye"></i>
                                Ver descripción

                            </button>
                            `
                            : ""}

                        <button
                            class="actions-item"
                            onclick="editarArticulo(${a.id})">

                            <i class="fas fa-edit"></i>
                            Editar

                        </button>

                        <button
                            class="actions-item"
                            onclick="ajustarStock(${a.id})">

                            <i class="fas fa-boxes"></i>
                            Ajustar stock

                        </button>

                        <button
                            class="actions-item"
                            onclick="administrarImagenes(${a.id})">

                            <i class="fas fa-images"></i>
                            Administrar imágenes

                        </button>

                        <button
                            class="actions-item danger"
                            onclick="cambiarEstado(${a.id}, ${a.activo})">

                            <i class="fas fa-trash"></i>

                            ${a.activo ? 'Eliminar' : 'Restaurar'}

                        </button>

                    </div>

                </div>

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
function renderEditCategorias() {

    const container = document.getElementById("editCategoriasSeleccionadas");

    container.innerHTML = "";

    editCategorias.forEach(cat => {

        const option = document.querySelector(
            `#editCategoriaSelect option[value="${cat}"]`
        );

        const nombre = option?.textContent || "Categoria";

        container.innerHTML += `
            <div class="categoria-tag">

                ${nombre}

                <i
                    class="fas fa-times"
                    onclick="eliminarCategoriaEdit('${cat}')">
                </i>

            </div>
        `;

    });

}

document.getElementById("editCategoriaSelect")
    .addEventListener("change", function () {
        const id = this.value;
        const text = this.options[this.selectedIndex].text;

        if (!id || editCategorias.includes(id)) return;

        editCategorias.push(id);

        renderEditCategorias();

        this.value = "";
    });

document.getElementById("editInputImagenes")
    .addEventListener("change", function () {

        const files = this.files;

        editImagenesBase64 = [];

        const preview = document.getElementById("editPreviewNuevas");

        preview.innerHTML = "";

        Array.from(files).forEach(file => {

            const reader = new FileReader();

            reader.onload = function (e) {

                editImagenesBase64.push(e.target.result);

                preview.innerHTML += `
                <img
                    src="${e.target.result}"
                    class="preview-img">
            `;

            };

            reader.readAsDataURL(file);

        });

    });

async function editarArticulo(id) {

    try {

        const res = await fetch(`/Articulo/ObtenerPorId?id=${id}`);

        const articulo = await res.json();

        if (!res.ok) {
            throw new Error(articulo.message);
        }

        // ==========================
        // ABRIR MODAL
        // ==========================

        $("#modalEditarArticulo").modal("show");

        // ==========================
        // INPUTS
        // ==========================

        document.getElementById("editIdArticulo").value = articulo.id;

        document.getElementById("editNombre").value = articulo.nombre;

        document.getElementById("editDescripcion").value =
            articulo.descripcion || "";

        document.getElementById("editPrecio").value = articulo.precio;

        document.getElementById("editStock").value = articulo.stock;

        document.getElementById("editUnidadMedida").value =
            articulo.idumedida;

        document.getElementById("editActivo").value =
            articulo.activo.toString();

        // ==========================
        // CATEGORIAS
        // ==========================

        editCategorias = [];

        if (articulo.categorias) {

            articulo.categorias.forEach(cat => {

                editCategorias.push(cat.idcategoria.toString());

            });

        }

        renderEditCategorias();

        // ==========================
        // IMAGENES ACTUALES
        // ==========================

        const preview = document.getElementById("editPreview");

        preview.innerHTML = "";

        if (articulo.imagen) {

            articulo.imagen.forEach(img => {

                preview.innerHTML += `
                    <div class="preview-edit-card">

                        <img
                            src="${img.ruta}">

                    </div>
                `;

            });

        }

    }
    catch (error) {

        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: error.message
        });

    }

}

function eliminarCategoriaEdit(id) {

    editCategorias = editCategorias.filter(c => c != id);

    renderEditCategorias();

}

//Guardar cambios
document.getElementById("btnActualizarArticulo")
    .addEventListener("click", async function () {

        try {

            const data = {

                id: parseInt(
                    document.getElementById("editIdArticulo").value
                ),

                nombre: document.getElementById("editNombre").value,

                descripcion:
                    document.getElementById("editDescripcion").value,

                precio: parseFloat(
                    document.getElementById("editPrecio").value
                ),

                stock: parseInt(
                    document.getElementById("editStock").value
                ),

                idumedida: parseInt(
                    document.getElementById("editUnidadMedida").value
                ),

                activo:
                    document.getElementById("editActivo").value === "true",

                categoriasId: editCategorias,

                imagenesBase64: editImagenesBase64

            };

            const res = await fetch("/Articulo/Actualizar", {

                method: "PUT",

                headers: {
                    "Content-Type": "application/json"
                },

                body: JSON.stringify(data)

            });

            const result = await res.json();

            if (!res.ok) {
                throw new Error(result.message);
            }

            await Swal.fire({
                icon: 'success',
                title: 'Éxito',
                text: 'Artículo actualizado correctamente',
                timer: 1800,
                showConfirmButton: false
            });

            $("#modalEditarArticulo").modal("hide");

            listarArticulos();

        }
        catch (error) {

            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: error.message
            });

        }

    });

// =============================
// Funcion CambiarEstado
// =============================
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

// =====================================
// MENU ACCIONES
// =====================================

function toggleActionsMenu(button) {

    // cerrar otros
    document.querySelectorAll(".actions-menu").forEach(menu => {

        if (menu !== button.nextElementSibling) {
            menu.classList.remove("show");
        }

    });

    // abrir actual
    button.nextElementSibling.classList.toggle("show");
}

// cerrar al hacer click afuera
document.addEventListener("click", function (e) {

    if (!e.target.closest(".actions-dropdown")) {

        document.querySelectorAll(".actions-menu").forEach(menu => {
            menu.classList.remove("show");
        });

    }

});