#!/bin/bash
set -e
echo "Creating central_db database..."
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE DATABASE central_db;
    GRANT ALL PRIVILEGES ON DATABASE central_db TO $POSTGRES_USER;
EOSQL

echo "Database central_db created successfully"