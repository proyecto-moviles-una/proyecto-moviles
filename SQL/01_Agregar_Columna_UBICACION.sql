/*
================================================================================
SCRIPT: Agregar columna UBICACION a TB_PARADA
================================================================================

PROPÓSITO:
----------
Agrega el campo UBICACION (tipo geography) a la tabla TB_PARADA si no existe.

NOTA IMPORTANTE:
----------------
Este script debe ejecutarse ANTES del trigger TR_TB_PARADA_SYNC_UBICACION

================================================================================
*/

-- Verificar si la columna ya existe antes de agregarla
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.TB_PARADA') 
    AND name = 'UBICACION'
)
BEGIN
    -- Agregar la columna UBICACION de tipo geography
    ALTER TABLE dbo.TB_PARADA
    ADD UBICACION geography NULL;

    PRINT 'Columna UBICACION agregada exitosamente a TB_PARADA';
END
ELSE
BEGIN
    PRINT 'La columna UBICACION ya existe en TB_PARADA';
END
GO

-- (OPCIONAL) Si ya tienes datos en la tabla, actualizar UBICACION con los datos existentes
-- Descomenta las siguientes líneas si necesitas inicializar datos existentes:

/*
UPDATE TB_PARADA
SET UBICACION = geography::Point(LATITUD, LONGITUD, 4326)
WHERE LATITUD IS NOT NULL 
  AND LONGITUD IS NOT NULL
  AND UBICACION IS NULL;

PRINT 'Datos existentes actualizados con valores de UBICACION';
*/
GO
