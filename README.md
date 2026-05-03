# Shapper

<p align="center">
  <img src="https://github.com/AlbinJunLiang/shapper/blob/main/frontend/public/Shapper.png?raw=true" width="800"/>
</p>

<p align="center">
  <a href="https://albinjunliang.github.io/shapper/" target="_blank">
    <img src="https://img.shields.io/badge/Ver%20App-Online-blue?style=for-the-badge">
  </a>
</p>

Aplicación web tipo tienda en línea o comercio electrónico, con carrito de compras, filtros y solicitud de pedidos. Desarrallado con la arquitectura de **N-Layers** y el uso del patrón **Strategy** para el manejo de diferentes proveedores de servicio.

- Pasarelas de pagos por diferentes proveedores como Stripe y Paypal.
- Acceso y registro por provedores como Google (Firebase), Supabase y etc.
- Seguimiento de pedidos y notificación por correo electrónico y SMTP (Brevo).
- Funcionalidades de administración de los recursos.
- Almacenamiento de imágenes de los recursos en un storage (Cloudinary o Local).
- Generación de informes de la orden por PDF para los clientes.

El proyecto está realizado con fines prácticos y se encuentra en proceso de adaptación para ser **Multitenant**.

## Configuraciones necesarias para el despliegue.

### Configuraciones de servicios

#### Configuración del proveedor de autenticación

En el caso de Firebase crear el proyecto, ingresar los dominios autorizados, métodos de acceso y etc.

Firebase:

```JSON
  "Firebase": {
    "CredentialPath": "albinia.json"
  }
```

Supabase:

```JSON
 "Supabase": {
    "JwtSecret": "Hbk"
  }
```

#### Configuración de servicio para envíos de correo

Obtener un servidor SMTP o API para el envío de correo y sus credenciales de acceso. En este caso se usó tanto como SMTP y Servicio de envío de corro por API REST con brevo:

Credenciales de configuración para SMTP

```json
  "SmtpSettings": {
    "SmtpHost": "smtp.zoho.com",
    "SmtpPort": 587,
    "SmtpUser": "user@zohomail.com",
    "SmtpPass": "?????PASSWORD",
    "SenderName": "SMTP ENVÍO"
  }
```

Credenciales de BREVO

```json
  "Brevo": {
    "ApiKey": "xkey...",
    "BrevoEndpoint": "https://api.brevo.com/v3/smtp/email",
    "BrevoEmail": "mybrevouser@mail.com"
  }

```

#### Configuración de servicio de pago

Se utilizó Stripe y Paypal, ambase requiere crear una cuenta para obtener el acceso a los credenciales. Es necesario también configurar los eventos con una URL de mi backend para los webhooks.

PAYPAL

```json
"PayPal": {
    "Api": "https://api-m.sandbox.paypal.com",
    "ClientId": "AVL...",
    "Secret": "EH..."
  }
```

STRIPE

```json
 "Stripe": {
    "SecretKey": "sk_test...",
    "WebhookSecret": "key..."
 }
```

#### Registrar los webhooks en ambas plataformas

Para este paso es importante tener el backend configurado y preparado para su compilación.

Configuración para PAYPAL

```yaml
webhooks:
  paypal:
    url: "api/paypal/webhook"
    events:
      - "PAYMENT.CAPTURE.COMPLETED"

  stripe:
    url: "api/stripe/webhook"
    events:
      - "checkout.session.completed"
```

> [!IMPORTANT]
> Recordar que para stripe ese ncesario configurar en el appSettings.json las credenciales para usar el webhook en la plataforma. WebhookSecret


#### Servicio de almacenamiento de imágenes

Crear una cuenta en cloudinary para acceder las credenciales.

```json
  "ImageStorage": "Cloudinary",
  "Cloudinary": {
    "ImageStorageUrl": "cloudinary://585:???"
  }
```
En caso de que sea local, por defecto al configurar el backend se guarda en ***wwwroot***.

### Configuración de la base de datos

Existen tres maneras de configurar la base de datos para este proyecto:

1.  **Por Scripts SQL:** Ejecutar los archivos con extensión **.sql** incluidos en el proyecto.
2.  **Restauración de Backup:** Restaurar un archivo de respaldo con extensión **.bak** en SQL Server.
3.  **Entity Framework Migrations:** Generar la estructura automáticamente mediante comandos desde la terminal.

---

##### Pasos para configuración con Migrations

Si utilizas el método de **Migrations**, sigue este flujo de trabajo:

1.  **Cadena de Conexión:**
    Configura el *Connection String* dentro del archivo `appsettings.json` con el nombre de tu servidor y base de datos local.

2.  **Crear Migración:**
    Ejecuta el siguiente comando para generar el archivo de migración inicial:
    ```bash
    dotnet ef migrations add InitialCreate
    ```

3.  **Actualizar Base de Datos:**
    Aplica los cambios al servidor para crear las tablas físicamente:
    ```bash
    dotnet ef database update
    ```

4.  **Datos Semilla (Seed Data):**
    Por último, ejecuta el script **Init_shapper.sql** de configuración inicial para poblar la base de datos con los datos necesarios para el funcionamiento del sistema.
---

> [!IMPORTANT]
> Asegúrate de tener instalada la herramienta `dotnet-ef` globalmente antes de ejecutar los comandos anteriores.

### Configuración del backend 

Configurar el archivo **appSettings.json** con las siguientes claves:
 
 Importantes
- El CORS y los orígenes permitidos para el consumo de la API.
- La cadena de conexión.
- Los servicios y sus credenciales
- El correo de administrador.

```JSON
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },

  "ConnectionStrings": {
    "DefaultConnection": "Server=MY_COMPUTER;Database=shapper;encrypt=false;Trusted_Connection=True"
  },

  "AllowedHosts": "*",
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:4200",
      "http://localhost:8080",
      "http://localhost:5127"
    ]
  },

  "AdminSettings": {
    "AdminEmail": "admin@mail.com"
  },

  "Firebase": {
    "CredentialPath": "firebase.json"
  },

  "Stripe": {
    "SecretKey": "sk_test_...",
    "WebhookSecret": "..."
  },
  "PaymentSettings": {
    "AllowedHosts": [
      "http://localhost:4200/success",
      "http://localhost:5127/index.html",
      "http://localhost:5127/cancel.html",
    ]
  },
  "Brevo": {
    "ApiKey": "xkeysib--...",
    "BrevoEndpoint": "https://api.brevo.com/v3/smtp/email",
    "BrevoEmail": "myAccount@email.com"
  },
  "SmtpSettings": {
    "SmtpHost": "smtp.zoho.com",
    "SmtpPort": 587,
    "SmtpUser": "myAccount@email.com",
    "SmtpPass": "",
    "SenderName": "SMTP ENVÍO"
  },

  "PayPal": {
    "Api": "https://api-m.sandbox.paypal.com",
    "ClientId": "",
    "Secret": ""
  },
  "Auth": {
    "Provider": "Firebase"
  },
  "Supabase": {
    "JwtSecret": "Hb..."
  },
  "ImageStorage": "Cloudinary",
  "Cloudinary": {
    "ImageStorageUrl": ""
  }
}

Es nesario también agregar los host pdonde se encuentra la solicitud de pago:
```json
 "PaymentSettings": {
    "AllowedHosts": [
      "http://localhost:4200/success",
      "http://localhost:5127/index.html",
      "http://localhost:5127/cancel.html",
    ]
 }
```

```

Dentro del código:

- La estrategia de verificación de usuario o proveedor `SUPABASE O FIREBASE`, solo se puede agregar como scoped unos de los dos:

```c#
// Solo si es FIREBASE
builder.Services.AddScoped<FirebaseVerificationStrategy>();

// Si es SUPABASE
 builder.Services.AddScoped<SupabaseVerificationStrategy>();
```

Instalar las dependencias
```shell
dotnet restore
```


#### Poner a produción

Ejecutar desde la terminal:

```shell
dotnet publish -c Release -o ./publish
```

Para probar el ddl y el proyecto, ejecutar el siguiente comando:

```shell
dotnet shapper.dll
```

### Configuración del frontend

Al ser un proyecto de Angular se debe ubicar el archivo enviromnet y colocar lo siguiente:

```javascript

export const environment = {
  production: false, // true para producción
  apiUrl: 'http://localhost:5127/api',
  cartStorageKey: 'my_cart_data',
  storeReference: 'ST-MANUAL01',
  currency: '$',
  firebaseConfig: {
    apiKey: "",
    authDomain: "",
    projectId: "",
    storageBucket: "",
    messagingSenderId: "",
    appId: "",
    measurementId: ""
  }
};
```
- apiUrl: servidor donde esta alojado el backend.
- storeReference: Código identificador de la tienda.
- currency: Moneda que se utilizará.
- firebaseConfig: Sería los datos para el cliente del proveedor de autenticación.

Instalar las dependencias
```shell
npm install
```

#### Poner a produción
Desde la terminal y en la ubicación del proyecto ejecutar:

```bash
ng build --configuration=production

# Algunos casos importa mucho que sea con el mismo nombre del proyecto
ng build --configuration production --output-path docs --base-href "/shapper/" 
```

En caso de que tenga node.js instalado y http-server puede probar desde la ubicación de la carpeta public del proyecto con el comando:

```bash
http-server -p 8080 -c-1 --spa
```


Si esta usando github pages, debe crear el archivo **404.html** para que enrute bien:

```html
<script>
  sessionStorage.redirect = location.pathname;
</script>
<meta http-equiv="refresh" content="0;URL='/shapper/'" />
```

Agrega esto al inicio de la etiqueta head en el index.html

```html
<base href="/shapper/">
```
```javascript
<script>
  (function() {
    const redirect = sessionStorage.redirect;
    if (redirect) {
      sessionStorage.removeItem('redirect');
      history.replaceState(null, null, redirect);
    }
  })();
</script>

```

# Recursos y accesos a los endpoints más relevantes

## Endpoints públicos

- Cualquiera puede ver estos endpoint, ya sea registrado o no.

### 🗂️ Categories

GET /api/categories/with-price-range  
GET /api/categories?page=1&pageSize=8

### 🛍️ Products

GET /api/products/store?page=1&pageSize=20&featured=true  
GET /api/products/filter?minPrice=0&maxPrice=575&pageSize=8  
GET /api/products/{id}  
GET /api/products/search?term=s&count=10

### 🏪 Store

GET /api/store/code/{code}

### ❓ FAQs

GET /api/faqs?page=1&pageSize=100

### 📦 Orders

POST /api/orders

### 💳 Payments

POST /api/payment/checkout  
GET /api/payment/capture-payment?provider=paypal&token={token}

## Endpoint solo usuario registrado y VERIFICADOS y acceso solo para el el propietario del recurso

POST /api/users  
POST /api/users/customer/{email}  
PATCH /api/users/customer/{email}

### ⭐ Reviews

DELETE /api/reviews/{id}  
PUT /api/reviews/{id}

### 📦 Orders

GET /api/orders/user/{userId}?page=1&pageSize=10  
GET /api/orders/reference/{reference}

## Solo para el correo del admin

- Aplica para todo el módulo administrador


Dependencias
```json
"@angular/fire": "^20.0.1",
"@angular/forms": "^21.0.0",
"@angular/material": "~21.2.0",
"@angular/platform-browser": "^21.0.0",
"@angular/router": "^21.0.0",
"@ngx-translate/core": "^17.0.0",
"@ngx-translate/http-loader": "^17.0.0",
"firebase": "^12.11.0",
```