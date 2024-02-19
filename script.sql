CREATE UNLOGGED TABLE clientes (
   id SERIAL PRIMARY KEY,
   nome VARCHAR(50) NOT NULL,
   limite INTEGER NOT NULL,
   saldo INTEGER NOT NULL DEFAULT 0
);

CREATE UNLOGGED TABLE transacoes (
    id         SERIAL PRIMARY KEY,
    cliente_id INTEGER     NOT NULL,
    valor      INTEGER     NOT NULL,
    tipo   CHAR(1)     NOT NULL,
    descricao  VARCHAR(10) NOT NULL,
    realizado_em  TIMESTAMP   NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    CONSTRAINT fk_transacoes_cliente_id
        FOREIGN KEY (cliente_id) REFERENCES clientes(id)
);


INSERT INTO clientes (nome, limite)
  VALUES
    ('o barato sai caro', 1000 * 100),
    ('zan corp ltda', 800 * 100),
    ('les cruders', 10000 * 100),
    ('padaria joia de cocaia', 100000 * 100),
    ('kid mais', 5000 * 100);


CREATE OR REPLACE PROCEDURE create_transacao_cliente(
    cliente_id INTEGER,
    transacao_valor INTEGER,
    valor_atual INTEGER,
    tipo CHAR(1),
    descricao VARCHAR(10),
    realizado_em TIMESTAMP
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Todo: Tornar essa store procedure resiliente a concorrência, a forma mais fácil seria implementar uma estratégia pessimista.
    -- Para isso existem duas formas uma delas é realizar um select com a opção "for update" para opter o saldo atual do cliente e só então calcular o novo saldo. A diretiva "for update" vai garantir que nenhuma outra requisição da procedure possa alterar esse cliente enquanto essa procedure está sendo executada.
    -- Uma outra forma seria usar um pg_advisory_lock(cliente_id), essa abordagem tem a mesma finalidade mas recomendo procurar a documentação.
    -- Exemplo de select com bloqueio na linha 
    -- select saldo from clientes where id = cliente_id for update

    -- Esse bloqueio vai permanecer até que a procedure termine sua execução, seja com sucesso ou em caso de erro.

    -- Nessa abordagem a procedure não recebe por parametro o novo valor de saldo, esse valor precisa ser calculado após o 'select for update'.


    UPDATE clientes
    SET saldo = valor_atual
    WHERE id = cliente_id;

    INSERT INTO transacoes (valor, cliente_id, tipo, descricao, realizado_em)
    VALUES (transacao_valor, cliente_id, tipo, descricao, realizado_em);
END;
$$;