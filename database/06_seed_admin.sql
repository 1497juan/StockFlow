
INSERT INTO users (username, password_hash)
VALUES (
    'administrador',
    'AQAAAAIAAYagAAAAELxWIm70looCmFa81T/V6VVdOmdXBvEe67B9nfPFnUQXANRuX/NqFWJCKsLl/IdUVA=='
)
ON CONFLICT (username) DO NOTHING;
