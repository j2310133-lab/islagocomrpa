using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//OTHERS
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ISLAGO_V3.Datos.DBContext;
using Microsoft.EntityFrameworkCore;
using ISLAGO_V3.Datos.Interfaces;
using ISLAGO_V3.Datos.Implementaciones;
using ISLAGO_V3.Entidad.Models.Options;
//using ISLAGO_V3.Negocio.Interfaces;
//using ISLAGO_V3.Negocio.Implementaciones;

namespace ISLAGO_V3.IoC
{
    public static class Dependency
    {
        public static void InyectarDependencias(this IServiceCollection serv, IConfiguration conf)
        {
            //Dependency of Data Base Conection
            serv.AddDbContext<DBContextISLAGO>(options =>
            {
                options.UseNpgsql(conf.GetConnectionString("CadenaNPGSQL"));
            });

            //Generic Dependency || General CRUD

            //Others Dependencys

            // --------------------------------------
            // CONFIGURACIONES (Options Pattern)
            // Configuración general de rutas, tamaños y formatos
            // para almacenamiento de imágenes, videos y archivos.
            // --------------------------------------
            serv.Configure<StorageOptions>(opt =>
            {
                //URL Base
                opt.PublicBaseURL = "/ISLagoIMG";

                opt.RutaBase = new Dictionary<string, string>
                {
                    { "imagen-articulo", @"C:\Users\Jonathan\source\repos\ISLAGO_V3\ISLAGO_V3\wwwroot\img\articulos-img\"}
                };

                opt.FormatosPermitidos = new Dictionary<string, string[]>
                {
                    { "imagen-articulo", new [] {"jpg", "jpeg", "svg", "png"} }
                };

                opt.TamañosMaximosMB = new Dictionary<string, int>
                {
                    { "imagen-articulo", 1000 }
                };
            });
        }
    }
}
