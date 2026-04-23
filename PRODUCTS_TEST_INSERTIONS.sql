USE shapper;

INSERT INTO Stores (StoreCode, Name, Description, Email, PhoneNumber, CreatedAt, MainLocation)
VALUES (
    'ST-MANUAL01', -- Debes ponerlo tú
    'LIMÓN', 
    'Store', 
    'albinliang081@gmail.com', 
    '65565665', 
    GETUTCDATE(), 
    '100 metros del palo de mango.'
);
-- 1. Link de Red Social - Facebook
INSERT INTO StoreLinks (Name, Url, Type, Status, StoreId, CreatedAt)
VALUES (
    'Facebook',
    'https://facebook.com/limon2',
    'social',
    'ACTIVE',
    1,
    GETUTCDATE()
);

-- 2. Link de Red Social - Instagram
INSERT INTO StoreLinks (Name, Url, Type, Status, StoreId, CreatedAt)
VALUES (
    'Instagram',
    'https://instagram.com/limon2',
    'social',
    'ACTIVE',
    1,
    GETUTCDATE()
);

-- 3. Link de Red Social - WhatsApp
INSERT INTO StoreLinks (Name, Url, Type, Status, StoreId, CreatedAt)
VALUES (
    'WhatsApp',
    'https://wa.me/50683919528',
    'social',
    'ACTIVE',
    1,
    GETUTCDATE()
);

-- 4. Link de Soporte
INSERT INTO StoreLinks (Name, Url, Type, Status, StoreId, CreatedAt)
VALUES (
    'Centro de Ayuda',
    'https://limon2.com/soporte',
    'support',
    'ACTIVE',
    1,
    GETUTCDATE()
);

-- 5. Link de Términos y Condiciones
INSERT INTO StoreLinks (Name, Url, Type, Status, StoreId, CreatedAt)
VALUES (
    'Términos y Condiciones',
    'https://limon2.com/terminos',
    'legal',
    'ACTIVE',
   1,
    GETUTCDATE()
);

-- 6. Link de Política de Privacidad
INSERT INTO StoreLinks (Name, Url, Type, Status, StoreId, CreatedAt)
VALUES (
    'Política de Privacidad',
    'https://limon2.com/privacidad',
    'legal',
    'ACTIVE',
    1,
    GETUTCDATE()
);

-- 7. Link de Métodos de Pago
INSERT INTO StoreLinks (Name, Url, Type, Status, StoreId, CreatedAt)
VALUES (
    'Métodos de Pago',
    'https://limon2.com/pagos',
    'payment',
    'ACTIVE',
    1,
    GETUTCDATE()
);

-- 8. Link de Envíos
INSERT INTO StoreLinks (Name, Url, Type, Status, StoreId, CreatedAt)
VALUES (
    'TIENDA',
    'http://localhost:4200/index.html',
    'store',
    'ACTIVE',
    1,
    GETUTCDATE()
);

-- 9. Link de YouTube
INSERT INTO StoreLinks (Name, Url, Type, Status, StoreId, CreatedAt)
VALUES (
    'CANCEL',
    'http://localhost:5127/cancel.html',
    'CANCEL',
    'ACTIVE',
    1,
    GETUTCDATE()
);

-- 10. Link de TikTok
INSERT INTO StoreLinks (Name, Url, Type, Status, StoreId, CreatedAt)
VALUES (
    'SUCCESS',
    'http://localhost:5127/sucess.html',
    'SUCCESS',
    'ACTIVE',
    1,
    GETUTCDATE()
);


INSERT INTO Roles (Name, Description) VALUES ('CUSTOMER', 'STORE USER');

INSERT INTO Categories (Name, Description, ImageUrl) VALUES 
('Hogar', 'Todo para la comodidad de tu casa', 'https://images.unsplash.com/photo-1513519245088-0e12902e5a38?q=80&w=400'),
('Moda', 'Tendencias en ropa y accesorios', 'https://images.unsplash.com/photo-1445205170230-053b83016050?q=80&w=400'),
('Belleza', 'Cuidado personal y maquillaje', 'https://images.unsplash.com/photo-1522335789203-aabd1fc54bc9?q=80&w=400'),
('Libros', 'Literatura, ciencia y tecnología', 'https://images.unsplash.com/photo-1495446815901-a7297e633e8d?q=80&w=400'),
('Alimentos', 'Productos de consumo diario', 'https://m.media-amazon.com/images/I/81AW1E9DmjL._SL1000_.jpg');

INSERT INTO Subcategories (Name, Description, ImageUrl, CategoryId) VALUES 
('Muebles', 'Sillas, mesas y sofás', 'url_muebles', 1),         -- Categoria Hogar (ID 1)
('Ropa Deportiva', 'Gorras, camisetas y más', 'url_ropa', 2),   -- Categoria Moda (ID 2)
('Cosméticos', 'Labiales y cuidado facial', 'url_beauty', 3),   -- Categoria Belleza (ID 3)
('Ingeniería', 'Libros técnicos y software', 'url_libros', 4),  -- Categoria Libros (ID 4)
('Salsas y Condimentos', 'Sabor costarricense', 'url_salsa', 5); -- Categoria Alimentos (ID 5)

INSERT INTO Products (Name, Description, Price, TaxAmount, Quantity, Discount, Details, Status, SubcategoryId) VALUES 
('Minimal Chair', 'Silla de diseño minimalista para oficina o hogar', 125, 16.25, 10, 0, 'Color blanco, madera', 'Active', 1),
('Gorra Nike', 'Gorra deportiva ajustable color negro', 455, 59.15, 25, 5, 'Material transpirable', 'Active', 2),
('Lip Gloss', 'Brillo labial de larga duración Dior', 55, 7.15, 50, 0, 'Tono rosado natural', 'Active', 3),
('Salsa Lizano', 'La salsa original de Costa Rica 700ml', 65, 8.45, 100, 0, 'Botella grande', 'Active', 5),
('Libro Kleppman', 'Designing Data-Intensive Applications', 55, 0, 15, 10, 'Tapa dura, edición en español', 'Active', 4);