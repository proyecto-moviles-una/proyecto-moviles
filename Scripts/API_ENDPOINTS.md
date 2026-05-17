# DocumentaciÃ³n de Endpoints de la API

> **Base URL:** `http://localhost:{puerto}`
> Los endpoints marcados con ðŸ”’ requieren el header `Authorization: Bearer {jwt_token}` obtenido en el login.
> **Correo de prueba:** `nayelijironcastellon@gmail.com`
>
> **Orden recomendado para pruebas:** Sigue los bloques del 1 al 7 en orden.
> Necesitas el **JWT** del login (paso 3) para los bloques 2 y 6.
> Los **GUIDs** de empresa, zona, parada y ruta los obtienes al crearlos en los bloques 3 y 4.

---

## ðŸ” BLOQUE 1 â€” AutenticaciÃ³n (sin token, probar primero)

### 1. Registrar usuario
**POST** `/api/auth/registro`

Crea una cuenta nueva. El usuario queda **inactivo** hasta que confirme su correo.

```json
{
  "nombre": "Nayeli",
  "apellidos": "Jiron Castellon",
  "email": "nayelijironcastellon@gmail.com",
  "password": "Test1234!"
}
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "error": []
}
```

**Errores posibles:** `nombreFaltante (1)`, `apellidosFaltante (2)`, `emailFaltante (3)`, `emailInvalido (4)`, `passwordVacio (5)`, `correoYaRegistrado (10)`

---

### 2. Reenviar cÃ³digo de activaciÃ³n *(si no llegÃ³ el correo)*
**POST** `/api/auth/reenviar-activacion`

ReenvÃ­a el correo de activaciÃ³n si el usuario no lo recibiÃ³ o expirÃ³. Solo para cuentas **inactivas**.

```json
{
  "correo": "nayelijironcastellon@gmail.com"
}
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "error": []
}
```

**Errores posibles:** `emailFaltante (3)`, `correoNoRegistrado (14)`, `cuentaYaActiva (13)`

---

### 3. Activar cuenta *(con el cÃ³digo que llegÃ³ al correo)*
**POST** `/api/auth/activar`

Activa la cuenta usando el cÃ³digo recibido por correo al registrarse.

```json
{
  "correo": "nayelijironcastellon@gmail.com",
  "token": "CODIGO_DEL_CORREO"
}
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "error": []
}
```

**Errores posibles:** `emailFaltante (3)`, `guidDeUsuarioFaltante (6)`, `errorActivandoUsuario (7)`, `correoNoRegistrado (14)`, `cuentaYaActiva (13)`, `codigoExpirado (12)`

---

### 4. Login *(guarda el token JWT y guidUsuario que devuelve)*
**POST** `/api/auth/login`

Autentica al usuario y devuelve un JWT para usar en los endpoints protegidos.

```json
{
  "email": "nayelijironcastellon@gmail.com",
  "password": "Test1234!"
}
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "token": "eyJhbGci...",
  "guidUsuario": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "error": []
}
```

> âœ… Guarda el `token` y el `guidUsuario` â€” los necesitarÃ¡s en todos los endpoints protegidos.

**Errores posibles:** `emailFaltante (3)`, `loginIncorrecto (8)`, `usuarioInactivo (9)`, `usuarioDesactivado (11)`

---

## ðŸ‘¤ BLOQUE 2 â€” Usuarios ðŸ”’
> Todos requieren `Authorization: Bearer {token}`.
> El `{guid}` de la URL **debe coincidir** con el `guidUsuario` del token.

### 5. Listar usuarios
**GET** `/api/usuarios/listar`

Devuelve la lista de todos los usuarios registrados.

```
Body: (vacÃ­o)
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "usuarios": [
    {
      "guid": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
      "nombre": "Nayeli",
      "apellidos": "Jiron Castellon",
      "email": "nayelijironcastellon@gmail.com"
    }
  ],
  "error": []
}
```

---

### 6. Ver perfil propio
**GET** `/api/usuarios/perfil/{guid}`

```
Ejemplo: GET /api/usuarios/perfil/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
Body: (vacÃ­o)
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "usuario": {
    "guid": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
    "nombre": "Nayeli",
    "apellidos": "Jiron Castellon",
    "email": "nayelijironcastellon@gmail.com"
  },
  "error": []
}
```

---

### 7. Actualizar perfil
**PUT** `/api/usuarios/perfil/{guid}`

```json
{
  "nombre": "Nayeli",
  "apellidos": "Jiron Castellon Editado"
}
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "error": []
}
```

**Errores posibles:** `nombreFaltante (1)`, `apellidosFaltante (2)`

---

### 8. Cambiar password
**PUT** `/api/usuarios/perfil/{guid}/password`

```json
{
  "passwordActual": "Test1234!",
  "passwordNueva": "NuevoPass456!",
  "confirmarPassword": "NuevoPass456!"
}
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "error": []
}
```

**Errores posibles:** `guidDeUsuarioFaltante (6)`, `passwordActualIncorrecto (40)`, `passwordNuevoVacio (41)`, `passwordsNoCoinciden (42)`

---

### 9. Solicitar cambio de correo â€” Paso 1
**POST** `/api/usuarios/perfil/{guid}/correo/solicitar`

Valida la contraseÃ±a actual, guarda el correo pendiente y envÃ­a un cÃ³digo de verificaciÃ³n **al nuevo correo**.

```json
{
  "passwordActual": "Test1234!",
  "correoNuevo": "nayeli_nuevo@example.com"
}
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "error": []
}
```

**Errores posibles:** `passwordVacio (5)`, `correoNuevoFaltante (43)`, `emailInvalido (4)`, `passwordActualIncorrecto (40)`, `correoYaRegistrado (10)`

---

### 10. Confirmar cambio de correo â€” Paso 2
**POST** `/api/usuarios/perfil/{guid}/correo/confirmar`

Valida el cÃ³digo recibido en el nuevo correo y actualiza el email oficialmente.

```json
{
  "codigo": "CODIGO_DEL_CORREO"
}
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "error": []
}
```

**Errores posibles:** `codigoVerificacionFaltante (44)`, `codigoVerificacionInvalido (45)`, `codigoExpirado (12)`, `sinSolicitudCambioCorreo (46)`

---

### 11. Logout ðŸ”’
**POST** `/api/auth/logout`

Cierra la sesiÃ³n activa del usuario.

```
Body: (vacÃ­o)
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "error": []
}
```

**Errores posibles:** `guidSesionFaltante (20)`, `sesionInvalida (21)`, `sesionYaCerrada (23)`

---

### 12. Desactivar cuenta
**PUT** `/api/usuarios/{guid}/desactivar`

```
Ejemplo: PUT /api/usuarios/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/desactivar
Body: (vacÃ­o)
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "error": []
}
```

---

### 13. Solicitar reactivaciÃ³n *(cuenta desactivada, sin token)*
**POST** `/api/auth/solicitar-reactivacion`

Para cuentas **desactivadas** (estado 2). Genera un cÃ³digo y lo envÃ­a al correo.

```json
{
  "email": "nayelijironcastellon@gmail.com"
}
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "error": []
}
```

**Errores posibles:** `emailFaltante (3)`, `correoNoRegistrado (14)`, `usuarioNoDesactivado (47)`

---

### 14. Reactivar cuenta *(con cÃ³digo del correo)*
**POST** `/api/auth/reactivar`

Confirma el cÃ³digo de reactivaciÃ³n y vuelve a activar la cuenta (estado 2 â†’ 1).

```json
{
  "email": "nayelijironcastellon@gmail.com",
  "token": "CODIGO_DEL_CORREO"
}
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "error": []
}
```

**Errores posibles:** `emailFaltante (3)`, `correoNoRegistrado (14)`, `codigoExpirado (12)`, `usuarioNoDesactivado (47)`

---

### 15. Eliminar usuario
**DELETE** `/api/usuarios/{guid}`

Elimina permanentemente la cuenta del usuario.

```
Ejemplo: DELETE /api/usuarios/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
Body: (vacÃ­o)
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "error": []
}
```

---

## ðŸ¢ BLOQUE 3 â€” Empresa y Zona *(sin token)*

### 16. Crear empresa
**POST** `/api/empresa/crear`

```json
{
  "nombre": "Buses Nayeli",
  "telefono": "88001234",
  "correo": "nayelijironcastellon@gmail.com"
}
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "guidEmpresa": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
  "error": []
}
```

> âœ… Guarda el `guidEmpresa`.

---

### 17. Listar empresas
**GET** `/api/empresa/listar`

```
Body: (vacÃ­o)
```

---

### 18. Crear zona
**POST** `/api/zona/crear`

```json
{
  "nombre": "Zona Norte",
  "descripcion": "Rutas del norte"
}
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "guidZona": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
  "error": []
}
```

> âœ… Guarda el `guidZona`.

---

### 19. Listar zonas
**GET** `/api/zona/listar`

```
Body: (vacÃ­o)
```

---

## ðŸšŒ BLOQUE 4 â€” Paradas y Rutas

### 20. Crear parada
**POST** `/api/parada/crear`

```json
{
  "nombre": "Parada Central",
  "descripcion": "Frente al parque",
  "latitud": 10.4317,
  "longitud": -84.4322
}
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "guidParada": "cccccccc-cccc-cccc-cccc-cccccccccccc",
  "error": []
}
```

> âœ… Guarda el `guidParada`.

---

### 21. Listar paradas
**GET** `/api/parada/listar`

```
Body: (vacÃ­o)
```

---

### 22. Editar parada
**PUT** `/api/parada/editar/{guidParada}`

```json
{
  "nombre": "Parada Central Editada",
  "descripcion": "Nueva descripciÃ³n",
  "latitud": 10.4317,
  "longitud": -84.4322
}
```

---

### 23. Eliminar parada
**DELETE** `/api/parada/eliminar/{guidParada}`

```
Body: (vacÃ­o)
```

---

### 24. Crear ruta
**POST** `/api/ruta/crear`

```json
{
  "guidEmpresa": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
  "guidZona": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
  "numeroRuta": "1",
  "nombre": "Ruta Nayeli",
  "horaInicio": "06:00:00",
  "horaFin": "22:00:00"
}
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "guidRuta": "dddddddd-dddd-dddd-dddd-dddddddddddd",
  "error": []
}
```

> âœ… Guarda el `guidRuta`.

---

### 25. Listar rutas
**GET** `/api/ruta/listar`

```
Body: (vacÃ­o)
```

---

### 26. Obtener ruta por GUID
**GET** `/api/ruta/obtener/{guidRuta}`

```
Body: (vacÃ­o)
```

---

### 27. Editar ruta
**PUT** `/api/ruta/editar/{guidRuta}`

```json
{
  "guidEmpresa": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
  "guidZona": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
  "numeroRuta": "1",
  "nombre": "Ruta Nayeli Editada",
  "horaInicio": "06:00:00",
  "horaFin": "23:00:00"
}
```

---

### 28. Asociar parada a ruta
**POST** `/api/ruta/asociar-parada/{guidRuta}`

```json
{
  "guidParada": "cccccccc-cccc-cccc-cccc-cccccccccccc",
  "orden": 1
}
```

---

### 29. Eliminar ruta
**DELETE** `/api/ruta/eliminar/{guidRuta}`

```
Body: (vacÃ­o)
```

---

## â° BLOQUE 5 â€” Horarios y Tarifas

### 30. Crear horario
**POST** `/api/horario/crear`

```json
{
  "guidRuta": "dddddddd-dddd-dddd-dddd-dddddddddddd",
  "horaSalida": "07:30:00",
  "diasServicio": "Lunes,Martes,MiÃ©rcoles,Jueves,Viernes"
}
```

---

### 31. Listar horarios por ruta
**GET** `/api/horario/listar/{guidRuta}`

```
Body: (vacÃ­o)
```

---

### 32. Crear tarifa
**POST** `/api/tarifa/crear`

```json
{
  "guidRuta": "dddddddd-dddd-dddd-dddd-dddddddddddd",
  "monto": 500.00,
  "fechaVigencia": "2025-01-01T00:00:00"
}
```

---

## â­ BLOQUE 6 â€” Favoritos ðŸ”’
> Todos requieren `Authorization: Bearer {token}`.
> El `{guidUsuario}` de la URL debe coincidir con el del token.

### 33. Obtener favoritos
**GET** `/api/favoritos/{guidUsuario}`

```
Ejemplo: GET /api/favoritos/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
Body: (vacÃ­o)
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "favoritos": [
    {
      "guidFavorito": "yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy",
      "guidRuta": "dddddddd-dddd-dddd-dddd-dddddddddddd",
      "nombreRuta": "Ruta Nayeli"
    }
  ],
  "error": []
}
```

**Errores posibles:** `guidDeUsuarioFaltante (6)`

---

### 34. Agregar favorito
**POST** `/api/favoritos/{guidUsuario}`

```json
{
  "guidRuta": "dddddddd-dddd-dddd-dddd-dddddddddddd"
}
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "error": []
}
```

**Errores posibles:** `guidRutaFaltante (30)`, `favoritoYaExiste (31)`

---

### 35. Eliminar favorito
**DELETE** `/api/favoritos/{guidUsuario}/{guidFavorito}`

```
Ejemplo: DELETE /api/favoritos/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy
Body: (vacÃ­o)
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "error": []
}
```

**Errores posibles:** `guidFavoritoFaltante (33)`, `favoritoNoExiste (32)`

---

## ðŸ“œ BLOQUE 7 â€” Historial

### 36. Registrar en historial
**POST** `/api/historial/registrar`

```json
{
  "guidUsuario": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "guidRuta": "dddddddd-dddd-dddd-dddd-dddddddddddd"
}
```

---

### 37. Ver historial del usuario
**GET** `/api/historial/usuario/{guidUsuario}`

```
Ejemplo: GET /api/historial/usuario/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
Body: (vacÃ­o)
```

---

## ðŸ“‹ Tabla rÃ¡pida de todos los endpoints

| # | MÃ©todo | URL | Auth | DescripciÃ³n |
|---|--------|-----|------|-------------|
| 1 | POST | `/api/auth/registro` | âŒ | Registrar usuario |
| 2 | POST | `/api/auth/reenviar-activacion` | âŒ | Reenviar cÃ³digo de activaciÃ³n |
| 3 | POST | `/api/auth/activar` | âŒ | Activar cuenta con cÃ³digo |
| 4 | POST | `/api/auth/login` | âŒ | Login â†’ obtiene JWT |
| 5 | GET  | `/api/usuarios/listar` | ðŸ”’ | Listar usuarios |
| 6 | GET  | `/api/usuarios/perfil/{guid}` | ðŸ”’ | Ver perfil propio |
| 7 | PUT  | `/api/usuarios/perfil/{guid}` | ðŸ”’ | Actualizar nombre/apellidos |
| 8 | PUT  | `/api/usuarios/perfil/{guid}/password` | ðŸ”’ | Cambiar password |
| 9 | POST | `/api/usuarios/perfil/{guid}/correo/solicitar` | ðŸ”’ | Solicitar cambio de correo (paso 1) |
| 10 | POST | `/api/usuarios/perfil/{guid}/correo/confirmar` | ðŸ”’ | Confirmar cambio de correo (paso 2) |
| 11 | POST | `/api/auth/logout` | ðŸ”’ | Cerrar sesiÃ³n |
| 12 | PUT  | `/api/usuarios/{guid}/desactivar` | ðŸ”’ | Desactivar cuenta |
| 13 | POST | `/api/auth/solicitar-reactivacion` | âŒ | Solicitar reactivaciÃ³n (cuenta desactivada) |
| 14 | POST | `/api/auth/reactivar` | âŒ | Confirmar reactivaciÃ³n con cÃ³digo |
| 15 | DELETE | `/api/usuarios/{guid}` | ðŸ”’ | Eliminar cuenta |
| 16 | POST | `/api/empresa/crear` | âŒ | Crear empresa |
| 17 | GET  | `/api/empresa/listar` | âŒ | Listar empresas |
| 18 | POST | `/api/zona/crear` | âŒ | Crear zona |
| 19 | GET  | `/api/zona/listar` | âŒ | Listar zonas |
| 20 | POST | `/api/parada/crear` | âŒ | Crear parada |
| 21 | GET  | `/api/parada/listar` | âŒ | Listar paradas |
| 22 | PUT  | `/api/parada/editar/{guid}` | âŒ | Editar parada |
| 23 | DELETE | `/api/parada/eliminar/{guid}` | âŒ | Eliminar parada |
| 24 | POST | `/api/ruta/crear` | âŒ | Crear ruta |
| 25 | GET  | `/api/ruta/listar` | âŒ | Listar rutas |
| 26 | GET  | `/api/ruta/obtener/{guid}` | âŒ | Obtener ruta por GUID |
| 27 | PUT  | `/api/ruta/editar/{guid}` | âŒ | Editar ruta |
| 28 | POST | `/api/ruta/asociar-parada/{guidRuta}` | âŒ | Asociar parada a ruta |
| 29 | DELETE | `/api/ruta/eliminar/{guid}` | âŒ | Eliminar ruta |
| 30 | POST | `/api/horario/crear` | âŒ | Crear horario |
| 31 | GET  | `/api/horario/listar/{guidRuta}` | âŒ | Listar horarios por ruta |
| 32 | POST | `/api/tarifa/crear` | âŒ | Crear tarifa |
| 33 | GET  | `/api/favoritos/{guidUsuario}` | ðŸ”’ | Listar favoritos |
| 34 | POST | `/api/favoritos/{guidUsuario}` | ðŸ”’ | Agregar favorito |
| 35 | DELETE | `/api/favoritos/{guidUsuario}/{guidFavorito}` | ðŸ”’ | Eliminar favorito |
| 36 | POST | `/api/historial/registrar` | âŒ | Registrar en historial |
