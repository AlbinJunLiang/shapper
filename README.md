# Recursos y accesos a los endpoints más relevantes

## Endpoints públicos
- Cualquiera puede ver estos endpoint, ya sea registrado o no.

1-http://localhost:5127/api/Categories/with-price-range
2-http://localhost:5127/api/Products/store?page=1&pageSize=20&featured=true
3-http://localhost:5127/api/Store/code/ST-MANUAL01
4-http://localhost:5127/api/Categories/?page=1&pageSize=8
5-http://localhost:5127/api/Products/filter?minPrice=0&maxPrice=575&pageSize=8
6-http://localhost:5127/api/Products/4
7-http://localhost:5127/api/Faqs/?page=1&pageSize=100
8-http://localhost:5127/api/Orders
9-http://localhost:5127/api/payment/Checkout
10-http://localhost:5127/api/payment/capture-payment?provider=paypal&token=07L70705VJ7452737
11-http://localhost:5127/api/Products/search?term=s&count=10




## Endpoint solo usuario registrado y VERIFICADOS y acceso solo para el el propietario del recurso
1-http://localhost:5127/api/Users POST
2-http://localhost:5127/api/reviews DELETE
3-http://localhost:5127/api/reviews/2 PUT
4-http://localhost:5127/api/users/customer/LIANGALBIN9%40GMAIL.COM POST
5-http://localhost:5127/api/users/customer/LIANGALBIN9%40GMAIL.COM PATCH
6-http://localhost:5127/api/Orders/user/1?page=1&pageSize=10
7-http://localhost:5127/api/Orders/reference/ORD-20260428-ED915593

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