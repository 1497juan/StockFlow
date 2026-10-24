
-- StockFlow - Registro de entradas de inventario.

CREATE OR REPLACE PROCEDURE sp_register_inventory_entry(
    p_product_id BIGINT,
    p_quantity INTEGER,
    p_user_id BIGINT
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF p_quantity IS NULL OR p_quantity <= 0 THEN
        RAISE EXCEPTION
            'La cantidad de entrada debe ser mayor que cero.';
    END IF;

    UPDATE products
    SET stock = stock + p_quantity
    WHERE id = p_product_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION
            'El producto con ID % no existe.', p_product_id;
    END IF;

    INSERT INTO inventory_movements (
        product_id,
        user_id,
        movement_type,
        quantity
    )
    VALUES (
        p_product_id,
        p_user_id,
        'IN',
        p_quantity
    );
END;
$$;
