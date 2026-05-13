using AccesoDatos;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using Core.Enum.Empresa;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logica.Empresa
{
    public class LogEmpresa
    {
        public ResCrearEmpresa Crear(ReqCrearEmpresa req)
        {
            var res = new ResCrearEmpresa { resultado = false, error = new List<Error>() };

            try
            {
                if (string.IsNullOrEmpty(req.Nombre))
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresEmpresa.nombreFaltante, Mensaje = "El nombre es obligatorio" });
                    return res;
                }

                System.Nullable<System.Guid> guidEmpresa = null;
                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (var db = new ConexionLinqDataContext())
                {
                    db.SP_INGRESAR_EMPRESA(req.Nombre, req.Telefono, req.Correo, ref guidEmpresa, ref idReturn, ref errorIdBD, ref errorDescBD);
                }

                if (guidEmpresa == null || guidEmpresa == Guid.Empty)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresEmpresa.errorCreandoEmpresa, Mensaje = errorDescBD ?? "Error al crear la empresa" });
                    return res;
                }

                res.Empresa = new Core.Entidades.Empresa { Guid = guidEmpresa.Value, Nombre = req.Nombre, Telefono = req.Telefono, Correo = req.Correo, Estado = true };
                res.resultado = true;
                res.error = null;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresEmpresa.errorCreandoEmpresa, Mensaje = ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "") });
            }

            return res;
        }

        public ResListarEmpresas Listar()
        {
            var res = new ResListarEmpresas { resultado = false, error = new List<Error>() };

            try
            {
                using (var db = new ConexionLinqDataContext())
                {
                    res.Empresas = db.SP_OBTENER_EMPRESAS().Select(x => new Core.Entidades.Empresa
                    {
                        Guid = x.GUID_EMPRESA,
                        Nombre = x.NOMBRE,
                        Telefono = x.TELEFONO,
                        Correo = x.CORREO,
                        Estado = x.ESTADO
                    }).ToList();
                }

                res.resultado = true;
                res.error = null;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresEmpresa.errorCreandoEmpresa, Mensaje = ex.Message });
            }

            return res;
        }
    }
}
