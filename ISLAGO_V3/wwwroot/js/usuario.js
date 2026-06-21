let imagenBase64 = null;
let rolesSeleccionados = [];

document.addEventListener("DOMContentLoaded", () => {

    cargarUsuarios();

    cargarDatosFormulario();

});

async function cargarUsuarios() {

    try {

        const response = await fetch("Usuario/Listar");

        if (!response.ok) throw new Error("Error al obtener los usuarios")

        const data = await response.json();

        actualizarDashboard(data);
        renderUsuarios(data);

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

// ==================================
// Dashboard superior
// =================================
function actualizarDashboard(data) {

    document.getElementById("totalUsuarios")
        .textContent = data.length;

    document.getElementById("usuariosActivos")
        .textContent =
        data.filter(x => x.activo).length;

    document.getElementById("usuariosBloqueados")
        .textContent =
        data.filter(x => x.bloqueado).length;

}

// ==================================
// Render del Grid
// ==================================
function renderUsuarios(data) {

    const grid =
        document.getElementById("usuariosGrid");

    grid.innerHTML = "";

    data.forEach(usuario => {

        grid.innerHTML += crearCardUsuario(usuario);

    });

}

// ==================================
// Crear Data || Estructura visual
// ==================================
function crearCardUsuario(usuario) {

    const roles =
        usuario.rol?.length > 0
            ? usuario.rol
                .map(r =>
                    `<span class="rol-tag">${r}</span>`)
                .join("")
            : "<span class='rol-tag'>Sin rol</span>";

    return `
    
    <div class="usuario-card">

        <div class="usuario-cover"></div>

        <div class="usuario-body">

            <img class="usuario-avatar"
                 src="${obtenerImagen(usuario.imagen)}"
                 alt="Not image">

            <h4 class="usuario-nombre">
                ${usuario.usarname}
            </h4>

            <div class="usuario-email">
                ${ocultarEmail(usuario.email)}
            </div>

            <div class="usuario-persona">
                ${usuario.persona}
            </div>

            <div class="usuario-roles">
                ${roles}
            </div>

            <div class="usuario-info">

                <div class="info-box">

                    <span class="info-title">
                        Activo
                    </span>

                    <span class="
                        ${usuario.activo
            ? 'badge-success'
            : 'badge-danger'}">

                        ${usuario.activo
            ? 'Sí'
            : 'No'}

                    </span>

                </div>

                <div class="info-box">

                    <span class="info-title">
                        Bloqueado
                    </span>

                    <span class="
                        ${usuario.bloqueado
            ? 'badge-danger'
            : 'badge-success'}">

                        ${usuario.bloqueado
            ? 'Sí'
            : 'No'}

                    </span>

                </div>

            </div>

        </div>

    </div>

    `;
}

// ============================
// Obtener imagen emergente
// ============================

function obtenerImagen(imagen) {

    if (!imagen)
        return "/img/user-default.png";

    return imagen;

}


// Ocultar email
function ocultarEmail(email) {

    if (!email)
        return "";

    const partes =
        email.split("@");

    if (partes.length !== 2)
        return email;

    const usuario =
        partes[0];

    const dominio =
        partes[1];

    if (usuario.length <= 3)
        return email;

    const inicio =
        usuario.substring(0, 2);

    const final =
        usuario.substring(usuario.length - 1);

    return `${inicio}*****${final}@${dominio}`;

}

// ==============================
// cargamos roles y personas
// ==============================
let personas = [];
let roles = [];

async function cargarDatosFormulario() {

    try {

        const response =
            await fetch("/Usuario/ObtenerDatosFormulario");

        if (!response.ok)
            throw new Error("Error al cargar datos");

        const data =
            await response.json();

        personas = data.personas;
        roles = data.roles;

        llenarPersonas();
        llenarRoles();
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

function llenarPersonas() {

    const select =
        document.getElementById("persona");

    select.innerHTML =
        `<option value="">Seleccione una persona</option>`;

    personas.forEach(persona => {

        select.innerHTML += `
            <option value="${persona.id}">
                ${persona.nombreCompleto}
            </option>
        `;

    });

}

function llenarRoles() {

    const select =
        document.getElementById("rolSelect");

    select.innerHTML =
        `<option value="">
            Seleccione un rol
        </option>`;

    roles.forEach(rol => {

        select.innerHTML += `
            <option value="${rol.id}">
                ${rol.nombre}
            </option>
        `;

    });

}

document.addEventListener("change", function (e) {

    if (e.target.id === "rolSelect") {

        agregarRol(
            e.target.value,
            e.target.options[e.target.selectedIndex].text
        );

        e.target.value = "";
    }

});

function agregarRol(id, nombre) {

    if (!id)
        return;

    const existe =
        rolesSeleccionados.some(x => x.id == id);

    if (existe)
        return;

    rolesSeleccionados.push({
        id,
        nombre
    });

    renderRolesSeleccionados();

}

function renderRolesSeleccionados() {

    const container =
        document.getElementById("rolesSeleccionados");

    container.innerHTML = "";

    rolesSeleccionados.forEach(rol => {

        container.innerHTML += `
        
        <span class="rol-badge">

            ${rol.nombre}

            <button
                type="button"
                onclick="eliminarRol(${rol.id})">

                ×

            </button>

        </span>
        
        `;

    });

}

function eliminarRol(id) {

    rolesSeleccionados =
        rolesSeleccionados.filter(
            x => x.id != id
        );

    renderRolesSeleccionados();

}

function convertirImagen(e) {

    const file =
        e.target.files[0];

    if (!file)
        return;

    const reader =
        new FileReader();

    reader.onload = function () {

        imagenBase64 =
            reader.result;

        document
            .getElementById("previewUsuario")
            .src = imagenBase64;
    };

    reader.readAsDataURL(file);

}