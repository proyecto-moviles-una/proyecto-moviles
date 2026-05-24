namespace Core.Enum.Autenticacion
{
    public enum enumErroresAutenticacion
    {
        // Registro / activación
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

        // Sesión
        guidSesionFaltante      = 20,
        sesionInvalida          = 21,
        errorAbrirSesion        = 22,
        sesionYaCerrada         = 23,
        accesoNoAutorizado      = 24,

        // Reactivación
        usuarioNoDesactivado    = 47,

        // Notificaciones
        tokenFCMFaltante         = 48,
        errorActualizandoTokenFCM = 49,
    }
}
