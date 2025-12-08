#!/bin/bash
set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE TABLE transaction_types (
        type SMALLINT PRIMARY KEY,
        description VARCHAR(255) NOT NULL,
        nature VARCHAR(255) NOT NULL,
        sign CHAR(1) NOT NULL
    );

    CREATE TABLE transactions (
        id BIGSERIAL PRIMARY KEY,
        type SMALLINT NOT NULL,
        date DATE NOT NULL,
        value NUMERIC(12,2) NOT NULL,
        cpf CHAR(11) NOT NULL,
        card VARCHAR(12) NOT NULL,
        time TIME NOT NULL,
        store_owner VARCHAR(14) NOT NULL,
        store_name VARCHAR(19) NOT NULL,
        
        CONSTRAINT fk_transactions_types
          FOREIGN KEY (type)
          REFERENCES transaction_types(type)
          ON UPDATE CASCADE
    );

    INSERT INTO transaction_types (type, description, nature, sign) VALUES
        (1, 'Debit',        'Income',  '+'),
        (2, 'Boleto',       'Expense', '-'),
        (3, 'Financing',    'Expense', '-'),
        (4, 'Credit',       'Income',  '+'),
        (5, 'Loan Receipt', 'Income',  '+'),
        (6, 'Sales',        'Income',  '+'),
        (7, 'TED Receipt',  'Income',  '+'),
        (8, 'DOC Receipt',  'Income',  '+'),
        (9, 'Rent',         'Expense', '-');

EOSQL

