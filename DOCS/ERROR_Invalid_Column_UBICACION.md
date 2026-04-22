# ?? ERROR RESUELTO: "Invalid column name 'UBICACION'"

## ? Problema:
Al intentar crear el trigger, SQL Server muestra:
```
Mensaje 207, nivel 16, estado 1, procedimiento TR_TB_PARADA_SYNC_UBICACION, línea 9
Invalid column name 'UBICACION'.
```

## ?? Causa:
La columna `UBICACION` (tipo `geography`) **no existe** en la tabla `TB_PARADA`.

---

## ? SOLUCIÓN (OPCIÓN 1 - RECOMENDADA):

### **Ejecutar el script completo en un solo paso:**

1. Abre **SQL Server Management Studio**
2. Conecta a tu base de datos `bdMiBus`
3. Abre el archivo: `SQL\00_Setup_Completo_TB_PARADA.sql`
4. Ejecuta el script completo (F5)

**Este script hace TODO automáticamente:**
- ? Crea la columna `UBICACION`
- ? Crea el trigger
- ? Actualiza datos existentes (si los hay)
- ? Muestra mensajes de confirmación

---

## ? SOLUCIÓN (OPCIÓN 2 - PASO A PASO):

Si prefieres ejecutar paso a paso:

### **Paso 1: Crear la columna UBICACION**
```sql
-- Ejecuta primero: SQL\01_Agregar_Columna_UBICACION.sql
ALTER TABLE dbo.TB_PARADA
ADD UBICACION geography NULL;
```

### **Paso 2: Crear el trigger**
```sql
-- Ejecuta después: SQL\02_Trigger_Parada_SyncUbicacion.sql
CREATE TRIGGER TR_TB_PARADA_SYNC_UBICACION
ON TB_PARADA
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE TB_PARADA
    SET UBICACION = geography::Point(i.LATITUD, i.LONGITUD, 4326)
    FROM TB_PARADA p
    INNER JOIN inserted i ON p.GUID_PARADA = i.GUID_PARADA
    WHERE i.LATITUD IS NOT NULL 
      AND i.LONGITUD IS NOT NULL;
END
GO
```

---

## ?? Verificar que funcionó:

### **1. Verificar que la columna existe:**
```sql
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'TB_PARADA' AND COLUMN_NAME = 'UBICACION';
```

**Resultado esperado:**
```
COLUMN_NAME    DATA_TYPE
-----------    ---------
UBICACION      geography
```

### **2. Verificar que el trigger existe:**
```sql
SELECT name, is_disabled 
FROM sys.triggers 
WHERE name = 'TR_TB_PARADA_SYNC_UBICACION';
```

**Resultado esperado:**
```
name                              is_disabled
--------------------------------  -----------
TR_TB_PARADA_SYNC_UBICACION       0
```

---

## ?? Prueba Manual (OPCIONAL):

Puedes probar que todo funciona insertando una parada de prueba:

```sql
-- Insertar parada de prueba
INSERT INTO TB_PARADA (GUID_PARADA, NOMBRE, DESCRIPCION, LATITUD, LONGITUD, ESTADO, FECHA_REGISTRO)
VALUES (
    NEWID(),
    'Parada Test',
    'Prueba del trigger',
    9.9281,   -- San José, Costa Rica
    -84.0907,
    1,
    GETDATE()
);

-- Verificar que UBICACION se calculó automáticamente
SELECT TOP 1
    NOMBRE,
    LATITUD,
    LONGITUD,
    UBICACION.Lat as Lat_Calculada,
    UBICACION.Long as Long_Calculada,
    CASE WHEN UBICACION IS NOT NULL THEN '? OK' ELSE '? FALLO' END as Estado_Trigger
FROM TB_PARADA
ORDER BY FECHA_REGISTRO DESC;
```

**Resultado esperado:**
```
NOMBRE        LATITUD   LONGITUD   Lat_Calculada  Long_Calculada  Estado_Trigger
----------    --------  ---------  -------------  --------------  --------------
Parada Test   9.9281    -84.0907   9.9281         -84.0907        ? OK
```

---

## ?? Después de ejecutar el script:

Tu código C# ya funcionará sin problemas:

```csharp
TB_PARADA nueva = new TB_PARADA
{
    GUID_PARADA = Guid.NewGuid().ToString(),
    NOMBRE = "Parada Central",
    LATITUD = 9.9281m,
    LONGITUD = -84.0907m,
    ESTADO = true,
    FECHA_REGISTRO = DateTime.Now
};

db.TB_PARADAs.InsertOnSubmit(nueva);
db.SubmitChanges();
// ? El trigger calculará UBICACION automáticamente
```

---

## ?? Archivos SQL creados:

| Archivo | Descripción |
|---------|-------------|
| `SQL\00_Setup_Completo_TB_PARADA.sql` | ? **Script completo (RECOMENDADO)** |
| `SQL\01_Agregar_Columna_UBICACION.sql` | Solo agregar columna |
| `SQL\02_Trigger_Parada_SyncUbicacion.sql` | Solo crear trigger |

---

## ?? Siguiente paso:

**Ejecuta `SQL\00_Setup_Completo_TB_PARADA.sql` y ya estarás listo para usar el API** ?
