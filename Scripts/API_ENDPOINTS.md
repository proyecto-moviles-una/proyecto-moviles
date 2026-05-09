# Documentación de Endpoints de la API

> **Base URL:** `http://localhost:{puerto}/api`  
> Los endpoints marcados con ?? requieren el header `Authorization: Bearer {jwt_token}` obtenido en el login.

---

## ?? Auth — `/api/auth`

### 1. Registrar usuario
**POST** `/api/auth/registro`

Crea una cuenta nueva. El usuario queda **inactivo** hasta que confirme su correo.

```json
{
  "nombre": "Juan",
  "apellidos": "Pérez López",
  "email": "juan@example.com",
  "password": "MiPassword123"
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

### 2. Activar cuenta
**POST** `/api/auth/activar`

Activa la cuenta usando el código recibido por correo al registrarse.

```json
{
  "correo": "juan@example.com",
  "token": "ABC123"
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

### 3. Login
**POST** `/api/auth/login`

Autentica al usuario y devuelve un JWT para usar en los endpoints protegidos.

```json
{
  "email": "juan@example.com",
  "password": "MiPassword123"
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

> ?? Guardá el `token` y el `guidUsuario` — los necesitarás en todos los endpoints protegidos.

**Errores posibles:** `emailFaltante (3)`, `loginIncorrecto (8)`, `usuarioInactivo (9)`, `usuarioDesactivado (11)`

---

### 4. Logout ??
**POST** `/api/auth/logout`

Cierra la sesión activa del usuario. Requiere JWT.

```
Body: (vacío)
```

**Headers:**
```
Authorization: Bearer eyJhbGci...
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

### 5. Reenviar código de activación
**POST** `/api/auth/reenviar-activacion`

Reenvía el correo de activación si el usuario no lo recibió o expiró. Solo para cuentas **inactivas**.

```json
{
  "correo": "juan@example.com"
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

### 6. Solicitar reactivación
**POST** `/api/auth/solicitar-reactivacion`

Para cuentas **desactivadas** (estado 2). Genera un código y lo envía al correo.

```json
{
  "email": "juan@example.com"
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

### 7. Reactivar cuenta
**POST** `/api/auth/reactivar`

Confirma el código de reactivación y vuelve a activar la cuenta (estado 2 ? 1).

```json
{
  "email": "juan@example.com",
  "token": "XYZ789"
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

## ?? Usuarios — `/api/usuarios` ??

> Todos requieren `Authorization: Bearer {token}`.  
> El `{guid}` de la URL **debe coincidir** con el `guidUsuario` del token (ownership check).

### 8. Listar usuarios
**GET** `/api/usuarios/listar`

Devuelve la lista de todos los usuarios registrados.

```
Body: (vacío)
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "usuarios": [
    {
      "guid": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
      "nombre": "Juan",
      "apellidos": "Pérez López",
      "email": "juan@example.com"
    }
  ],
  "error": []
}
```

---

### 9. Ver perfil
**GET** `/api/usuarios/perfil/{guid}`

Devuelve los datos del perfil del usuario con ese GUID.

```
Ejemplo: GET /api/usuarios/perfil/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
Body: (vacío)
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "usuario": {
    "guid": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
    "nombre": "Juan",
    "apellidos": "Pérez López",
    "email": "juan@example.com"
  },
  "error": []
}
```

---

### 10. Actualizar perfil
**PUT** `/api/usuarios/perfil/{guid}`

Actualiza nombre y apellidos del usuario.

```json
{
  "nombre": "Juan Carlos",
  "apellidos": "Pérez Rodríguez"
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

### 11. Eliminar usuario
**DELETE** `/api/usuarios/{guid}`

Elimina permanentemente la cuenta del usuario.

```
Ejemplo: DELETE /api/usuarios/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
Body: (vacío)
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "error": []
}
```

---

### 12. Desactivar usuario
**PUT** `/api/usuarios/{guid}/desactivar`

Desactiva la cuenta (estado activo ? desactivado). No la elimina.

```
Ejemplo: PUT /api/usuarios/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/desactivar
Body: (vacío)
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "error": []
}
```

---

### 13. Cambiar password
**PUT** `/api/usuarios/perfil/{guid}/password`

Cambia la contraseña del usuario. Requiere la contraseña actual para confirmar identidad.

```json
{
  "passwordActual": "MiPassword123",
  "passwordNueva": "NuevoPass456",
  "confirmarPassword": "NuevoPass456"
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

### 14. Solicitar cambio de correo (Paso 1)
**POST** `/api/usuarios/perfil/{guid}/correo/solicitar`

Valida la contraseña actual, guarda el correo pendiente y envía un código de verificación **al nuevo correo**.

```json
{
  "passwordActual": "MiPassword123",
  "correoNuevo": "juan_nuevo@example.com"
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

### 15. Confirmar cambio de correo (Paso 2)
**POST** `/api/usuarios/perfil/{guid}/correo/confirmar`

Valida el código recibido en el nuevo correo y actualiza el email oficialmente.

```json
{
  "codigo": "DEF456"
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

## ? Favoritos — `/api/favoritos` ??

> Todos requieren `Authorization: Bearer {token}`.  
> El `{guidUsuario}` de la URL debe coincidir con el del token.

### 16. Obtener favoritos
**GET** `/api/favoritos/{guidUsuario}`

Lista todas las rutas favoritas del usuario.

```
Ejemplo: GET /api/favoritos/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
Body: (vacío)
```

**Respuesta exitosa:**
```json
{
  "resultado": true,
  "favoritos": [
    {
      "guidFavorito": "yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy",
      "guidRuta": "zzzzzzzz-zzzz-zzzz-zzzz-zzzzzzzzzzzz",
      "nombreRuta": "San José - Alajuela",
      "origen": "Terminal 7-10, San José",
      "destino": "Alajuela Centro",
      "tarifaActual": 730.00,
      "fechaRegistro": "2025-01-15T10:30:00"
    }
  ],
  "error": []
}
```

**Errores posibles:** `guidDeUsuarioFaltante (6)`

---

### 17. Agregar favorito
**POST** `/api/favoritos/{guidUsuario}`

Agrega una ruta a los favoritos del usuario.

```json
{
  "guidRuta": "zzzzzzzz-zzzz-zzzz-zzzz-zzzzzzzzzzzz"
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

### 18. Eliminar favorito
**DELETE** `/api/favoritos/{guidUsuario}/{guidFavorito}`

Elimina un favorito específico del usuario.

```
Ejemplo: DELETE /api/favoritos/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy
Body: (vacío)
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

## ?? Tabla rápida de todos los endpoints

| # | Método | URL | Auth | Descripción |
|---|--------|-----|------|-------------|
| 1 | POST | `/api/auth/registro` | ? | Registrar usuario |
| 2 | POST | `/api/auth/activar` | ? | Activar cuenta con código |
| 3 | POST | `/api/auth/login` | ? | Login ? obtiene JWT |
| 4 | POST | `/api/auth/logout` | ?? | Cerrar sesión |
| 5 | POST | `/api/auth/reenviar-activacion` | ? | Reenviar código de activación |
| 6 | POST | `/api/auth/solicitar-reactivacion` | ? | Solicitar reactivación (cuenta desactivada) |
| 7 | POST | `/api/auth/reactivar` | ? | Confirmar reactivación con código |
| 8 | GET | `/api/usuarios/listar` | ?? | Listar usuarios |
| 9 | GET | `/api/usuarios/perfil/{guid}` | ?? | Ver perfil propio |
| 10 | PUT | `/api/usuarios/perfil/{guid}` | ?? | Actualizar nombre/apellidos |
| 11 | DELETE | `/api/usuarios/{guid}` | ?? | Eliminar cuenta |
| 12 | PUT | `/api/usuarios/{guid}/desactivar` | ?? | Desactivar cuenta |
| 13 | PUT | `/api/usuarios/perfil/{guid}/password` | ?? | Cambiar password |
| 14 | POST | `/api/usuarios/perfil/{guid}/correo/solicitar` | ?? | Solicitar cambio de correo (paso 1) |
| 15 | POST | `/api/usuarios/perfil/{guid}/correo/confirmar` | ?? | Confirmar cambio de correo (paso 2) |
| 16 | GET | `/api/favoritos/{guidUsuario}` | ?? | Listar favoritos |
| 17 | POST | `/api/favoritos/{guidUsuario}` | ?? | Agregar favorito |
| 18 | DELETE | `/api/favoritos/{guidUsuario}/{guidFavorito}` | ?? | Eliminar favorito |
