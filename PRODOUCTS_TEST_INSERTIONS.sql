USE shapper;

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