# ?? ERROR RESUELTO: "Maximum nesting level exceeded"

## ? Error Completo:
```
Mensaje 217, nivel 16, estado 1, procedimiento TR_TB_PARADA_SYNC_PUNTO_GEOGRAFICO, línea 10
Maximum stored procedure, function, trigger, or view nesting level exceeded (limit 32).
```

---

## ?? **Causa del Problema:**

### **Recursividad Infinita**

El trigger tiene un **loop infinito**:

```
1. INSERT/UPDATE en TB_PARADA
   ?
2. Trigger se ejecuta
   ?
3. Trigger hace UPDATE en TB_PARADA (para actualizar PUNTO_GEOGRAFICO)
   ?
4. UPDATE dispara el trigger otra vez
   ?
5. Vuelve al paso 3... INFINITAMENTE ?
   ?
6. SQL Server detiene después de 32 llamadas ? ERROR
```

### **Diagrama del problema:**

```
INSERT ? Trigger ? UPDATE ? Trigger ? UPDATE ? Trigger ? ... (x32) ? ERROR!
```

---

## ? **SOLUCIÓN:**

### **Agregar condición IF UPDATE() para evitar recursividad**

**El trigger solo debe ejecutarse cuando:**
- ? Es un INSERT (nueva parada)
- ? LATITUD o LONGITUD cambiaron
- ? NO cuando solo cambia PUNTO_GEOGRAFICO

### **Código INCORRECTO (con recursividad):**
```sql
CREATE TRIGGER TR_TB_PARADA_SYNC_PUNTO_GEOGRAFICO
ON TB_PARADA
AFTER INSERT, UPDATE
AS
BEGIN
    -- ? PROBLEMA: Se ejecuta SIEMPRE en cualquier UPDATE
    UPDATE TB_PARADA
    SET PUNTO_GEOGRAFICO = geography::Point(i.LATITUD, i.LONGITUD, 4326)
    FROM TB_PARADA p
    INNER JOIN inserted i ON p.GUID_PARADA = i.GUID_PARADA;
END
```

### **Código CORRECTO (sin recursividad):**
```sql
CREATE TRIGGER TR_TB_PARADA_SYNC_PUNTO_GEOGRAFICO
ON TB_PARADA
AFTER INSERT, UPDATE
AS
BEGIN
    -- ? SOLUCIÓN: Solo ejecutar si LATITUD o LONGITUD cambiaron
    IF UPDATE(LATITUD) OR UPDATE(LONGITUD) OR NOT EXISTS(SELECT 1 FROM deleted)
    BEGIN
        UPDATE TB_PARADA
        SET PUNTO_GEOGRAFICO = geography::Point(i.LATITUD, i.LONGITUD, 4326)
        FROM TB_PARADA p
        INNER JOIN inserted i ON p.GUID_PARADA = i.GUID_PARADA
        WHERE i.LATITUD IS NOT NULL AND i.LONGITUD IS NOT NULL;
    END
END
```

---

## ?? **Explicación de la condición:**

```sql
IF UPDATE(LATITUD) OR UPDATE(LONGITUD) OR NOT EXISTS(SELECT 1 FROM deleted)
```

| Parte | Significado | Cuándo es TRUE |
|-------|-------------|----------------|
| `UPDATE(LATITUD)` | ¿La columna LATITUD cambió? | Al hacer UPDATE de LATITUD |
| `UPDATE(LONGITUD)` | ¿La columna LONGITUD cambió? | Al hacer UPDATE de LONGITUD |
| `NOT EXISTS(SELECT 1 FROM deleted)` | ¿Es un INSERT? | No hay fila "anterior" = es nuevo registro |

**Resultado:**
- ? INSERT nuevo ? Se ejecuta (no existe fila anterior)
- ? UPDATE LATITUD ? Se ejecuta
- ? UPDATE LONGITUD ? Se ejecuta
- ? UPDATE solo NOMBRE ? NO se ejecuta
- ? UPDATE solo PUNTO_GEOGRAFICO ? NO se ejecuta (evita loop)

---

## ?? **EJECUTA EL SCRIPT CORREGIDO:**

### **Archivo:** `SQL\CORRECCION_Trigger_Sin_Recursividad.sql`

Este script:
1. ? Elimina el trigger con error
2. ? Crea el trigger corregido (sin recursividad)
3. ? Actualiza datos existentes
4. ? Hace una prueba automática

---

## ?? **Cómo Verificar que Funciona:**

Después de ejecutar el script, prueba:

### **Prueba 1: INSERT**
```sql
INSERT INTO TB_PARADA (GUID_PARADA, NOMBRE, DESCRIPCION, LATITUD, LONGITUD, ESTADO, FECHA_REGISTRO)
VALUES (NEWID(), 'Parada Prueba', 'Test', 9.9281, -84.0907, 1, GETDATE());

-- Verificar
SELECT TOP 1 NOMBRE, LATITUD, LONGITUD, 
       PUNTO_GEOGRAFICO.Lat, PUNTO_GEOGRAFICO.Long
FROM TB_PARADA
ORDER BY FECHA_REGISTRO DESC;
```

**Resultado esperado:**
```
NOMBRE          LATITUD   LONGITUD   Lat      Long
Parada Prueba   9.9281    -84.0907   9.9281   -84.0907  ?
```

### **Prueba 2: UPDATE coordenadas**
```sql
UPDATE TB_PARADA
SET LATITUD = 10.0000, LONGITUD = -85.0000
WHERE NOMBRE = 'Parada Prueba';

-- Verificar que PUNTO_GEOGRAFICO se actualizó
SELECT NOMBRE, PUNTO_GEOGRAFICO.Lat, PUNTO_GEOGRAFICO.Long
FROM TB_PARADA
WHERE NOMBRE = 'Parada Prueba';
```

**Resultado esperado:**
```
NOMBRE          Lat      Long
Parada Prueba   10.0000  -85.0000  ?
```

### **Prueba 3: UPDATE otro campo (NO debe disparar trigger)**
```sql
UPDATE TB_PARADA
SET DESCRIPCION = 'Nueva descripción'
WHERE NOMBRE = 'Parada Prueba';

-- No debe haber error de recursividad ?
```

---

## ?? **Resumen:**

| Aspecto | Antes (ERROR) | Ahora (CORRECTO) |
|---------|---------------|------------------|
| **Recursividad** | ? Loop infinito | ? Controlada |
| **INSERT** | ? Funciona | ? Funciona |
| **UPDATE LATITUD/LONGITUD** | ? Loop infinito | ? Funciona |
| **UPDATE otros campos** | ? Loop infinito | ? No dispara trigger |
| **Rendimiento** | ? Muy malo | ? Óptimo |

---

## ?? **Siguiente Paso:**

**Ejecuta:** `SQL\CORRECCION_Trigger_Sin_Recursividad.sql`

Verás mensajes como:
```
? Trigger anterior eliminado
? Trigger TR_TB_PARADA_SYNC_PUNTO_GEOGRAFICO creado correctamente (SIN recursividad)
? Parada insertada
? OK - Trigger funcionó
```

**Después de eso, tu código C# funcionará perfectamente sin errores!** ??

---

## ?? **Lección Aprendida:**

Cuando un trigger hace UPDATE en la **misma tabla** donde está definido, siempre:
1. ? Agregar condición `IF UPDATE(columna)`
2. ? Verificar que no cause recursividad
3. ? Probar con diferentes tipos de operaciones

**Tu aplicación ahora está lista para usarse!** ??
