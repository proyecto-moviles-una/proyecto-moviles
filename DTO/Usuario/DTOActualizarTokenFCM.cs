namespace DTO.Usuario
{
    /// <summary>
    /// DTO que recibe el endpoint PUT api/usuarios/tokenFCM.
    /// La app MAUI lo envia como JSON al arrancar.
    /// </summary>
    public class DTOActualizarTokenFCM
    {
        public string tokenFCM { get; set; }
    }
}
