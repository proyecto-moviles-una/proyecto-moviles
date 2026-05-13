using AccesoDatos;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using Core.Enum.Zona;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logica.Zona
{
    public class LogZona
    {
        public ResCrearZona Crear(ReqCrearZona req)
        {
            var res = new ResCrearZona { resultado = false, error = new List<Error>() };

            try
            {
                if (string.IsNullOrEmpty(req.Nombre))
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresZona.nombreFaltante, Mensaje = "El nombre es obligatorio" });
                    return res;
                }

                System.Nullable<System.Guid> guidZona = null;
                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (var db = new ConexionLinqDataContext())
                {
                    db.SP_INGRESAR_ZONA(req.Nombre, req.Descripcion, ref guidZona, ref idReturn, ref errorIdBD, ref errorDescBD);
                }

                if (guidZona == null || guidZona == Guid.Empty)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresZona.errorCreandoZona, Mensaje = errorDescBD ?? "Error al crear la zona" });
                    return res;
                }

                res.Zona = new Core.Entidades.Zona { Guid = guidZona.Value, Nombre = req.Nombre, Descripcion = req.Descripcion, Estado = true };
                res.resultado = true;
                res.error = null;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresZona.errorCreandoZona, Mensaje = ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "") });
            }

            return res;
        }

        public ResListarZonas Listar()
        {
            var res = new ResListarZonas { resultado = false, error = new List<Error>() };

            try
            {
                using (var db = new ConexionLinqDataContext())
                {
                    res.Zonas = db.SP_OBTENER_ZONAS().Select(x => new Core.Entidades.Zona
                    {
                        Guid = x.GUID_ZONA,
                        Nombre = x.NOMBRE,
                        Descripcion = x.DESCRIPCION,
                        Estado = x.ESTADO
                    }).ToList();
                }

                res.resultado = true;
                res.error = null;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresZona.errorCreandoZona, Mensaje = ex.Message });
            }

            return res;
        }
    }
}
