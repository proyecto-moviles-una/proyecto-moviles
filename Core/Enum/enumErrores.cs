using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enum
{
    public enum enumErrores
    {
        // Generales
        errorNoControlado       = -2,
        errorBaseDatos          = -1,

        // Usuario
        nombreFaltante          = 1,
        apellidosFaltante       = 2,
        emailFaltante           = 3,
        emailInvalido           = 4,
        passwordVacio           = 5,
        guidDeUsuarioFaltante   = 6,
        errorActivandoUsuario   = 7,
        loginIncorrecto         = 8,
        usuarioInactivo         = 9,
        correoYaRegistrado      = 10,
        usuarioDesactivado      = 11,
        codigoExpirado          = 12,
        cuentaYaActiva          = 13,
        correoNoRegistrado      = 14,

        // Sesi�n
        guidSesionFaltante      = 20,
        sesionInvalida          = 21,
        errorAbrirSesion        = 22,
        sesionYaCerrada         = 23,
        accesoNoAutorizado      = 24,

        // Favoritos
        guidRutaFaltante        = 30,
        favoritoYaExiste        = 31,
        favoritoNoExiste        = 32,
        guidFavoritoFaltante    = 33,

        // Password
        passwordActualIncorrecto = 40,
        passwordNuevoVacio       = 41,
        passwordsNoCoinciden     = 42,

        // Cambio de correo
        correoNuevoFaltante      = 43,
        codigoVerificacionFaltante = 44,
        codigoVerificacionInvalido = 45,
        sinSolicitudCambioCorreo = 46,

        // Reactivar usuario
        usuarioNoDesactivado     = 47,
        // Ruta
        nombreRutaFaltante      = 40,
        guidEmpresaFaltante     = 41,
        errorInsertandoRuta     = 42,
        errorActualizandoRuta   = 43,
        errorDesactivandoRuta   = 44,

        // Horario
        horaSalidaFaltante      = 50,
        diasServicioFaltante    = 51,
        errorInsertandoHorario  = 52,

        // Empresa
        nombreEmpresaFaltante   = 60,
        errorInsertandoEmpresa  = 61,

        // Zona
        nombreZonaFaltante      = 70,
        errorInsertandoZona     = 71,
    }
}
