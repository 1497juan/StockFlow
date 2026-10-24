
-- StockFlow - Registro de salidas de inventario.

CREATE OR REPLACE PROCEDURE sp_register_inventory_exit(
    p_product_id BIGINT,
    p_quantity INTEGER,
    p_user_id BIGINT
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_current_stock INTEGER;
BEGIN
    IF p_quantity IS NULL OR p_quantity <= 0 THEN
        RAISE EXCEPTION
            'La cantidad de salida debe ser mayor que cero.';
    END IF;

    -- Bloquea la fila para evitar salidas concurrentes
    -- que consuman el mismo inventario disponible.
    SELECT stock
    INTO v_current_stock
    FROM products
    WHERE id = p_product_id
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION
            'El producto con ID % no existe.', p_product_id;
    END IF;

    IF v_current_stock < p_quantity THEN
        RAISE EXCEPTION
            'Stock insuficiente. Disponible: %, solicitado: %.',
            v_current_stock,
            p_quantity;
    END IF;

    UPDATE products
    SET stock = stock - p_quantity
    WHERE id = p_product_id;

    INSERT INTO inventory_movements (
        product_id,
        user_id,
        movement_type,
        quantity
    )
    VALUES (
        p_product_id,
        p_user_id,
        'OUT',
        p_quantity
    );
END;
$$;
