using AccesoDatos;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using Core.Enum.Tarifa;
using System;
using System.Collections.Generic;

namespace Logica.Tarifa
{
    public class LogTarifa
    {
        public ResCrearTarifa Crear(ReqCrearTarifa req)
        {
            var res = new ResCrearTarifa { resultado = false, error = new List<Error>() };

            try
            {
                if (req.Monto <= 0)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresTarifa.montoInvalido, Mensaje = "El monto debe ser mayor a cero" });
                    return res;
                }

                if (req.FechaVigencia == default)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresTarifa.fechaVigenciaInvalida, Mensaje = "La fecha de vigencia es obligatoria" });
                    return res;
                }

                System.Nullable<System.Guid> guidTarifa = null;
                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (var db = new ConexionLinqDataContext())
                {
                    db.SP_INGRESAR_TARIFA(
                        req.GuidRuta,
                        req.Monto,
                        req.FechaVigencia,
                        ref guidTarifa,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                if (guidTarifa == null || guidTarifa == Guid.Empty)
                {
                    res.error.Add(new Error { Codigo = (int)EnumErroresTarifa.errorCreandoTarifa, Mensaje = errorDescBD ?? "Error al crear la tarifa" });
                    return res;
                }

                res.Tarifa = new Core.Entidades.Tarifa
                {
                    Guid = guidTarifa.Value,
                    GuidRuta = req.GuidRuta,
                    Monto = req.Monto,
                    FechaVigencia = req.FechaVigencia,
                    Estado = true
                };
                res.resultado = true;
                res.error = null;
            }
            catch (Exception ex)
            {
                res.error.Add(new Error { Codigo = (int)EnumErroresTarifa.errorCreandoTarifa, Mensaje = ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "") });
            }

            return res;
        }
    }
}
