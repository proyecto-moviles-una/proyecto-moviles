namespace AccesoDatos
{
    public partial class ConexionLinqDataContext
    {
        public ConexionLinqDataContext() :
            base(global::AccesoDatos.Properties.Settings.Default.bdMiBusConnectionString1, mappingSource)
        {
            OnCreated();
        }
    }
}
