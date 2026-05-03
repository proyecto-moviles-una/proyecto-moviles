using AccesoDatos;
using Core.Entidades;
using Core.Entidades.Request;
using Core.Entidades.Response;
using Core.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Logica.Ruta
{
    public class LogHorario
    {
        // =====================================================================
        // INSERTAR HORARIO
        // =====================================================================
        public ResInsertarHorario insertar(ReqInsertarHorario req)
        {
            ResInsertarHorario res = new ResInsertarHorario();
            res.resultado = false;
            res.error = new List<Error>();

            enumBitacora tipoBitacora = enumBitacora.fallido;
            int errorId = 0;
            string errorDesc = string.Empty;

            try
            {
                if (req.guidRuta == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.guidRutaFaltante));
                    return res;
                }
                if (req.horaSalida == TimeSpan.Zero)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.horaSalidaFaltante));
                    return res;
                }
                if (req.diasServicio == 0)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.diasServicioFaltante));
                    return res;
                }

                System.Nullable<System.Guid> guidHorario = null;
                System.Nullable<int> idReturn = null;
                System.Nullable<int> errorIdBD = null;
                string errorDescBD = null;

                using (ConexionLinqDataContext linq = DataContextFactory.Create())
                {
                    linq.SP_INGRESAR_HORARIO(
                        req.guidRuta,
                        req.horaSalida,
                        req.diasServicio,
                        ref guidHorario,
                        ref idReturn,
                        ref errorIdBD,
                        ref errorDescBD);
                }

                if (guidHorario == null || guidHorario == Guid.Empty)
                {
                    res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorInsertandoHorario));
                    errorId = (int)enumErrores.errorInsertandoHorario;
                    errorDesc = errorDescBD ?? enumErrores.errorInsertandoHorario.ToString();
                    return res;
                }

                res.resultado = true;
                res.guidHorario = guidHorario;
                res.error = null;
                tipoBitacora = enumBitacora.exitoso;
            }
            catch (Exception ex)
            {
                res.error.Add(Utilitarios.Utilitarios.crearError(enumErrores.errorNoControlado));
                errorId = (int)enumErrores.errorNoControlado;
                errorDesc = ex.Message;
            }
            finally
            {
                bitacorear(null, tipoBitacora, errorId, errorDesc, req, res);
            }

            return res;
        }

        // =====================================================================
        // BITÁCORA
        // =====================================================================
        private void bitacorear(Guid? guidUsuario, enumBitacora tipo, int errorId,
                                string errorDesc, object req, object res)
        {
            try
            {
                ReqBitacorear reqBit = new ReqBitacorear();
                reqBit.bitacora = new Bitacora
                {
                    guidUsuario = guidUsuario,
                    clase = GetType().Name,
                    metodo = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name,
                    tipo = tipo,
                    errorId = errorId,
                    descripcion = errorDesc,
                    request = JsonConvert.SerializeObject(req),
                    response = JsonConvert.SerializeObject(res)
                };
                Utilitarios.Utilitarios.bitacorear(reqBit);
            }
            catch { }
        }
    }
}