
-- StockFlow - Consulta del inventario actual.

CREATE OR REPLACE FUNCTION fn_get_current_inventory()
RETURNS TABLE (
    id BIGINT,
    sku VARCHAR(50),
    name VARCHAR(150),
    description TEXT,
    unit_price NUMERIC(12, 2),
    stock INTEGER
)
LANGUAGE sql
STABLE
AS $$
    SELECT
        p.id,
        p.sku,
        p.name,
        p.description,
        p.unit_price,
        p.stock
    FROM products AS p
    ORDER BY p.name;
$$;
