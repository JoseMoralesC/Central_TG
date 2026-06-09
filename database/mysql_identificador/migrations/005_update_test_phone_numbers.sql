USE central_identificador;

UPDATE telefonos
SET numero_cifrado = CASE telefono_id
    WHEN 1 THEN 'Ssy0wADM0R8i4alx8T1aHg=='
    WHEN 2 THEN '8rdtdFHBFJKlmaAFgVLhZA=='
    WHEN 3 THEN 'hr6q8rF6+HGpYj9AW2+Y6g=='
    WHEN 4 THEN 'M2xnYxuwGO9DlT1MKVNhxA=='
    WHEN 5 THEN 'u9B1NnVLJVIzByibOC/Z1w=='
    WHEN 6 THEN '1q1Ah6c4Huk33eD3hK7yOg=='
    WHEN 7 THEN 'o+zuVoRkAiJRQRwByI/u3Q=='
    ELSE numero_cifrado
END
WHERE telefono_id IN (1, 2, 3, 4, 5, 6, 7);
