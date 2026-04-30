namespace DTO.Usuario
{
    /// <summary>
    /// DTO recibido en el body del endpoint POST /api/auth/activar
    /// </summary>
    public class DTOActivarUsuario
    {
        public string correo { get; set; }
        public string token  { get; set; }
    }
}
