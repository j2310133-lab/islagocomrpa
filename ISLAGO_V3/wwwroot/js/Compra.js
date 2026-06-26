// =========================================
// VARIABLES GLOBALES
// =========================================

let listaCompras = [];
let articulosProveedor = [];

// =========================================
// INIT
// =========================================

$(document).ready(function () {

    cargarProveedores();

    listarCompras();

});

// =========================================
// LISTAR COMPRAS
// =========================================

async function listarCompras() {

    try {

        const res = await fetch("/Compra/Listar");

        if (!res.ok) {

            throw new Error(
                "No se pudieron cargar las compras."
            );

        }

        const data = await res.json();

        listaCompras = data;

        renderCompras(data);

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
// RENDER COMPRAS
// =========================================

function renderCompras(data) {

    const tbody = $("#tablaCompras");

    tbody.empty();

    if (!data || data.length === 0) {

        tbody.append(`

            <tr>

                <td colspan="5"
                    class="text-center text-muted">

                    No hay compras registradas.

                </td>

            </tr>

        `);

        return;

    }

    data.forEach(compra => {

        tbody.append(`

            <tr>

                <td class="compra-id">

                    ${compra.id}

                </td>

                <td class="proveedor">

                    ${compra.proveedor}

                </td>

                <td class="fecha"
                    data-fecha="${compra.fechaBusqueda}">

                    ${compra.fecha}

                </td>

                <td>

                    C$ ${Number(compra.total).toFixed(2)}

                </td>

                <td>

                    <button
                        class="btn btn-info btnVerDetalle"
                        data-id="${compra.id}">

                        Ver

                    </button>

<a
    class="btn btn-danger"
    href="/Compra/DescargarPdf?id=${compra.id}"
    target="_blank"
    title="Descargar PDF">

    <i class="fas fa-file-pdf"></i>

</a>

                </td>

            </tr>

        `);

    });

}

// =========================================
// CARGAR PROVEEDORES
// =========================================

function cargarProveedores() {

    $.get("/Compra/ObtenerProveedores", function (data) {

        const select = $("#proveedor");

        select.empty();

        select.append(`

            <option value="">

                Seleccione proveedor

            </option>

        `);

        data.forEach(p => {

            select.append(`

                <option value="${p.id}">

                    ${p.nombre}

                </option>

            `);

        });

    });

}

// =========================================
// CAMBIO DE PROVEEDOR
// =========================================

$(document).on("change", "#proveedor", function () {

    const idProveedor = $(this).val();

    $("#detalleCompra").empty();

    $("#totalCompra").text("0.00");

    articulosProveedor = [];

    if (!idProveedor)
        return;

    $.get(

        `/Compra/ObtenerArticulosPorProveedor?idProveedor=${idProveedor}`,

        function (data) {

            articulosProveedor = data;

            agregarFilaArticulo();

        }

    );

});

// =========================================
// AGREGAR FILA
// =========================================

function agregarFilaArticulo() {

    let opciones = `

        <option value="">

            Seleccione artículo

        </option>

    `;

    articulosProveedor.forEach(a => {

        opciones += `

            <option
                value="${a.id}"
                data-precio="${a.precio}">

                ${a.nombre}

            </option>

        `;

    });

    $("#detalleCompra").append(`

        <tr>

            <td>

                <select
                    class="form-control articuloSelect">

                    ${opciones}

                </select>

            </td>

            <td>

                <input
                    type="number"
                    class="form-control cantidad"
                    value="1"
                    min="1">

            </td>

            <td>

                <input
                    type="number"
                    class="form-control precio"
                    readonly>

            </td>

            <td>

                <span class="subtotal">

                    0.00

                </span>

            </td>

            <td>

                <button
                    class="btn btn-danger btnEliminar"
                    type="button">

                    X

                </button>

            </td>

        </tr>

    `);

}

// =========================================
// BOTON AGREGAR ARTICULO
// =========================================

$(document).on("click", ".btnAgregarArticulo", function () {

    if (articulosProveedor.length === 0) {

        Swal.fire({

            icon: "warning",

            title: "Proveedor requerido",

            text: "Seleccione un proveedor primero."

        });

        return;

    }

    agregarFilaArticulo();

});

// =========================================
// SELECCIONAR ARTICULO
// =========================================

$(document).on("change", ".articuloSelect", function () {

    const articulo = $(this).val();

    let repetido = false;

    $(".articuloSelect").not(this).each(function () {

        if ($(this).val() == articulo) {

            repetido = true;

        }

    });

    if (repetido) {

        Swal.fire({

            icon: "warning",

            title: "Artículo duplicado",

            text: "Ese artículo ya fue agregado."

        });

        $(this).val("");

        return;

    }

    const fila = $(this).closest("tr");

    const precio = $(this)

        .find(":selected")

        .data("precio") || 0;

    fila.find(".precio").val(precio);

    calcularFila(fila);

});

// =========================================
// CAMBIO CANTIDAD
// =========================================

$(document).on("input", ".cantidad", function () {

    calcularFila(

        $(this).closest("tr")

    );

});

// =========================================
// ELIMINAR FILA
// =========================================

$(document).on("click", ".btnEliminar", function () {

    $(this)

        .closest("tr")

        .remove();

    calcularTotal();

});

// =========================================
// CALCULAR FILA
// =========================================

function calcularFila(fila) {

    const cantidad =

        parseFloat(

            fila.find(".cantidad").val()

        ) || 0;

    const precio =

        parseFloat(

            fila.find(".precio").val()

        ) || 0;

    const subtotal = cantidad * precio;

    fila.find(".subtotal")

        .text(subtotal.toFixed(2));

    calcularTotal();

}

// =========================================
// CALCULAR TOTAL
// =========================================

function calcularTotal() {

    let total = 0;

    $(".subtotal").each(function () {

        total +=

            parseFloat(

                $(this).text()

            ) || 0;

    });

    $("#totalCompra")

        .text(total.toFixed(2));

}

// =========================================
// LIMPIAR MODAL
// =========================================

$('#modalCompra').on('hidden.bs.modal', function () {

    $("#proveedor").val("");

    $("#detalleCompra").empty();

    $("#totalCompra").text("0.00");

    $("#btnGuardarCompra")
        .prop("disabled", false);

    articulosProveedor = [];

});

// =========================================
// GUARDAR COMPRA
// =========================================

$("#btnGuardarCompra").click(function () {

    const idProveedor = $("#proveedor").val();

    if (!idProveedor) {

        Swal.fire({
            icon: "warning",
            title: "Proveedor requerido",
            text: "Seleccione un proveedor."
        });

        return;
    }

    let filaIncompleta = false;


    let detalles = [];

    $("#detalleCompra tr").each(function () {

        const articulo = $(this).find(".articuloSelect").val();

        if (!articulo) {

            filaIncompleta = true;

            return;

        }

        const cantidad = parseFloat(
            $(this).find(".cantidad").val()
        );

        const precio = parseFloat(
            $(this).find(".precio").val()
        );

        if (isNaN(cantidad) || cantidad <= 0) {

            Swal.fire({

                icon: "warning",

                title: "Cantidad inválida",

                text: "La cantidad debe ser mayor que cero."

            });

            filaIncompleta = true;

            return false;

        }

        detalles.push({

            idarticulo: parseInt(articulo),

            cantidad: cantidad,

            precio: precio,

            subtotal: cantidad * precio

        });

    });

    if (filaIncompleta) {

        Swal.fire({

            icon: "warning",

            title: "Filas incompletas",

            text: "Complete o elimine los artículos sin seleccionar."

        });

        return;

    }

    if (detalles.length === 0) {

        Swal.fire({
            icon: "warning",
            title: "Compra vacía",
            text: "Debe agregar al menos un artículo."
        });

        return;
    }

    const compraVM = {

        compra: {

            idproveedor: parseInt(idProveedor),

            total: parseFloat(
                $("#totalCompra").text()
            )

        },

        detalles: detalles

    };

    guardarCompra(compraVM);

});

function guardarCompra(compraVM) {

    $("#btnGuardarCompra").prop("disabled", true);

    $.ajax({

        url: "/Compra/Crear",

        type: "POST",

        contentType: "application/json",

        data: JSON.stringify(compraVM),

        success: async function (response) {

            if (!response.success) {

                $("#btnGuardarCompra")
                    .prop("disabled", false);

                Swal.fire({

                    icon: "error",

                    title: "Error",

                    text: response.message

                });

                return;

            }

            await Swal.fire({

                icon: "success",

                title: "Compra registrada",

                text: response.message,

                timer: 1800,

                showConfirmButton: false

            });

            $("#modalCompra").modal("hide");

            listarCompras();

        },

        error: function () {

            $("#btnGuardarCompra")
                .prop("disabled", false);

            Swal.fire({
                icon: "error",
                title: "Error",
                text: "Ocurrió un error al guardar la compra."
            });

        }

    });

}

// =========================================
// VER DETALLE
// =========================================

$(document).on("click", ".btnVerDetalle", function () {

    const idCompra = $(this).data("id");

    $("#detalleCompraModal").empty();

    $("#totalDetalleCompra").text("0.00");

    $.get(

        `/Compra/ObtenerDetalleCompra?idCompra=${idCompra}`,

        function (data) {

            let total = 0;

            data.forEach(d => {

                total += Number(d.subtotal);

                $("#detalleCompraModal").append(`

                    <tr>

                        <td>${d.articulo}</td>

                        <td>${d.cantidad}</td>

                        <td>C$ ${Number(d.precio).toFixed(2)}</td>

                        <td>C$ ${Number(d.subtotal).toFixed(2)}</td>

                    </tr>

                `);

            });

            $("#totalDetalleCompra")
                .text(total.toFixed(2));

            $("#modalDetalleCompra")
                .modal("show");

        }

    );

});

// =========================================
// FILTRAR COMPRAS
// =========================================

$("#buscarId, #buscarProveedor, #buscarFecha").on("input change", function () {

    const idFiltro = $("#buscarId").val().trim();

    const proveedorFiltro = $("#buscarProveedor").val().trim().toLowerCase();

    const fechaFiltro = $("#buscarFecha").val();

    const comprasFiltradas = listaCompras.filter(c => {

        // ID (exacto)
        if (idFiltro !== "" && c.id.toString() !== idFiltro)
            return false;

        // Proveedor
        if (
            proveedorFiltro !== "" &&
            !c.proveedor.toLowerCase().includes(proveedorFiltro)
        )
            return false;

        // Fecha
        if (
            fechaFiltro !== "" &&
            c.fechaBusqueda !== fechaFiltro
        )
            return false;

        return true;

    });

    renderCompras(comprasFiltradas);

});

$(document).on("input", ".cantidad", function () {

    let cantidad = parseFloat($(this).val());

    if (isNaN(cantidad) || cantidad < 1) {

        $(this).val(1);

    }

    calcularFila($(this).closest("tr"));

});

