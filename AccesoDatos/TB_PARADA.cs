using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace AccesoDatos
{
    /// <summary>
    /// Clase de mapeo manual para la tabla TB_PARADA.
    /// 
    /// ¿POR QUÉ SE CREÓ MANUALMENTE EN LUGAR DE ARRASTRAR LA TABLA AL DBML?
    /// 
    /// Problema: La tabla TB_PARADA tiene un campo de tipo GEOGRAPHY (UBICACION) que
    /// LINQ to SQL no soporta nativamente. Al intentar arrastrar la tabla al diseñador
    /// DBML, Visual Studio genera el error:
    /// "Uno o varios elementos seleccionados contienen un tipo de datos que el diseñador no admite"
    /// 
    /// Solución implementada:
    /// 1. Crear esta clase manualmente con TODOS los campos EXCEPTO el campo UBICACION (geography)
    /// 2. Solo mapeamos LATITUD y LONGITUD (double) que son tipos soportados por LINQ to SQL
    /// 3. El campo UBICACION (geography) se sincroniza automáticamente en SQL Server mediante
    ///    un trigger (TR_TB_PARADA_SYNC_UBICACION) que calcula:
    ///    UBICACION = geography::Point(LATITUD, LONGITUD, 4326)
    /// 
    /// Ventajas de esta solución:
    /// - ? Usamos LINQ to SQL normalmente (InsertOnSubmit, SubmitChanges, etc.)
    /// - ? No necesitamos escribir SQL crudo con ExecuteCommand
    /// - ? Mantenemos IntelliSense y type-safety
    /// - ? El campo geography se mantiene sincronizado automáticamente
    /// - ? Código más limpio y mantenible
    /// 
    /// Alternativas descartadas:
    /// - ? Usar vistas + ExecuteCommand: Pierde las ventajas de LINQ to SQL
    /// - ? Cambiar a Entity Framework: Requiere migración completa del proyecto
    /// - ? Usar stored procedures para todo: Más código SQL y menos reutilizable
    /// </summary>
    [Table(Name = "dbo.TB_PARADA")]
    public partial class TB_PARADA : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        // Campos privados para almacenar los valores de las propiedades
        // Nota: El campo PUNTO_GEOGRAFICO (geography) NO está incluido aquí
        // porque LINQ to SQL no lo soporta. Se maneja mediante trigger en SQL Server.
        private int _ID_PARADA;
        private string _GUID_PARADA;
        private string _NOMBRE;
        private string _DESCRIPCION;
        private decimal _LATITUD;    // Cambiado a decimal para coincidir con SQL Server
        private decimal _LONGITUD;   // Cambiado a decimal para coincidir con SQL Server
        private bool _ESTADO;
        private DateTime _FECHA_REGISTRO;

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnID_PARADAChanging(int value);
        partial void OnID_PARADAChanged();
        partial void OnGUID_PARADAChanging(string value);
        partial void OnGUID_PARADAChanged();
        partial void OnNOMBREChanging(string value);
        partial void OnNOMBREChanged();
        partial void OnDESCRIPCIONChanging(string value);
        partial void OnDESCRIPCIONChanged();
        partial void OnLATITUDChanging(decimal value);
        partial void OnLATITUDChanged();
        partial void OnLONGITUDChanging(decimal value);
        partial void OnLONGITUDChanged();
        partial void OnESTADOChanging(bool value);
        partial void OnESTADOChanged();
        partial void OnFECHA_REGISTROChanging(DateTime value);
        partial void OnFECHA_REGISTROChanged();
        #endregion

        public TB_PARADA()
        {
            OnCreated();
        }

        [Column(Storage = "_ID_PARADA", AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ID_PARADA
        {
            get { return this._ID_PARADA; }
            set
            {
                if ((this._ID_PARADA != value))
                {
                    this.OnID_PARADAChanging(value);
                    this.SendPropertyChanging();
                    this._ID_PARADA = value;
                    this.SendPropertyChanged("ID_PARADA");
                    this.OnID_PARADAChanged();
                }
            }
        }

        [Column(Storage = "_GUID_PARADA", DbType = "VarChar(50) NOT NULL", CanBeNull = false)]
        public string GUID_PARADA
        {
            get { return this._GUID_PARADA; }
            set
            {
                if ((this._GUID_PARADA != value))
                {
                    this.OnGUID_PARADAChanging(value);
                    this.SendPropertyChanging();
                    this._GUID_PARADA = value;
                    this.SendPropertyChanged("GUID_PARADA");
                    this.OnGUID_PARADAChanged();
                }
            }
        }

        [Column(Storage = "_NOMBRE", DbType = "NVarChar(100) NOT NULL", CanBeNull = false)]
        public string NOMBRE
        {
            get { return this._NOMBRE; }
            set
            {
                if ((this._NOMBRE != value))
                {
                    this.OnNOMBREChanging(value);
                    this.SendPropertyChanging();
                    this._NOMBRE = value;
                    this.SendPropertyChanged("NOMBRE");
                    this.OnNOMBREChanged();
                }
            }
        }

        [Column(Storage = "_DESCRIPCION", DbType = "NVarChar(500)")]
        public string DESCRIPCION
        {
            get { return this._DESCRIPCION; }
            set
            {
                if ((this._DESCRIPCION != value))
                {
                    this.OnDESCRIPCIONChanging(value);
                    this.SendPropertyChanging();
                    this._DESCRIPCION = value;
                    this.SendPropertyChanged("DESCRIPCION");
                    this.OnDESCRIPCIONChanged();
                }
            }
        }

        /// <summary>
        /// Latitud de la ubicación de la parada (coordenada geográfica).
        /// Rango válido: -90 a 90 grados.
        /// Tipo: DECIMAL para máxima precisión (coincide con SQL Server).
        /// Este valor se usa junto con LONGITUD para calcular automáticamente 
        /// el campo PUNTO_GEOGRAFICO (geography) mediante el trigger en SQL Server.
        /// </summary>
        [Column(Storage = "_LATITUD", DbType = "Decimal(9,6) NOT NULL")]
        public decimal LATITUD
        {
            get { return this._LATITUD; }
            set
            {
                if ((this._LATITUD != value))
                {
                    this.OnLATITUDChanging(value);
                    this.SendPropertyChanging();
                    this._LATITUD = value;
                    this.SendPropertyChanged("LATITUD");
                    this.OnLATITUDChanged();
                }
            }
        }

        /// <summary>
        /// Longitud de la ubicación de la parada (coordenada geográfica).
        /// Rango válido: -180 a 180 grados.
        /// Tipo: DECIMAL para máxima precisión (coincide con SQL Server).
        /// Este valor se usa junto con LATITUD para calcular automáticamente 
        /// el campo PUNTO_GEOGRAFICO (geography) mediante el trigger en SQL Server.
        /// </summary>
        [Column(Storage = "_LONGITUD", DbType = "Decimal(9,6) NOT NULL")]
        public decimal LONGITUD
        {
            get { return this._LONGITUD; }
            set
            {
                if ((this._LONGITUD != value))
                {
                    this.OnLONGITUDChanging(value);
                    this.SendPropertyChanging();
                    this._LONGITUD = value;
                    this.SendPropertyChanged("LONGITUD");
                    this.OnLONGITUDChanged();
                }
            }
        }

        [Column(Storage = "_ESTADO", DbType = "Bit NOT NULL")]
        public bool ESTADO
        {
            get { return this._ESTADO; }
            set
            {
                if ((this._ESTADO != value))
                {
                    this.OnESTADOChanging(value);
                    this.SendPropertyChanging();
                    this._ESTADO = value;
                    this.SendPropertyChanged("ESTADO");
                    this.OnESTADOChanged();
                }
            }
        }

        [Column(Storage = "_FECHA_REGISTRO", DbType = "DateTime NOT NULL")]
        public DateTime FECHA_REGISTRO
        {
            get { return this._FECHA_REGISTRO; }
            set
            {
                if ((this._FECHA_REGISTRO != value))
                {
                    this.OnFECHA_REGISTROChanging(value);
                    this.SendPropertyChanging();
                    this._FECHA_REGISTRO = value;
                    this.SendPropertyChanged("FECHA_REGISTRO");
                    this.OnFECHA_REGISTROChanged();
                }
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SendPropertyChanging()
        {
            if ((this.PropertyChanging != null))
            {
                this.PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
