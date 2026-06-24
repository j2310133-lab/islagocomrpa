$(document).ready(function () {

    cargarProveedores();

});

// ==============================
// CARGAR PROVEEDORES
// ==============================

function cargarProveedores() {

    $.get("/Compra/ObtenerProveedores", function (data) {

        $("#proveedor").empty();

        $("#proveedor").append(`
            <option value="">
                Seleccione proveedor
            </option>
        `);

        data.forEach(p => {

            $("#proveedor").append(`
                <option value="${p.id}">
                    ${p.nombre}
                </option>
            `);

        });

    });

}

let articulosProveedor = [];

// ==============================
// CAMBIO DE PROVEEDOR
// ==============================

$(document).on("change", "#proveedor", function () {

    let idProveedor = $(this).val();

    $("#detalleCompra").empty();

    $("#totalCompra").text("0.00");

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

function agregarFilaArticulo() {

    let opciones = `
        <option value="">
            Seleccione artículo
        </option>
    `;

    articulosProveedor.forEach(a => {

        opciones += `
            <option value="${a.id}"
                    data-precio="${a.precio}">
                ${a.nombre}
            </option>
        `;

    });

    $("#detalleCompra").append(`

        <tr>

            <td>
                <select class="form-control articuloSelect">
                    ${opciones}
                </select>
            </td>

            <td>
                <input type="number"
                       class="form-control cantidad"
                       min="1"
                       value="1">
            </td>

            <td>
                <input type="number"
                       class="form-control precio"
                       readonly>
            </td>

            <td>
                <span class="subtotal">
                    0.00
                </span>
            </td>

            <td>
                <button type="button"
                        class="btn btn-danger btnEliminar">

                    X

                </button>
            </td>

        </tr>

    `);

}

$(document).on("click", ".btnAgregarArticulo", function () {

    if (articulosProveedor.length === 0) {

        alert("Seleccione un proveedor primero");

        return;
    }

    agregarFilaArticulo();

});

// ==============================
// SELECCIONAR ARTICULO
// ==============================

$(document).on("change", ".articuloSelect", function () {

    let articuloSeleccionado = $(this).val();

    let repetido = false;

    $(".articuloSelect").not(this).each(function () {

        if ($(this).val() == articuloSeleccionado) {

            repetido = true;
        }

    });

    if (repetido) {

        alert("Este artículo ya fue agregado");

        $(this).val("");

        return;
    }

    let fila = $(this).closest("tr");

    let precio = $(this)
        .find(":selected")
        .data("precio");

    fila.find(".precio").val(precio);

    calcularFila(fila);

});

function calcularFila(fila) {

    let cantidad =
        parseFloat(
            fila.find(".cantidad").val()
        ) || 0;

    let precio =
        parseFloat(
            fila.find(".precio").val()
        ) || 0;

    let subtotal = cantidad * precio;

    fila.find(".subtotal")
        .text(subtotal.toFixed(2));

    calcularTotal();
}

function calcularTotal() {

    let total = 0;

    $(".subtotal").each(function () {

        total +=
            parseFloat($(this).text()) || 0;

    });

    $("#totalCompra")
        .text(total.toFixed(2));
}

$(document).on(
    "input",
    ".cantidad",
    function () {

        let fila =
            $(this).closest("tr");

        calcularFila(fila);

    }
);

$(document).on("click", ".btnEliminar", function () {

    $(this)
        .closest("tr")
        .remove();

    calcularTotal();

});

$('#modalCompra').on('hidden.bs.modal', function () {

    $("#proveedor").val("");

    $("#detalleCompra").empty();

    $("#totalCompra").text("0.00");

    articulosProveedor = [];

});

// ==============================
// GUARDAR COMPRA
// ==============================

$("#btnGuardarCompra").click(function () {

    let idProveedor = $("#proveedor").val();

    if (!idProveedor) {

        alert("Seleccione un proveedor");

        return;
    }

    let detalles = [];

    $("#detalleCompra tr").each(function () {

        let articulo =
            $(this)
                .find(".articuloSelect")
                .val();

        let cantidad =
            $(this)
                .find(".cantidad")
                .val();

        let precio =
            $(this)
                .find(".precio")
                .val();

        let subtotal =
            parseFloat(cantidad) *
            parseFloat(precio);

        if (articulo) {

            detalles.push({

                idarticulo: parseInt(articulo),
                cantidad: parseFloat(cantidad),
                precio: parseFloat(precio),
                subtotal: subtotal

            });

        }

    });

    if (detalles.length === 0) {

        alert("Debe agregar al menos un artículo");

        return;
    }

    let compraVM = {

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

    $.ajax({

        url: "/Compra/Crear",

        type: "POST",

        contentType: "application/json",

        data: JSON.stringify(compraVM),

        success: function (response) {

            if (response.success) {

                alert(response.message);

                location.reload();

            }
            else {

                alert(response.message);

            }

        },

        error: function () {

            alert("Error al guardar");

        }

    });

}

// ==============================
// VER DETALLE COMPRA
// ==============================

$(document).on("click", ".btnVerDetalle", function () {

    let idCompra = $(this).data("id");

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

                        <td>C$ ${d.precio}</td>

                        <td>C$ ${d.subtotal}</td>

                    </tr>

                `);

            });

            $("#totalDetalleCompra").text(
                total.toFixed(2)
            );

            $("#modalDetalleCompra").modal("show");

        }
    );

});

// ==============================
// FILTRAR COMPRAS
// ==============================

$("#buscarId, #buscarProveedor, #buscarFecha")
    .on("keyup change", function () {

        let idFiltro =
            $("#buscarId").val().toLowerCase();

        let proveedorFiltro =
            $("#buscarProveedor").val().toLowerCase();

        let fechaFiltro =
            $("#buscarFecha").val();

        $(".compra-table tbody tr").each(function () {

            let id =
                $(this)
                    .find(".compra-id")
                    .text()
                    .trim()
                    .toLowerCase();

            let proveedor =
                $(this)
                    .find(".proveedor")
                    .text()
                    .trim()
                    .toLowerCase();

            let fecha =
                $(this)
                    .find(".fecha")
                    .data("fecha");

            let mostrar = true;

            // FILTRO ID

            if (idFiltro !== "" &&
                !id.includes(idFiltro)) {

                mostrar = false;
            }

            // FILTRO PROVEEDOR

            if (proveedorFiltro !== "" &&
                !proveedor.includes(proveedorFiltro)) {

                mostrar = false;
            }

            // FILTRO FECHA

            if (fechaFiltro !== "" &&
                fecha !== fechaFiltro) {

                mostrar = false;
            }

            $(this).toggle(mostrar);

        });

    });