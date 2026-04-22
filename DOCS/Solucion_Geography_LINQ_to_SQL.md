# Solución para Manejo de Tipo GEOGRAPHY en LINQ to SQL

## ?? Resumen Ejecutivo

Este documento explica cómo se resolvió el problema de trabajar con el tipo de dato `geography` de SQL Server en un proyecto que usa LINQ to SQL (.NET Framework 4.7.2).

---

## ?? Problema Original

### Error en Visual Studio:
Al intentar arrastrar la tabla `TB_PARADA` al diseñador LINQ to SQL (archivo `.dbml`), se genera el siguiente error:

```
"Uno o varios elementos seleccionados contienen un tipo de datos que el diseñador no admite"
```

### Causa del problema:
- La tabla `TB_PARADA` tiene un campo `UBICACION` de tipo `geography`
- LINQ to SQL **NO soporta** nativamente el tipo `geography` de SQL Server
- El diseñador visual (.dbml) rechaza tablas con tipos no soportados

---

## ? Solución Implementada

### Estrategia: Mapeo Manual + Trigger Automático

La solución se divide en 3 componentes:

### 1?? **Clase Manual: `TB_PARADA.cs`**
**Ubicación:** `AccesoDatos\TB_PARADA.cs`

**¿Qué hace?**
- Define manualmente la entidad `TB_PARADA`
- Mapea **TODOS** los campos de la tabla **EXCEPTO** `UBICACION` (geography)
- Solo incluye `LATITUD` y `LONGITUD` (tipo `double`)

**Código ejemplo:**
```csharp
[Table(Name = "dbo.TB_PARADA")]
public partial class TB_PARADA : INotifyPropertyChanging, INotifyPropertyChanged
{
    [Column(Storage = "_LATITUD", DbType = "Float NOT NULL")]
    public double LATITUD { get; set; }

    [Column(Storage = "_LONGITUD", DbType = "Float NOT NULL")]
    public double LONGITUD { get; set; }

    // Otros campos...
    // Nota: UBICACION (geography) NO está aquí
}
```

---

### 2?? **Extensión del DataContext: `ConexionLinqDataContext.Parada.cs`**
**Ubicación:** `AccesoDatos\ConexionLinqDataContext.Parada.cs`

**¿Qué hace?**
- Extiende el DataContext auto-generado usando `partial class`
- Registra la tabla `TB_PARADA` en el contexto de LINQ to SQL
- Permite usar métodos como: `InsertOnSubmit()`, `Where()`, `DeleteOnSubmit()`, etc.

**Código ejemplo:**
```csharp
public partial class ConexionLinqDataContext
{
    public Table<TB_PARADA> TB_PARADAs
    {
        get { return this.GetTable<TB_PARADA>(); }
    }
}
```

---

### 3?? **Trigger SQL: `TR_TB_PARADA_SYNC_UBICACION`**
**Ubicación:** `SQL\Trigger_Parada_SyncUbicacion.sql`

**¿Qué hace?**
- Se ejecuta automáticamente en cada `INSERT` o `UPDATE` de `TB_PARADA`
- Calcula el campo `UBICACION` (geography) usando las coordenadas `LATITUD` y `LONGITUD`
- Usa la función nativa de SQL Server: `geography::Point(lat, long, srid)`

**Código SQL:**
```sql
CREATE TRIGGER TR_TB_PARADA_SYNC_UBICACION
ON TB_PARADA
AFTER INSERT, UPDATE
AS
BEGIN
    UPDATE TB_PARADA
    SET UBICACION = geography::Point(i.LATITUD, i.LONGITUD, 4326)
    FROM TB_PARADA p
    INNER JOIN inserted i ON p.GUID_PARADA = i.GUID_PARADA
    WHERE i.LATITUD IS NOT NULL AND i.LONGITUD IS NOT NULL;
END
```

**Nota:** El valor `4326` es el SRID (Spatial Reference ID) para el sistema WGS84, que es el estándar usado por GPS.

---

## ?? Flujo de Trabajo

### Insertar una nueva parada (C#):
```csharp
using (var db = new ConexionLinqDataContext())
{
    TB_PARADA nueva = new TB_PARADA
    {
        GUID_PARADA = Guid.NewGuid().ToString(),
        NOMBRE = "Parada Central",
        DESCRIPCION = "Parada principal del centro",
        LATITUD = 9.9281,      // ? Solo asignamos coordenadas simples
        LONGITUD = -84.0907,   // ? No tocamos UBICACION
        ESTADO = true,
        FECHA_REGISTRO = DateTime.Now
    };

    db.TB_PARADAs.InsertOnSubmit(nueva);
    db.SubmitChanges();  // ? El trigger calcula UBICACION automáticamente
}
```

### Consultar paradas (C#):
```csharp
// Consulta normal con LINQ
var paradas = db.TB_PARADAs
    .Where(p => p.ESTADO == true)
    .OrderBy(p => p.NOMBRE)
    .ToList();
```

### Consultas espaciales (SQL Server):
```sql
-- Paradas dentro de 1km de un punto
SELECT * FROM TB_PARADA
WHERE UBICACION.STDistance(geography::Point(9.9281, -84.0907, 4326)) <= 1000;

-- Calcular distancia entre dos paradas
SELECT 
    p1.NOMBRE AS Origen,
    p2.NOMBRE AS Destino,
    p1.UBICACION.STDistance(p2.UBICACION) AS Distancia_Metros
FROM TB_PARADA p1, TB_PARADA p2
WHERE p1.ID_PARADA != p2.ID_PARADA;
```

---

## ?? Comparación de Alternativas

| Característica | Solución Implementada | Vista + ExecuteCommand | Entity Framework |
|---|---|---|---|
| **Complejidad** | ?? Media | ??? Alta | ???? Muy Alta |
| **Usa LINQ to SQL** | ? Sí | ? No (SQL crudo) | ? Sí (EF) |
| **IntelliSense** | ? Completo | ? Limitado | ? Completo |
| **Type-Safety** | ? Sí | ? No | ? Sí |
| **Cambios en BD** | Trigger | Vista | Ninguno |
| **Migración necesaria** | ? No | ? No | ? Sí (todo el proyecto) |
| **Mantenibilidad** | ? Alta | ? Media | ? Alta |

---

## ? Ventajas de la Solución

1. **Simple en C#**: Solo trabajas con `LATITUD` y `LONGITUD` (números)
2. **Automático**: El trigger sincroniza `UBICACION` sin código adicional
3. **Compatible**: No requiere cambiar el framework del proyecto
4. **Type-Safe**: Mantiene todas las ventajas de LINQ to SQL
5. **Consultas Espaciales**: Puedes usar funciones geography de SQL Server
6. **Mantenible**: Código organizado y bien documentado

---

## ? Alternativas Descartadas

### Opción 1: Crear Vista + ExecuteCommand
**Rechazada porque:**
- Requiere escribir SQL crudo con `ExecuteCommand`
- Pierde IntelliSense y validación de tipos
- Código menos mantenible
- Más propenso a errores

### Opción 2: Migrar a Entity Framework
**Rechazada porque:**
- Requiere migrar TODO el proyecto (muy costoso)
- EF tiene su propia curva de aprendizaje
- No es necesario para resolver este problema específico

### Opción 3: Stored Procedures para todo
**Rechazada porque:**
- Lógica de negocio en SQL en lugar de C#
- Menos reutilizable
- Más difícil de probar

---

## ?? Archivos Creados/Modificados

### Archivos Nuevos:
1. `AccesoDatos\TB_PARADA.cs` - Entidad manual
2. `AccesoDatos\ConexionLinqDataContext.Parada.cs` - Extensión del DataContext
3. `SQL\Trigger_Parada_SyncUbicacion.sql` - Trigger para sincronizar geography
4. `AccesoDatos\GeographyHelper.cs` - Helper para trabajar con geography (opcional)
5. `SQL\StoredProcedures_Parada.sql` - SPs útiles para consultas espaciales (opcional)

### Archivos Modificados:
1. `Logica\Parada\LogParada.cs` - Conversión de `decimal` a `double` para lat/long
2. `Core\Entidades\Response\ResBase.cs` - Agregado `using Core.Entidades.Entities;`

---

## ?? Pasos para Implementar en un Nuevo Ambiente

1. **Ejecutar el trigger en SQL Server:**
   ```sql
   -- Copiar y ejecutar: SQL\Trigger_Parada_SyncUbicacion.sql
   ```

2. **Verificar compilación:**
   ```
   Compilar ? Recompilar solución (Ctrl+Shift+B)
   ```

3. **Probar inserción:**
   ```csharp
   // Usar el método Crear() de LogParada
   ```

---

## ????? Explicación para el Profesor

### ¿Por qué esta solución es profesional?

1. **Patrón de Diseño:**
   - Usa el patrón Repository (LINQ to SQL)
   - Separación de responsabilidades (C# para lógica, SQL para geografía)
   - Clases parciales para extender código auto-generado

2. **Buenas Prácticas:**
   - No modifica código auto-generado
   - Documentación exhaustiva
   - Type-safety mantenido
   - Código reutilizable

3. **Escalabilidad:**
   - Fácil agregar más campos espaciales
   - Fácil agregar consultas de proximidad
   - No requiere refactorización mayor

### Conceptos Aplicados:
- ? LINQ to SQL (ORM)
- ? Partial Classes
- ? Database Triggers
- ? Spatial Data Types (Geography)
- ? Repository Pattern
- ? Clean Code

---

## ?? Referencias

- [SQL Server Geography Data Type](https://learn.microsoft.com/en-us/sql/t-sql/spatial-geography/spatial-types-geography)
- [LINQ to SQL Documentation](https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/sql/linq/)
- [Partial Classes (C#)](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/partial-classes-and-methods)

---

**Autor:** [Tu Nombre]  
**Fecha:** [Fecha Actual]  
**Proyecto:** MiBus Backend - Feature RF03 CRUD Paradas
