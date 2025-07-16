# Kiosko Microservice

Un microservicio desarrollado con .NET 9 que proporciona integración completa con la API de MercadoLibre, incluyendo autenticación OAuth 2.0 y gestión de productos de usuarios.

## 🚀 Características

- **Framework**: .NET 9 (Core)
- **Arquitectura**: Microservicio REST API
- **Integración**: MercadoLibre API con OAuth 2.0 + PKCE
- **Autenticación**: Flujo completo de autorización OAuth 2.0
- **Multiplataforma**: Funciona en Windows, Linux y macOS
- **Dockerizable**: Listo para contenedores
- **Deploy**: Desplegado en Railway
- **Calidad**: Pre-commit hooks para validación de build

## 📋 Prerrequisitos

- .NET 9 SDK
- Git

### Instalación de .NET 9

#### macOS (con Homebrew):
```bash
brew install dotnet
export DOTNET_ROOT="/opt/homebrew/opt/dotnet/libexec"
```

#### Linux (Ubuntu/Debian):
```bash
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-9.0
```

#### Windows:
Descarga desde [dotnet.microsoft.com](https://dotnet.microsoft.com/download)

## 🛠️ Instalación

1. Clona el repositorio:
```bash
git clone git@github.com:gtala/kiosko.git
cd kiosko
```

2. Restaura las dependencias:
```bash
dotnet restore KioskoMicroservice
```

## 🏃‍♂️ Ejecución

### Usando Makefile (recomendado):
```bash
make run
```

### Usando dotnet directamente:
```bash
dotnet run --project KioskoMicroservice
```

### Usando Visual Studio Code:
1. Abre la carpeta del proyecto en VS Code
2. Presiona `F5` o usa la pestaña "Run and Debug"

## 🌐 Endpoints Disponibles

### Health Check
- **URL**: `GET /health`
- **Descripción**: Verifica el estado del servicio
- **Respuesta**:
```json
{
  "status": "OK",
  "message": "Servicio funcionando",
  "timestamp": "2024-01-15T..."
}
```

## 🛒 Integración MercadoLibre

### Autenticación OAuth 2.0

El microservicio implementa el flujo completo de OAuth 2.0 con PKCE (Proof Key for Code Exchange) para máxima seguridad.

#### 1. Iniciar Autenticación
- **URL**: `GET /api/mercadolibre/auth`
- **Descripción**: Inicia el flujo de autenticación OAuth 2.0
- **Respuesta**:
```json
{
  "authUrl": "https://auth.mercadolibre.com.ar/authorization?response_type=code&client_id=TU_CLIENT_ID&scope=read&code_challenge=...&code_challenge_method=S256"
}
```

#### 2. Callback de Autenticación
- **URL**: `GET /api/mercadolibre/callback?code={authorization_code}`
- **Descripción**: Intercambia el código de autorización por un token de acceso
- **Respuesta**:
```json
{
  "success": true,
  "accessToken": "APP_USR-TU_CLIENT_ID-...",
  "userId": 12345678,
  "message": "Autenticación exitosa"
}
```

### Datos de Usuario

#### Información del Usuario
- **URL**: `GET /api/mercadolibre/user/with-token?accessToken={token}`
- **Descripción**: Obtiene información del usuario autenticado
- **Respuesta**:
```json
{
  "id": 12345678,
  "nickname": "usuario_ejemplo",
  "first_name": "Juan",
  "last_name": "Pérez",
  "email": "juan.perez@example.com",
  "country_id": "AR",
  "site_id": "MLA"
}
```

#### Productos del Usuario
- **URL**: `GET /api/mercadolibre/products/with-token?accessToken={token}`
- **Descripción**: Obtiene todos los productos del usuario autenticado
- **Respuesta**:
```json
{
  "userId": "12345678",
  "products": [
    {
      "id": "MLA123456789",
      "title": "Producto de ejemplo",
      "price": 1000,
      "currency": "ARS",
      "condition": "new",
      "category": "MLA123456",
      "pictures": [
        "https://http2.mlstatic.com/D_123456-MLA123456789_123456-O.jpg"
      ],
      "permalink": "https://articulo.mercadolibre.com.ar/MLA-123456789-producto-de-ejemplo-_JM"
    }
  ],
  "totalProducts": 1
}
```

### Endpoints de Prueba

#### Test de Token
- **URL**: `GET /api/mercadolibre/test-token?accessToken={token}`
- **Descripción**: Prueba la validez del token de acceso

#### Test de Productos
- **URL**: `GET /api/mercadolibre/test-products?accessToken={token}`
- **Descripción**: Prueba la obtención básica de productos

## 🔧 Configuración

### Variables de Entorno

El microservicio requiere las siguientes variables de entorno:

```json
{
  "MercadoLibre": {
    "ClientId": "TU_CLIENT_ID",
    "ClientSecret": "TU_CLIENT_SECRET",
    "RedirectUri": "https://tu-dominio.com/api/mercadolibre/callback",
    "AuthUrl": "https://auth.mercadolibre.com.ar/authorization",
    "TokenUrl": "https://api.mercadolibre.com/oauth/token"
  }
}
```

### Configuración en Railway

Para el deploy en Railway, las variables se configuran en el dashboard:
- `MercadoLibre__ClientId`
- `MercadoLibre__ClientSecret`
- `MercadoLibre__RedirectUri`
- `MercadoLibre__AuthUrl`
- `MercadoLibre__TokenUrl`

## 🐳 Dockerización

Para dockerizar el microservicio:

1. Construir la imagen:
```bash
docker build -t kiosko-microservice .
```

2. Ejecutar el contenedor:
```bash
docker run -p 5113:80 kiosko-microservice
```

## 🧪 Testing

Para ejecutar las pruebas:
```bash
dotnet test KioskoMicroservice
```

## 📁 Estructura del Proyecto

```
kiosko/
├── .gitignore          # Archivos excluidos de Git
├── .git/hooks/         # Pre-commit hooks
├── Makefile           # Comandos de automatización
├── README.md          # Este archivo
├── Dockerfile         # Configuración de Docker
├── railway.json       # Configuración de Railway
└── KioskoMicroservice/
    ├── Controllers/   # Controladores de la API
    │   ├── HealthController.cs
    │   └── MercadoLibreController.cs
    ├── Models/        # Modelos de datos
    │   ├── MercadoLibreConfig.cs
    │   └── MercadoLibreModels.cs
    ├── Services/      # Servicios de negocio
    │   ├── IMercadoLibreService.cs
    │   └── MercadoLibreService.cs
    ├── Properties/    # Configuración de lanzamiento
    ├── Program.cs     # Punto de entrada de la aplicación
    ├── appsettings.json # Configuración de la aplicación
    └── KioskoMicroservice.csproj # Archivo de proyecto
```

## 🔒 Seguridad

### OAuth 2.0 con PKCE

El microservicio implementa el flujo de OAuth 2.0 con PKCE para máxima seguridad:

1. **Code Verifier**: Generación de 32 bytes aleatorios
2. **Code Challenge**: SHA256 del verifier
3. **Session Storage**: Almacenamiento seguro del verifier
4. **Token Exchange**: Intercambio seguro del código por token

### Variables de Entorno

- Los secrets se almacenan como variables de entorno
- No se incluyen en el código fuente
- Se configuran en Railway para producción

## 🚀 Deploy

### Railway (Recomendado)

1. Conecta tu repositorio de GitHub a Railway
2. Configura las variables de entorno en el dashboard
3. El deploy se ejecuta automáticamente en cada push

### URL de Producción

- **Health Check**: https://kiosko-production.up.railway.app/health
- **API Base**: https://kiosko-production.up.railway.app/api/mercadolibre/

## 🛠️ Desarrollo

### Pre-commit Hooks

El proyecto incluye un pre-commit hook que:
- ✅ Verifica que el proyecto compile correctamente
- ✅ Previene commits con errores de compilación
- ✅ Muestra mensajes claros de éxito/error

### Agregar nuevos endpoints:

1. Crea un nuevo controlador en `KioskoMicroservice/Controllers/`
2. Sigue el patrón del `MercadoLibreController.cs`
3. Los controladores se registran automáticamente

### Configuración:

- Modifica `appsettings.json` para configuraciones de la aplicación
- Modifica `Properties/launchSettings.json` para configuraciones de desarrollo

## 🤝 Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para detalles.

## 👨‍💻 Autor

**Guillermo Tala** - [gtala](https://github.com/gtala)

---

⭐ Si este proyecto te ayuda, dale una estrella al repositorio! 