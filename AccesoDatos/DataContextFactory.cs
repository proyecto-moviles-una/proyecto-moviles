using System.Configuration;

namespace AccesoDatos
{
    public static class DataContextFactory
    {
        private static readonly string _cs =
            ConfigurationManager.ConnectionStrings["bdMiBus"]?.ConnectionString
            ?? "Data Source=(localdb)\\localhost;Initial Catalog=bdMiBus;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";

        public static ConexionLinqDataContext Create() => new ConexionLinqDataContext(_cs);
    }
}
