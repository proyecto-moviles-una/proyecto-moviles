namespace DTO.Usuario
{
    /// <summary>
    /// DTO recibido en el body del endpoint POST /api/auth/registro
    /// </summary>
    public class DTOInsertarUsuario
    {
        public string nombre    { get; set; }
        public string apellidos { get; set; }
        public string email     { get; set; }
        public string password  { get; set; }
    }
}
