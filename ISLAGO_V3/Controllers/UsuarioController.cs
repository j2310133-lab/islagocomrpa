using ISLAGO_V3.Datos;
using ISLAGO_V3.Entidad.Models;
using ISLAGO_V3.Models.ViewModels;
using ISLAGO_V3.Negocio.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ISLAGO_V3.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly DBContextISLAGO _context;

        public UsuarioController(
            IUsuarioService usuarioService,
            DBContextISLAGO context)
        {
            _usuarioService = usuarioService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Lista de usuarios
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            try
            {

                var listsa = await _context.Usuarios
                    .Where(u => u.Activo == true)
                    .Select(u => new
                    {
                        u.Id,
                        u.Usarname,
                        u.Email,
                        Activo = u.Activo ?? false,
                        Bloqueado = u.Bloqueado ?? false,
                        u.IntentosFallidos,
                        u.UltimoLogin,

                        Persona = u.IdpersonaNavigation != null
                            ? $"{u.IdpersonaNavigation.Nombres} {u.IdpersonaNavigation.Apellidos}"
                            : "Not asigned person",

                        Imagen = _context.Usuarioimagens
                            .Where(ui => ui.Idusuario == u.Id)
                            .Join(
                                _context.Imagens,
                                ui => ui.Idimagen,
                                img => img.Id,
                                (ui, img) => new
                                {
                                    img.Nombre,
                                    img.Ruta,
                                    img.Fechapublicada
                                }
                            )
                            .OrderByDescending(x => x.Fechapublicada)
                            .Select(x => x.Ruta)
                            .FirstOrDefault(),

                        Rol = _context.UsuarioRols
                            .Where(ui => ui.Idusuario == u.Id)
                            .Join(
                                _context.Rols,
                                ur => ur.Idrol,
                                rol => rol.Id,
                                (ur, rol) => rol.Nombre
                            )
                            .ToList()

                    }).ToListAsync();

                return Ok(listsa);

            }
            catch(Exception e)
            {
                return BadRequest(new {
                    message = $"Error al intentar listar los usuarios:{e.Message}"
                });
            }
        }

        // =======================
        // CREAR USUARIO
        // ======================
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] UsuarioVM uvm)
        {

            try
            {

                if (uvm == null)
                {
                    return BadRequest(new
                    {
                        message = $"Data not recived"
                    });
                }

                if (string.IsNullOrWhiteSpace(uvm.Usarname))
                {
                    return BadRequest(new
                    {
                        message = "The username is requierd"
                    });
                }

                if (string.IsNullOrWhiteSpace(uvm.Email))
                {
                    return BadRequest(new
                    {
                        message = "The email is required"
                    });
                }

                if (string.IsNullOrWhiteSpace(uvm.Password))
                {
                    return BadRequest(new
                    {
                        message = "The password is required"
                    });
                }

                if (uvm.PersonaId == null || uvm.PersonaId <= 0)
                {
                    return BadRequest(new
                    {
                        message = "The person is required"
                    });
                }

                var usuario = new Usuario
                {
                    Usarname = uvm.Usarname.Trim(),
                    Email = uvm.Email.Trim(),
                    Activo = true,
                    Bloqueado = false,
                    IntentosFallidos = 0,
                    Idpersona = uvm.PersonaId
                };

                var creado = await _usuarioService.Crear(
                       usuario,
                       uvm.Password,
                       uvm.RolesSeleccionados ?? new List<int>(),
                       uvm.ImagenBase64
                );

                return Ok(new
                {
                    success = true,
                    message = "Usuario creado correctamente.",
                    id = creado.Id
                });

            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    message = $"Sorry, you have an error to try to create this user, type of error: {e.Message}"
                });
            }

        }

        // ===========================
        // Obtener personas y roles
        // ===========================
        [HttpGet]
        public async Task<IActionResult> ObtenerDatosFormulario()
        {
            try
            {

                var personas = await _context.Personas
                    .Where(ep => ep.Estado == true)
                    .Select(x => new
                    {
                        x.Id,

                        NombreCompleto = x.Nombres + " " + x.Apellidos,

                        x.Telefono
                    })
                    .OrderBy(x => x.NombreCompleto)
                    .ToListAsync();

                var roles = await _context.Rols
                    .Select(r => new
                    {
                        r.Id,
                        r.Nombre
                    })
                    .OrderBy(x => x.Nombre)
                    .ToListAsync();

                return Ok(new
                {
                    personas,
                    roles
                });

            }
            catch(Exception e)
            {
                return BadRequest(new
                {
                    message = $"Error trying to get the data form: {e.Message}"
                });
            }
        }

    }
}