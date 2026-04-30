namespace DTO.Usuario
{
    /// <summary>
    /// DTO recibido en el body del endpoint PUT /api/usuarios/perfil
    /// </summary>
    public class DTOActualizarUsuario
    {
        public string nombre    { get; set; }
        public string apellidos { get; set; }
    }
}
