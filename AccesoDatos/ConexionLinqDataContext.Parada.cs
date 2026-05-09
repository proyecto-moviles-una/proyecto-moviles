using System.Data.Linq;

namespace AccesoDatos
{
    /// <summary>
    /// Extensión parcial del DataContext generado automáticamente (ConexionLinqDataContext).
    /// 
    /// ¿POR QUÉ CREAR ESTA CLASE PARCIAL?
    /// 
    /// Cuando creamos manualmente la clase TB_PARADA (porque no se puede arrastrar al DBML
    /// debido al tipo geography), también necesitamos registrarla en el DataContext para
    /// que LINQ to SQL pueda usarla.
    /// 
    /// Opciones para agregar una tabla al DataContext:
    /// 
    /// 1. Opción automática (no funciona en este caso):
    ///    - Arrastrar la tabla al diseñador DBML
    ///    - El diseñador genera automáticamente la propiedad Table<T>
    ///    - ? No funciona porque la tabla tiene el campo geography
    /// 
    /// 2. Opción manual (la que usamos aquí):
    ///    - Crear una clase parcial del DataContext
    ///    - Agregar manualmente la propiedad Table<TB_PARADA>
    ///    - ? Funciona perfectamente y mantiene el código organizado
    /// 
    /// ¿Qué hace esta clase?
    /// - Extiende ConexionLinqDataContext (que está en ConexionLinq.designer.cs)
    /// - Agrega la propiedad TB_PARADAs que permite acceder a la tabla
    /// - Permite usar: db.TB_PARADAs.InsertOnSubmit(), db.TB_PARADAs.Where(), etc.
    /// 
    /// Ventajas de usar clase parcial:
    /// - ? No modificamos el código auto-generado (ConexionLinq.designer.cs)
    /// - ? Si regeneramos el DBML, no perdemos esta configuración
    /// - ? Código organizado: cada tabla manual tiene su propio archivo
    /// - ? Sigue el patrón de separación de responsabilidades
    /// 
    /// Nota: El nombre "TB_PARADAs" (plural) es convención de LINQ to SQL para
    /// representar una colección de entidades TB_PARADA.
    /// </summary>
    public partial class ConexionLinqDataContext
    {
        /// <summary>
        /// Tabla TB_PARADA mapeada manualmente.
        /// Usa esta propiedad para realizar operaciones CRUD:
        /// 
        /// Ejemplos de uso:
        /// - Insertar: db.TB_PARADAs.InsertOnSubmit(nueva);
        /// - Consultar: db.TB_PARADAs.Where(p => p.ESTADO == true);
        /// - Eliminar: db.TB_PARADAs.DeleteOnSubmit(parada);
        /// </summary>
        public Table<TB_PARADA> TB_PARADAs
        {
            get
            {
                return this.GetTable<TB_PARADA>();
            }
        }
    }
}
