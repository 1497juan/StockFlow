-- StockFlow - 02_seed.sql
-- Productos iniciales para pruebas.
-- El inventario comienza en cero.

BEGIN;

INSERT INTO products (sku, name, description, unit_price, stock)
VALUES
    ('SKU-001', 'Teclado mecánico', 'Teclado USB para oficina', 120000.00, 0),
    ('SKU-002', 'Mouse inalámbrico', 'Mouse inalámbrico óptico', 65000.00, 0),
    ('SKU-003', 'Monitor 24 pulgadas', 'Monitor Full HD', 650000.00, 0),
    ('SKU-004', 'Cable HDMI', 'Cable HDMI de 2 metros', 25000.00, 0),
    ('SKU-005', 'Base para portátil', 'Soporte ergonómico para portátil', 85000.00, 0)
ON CONFLICT (sku) DO NOTHING;

COMMIT;