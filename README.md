# Shapper

<p align="center">
  <img src="https://github.com/AlbinJunLiang/shapper/blob/main/frontend/public/Shapper.png?raw=true" width="800"/>
</p>

<p align="center">
  <a href="https://albinjunliang.github.io/shapper/" target="_blank">
    <img src="https://img.shields.io/badge/View%20App-Online-blue?style=for-the-badge">
  </a>
</p>

---

# Table of Contents

- [About the Project](#about-the-project)
- [Main Features](#main-features)
- [Architecture](#architecture)
- [Project Stack](#project-stack)
- [Required Service Configurations](#required-service-configurations)
  - [Authentication Provider Configuration](#authentication-provider-configuration)
  - [Email Service Configuration](#email-service-configuration)
  - [Payment Service Configuration](#payment-service-configuration)
  - [Webhook Configuration](#webhook-configuration)
  - [Image Storage Configuration](#image-storage-configuration)
- [Database Configuration](#database-configuration)
  - [Database Setup Using Migrations](#database-setup-using-migrations)
- [Backend Configuration](#backend-configuration)
- [Frontend Configuration](#frontend-configuration)
- [Production Deployment](#production-deployment)
- [Public API Endpoints](#public-api-endpoints)
- [Protected API Endpoints](#protected-api-endpoints)
- [Admin Endpoints](#admin-endpoints)
- [Dependencies](#dependencies)

---

# About the Project

Shapper is an e-commerce or online store web application with shopping cart support, product filters, and order request management.

The project was developed using an **N-Layers Architecture** and the **Strategy Pattern** to support multiple service providers dynamically.

The project is intended for educational and practical purposes and is currently being adapted into a **Multitenant Architecture**.

---

# Main Features

- Multiple payment gateways such as Stripe and PayPal.
- Authentication and registration providers such as Firebase and Supabase.
- Order tracking and email notifications using SMTP and Brevo.
- Administrative resource management.
- Image storage using Cloudinary or local storage.
- PDF invoice/report generation for customers.

---

# Architecture

- N-Layers Architecture
- Strategy Pattern
- REST API
- JWT Authentication
- External Service Providers Integration

---

# Project Stack

<p align="center">
  <img src="documentation/Shapper-stack.drawio.svg" alt="stack" width="700">
</p>

---

# Required Service Configurations

## Authentication Provider Configuration

For Firebase, create a Firebase project and configure:

- Authorized domains
- Authentication methods
- Credentials

### Firebase Configuration

```json
"Firebase": {
  "CredentialPath": "albinia.json"
}
```

### Supabase Configuration

```json
"Supabase": {
  "JwtSecret": "Hbk"
}
```

---

# Email Service Configuration

You need an SMTP server or an email API provider.

This project supports:

- SMTP
- Brevo API

## SMTP Configuration

```json
"SmtpSettings": {
  "SmtpHost": "smtp.zoho.com",
  "SmtpPort": 587,
  "SmtpUser": "user@zohomail.com",
  "SmtpPass": "?????PASSWORD",
  "SenderName": "SMTP SENDER"
}
```

## Brevo Configuration

```json
"Brevo": {
  "ApiKey": "xkey...",
  "BrevoEndpoint": "https://api.brevo.com/v3/smtp/email",
  "BrevoEmail": "mybrevouser@mail.com"
}
```

---

# Payment Service Configuration

The project supports both Stripe and PayPal.

Both services require:

- Account creation
- API credentials
- Webhook configuration

## PayPal Configuration

```json
"PayPal": {
  "Api": "https://api-m.sandbox.paypal.com",
  "ClientId": "AVL...",
  "Secret": "EH..."
}
```

## Stripe Configuration

```json
"Stripe": {
  "SecretKey": "sk_test...",
  "WebhookSecret": "key..."
}
```

---

# Webhook Configuration

The backend must be configured and deployed before registering webhooks.

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
> Stripe requires the `WebhookSecret` value configured inside `appsettings.json`.

---

# Image Storage Configuration

Create a Cloudinary account and obtain your credentials.

```json
"ImageStorage": "Cloudinary",
"Cloudinary": {
  "ImageStorageUrl": "cloudinary://585:???"
}
```

If local storage is used, files are stored by default inside:

```txt
wwwroot
```

---

# Database Configuration

There are three ways to configure the database:

1. Execute the included `.sql` scripts.
2. Restore the `.bak` SQL Server backup.
3. Use Entity Framework Migrations.

---

## Database Setup Using Migrations

### 1. Configure Connection String

Update the `appsettings.json` file:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=MY_COMPUTER;Database=shapper;encrypt=false;Trusted_Connection=True"
}
```

### 2. Create Migration

```bash
dotnet ef migrations add InitialCreate
```

### 3. Update Database

```bash
dotnet ef database update
```

### 4. Seed Initial Data

Execute the script:

```txt
Init_shapper.sql
```

> [!IMPORTANT]
> Make sure `dotnet-ef` is installed globally before running migrations.

---

# Backend Configuration

Configure the `appsettings.json` file:

```json
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
      "http://localhost:5127/cancel.html"
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
```

## Payment Allowed Hosts

```json
"PaymentSettings": {
  "AllowedHosts": [
    "http://localhost:4200/success",
    "http://localhost:5127/index.html",
    "http://localhost:5127/cancel.html"
  ]
}
```

---

## Authentication Verification Strategy

Inside the backend code only one verification strategy should be registered.

### Firebase Strategy

```csharp
builder.Services.AddScoped<FirebaseVerificationStrategy>();
```

### Supabase Strategy

```csharp
builder.Services.AddScoped<SupabaseVerificationStrategy>();
```

---

## Install Dependencies

```bash
dotnet restore
```

---

# Production Deployment

## Backend Deployment

Publish the backend:

```bash
dotnet publish -c Release -o ./publish
```

Run the application:

```bash
dotnet shapper.dll
```

---

# Frontend Configuration

Update the Angular environment file:

```javascript
export const environment = {
  production: false,
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

## Configuration Description

- `apiUrl`: Backend server URL
- `storeReference`: Store identifier
- `currency`: Store currency
- `firebaseConfig`: Firebase client credentials

---

## Install Frontend Dependencies

```bash
npm install
```

---

## Frontend Production Build

```bash
ng build --configuration=production
```

GitHub Pages build example:

```bash
ng build --configuration production --output-path docs --base-href "/shapper/"
```

---

## Local Static Server

If `http-server` is installed:

```bash
http-server -p 8080 -c-1 --spa
```

---

# GitHub Pages Configuration

Create a `404.html` file:

```html
<script>
  sessionStorage.redirect = location.pathname;
</script>

<meta http-equiv="refresh" content="0;URL='/shapper/'" />
```

Add inside the `<head>` tag:

```html
<base href="/shapper/">
```

Add redirect recovery script:

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

---

# Public API Endpoints

Anyone can access these endpoints.

---

## Categories

```http
GET /api/categories/with-price-range
GET /api/categories?page=1&pageSize=8
```

---

## Products

```http
GET /api/products/store?page=1&pageSize=20&featured=true
GET /api/products/filter?minPrice=0&maxPrice=575&pageSize=8
GET /api/products/{id}
GET /api/products/search?term=s&count=10
```

---

## Store

```http
GET /api/store/code/{code}
```

---

## FAQs

```http
GET /api/faqs?page=1&pageSize=100
```

---

## Orders

```http
POST /api/orders
```

---

## Payments

```http
POST /api/payment/checkout
GET /api/payment/capture-payment?provider=paypal&token={token}
```

---

# Protected API Endpoints

Authenticated and verified users only.

Access is restricted to the resource owner.

---

## Users

```http
POST /api/users
POST /api/users/customer/{email}
PATCH /api/users/customer/{email}
```

---

## Reviews

```http
DELETE /api/reviews/{id}
PUT /api/reviews/{id}
```

---

## Orders

```http
GET /api/orders/user/{userId}?page=1&pageSize=10
GET /api/orders/reference/{reference}
```

---

# Admin Endpoints

Administrative module access is restricted to the configured administrator email.

---

# Dependencies

```json
{
  "@angular/fire": "^20.0.1",
  "@angular/forms": "^21.0.0",
  "@angular/material": "~21.2.0",
  "@angular/platform-browser": "^21.0.0",
  "@angular/router": "^21.0.0",
  "@ngx-translate/core": "^17.0.0",
  "@ngx-translate/http-loader": "^17.0.0",
  "firebase": "^12.11.0"
}
```