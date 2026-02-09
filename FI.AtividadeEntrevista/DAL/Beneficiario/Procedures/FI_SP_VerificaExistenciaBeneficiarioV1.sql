CREATE PROC FI_SP_VerificaExistenciaBeneficiarioV1  
 @ID BIGINT,    
 @CPF VARCHAR(50)      
AS      
BEGIN      
    SET NOCOUNT ON;

    -- Normaliza CPF recebido removendo pontos, traços e espaços
    DECLARE @cpfDigits VARCHAR(50) = REPLACE(REPLACE(REPLACE(@CPF, '.', ''), '-', ''), ' ', '');

    -- 1) Verifica se já existe beneficiário com o mesmo CPF (exclui o registro atual pelo ID)
    IF EXISTS (
        SELECT 1 FROM BENEFICIARIOS b
        WHERE REPLACE(REPLACE(REPLACE(b.CPF, '.', ''), '-', ''), ' ', '') = @cpfDigits
        AND b.ID <> @ID
    )
    BEGIN
        SELECT 1; RETURN;
    END

    -- 2) Verifica se já existe cliente com o mesmo CPF
    IF EXISTS (
        SELECT 1 FROM CLIENTES c
        WHERE REPLACE(REPLACE(REPLACE(c.CPF, '.', ''), '-', ''), ' ', '') = @cpfDigits
    )
    BEGIN
        SELECT 1; RETURN;
    END

    -- Não existe duplicidade
    SELECT 0;
END
