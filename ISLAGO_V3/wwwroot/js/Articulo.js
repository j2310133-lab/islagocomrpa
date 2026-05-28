// =========================================
// VARIABLES GLOBALES
// =========================================

let categorias = [];
let editCategorias = [];

let imagenesBase64 = [];
let editImagenesBase64 = [];

let listaArticulos = [];

// =========================================
// INIT
// =========================================

document.addEventListener("DOMContentLoaded", async () => {

    await cargarCategorias();

    await cargarUmedidas();

    await cargarProveedores();

    await listarArticulos();

});

// =========================================
// SELECT CATEGORIAS CREAR
// =========================================

document.getElementById("categoriaSelect")
    .addEventListener("change", function () {

        const id = this.value;

        const text =
            this.options[this.selectedIndex].text;

        if (!id || categorias.includes(id)) return;

        categorias.push(id);

        renderCategoriasSeleccionadas();

        this.value = "";

    });

// =========================================
// SELECT CATEGORIAS EDITAR
// =========================================

document.getElementById("editCategoriaSelect")
    .addEventListener("change", function () {

        const id = this.value;

        const text =
            this.options[this.selectedIndex].text;

        if (!id || editCategorias.includes(id)) return;

        editCategorias.push(id);

        renderEditCategorias();

        this.value = "";

    });

// =========================================
// LISTAR ARTICULOS
// =========================================

async function listarArticulos() {

    try {

        const res = await fetch("/Articulo/Listar");

        if (!res.ok) {
            throw new Error(
                "No se pudieron cargar los artículos."
            );
        }

        const data = await res.json();

        listaArticulos = data;

        renderArticulos(data);

    }
    catch (error) {

        console.error(error);

        Swal.fire({
            icon: "error",
            title: "Error",
            text: error.message
        });

    }

}

// =========================================
// RENDER TABLA
// =========================================

function renderArticulos(data) {

    const tbody =
        document.getElementById("tablaArticulos");

    tbody.innerHTML = "";

    // =========================
    // VACIO
    // =========================

    if (!data || data.length === 0) {

        tbody.innerHTML = `
            <tr>
                <td colspan="10" class="text-center text-muted">

                    <div style="padding:20px;">

                        <i
                            class="fas fa-search"
                            style="font-size:20px; opacity:0.6;">
                        </i>

                        <p style="margin-top:10px;">
                            No se encontraron artículos
                        </p>

                    </div>

                </td>
            </tr>
        `;

        return;

    }

    // =========================
    // RENDER
    // =========================

    data.forEach(a => {

        const tr = document.createElement("tr");

        tr.innerHTML = `

            <!-- IMAGEN -->
            <td>

                ${a.imagen
                ? `
                        <img
                            src="${a.imagen}"
                            class="img-table">
                    `
                : `
                        <span>Sin imagen</span>
                    `
            }

            </td>

            <!-- PRODUCTO -->
            <td>

                <div class="d-flex flex-column">

                    <strong>
                        ${a.nombreCompleto || a.nombre || "Sin nombre"}
                    </strong>

                </div>

            </td>

            <!-- DESCRIPCION -->
            <td>

                ${a.descripcionCorta || ""}

            </td>

            <!-- STOCK -->
            <td>

                ${a.stock ?? 0}

            </td>

            <!-- UNIDAD -->
            <td>

                ${a.unidadMedida || "Sin unidad"}

            </td>

            <!-- COMPRA -->
            <td>

                C$ ${a.precioCompra ?? 0}

            </td>

            <!-- VENTA -->
            <td>

                C$ ${a.precioVentaMinorista ?? 0}

            </td>

            <!-- PROVEEDOR -->
            <td>

                ${a.proveedor || "Ferreteria Lago"}

            </td>

            <!-- ESTADO -->
            <td>

                <span class="badge ${a.activo
                ? "bg-success"
                : "bg-danger"}">

                    ${a.activo
                ? "Activo"
                : "Inactivo"}

                </span>

            </td>

            <!-- ACCIONES -->
            <td>

                <div class="actions-dropdown">

                    <button
                        class="actions-btn"
                        onclick="toggleActionsMenu(this)">

                        <i class="fas fa-ellipsis-v"></i>

                    </button>

                    <div class="actions-menu">

                        ${a.descripcionCompleta &&
                a.descripcionCompleta.length > 60
                ? `
                            <button
                                class="actions-item"
                                onclick="verDescripcion('${a.descripcionCompleta.replace(/'/g, "\\'")}')">

                                <i class="fas fa-eye"></i>

                                Ver descripción

                            </button>
                        `
                : ""
            }

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

                            ${a.activo
                ? "Eliminar"
                : "Restaurar"}

                        </button>

                    </div>

                </div>

            </td>
        `;

        tbody.appendChild(tr);

    });

}

// =========================================
// VER DESCRIPCION
// =========================================

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

        confirmButtonText: "Cerrar"

    });

}

// =========================================
// CARGAR CATEGORIAS
// =========================================

async function cargarCategorias() {

    try {

        const res =
            await fetch("/Articulo/ObtenerCategorias");

        if (!res.ok) {
            throw new Error(
                "No se pudieron cargar las categorías."
            );
        }

        const data = await res.json();

        const select =
            document.getElementById("categoriaSelect");

        const editSelect =
            document.getElementById("editCategoriaSelect");

        select.innerHTML = `
            <option value="">
                -- Seleccionar categoría --
            </option>
        `;

        editSelect.innerHTML = `
            <option value="">
                -- Seleccionar categoría --
            </option>
        `;

        data.forEach(c => {

            const option =
                document.createElement("option");

            option.value = c.id;

            option.textContent = c.nombre;

            select.appendChild(option);

            const optionEdit =
                document.createElement("option");

            optionEdit.value = c.id;

            optionEdit.textContent = c.nombre;

            editSelect.appendChild(optionEdit);

        });

    }
    catch (error) {

        console.error(error);

    }

}

// =========================================
// CARGAR UNIDADES
// =========================================

async function cargarUmedidas() {

    try {

        const res =
            await fetch("/Articulo/ObtenerUnidadesMedida");

        if (!res.ok) {
            throw new Error(
                "No se pudieron cargar las unidades."
            );
        }

        const data = await res.json();

        const select =
            document.getElementById("unidadMedida");

        const editSelect =
            document.getElementById("editUnidadMedida");

        select.innerHTML = `
            <option value="">
                -- Seleccionar unidad --
            </option>
        `;

        editSelect.innerHTML = `
            <option value="">
                -- Seleccionar unidad --
            </option>
        `;

        data.forEach(u => {

            const option =
                document.createElement("option");

            option.value = u.id;

            option.textContent = u.nombre;

            select.appendChild(option);

            const optionEdit =
                document.createElement("option");

            optionEdit.value = u.id;

            optionEdit.textContent = u.nombre;

            editSelect.appendChild(optionEdit);

        });

    }
    catch (error) {

        console.error(error);

    }

}

// =========================================
// CARGAR PROVEEDORES
// =========================================

async function cargarProveedores() {

    try {

        const res =
            await fetch("/Articulo/ObtenerProveedores");

        if (!res.ok) {

            console.warn(
                "No existen proveedores todavía."
            );

            return;

        }

        const data = await res.json();

        const select =
            document.getElementById("proveedor");

        const editSelect =
            document.getElementById("editProveedor");

        select.innerHTML = `
            <option value="">
                -- Seleccionar proveedor --
            </option>
        `;

        editSelect.innerHTML = `
            <option value="">
                -- Seleccionar proveedor --
            </option>
        `;

        data.forEach(p => {

            const option =
                document.createElement("option");

            option.value = p.id;

            option.textContent = p.nombre;

            select.appendChild(option);

            const optionEdit =
                document.createElement("option");

            optionEdit.value = p.id;

            optionEdit.textContent = p.nombre;

            editSelect.appendChild(optionEdit);

        });

    }
    catch (error) {

        console.error(error);

    }

}

// =========================================
// RENDER CATEGORIAS CREAR
// =========================================

function renderCategoriasSeleccionadas() {

    const container =
        document.getElementById(
            "categoriasSeleccionadas"
        );

    container.innerHTML = "";

    categorias.forEach(cat => {

        const option = document.querySelector(
            `#categoriaSelect option[value="${cat}"]`
        );

        const nombre =
            option?.textContent || "Categoria";

        container.innerHTML += `
            <div class="categoria-tag">

                ${nombre}

                <i
                    class="fas fa-times"
                    onclick="eliminarCategoria('${cat}')">
                </i>

            </div>
        `;

    });

}

function eliminarCategoria(id) {

    categorias =
        categorias.filter(c => c != id);

    renderCategoriasSeleccionadas();

}

// =========================================
// RENDER CATEGORIAS EDIT
// =========================================

function renderEditCategorias() {

    const container =
        document.getElementById(
            "editCategoriasSeleccionadas"
        );

    container.innerHTML = "";

    editCategorias.forEach(cat => {

        const option = document.querySelector(
            `#editCategoriaSelect option[value="${cat}"]`
        );

        const nombre =
            option?.textContent || "Categoria";

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

function eliminarCategoriaEdit(id) {

    editCategorias =
        editCategorias.filter(c => c != id);

    renderEditCategorias();

}

// =========================================
// BUSCADOR
// =========================================

document.getElementById("buscadorArticulos")
    .addEventListener("input", function () {

        const filtro =
            this.value.toLowerCase().trim();

        if (!filtro) {

            renderArticulos(listaArticulos);

            return;

        }

        const filtrados =
            listaArticulos.filter(a =>

                (a.nombreCompleto &&
                    a.nombreCompleto
                        .toLowerCase()
                        .includes(filtro))

                ||

                (a.descripcionCompleta &&
                    a.descripcionCompleta
                        .toLowerCase()
                        .includes(filtro))

            );

        renderArticulos(filtrados);

    });

// =========================================
// IMAGENES CREAR
// =========================================

document.getElementById("inputImagenes")
    .addEventListener("change", function () {

        const files = this.files;

        imagenesBase64 = [];

        const preview =
            document.getElementById("preview");

        preview.innerHTML = "";

        Array.from(files).forEach(file => {

            const reader = new FileReader();

            reader.onload = function (e) {

                imagenesBase64.push(
                    e.target.result
                );

                preview.innerHTML += `
                    <img
                        src="${e.target.result}"
                        class="preview-img">
                `;

            };

            reader.readAsDataURL(file);

        });

    });

// =========================================
// IMAGENES EDITAR
// =========================================

document.getElementById("editInputImagenes")
    .addEventListener("change", function () {

        const files = this.files;

        editImagenesBase64 = [];

        const preview =
            document.getElementById(
                "editPreviewNuevas"
            );

        preview.innerHTML = "";

        Array.from(files).forEach(file => {

            const reader = new FileReader();

            reader.onload = function (e) {

                editImagenesBase64.push(
                    e.target.result
                );

                preview.innerHTML += `
                    <img
                        src="${e.target.result}"
                        class="preview-img">
                `;

            };

            reader.readAsDataURL(file);

        });

    });

// =========================================
// NORMALIZAR FRACCIONES
// =========================================

function normalizarFracciones(texto) {

    if (!texto) return texto;

    return texto

        // 1 y media
        .replace(/(\d+)\s+y\s+media/gi, "$1½")

        // 1 y cuarto
        .replace(/(\d+)\s+y\s+cuarto/gi, "$1¼")

        // 1 y tres cuartos
        .replace(/(\d+)\s+y\s+tres\s+cuartos/gi, "$1¾")

        // 1 1/2
        .replace(/(\d+)\s+1\/2/g, "$1½")

        // 1 1/4
        .replace(/(\d+)\s+1\/4/g, "$1¼")

        // 1 3/4
        .replace(/(\d+)\s+3\/4/g, "$1¾")

        // SOLO media
        .replace(/\bmedia\b/gi, "½")

        // SOLO cuarto
        .replace(/\bcuarto\b/gi, "¼")

        // SOLO 3/4
        .replace(/\b3\/4\b/g, "¾")

        // SOLO 1/2
        .replace(/\b1\/2\b/g, "½")

        // SOLO 1/4
        .replace(/\b1\/4\b/g, "¼");

}

// =========================================
// CREAR
// =========================================

document.getElementById("btnGuardar")
    .addEventListener("click", async function () {

        try {

            const data = {

                nombre:
                    normalizarFracciones(
                        document.getElementById("nombre").value
                    ),

                descripcion:
                    normalizarFracciones(
                        document.getElementById("descripcion").value
                    ),

                marca:
                    normalizarFracciones(
                        document.getElementById("marca").value
                    ),

                sku:
                    document.getElementById("sku").value,

                precioCompra:
                    parseFloat(
                        document.getElementById("precioCompra").value
                    ) || 0,

                precioVentaMinorista:
                    parseFloat(
                        document.getElementById("precioMinorista").value
                    ) || 0,

                precioVentaMayorista:
                    parseFloat(
                        document.getElementById("precioMayorista").value
                    ) || 0,

                stockMinimo:
                    parseInt(
                        document.getElementById("stockMinimo").value
                    ) || 0,

                idproveedor:
                    parseInt(
                        document.getElementById("proveedor").value
                    ) || null,

                stock:
                    parseInt(
                        document.getElementById("stock").value
                    ) || 0,

                activo:
                    document.getElementById("activo").value === "true",

                idumedida:
                    parseInt(
                        document.getElementById("unidadMedida").value
                    ) || null,

                categoriasId: categorias,

                imagenesBase64: imagenesBase64

            };

            // =========================
            // VALIDAR DUPLICADOS
            // =========================

            const existe = listaArticulos.some(a =>

                a.nombreCompleto?.trim().toLowerCase()
                === `${data.nombre} ${data.marca}`.trim().toLowerCase()

                ||

                (
                    a.sku &&
                    data.sku &&
                    a.sku.trim().toLowerCase()
                    === data.sku.trim().toLowerCase()
                )

            );

            if (existe) {

                throw new Error(
                    "Ya existe un artículo con ese nombre/marca o SKU."
                );

            }

            const res = await fetch(
                "/Articulo/Crear",
                {
                    method: "POST",

                    headers: {
                        "Content-Type":
                            "application/json"
                    },

                    body: JSON.stringify(data)
                }
            );

            const result = await res.json();

            if (!res.ok) {

                throw new Error(
                    result.message ||
                    "Error al guardar."
                );

            }

            await Swal.fire({
                icon: "success",
                title: "Éxito",
                text:
                    "Artículo creado correctamente",
                timer: 2000,
                showConfirmButton: false
            });

            $("#modalArticulo").modal("hide");

            listarArticulos();

        }
        catch (error) {

            console.error(error);

            Swal.fire({
                icon: "error",
                title: "Error",
                text: error.message
            });

        }

    });

// =========================================
// LIMPIAR MODAL CREAR
// =========================================

function limpiarModalCrear() {

    // =========================
    // INPUTS
    // =========================

    document.getElementById("nombre").value = "";

    document.getElementById("descripcion").value = "";

    document.getElementById("marca").value = "";

    document.getElementById("sku").value = "";

    document.getElementById("stock").value = "";

    document.getElementById("stockMinimo").value = "";

    document.getElementById("precioCompra").value = "";

    document.getElementById("precioMinorista").value = "";

    document.getElementById("precioMayorista").value = "";

    document.getElementById("activo").value = "true";

    document.getElementById("unidadMedida").value = "";

    document.getElementById("proveedor").value = "";

    // =========================
    // CATEGORIAS
    // =========================

    categorias = [];

    renderCategoriasSeleccionadas();

    // =========================
    // IMAGENES
    // =========================

    imagenesBase64 = [];

    document.getElementById("preview").innerHTML = "";

    document.getElementById("inputImagenes").value = "";

}

// =========================================
// LIMPIAR MODAL AL CERRAR
// =========================================

$('#modalArticulo').on('hidden.bs.modal', function () {

    limpiarModalCrear();

});

// =========================================
// EDITAR
// =========================================

async function editarArticulo(id) {

    try {

        const res =
            await fetch(
                `/Articulo/ObtenerPorId?id=${id}`
            );

        const articulo = await res.json();

        if (!res.ok) {
            throw new Error(
                articulo.message
            );
        }

        $("#modalEditarArticulo")
            .modal("show");

        // =====================
        // INPUTS
        // =====================

        document.getElementById("editIdArticulo").value =
            articulo.id;

        document.getElementById("editNombre").value =
            articulo.nombre || "";

        document.getElementById("editDescripcion").value =
            articulo.descripcion || "";

        document.getElementById("editMarca").value =
            articulo.marca || "";

        document.getElementById("editSku").value =
            articulo.sku || "";

        document.getElementById("editPrecioCompra").value =
            articulo.precioCompra || 0;

        document.getElementById("editPrecioMinorista").value =
            articulo.precioVentaMinorista || 0;

        document.getElementById("editPrecioMayorista").value =
            articulo.precioVentaMayorista || 0;

        document.getElementById("editStockMinimo").value =
            articulo.stockMinimo || 0;

        document.getElementById("editProveedor").value =
            articulo.idproveedor || "";

        document.getElementById("editStock").value =
            articulo.stock || 0;

        document.getElementById("editUnidadMedida").value =
            articulo.idumedida || "";

        document.getElementById("editActivo").value =
            articulo.activo.toString();

        // =====================
        // CATEGORIAS
        // =====================

        editCategorias = [];

        if (articulo.categorias) {

            articulo.categorias.forEach(cat => {

                editCategorias.push(
                    cat.idcategoria.toString()
                );

            });

        }

        renderEditCategorias();

        // =====================
        // IMAGENES
        // =====================

        const preview =
            document.getElementById("editPreview");

        preview.innerHTML = "";

        if (articulo.imagenes) {

            articulo.imagenes.forEach(img => {

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

        console.error(error);

        Swal.fire({
            icon: "error",
            title: "Error",
            text: error.message
        });

    }

}

// =========================================
// ACTUALIZAR
// =========================================

document.getElementById("btnActualizarArticulo")
    .addEventListener("click", async function () {

        try {

            const data = {

                id:
                    parseInt(
                        document.getElementById("editIdArticulo").value
                    ),

                // =====================================
                // NORMALIZAR FRACCIONES
                // =====================================

                nombre:
                    normalizarFracciones(
                        document.getElementById("editNombre").value
                    ),

                descripcion:
                    normalizarFracciones(
                        document.getElementById("editDescripcion").value
                    ),

                marca:
                    normalizarFracciones(
                        document.getElementById("editMarca").value
                    ),

                sku:
                    document.getElementById("editSku").value,

                precioCompra:
                    parseFloat(
                        document.getElementById("editPrecioCompra").value
                    ) || 0,

                precioVentaMinorista:
                    parseFloat(
                        document.getElementById("editPrecioMinorista").value
                    ) || 0,

                precioVentaMayorista:
                    parseFloat(
                        document.getElementById("editPrecioMayorista").value
                    ) || 0,

                stockMinimo:
                    parseInt(
                        document.getElementById("editStockMinimo").value
                    ) || 0,

                idproveedor:
                    parseInt(
                        document.getElementById("editProveedor").value
                    ) || null,

                stock:
                    parseInt(
                        document.getElementById("editStock").value
                    ) || 0,

                idumedida:
                    parseInt(
                        document.getElementById("editUnidadMedida").value
                    ) || null,

                activo:
                    document.getElementById("editActivo").value === "true",

                categoriasId:
                    editCategorias,

                imagenesBase64:
                    editImagenesBase64

            };

            const res = await fetch(
                "/Articulo/Actualizar",
                {
                    method: "PUT",

                    headers: {
                        "Content-Type":
                            "application/json"
                    },

                    body: JSON.stringify(data)
                }
            );

            const result = await res.json();

            if (!res.ok) {

                throw new Error(
                    result.message
                );

            }

            await Swal.fire({
                icon: "success",
                title: "Éxito",
                text:
                    "Artículo actualizado correctamente",
                timer: 1800,
                showConfirmButton: false
            });

            $("#modalEditarArticulo")
                .modal("hide");

            listarArticulos();

        }
        catch (error) {

            console.error(error);

            Swal.fire({
                icon: "error",
                title: "Error",
                text: error.message
            });

        }

    });

// =========================================
// CAMBIAR ESTADO
// =========================================

async function cambiarEstado(id, activoActual) {

    const nuevoEstado = !activoActual;

    const confirmacion =
        await Swal.fire({

            title:
                nuevoEstado
                    ? "Restaurar artículo"
                    : "Eliminar artículo",

            text:
                nuevoEstado
                    ? "¿Deseas restaurar este artículo?"
                    : "¿Deseas eliminar este artículo?",

            icon: "warning",

            showCancelButton: true,

            confirmButtonText:
                nuevoEstado
                    ? "Sí, restaurar"
                    : "Sí, eliminar",

            cancelButtonText:
                "Cancelar"

        });

    if (!confirmacion.isConfirmed) return;

    try {

        const res =
            await fetch(
                `/Articulo/CambiarEstado?id=${id}&activo=${nuevoEstado}`,
                {
                    method: "PUT"
                }
            );

        const result = await res.json();

        if (!res.ok) {

            throw new Error(
                result.message
            );

        }

        await Swal.fire({
            icon: "success",
            title: "Éxito",
            text: result.message,
            timer: 1800,
            showConfirmButton: false
        });

        listarArticulos();

    }
    catch (error) {

        console.error(error);

        Swal.fire({
            icon: "error",
            title: "Error",
            text: error.message
        });

    }

}

// =========================================
// MENU ACCIONES
// =========================================

function toggleActionsMenu(button) {

    document
        .querySelectorAll(".actions-menu")
        .forEach(menu => {

            if (
                menu !==
                button.nextElementSibling
            ) {

                menu.classList.remove("show");

            }

        });

    button.nextElementSibling
        .classList.toggle("show");

}

// =========================================
// CERRAR MENU
// =========================================

document.addEventListener("click", function (e) {

    if (
        !e.target.closest(".actions-dropdown")
    ) {

        document
            .querySelectorAll(".actions-menu")
            .forEach(menu => {

                menu.classList.remove("show");

            });

    }

});