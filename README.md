# Shapper

<p align="center">
  <img src="https://github.com/AlbinJunLiang/shapper/blob/main/frontend/public/Shapper.png?raw=true" width="800"/>
</p>

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


# Comando de prueba para el frontend 
http-server -p 8080 -c-1 --spa





















# ShapperFrontend

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 21.0.2.

## Development server

To start a local development server, run:

```bash
ng serve
```

Once the server is running, open your browser and navigate to `http://localhost:4200/`. The application will automatically reload whenever you modify any of the source files.

## Code scaffolding

Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```

## Building

To build the project run:

```bash
ng build
```

This will compile your project and store the build artifacts in the `dist/` directory. By default, the production build optimizes your application for performance and speed.

## Running unit tests

To execute unit tests with the [Vitest](https://vitest.dev/) test runner, use the following command:

```bash
ng test
```

## Running end-to-end tests

For end-to-end (e2e) testing, run:

```bash
ng e2e
```

Angular CLI does not come with an end-to-end testing framework by default. You can choose one that suits your needs.

## Additional Resources

For more information on using the Angular CLI, including detailed command references, visit the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.


   "@angular/fire": "^20.0.1",
    "@angular/forms": "^21.0.0",
    "@angular/material": "~21.2.0",
    "@angular/platform-browser": "^21.0.0",
    "@angular/router": "^21.0.0",
    "@ngx-translate/core": "^17.0.0",
    "@ngx-translate/http-loader": "^17.0.0",
    "firebase": "^12.11.0",