namespace DTO.Usuario
{
    /// <summary>
    /// DTO recibido en el body del endpoint POST /api/auth/login
    /// </summary>
    public class DTOLogin
    {
        public string email    { get; set; }
        public string password { get; set; }
    }
}
