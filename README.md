# StockFlow — Sistema de Gestión de Inventario

Aplicación web para administrar productos y controlar las entradas y salidas de inventario. El proyecto implementa una API REST con autenticación JWT, persistencia en PostgreSQL y una interfaz web desarrollada con Angular y Angular Material.

## 1. Tecnologías utilizadas

### Backend
- C# y .NET 8 Web API.
- Entity Framework Core y Dapper.
- PostgreSQL.
- Procedimientos almacenados para registrar movimientos de inventario.
- Autenticación y autorización mediante JWT.
- Swagger para documentar y probar los endpoints.

### Frontend
- Angular.
- TypeScript.
- Angular Material.
- Formularios reactivos.
- HttpClient para consumir la API REST.

### Infraestructura(Opcional)
- Docker y Docker Compose para ejecutar PostgreSQL.

## 2. Estructura del proyecto

```text
StockFlow/
├── backend/
│   ├── backend.sln
│   ├── StockFlow.Api/
│   ├── StockFlow.Application/
│   ├── StockFlow.Domain/
│   └── StockFlow.Infrastructure/
├── database/
│   ├── 01_schema.sql
│   ├── 02_seed.sql
│   ├── 03_sp_register_inventory_entry.sql
│   ├── 04_sp_register_inventory_exit.sql
│   ├── 05_fn_get_current_inventory.sql
│   ├── 06_seed_admin.sql
│   └── backup
├── frontend/
├── docker-compose.yml
├── README.md
└── evidencias/
```

## 3. Requisitos previos

Instalar las siguientes herramientas:

- Git.
- .NET SDK 8.
- Node.js y npm compatibles con la versión de Angular del proyecto.
- Angular CLI.
- Docker Desktop con Docker Compose.(Opcional)

## 4. Configuración de la base de datos (Opcional)

Desde la raíz del repositorio, iniciar PostgreSQL:

```bash
docker compose up -d
```

La configuración local de desarrollo utiliza los siguientes parámetros:

| Parámetro | Valor |
|---|---|
| Host | `localhost` |
| Puerto | `5432` |
| Base de datos | `stockflow_db` |
| Usuario | `stockflow_user` |
| Contraseña | `StockFlow_Local_2026` |

**Importante:** estos valores corresponden al entorno local de prueba. Para producción, las credenciales deben gestionarse mediante una configuración segura y no almacenarse en el repositorio.

### Crear el esquema y los datos iniciales

Ejecutar los scripts SQL de la carpeta `database/` en el siguiente orden:

1. `01_schema.sql`: crea las tablas y restricciones.
2. `02_seed.sql`: registra los productos iniciales.
3. `03_sp_register_inventory_entry.sql`: crea el procedimiento para registrar entradas.
4. `04_sp_register_inventory_exit.sql`: crea el procedimiento para registrar salidas.
5. `05_fn_get_current_inventory.sql`: crea la función de consulta de inventario actual.
6. `06_seed_admin.sql`: crea el usuario administrador de prueba.

Los scripts deben ejecutarse contra la base de datos `stockflow_db`.

## 5. Configuración del secreto JWT

La API necesita un secreto para firmar y validar los tokens JWT. Por seguridad, el secreto no se guardar en `appsettings.json`.

Para el entorno local se utiliza **.NET User Secrets**, que almacena la configuración fuera del repositorio.

### Paso 1. Entrar al proyecto de la API

Desde la raíz del repositorio:

```bash
cd backend/StockFlow.Api
```

### Paso 2. Inicializar User Secrets

Ejecutar una sola vez por proyecto:

```bash
dotnet user-secrets init
```

Si el proyecto ya tiene configurado `UserSecretsId`, este paso no es necesario.

### Paso 3. Generar y guardar el secreto

Ejecutar:

```bash
dotnet user-secrets set "Jwt:Secret" "$(openssl rand -base64 48)"
```

Este comando genera un valor aleatorio y lo guarda bajo la clave de configuración `Jwt:Secret`.

No es necesario copiar el secreto al código fuente ni compartirlo con otros desarrolladores.

> **Nota:** la clave `Jwt:Secret` debe coincidir con la que utiliza la API al crear y validar los tokens. Si el código utiliza otra ruta de configuración, hay que usar esa misma clave en el comando anterior. El valor también debe cumplir los requisitos de longitud del algoritmo de firma configurado.

### Paso 4. Verificar la configuración

Se puede comprobar que la clave existe ejecutando:

```bash
dotnet user-secrets list
```

Este comando muestra los nombres y valores de los secretos. No se debe compartir su salida si contiene información sensible.

### Para otros desarrolladores o evaluadores

Cada persona debe generar su propio secreto localmente siguiendo los pasos anteriores. Los secretos de User Secrets no se incluyen en el repositorio.

En un entorno de producción, se recomienda utilizar un gestor de secretos o un mecanismo seguro de configuración del entorno.

## 6. Ejecutar el backend

Desde la raíz del repositorio:

```bash
cd backend
dotnet restore
dotnet build backend.sln
dotnet run --project StockFlow.Api
```

La API está configurada para ejecutarse en:

- HTTP: `http://localhost:5054`
- HTTPS: `https://localhost:7239`

Swagger está disponible en la ruta configurada por el proyecto, normalmente:

`http://localhost:5054/swagger`


## 7. Ejecutar el frontend

En una segunda terminal, desde la raíz del repositorio:

```bash
cd frontend
npm install
npm start // ng serve
```

Abrir en el navegador:

`http://localhost:4200`

El frontend consume la API local configurada en los servicios de Angular.

## 8. Credenciales de prueba

La base de datos incluye un usuario administrador para validar el inicio de sesión.

| Campo | Valor |
|---|---|
| Usuario | `administrador` |
| Contraseña | `123prueba` |

Estas credenciales son exclusivamente para la prueba técnica. No deben utilizarse en producción.

## 9. Funcionalidades

- Inicio de sesión y autenticación mediante JWT.
- Almacenamiento local del token y su fecha de expiración.
- Consulta del inventario actual.
- Registro de entradas de productos.
- Registro de salidas de productos.
- Actualización del stock después de registrar movimientos.
- Validación de existencias para evitar salidas superiores al inventario disponible.
- Manejo de errores en la API y en la interfaz.
- Cierre de sesión.
- Persistencia de los movimientos en PostgreSQL.

## 10. Endpoints principales

| Método | Endpoint | Descripción |
|---|---|---|
| `POST` | `/api/Auth/login` | Autenticación de usuario. |
| `GET` | `/api/Inventory` | Consulta del inventario actual. |
| `POST` | `/api/Inventory/entries` | Registra una entrada de inventario. |
| `POST` | `/api/Inventory/exits` | Registra una salida de inventario. |

Los endpoints de inventario requieren un token JWT válido en el encabezado:

```http
Authorization: Bearer <token>
```

Ejemplo del cuerpo para registrar una entrada o salida:

```json
{
  "productId": 1,
  "quantity": 5
}
```

## 11. Respaldo de la base de datos

Los scripts de `database/` permiten reconstruir el esquema y cargar los datos iniciales.

Se incluye un respaldo PostgreSQL en formato (`.dump`), este representa una instantánea de la base de datos en el momento en que se generó. Para restaurarlo, se requiere PostgreSQL y el cliente `pg_restore`.

Los scripts SQL son la referencia para reproducir la estructura de la base de datos desde cero.

## 12. Evidencias de funcionamiento

La carpeta `evidencias/` contiene capturas de pantalla que demuestran el funcionamiento de la aplicación.

Las evidencias incluyen:

1. **Inicio de sesión** 
2. **Validaciones en inicio de sesión** 
3. **Registro de entradas y salidas de prductos** 
4. **Inventario actual** 
5. **Registro de moovimiento**
. **Movimientos en base de datos**

Las capturas permiten verificar visualmente las principales funcionalidades implementadas.


Juan Sebastian Benavides Scarpetta
Ingeniero de Sistemas